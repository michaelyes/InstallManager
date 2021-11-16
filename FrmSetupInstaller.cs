using SocketServer;
using STCT.DBInstall;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using YEasyInstaller.Comm;

namespace YEasyInstaller
{
    public partial class FrmSetupInstaller : Form
    {
        /// <summary>
        /// 当前有安装的软件名称
        /// </summary>
        List<string> InstallList = new List<string>();
        string IsMutiDataBase = "";
        /// <summary>
        /// 安装包资源目录
        /// </summary>
        string DirPath = Application.StartupPath + "\\Resources\\";
        /// <summary>
        /// 已安装软件
        /// </summary>
        List<InstalledModel> installedModelList;
        /// <summary>
        /// 安装包软件
        /// </summary>
        List<AppModel> appModelList;

        FrmLoading frmLoading = new FrmLoading();
        /// <summary>
        /// 扫描注册表，获取已安装软件
        /// </summary>
        Thread thread;
        /// <summary>
        /// 监听安装过程句柄
        /// </summary>
        Thread threadInstallerSettring;
        /// <summary>
        /// 获取本地硬盘：key-驱动器的名称，如"C:"；value-驱动器的名称信息
        /// </summary>
        SortedDictionary<string, DriveInfo> localDiskList;
        string Api_Host = string.Empty;
        /// <summary>
        /// 是否初始化运行类：初始化运行，检测数据库连接是否可用
        /// </summary>
        bool firstRun = true;
        bool dbConn = false;

        public FrmSetupInstaller()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            base.OnLoad(e);
            try
            {
                IsMutiDataBase = ParamSetting.GetAppSettingsValue("IsMutiDataBase");
                InitCtrl();
                InitAppList();
                new Thread(() =>
                {
                    localDiskList = DiskUtil.GetLocalDiskList();
                }).Start();
                threadInstallerSettring = new Thread(CheckInstallDirectory);

                HostUtils.CheckIIS();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                YEasyInstaller.Comm.Log.i(ex);
            }
        }

        private void InitCtrl()
        {
            btnExit.Click += BtnExit_Click;
            btnInstallApi.Click += BtnInstallApi_Click;
            btnInstallDatabase.Click += BtnInstallDatabase_Click;
            btnInstallSocket.Click += BtnInstallSocket_Click;
            btnInstallWeb.Click += BtnInstallWeb_Click;
            btnSetting.Click += BtnSetting_Click;
            btnRun.Click += BtnRun_Click;

            btnUnstallApi.Click += (ss, ee) =>
            {
                if (btnUnstallApi.Tag != null)
                    ExecUnstall(btnUnstallApi.Tag.ToString());
            };

            btnUnstallWeb.Click += (ss, ee) =>
            {
                if (btnUnstallWeb.Tag != null)
                    ExecUnstall(btnUnstallWeb.Tag.ToString());
            };

            btnUnstallSocket.Click += (ss, ee) =>
            {
                if (btnUnstallSocket.Tag != null)
                    ExecUnstall(btnUnstallSocket.Tag.ToString());
            };
        }

        private void InitAppList()
        {
            string path = Application.StartupPath + "\\Resources\\AppList.json";
            if (File.Exists(path))
            {
                string strJson = File.ReadAllText(path);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                this.appModelList = javaScriptSerializer.Deserialize<List<AppModel>>(strJson);
                //this.appModelList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AppModel>>(strJson);
                if (this.appModelList == null) return;
                string lblName = "lbl{0}VersionNew";
                foreach (var model in this.appModelList)
                {
                    if (!string.IsNullOrEmpty(model.SetupName))
                    {
                        model.DisplayVersion = NativeMethods.GetVersionInfo(DirPath + model.SetupName, NativeMethods.Version);
                        model.ProductName = NativeMethods.GetVersionInfo(DirPath + model.SetupName, NativeMethods.ProductName);
                    }

                    var ctrls = this.Controls.Find(string.Format(lblName, model.AppName), true);
                    if (ctrls != null && ctrls.Length > 0)
                    {
                        ctrls[0].Text = string.Format("【{0}】", model.DisplayVersion);
                    }
                }
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var model = installedModelList.Find(q => q.displayName == appModelList.Find(a => a.AppName == CommParas.SocketKey).displayName);
            Process p = new Process();
            p.StartInfo.FileName = model.AppDir + "SocketServer.exe";
            p.Start();
            Application.Exit();
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            FrmConnStringSetting frmConnStringSetting = new FrmConnStringSetting();
            if (frmConnStringSetting.ShowDialog() == DialogResult.OK)
            {
                //frmConnStringSetting.connectionStr;
                if (installedModelList == null) return;
                foreach (var model in installedModelList)
                {
                    if (string.IsNullOrEmpty(model.AppDir) || InstallList == null || !InstallList.Exists(q => q.Equals(model.displayName))) continue;
                    DirectoryInfo directoryInfo = new DirectoryInfo(model.AppDir);
                    var fileList = directoryInfo.GetFiles("*.config");
                    if (fileList != null && fileList.Length > 0)
                    {
                        var fi = fileList[0];
                        ParamSetting.config_file = fi.FullName;
                        ParamSetting.SetAppSettingsValue(CommParas.ConnectionStringKey, frmConnStringSetting.connectionStr);

                        InstallList.Remove(model.displayName);
                        MessageBox.Show(model.displayName + "数据库配置成功！");
                        YEasyInstaller.Comm.Log.i(model.displayName + "数据库配置成功！");
                    }
                }
            }
        }

        bool isLoading = false;

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (isLoading)
            {
                isLoading = false;
                return;
            }

            if (thread == null)
            {
                this.Text = "服务安装管理器 - 安装准备中，请稍候...";
                thread = new Thread(InstallCheck);
                thread.Start();
                //frmLoading.ShowDialog();
            }
            else if (thread.ThreadState != System.Threading.ThreadState.Running)
            {
                this.Text = "服务安装管理器 - 安装准备中，请稍候...";
                thread = new Thread(InstallCheck);
                thread.Start();
                //frmLoading.ShowDialog();
            }

            //InstallCheck();
        }

        private void ShowUnstall(Control control, bool show)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { ShowUnstall(control, show); }));
                return;
            }
            control.Visible = show;
        }

        private void InstallCheck()
        {
            installedModelList = HostUtils.GetProgramAndPath(appModelList);
            bool hasDb = CheckDB();
            bool hasWeb = CheckApp(CommParas.WebKey);
            bool hasApi = CheckApp(CommParas.ApiKey);
            bool hasSocket = CheckApp(CommParas.SocketKey);

            if (hasApi || hasDb || hasWeb || hasSocket)
            {
                btnSetting.Enabled = true;
            }
            else
            {
                btnSetting.Enabled = false;
            }

            if (hasSocket)
                btnRun.Enabled = true;
            else
                btnRun.Enabled = false;

            this.Text = "服务安装管理器";
            //isLoading = true;
            //frmLoading.Close();
            //thread.Abort();
        }

        /// <summary>
        /// 根据配置连接信息尝试访问数据库，并返回数据库名
        /// </summary>
        /// <param name="DBName">返回数据库名</param>
        /// <returns>是否连接成功</returns>
        private bool TryConnection(ref string DBName)
        {
            CreateDB db = new CreateDB();
            bool IsConnection = false;
            try
            {
                firstRun = false;
                //通过已安装软件获取连接字符串
                if (installedModelList != null && installedModelList.Count > 0)
                {
                    DirectoryInfo srcInfo = new DirectoryInfo(installedModelList[0].AppDir);
                    var srcList = srcInfo.GetFiles("*.config");
                    if (srcList != null && srcList.Length > 0)
                    {
                        string connString = string.Empty;
                        FileInfo srcFile = null;
                        foreach (FileInfo fileInfo in srcList)
                        {
                            if (!fileInfo.Name.ToLower().Contains(".debug.") && !fileInfo.Name.ToLower().Contains(".release."))
                            {
                                srcFile = fileInfo;
                                break;
                            }
                        }
                        ParamSetting.config_file = srcFile.FullName;
                        connString = ParamSetting.GetAppSettingsValue("ConnectionString");
                        //尝试连接字符串是否可用
                        if (!string.IsNullOrEmpty(connString) && db.IsConnection(connString + ";Connect Timeout=15000;"))
                        {
                            dbConn = IsConnection = true;
                            string[] config = connString.Split(';');
                            if (config.Length > 1)
                            {
                                DBName = config[1].Substring(config[1].IndexOf("=") + 1);
                            }
                            CreateDB.connectionStr = connString;
                            ParamSetting.config_file = Application.ExecutablePath + ".config";
                            ParamSetting.SetAppSettingsValue("ConnectionString", connString);
                            ParamSetting.SetAppSettingsValue("DatabaseName", DBName);
                            Console.WriteLine(installedModelList[0].displayName + " ConnectionString 尝试连接成功！");
                        }
                        else
                        {
                            Console.WriteLine("AppSettings.ConnectionString 错误，无法连接数据库");
                        }
                    }
                }

                if (!IsConnection)
                {
                    string connectionStr = ConfigurationManager.AppSettings["ConnectionString"];
                    //尝试连接字符串是否可用
                    if (db.IsConnection(connectionStr + ";Connect Timeout=15000;"))
                    {
                        dbConn = IsConnection = true;
                        CreateDB.connectionStr = connectionStr;
                        string[] config = connectionStr.Split(';');
                        if (config.Length > 1)
                        {
                            DBName = config[1].Substring(config[1].IndexOf("=") + 1);
                            ParamSetting.config_file = Application.ExecutablePath + ".config";
                            ParamSetting.SetAppSettingsValue("DatabaseName", DBName);
                        }
                        Console.WriteLine("AppSettings.ConnectionString 连接数据库成功");
                    }
                    else
                    {
                        Console.WriteLine("AppSettings.ConnectionString 错误，无法连接数据库");
                    }
                }
            }
            catch (Exception ex)
            {
                YEasyInstaller.Comm.Log.i(ex);
            }

            return IsConnection;
        }

        private bool CheckDB()
        {
            CreateDB db = new CreateDB();
            bool IsConnection = false;
            string DBName = string.Empty;
            if (firstRun)
            {
                IsConnection = TryConnection(ref DBName);
            }
            else
            {
                IsConnection = true;
            }

            if (string.IsNullOrEmpty(DBName))
            {
                DBName = ConfigurationManager.AppSettings["DatabaseName"];
            }
            bool result = false;
            if (IsConnection && !string.IsNullOrEmpty(DBName) && db.IsDBExist(DBName))
            {
                result = true;
                ///EXEC sys.sp_addextendedproperty @name=N'Version', @value=N'1.0.3.20200103'
                ///GO
                ///EXEC STVisitor.sys.sp_updateextendedproperty @name=N'Version', @value=N'1.0.1.2020010313' 
                ///GO
                ///select value from [STXF_Android].sys.extended_properties WHERE NAME = 'Version' and class_desc='DATABASE'
                ///select VALUE from [STXF_Android].sys.fn_listextendedproperty(default, default, default, default, default, default, default) WHERE NAME = 'Version'
                //读取版本号
                string version = db.GetDBVersion(DBName);
                if (string.IsNullOrEmpty(version))
                    version = "1.0.0";
                else
                {
                    version = version.Replace(".2020", ".20");
                    if (version.Length > 12)
                        version = version.Substring(0, 12);
                }
                lblDatabaseVersion.Text = version;
                SetVersionState(CommParas.DBKey);
            }
            SetDBTips(result);

            return result;
        }

        private bool CheckApp(string AppName)
        {
            var ctrls = this.Controls.Find(string.Format("btnUnstall{0}", AppName), true);
            if (ctrls == null || ctrls.Length == 0) return false;
            bool result = false;
            var model = installedModelList.Find(q => q.displayName == appModelList.Find(a => a.AppName == AppName).displayName);
            if (!string.IsNullOrEmpty(appModelList.Find(a => a.AppName == AppName).displayName) && model != null)
            {
                result = true;
                ctrls[0].Tag = model.uninstallString;
            }
            SetTips(result, AppName, model);
            ShowUnstall(ctrls[0], result);
            SetVersionState(AppName);

            return result;
        }

        private void SetVersionState(string name)
        {
            var lblVers = this.Controls.Find(string.Format("lbl{0}Version", name), true);
            if (lblVers == null || lblVers.Length == 0) return;
            var ctrls = this.Controls.Find(string.Format("lbl{0}VersionNew", name), true);
            if (ctrls != null && ctrls.Length > 0)
            {
                var btns = ctrls[0].Parent.Controls.Find(string.Format("btnInstall{0}", name), true);
                if (btns != null && btns.Length > 0)
                {
                    Image img = null;
                    var old = lblVers[0].Text.Replace("【", "").Replace("】", "").Replace("版本：", "");
                    if (string.IsNullOrEmpty(old) || old.Equals("版本号"))
                        old = "0.0.0";

                    var strNew = ctrls[0].Text.Replace("【", "").Replace("】", "");
                    if (string.IsNullOrEmpty(strNew) || strNew.Equals("版本号"))
                        strNew = "0.0.0";
                    Version oldVer = new Version(old);
                    Version newVer = new Version(strNew);

                    if (oldVer >= newVer)
                    {
                        if (name.Equals(CommParas.DBKey) && "1".Equals(IsMutiDataBase))
                        {
                            ((Button)btns[0]).Enabled = true;
                        }
                        else
                        {
                            img = YEasyInstaller.Properties.Resources._checked;
                            ((Button)btns[0]).Enabled = false;
                        }
                    }
                    else
                    {
                        ((Button)btns[0]).Enabled = true;
                        img = YEasyInstaller.Properties.Resources._new;
                    }
                    ((Button)btns[0]).Image = img;
                }
            }
        }

        private void ExecUnstall(string path)
        {
            path = path.Replace("MsiExec.exe /I", "");
            path = path.Replace("MsiExec.exe /X", "");
            Process p = new Process();
            p.StartInfo.FileName = "msiexec.exe";
            p.StartInfo.Arguments = "/X " + path;//"/x {C56BBAC8-0DD2-4CE4-86E0-F2BDEABDD0CF} /quiet /norestart";
            p.Start();
        }

        private void ExecInstall(string path, string appName)
        {
            if (!CommParas.SocketKey.Equals(appName) && !HostUtils.CheckIIS()) return;
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "msiexec";//Application.StartupPath+ "\\Resources\\" +
                psi.Arguments = string.Format("/i \"{0}\" /qf", path);//"/i \"{0}\" /qf"
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                YEasyInstaller.Comm.Log.i(ex);
            }
        }

        private void BtnInstallWeb_Click(object sender, EventArgs e)
        {
            int port = 0;
            var model = appModelList.Find(a => a.AppName == CommParas.WebKey);
            if (model.displayName.IndexOf("消费") >= 0)
            {
                port = CommParas.Web_XF_Port;
            }
            else
            {
                port = CommParas.Web_MJ_Port;
            }

            FrmChooseIP frmChooseIP = new FrmChooseIP(port);
            if (frmChooseIP.ShowDialog() == DialogResult.OK)
            {
                port = frmChooseIP.Port;
                model.Remark = frmChooseIP.IP + ":" + port;
                ExecInstall(DirPath + model.SetupName, model.AppName);

                threadInstallerSettring = new Thread(CheckInstallDirectory);
                threadInstallerSettring.Start(model);
            }
        }

        private void BtnInstallSocket_Click(object sender, EventArgs e)
        {
            int port = 0;
            var model = appModelList.Find(a => a.AppName == CommParas.SocketKey);
            if (model.displayName.IndexOf("消费") >= 0)
            {
                port = CommParas.Socket_XF_Port;
            }
            else
            {
                port = CommParas.Socket_MJ_Port;
            }

            FrmChooseIP frmChooseIP = new FrmChooseIP(port);
            if (frmChooseIP.ShowDialog() == DialogResult.OK)
            {
                port = frmChooseIP.Port;
                model.Remark = frmChooseIP.IP + ":" + port;
                ExecInstall(DirPath + model.SetupName, model.AppName);

                threadInstallerSettring = new Thread(CheckInstallDirectory);
                threadInstallerSettring.Start(model);
                //CheckInstallDirectory(new string[] { model.ProductName, model.AppName });
            }
        }

        private void BtnInstallDatabase_Click(object sender, EventArgs e)
        {
            DBinstall dBinstall = new DBinstall();
            if (dBinstall.ShowDialog() == DialogResult.OK)
            {
                dbConn = true;
                SetDBTips(true);
            }
        }

        private void SetDBTips(bool value)
        {
            if (value)
            {
                lblDatabaseHas.Text = "已安装";
                lblDatabaseHas.ForeColor = Color.Green;
            }
            else
            {
                lblDatabaseHas.Text = "未安装";
                lblDatabaseHas.ForeColor = Color.Black;
            }
            lblDatabaseVersion.Visible = value;
        }

        private void SetTips(bool value, string name, InstalledModel model = null)
        {
            var lblHas = (Label)this.Controls.Find(string.Format("lbl{0}Has", name), true)[0];
            var lblVers = (Label)this.Controls.Find(string.Format("lbl{0}Version", name), true)[0];
            if (value)
            {
                lblHas.Text = "已安装";
                lblHas.ForeColor = Color.Green;
                lblVers.Text = "版本：" + model.DisplayVersion;
            }
            else
            {
                lblVers.Text = "";
                lblHas.Text = "未安装";
                lblHas.ForeColor = Color.Black;
            }
            lblVers.Visible = value;
        }

        private void BtnInstallApi_Click(object sender, EventArgs e)
        {
            int port = 0;
            var model = appModelList.Find(a => a.AppName == CommParas.ApiKey);
            if (model.displayName.IndexOf("消费") >= 0)
            {
                port = CommParas.Api_XF_Port;
            }
            else
            {
                port = CommParas.Api_MJ_Port;
            }

            FrmChooseIP frmChooseIP = new FrmChooseIP(port);
            if (frmChooseIP.ShowDialog() == DialogResult.OK)
            {
                port = frmChooseIP.Port;
                model.Remark = frmChooseIP.IP + ":" + port;
                Api_Host = "http://" + model.Remark;
                ExecInstall(DirPath + model.SetupName, model.AppName);

                threadInstallerSettring = new Thread(CheckInstallDirectory);
                threadInstallerSettring.Start(model);
            }
            else
            {
                Api_Host = string.Empty;
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckInstallDirectory(object obj)
        {
            Thread.Sleep(300);//等待安装窗口
            if (obj != null && obj is AppModel)
            {
                AppModel args = (AppModel)obj;
                string productName = args.ProductName;
                string appName = args.AppName;
                string oldDir = string.Empty;
                string backupDir = string.Empty;
                if (this.installedModelList != null)
                {
                    var installModel = this.installedModelList.Find(q => q.displayName == args.displayName);
                    if (installModel != null)
                    {
                        oldDir = installModel.AppDir;
                        //TODO 备份旧安装目录文件，备份目录名称：原目录名称+“.old/”+ “版本号/”
                        backupDir = HostUtils.BackupOld(oldDir, installModel);
                    }
                }
                InstallList.Add(args.displayName);

                int port = 0;
                string ip = string.Empty;//localhost
                if (!string.IsNullOrEmpty(args.Remark))
                {
                    ip = args.Remark.Split(':')[0];
                    if (args.Remark.Split(':').Length > 1)
                    {
                        int.TryParse(args.Remark.Split(':')[1], out port);
                    }
                }

                int noneCount = 0;
                IntPtr hwndTemp = new IntPtr();
                string lParam = string.Empty;//"D:\\Program Files (x86)\\STKJ\\" + model.DirName + "\\";

                while (true)
                {
                    Thread.Sleep(200);

                    IntPtr windPtr = SpyUtil.FindWindow("MsiDialogCloseClass", productName);
                    IntPtr ptr = new IntPtr();
                    IntPtr ptrConfig = new IntPtr();
                    if (!windPtr.Equals(IntPtr.Zero))
                    {
                        ptr = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, null, "选择安装文件夹");
                        if (!ptr.Equals(IntPtr.Zero))
                        {
                            YEasyInstaller.Comm.Log.d("选择安装文件夹");
                        }
                        else
                        {
                            ptrConfig = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, null, "请输入网站配置");
                            if (!ptrConfig.Equals(IntPtr.Zero))
                            {
                                YEasyInstaller.Comm.Log.d("网站配置");
                            }
                        }
                        noneCount = 0;
                    }

                    if (!windPtr.Equals(IntPtr.Zero) && hwndTemp != windPtr && !ptr.Equals(IntPtr.Zero))
                    {
                        hwndTemp = windPtr;
                        IntPtr htextbox = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, "RichEdit20W", null);
                        if (!htextbox.Equals(IntPtr.Zero))
                        {
                            Thread.Sleep(200);//等待200毫秒，再修改安装目录
                            YEasyInstaller.Comm.Log.d("选择安装文件夹-RichEdit20W");
                            if (!string.IsNullOrEmpty(oldDir) && !oldDir.Substring(0, 1).ToUpper().Equals("C"))
                            {
                                lParam = oldDir;
                            }
                            else
                            {
                                int cTxtLen = SpyUtil.GetMessageLen(htextbox, SpyUtil.WM_GETTEXTLENGTH, 0, 0); //获取内容长度
                                Byte[] byt = new Byte[cTxtLen];
                                SpyUtil.GetMessage((int)htextbox, SpyUtil.WM_GETTEXT, cTxtLen + 1, byt); //获取内容
                                string ret = Encoding.Default.GetString(byt);
                                lParam = GetDiskName() + ret.Substring(1);
                            }
                            //设置新的安装目录
                            YEasyInstaller.Comm.Log.d(" Install dir textbox - " + lParam);
                            int resultCode = SpyUtil.SendMessage(htextbox, SpyUtil.WM_SETTEXT, IntPtr.Zero, lParam);
                            SpyUtil.SendMessage(htextbox, SpyUtil.WM_LBUTTONDOWN, IntPtr.Zero, null);//鼠标按下按钮

                            //退出句柄监听
                            //Console.WriteLine("Ptr is " + resultCode + " ,threadInstallerSettring.Abort()");
                            //noneCount = 0;
                            //threadInstallerSettring.Abort();
                            //break;
                        }
                    }
                    else if (!windPtr.Equals(IntPtr.Zero) && hwndTemp != windPtr && !ptrConfig.Equals(IntPtr.Zero))
                    {
                        hwndTemp = windPtr;
                        IntPtr htextbox = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, "RichEdit20W", null);
                        if (!htextbox.Equals(IntPtr.Zero) && !string.IsNullOrEmpty(ip))
                        {
                            Thread.Sleep(200);//等待200毫秒，再修改安装目录
                            YEasyInstaller.Comm.Log.d("localhost-RichEdit20W:" + ip);
                            int resultCode = SpyUtil.SendMessage(htextbox, SpyUtil.WM_SETTEXT, IntPtr.Zero, ip);
                            SpyUtil.SendMessage(htextbox, SpyUtil.WM_LBUTTONDOWN, IntPtr.Zero, null);//鼠标按下按钮
                        }
                        else
                        {
                            YEasyInstaller.Comm.Log.d("localhost-RichEdit20W Ptr is Zero");
                        }

                        IntPtr prtPort = SpyUtil.FindWindowEx(windPtr, htextbox, "RichEdit20W", null);
                        if (!prtPort.Equals(IntPtr.Zero) && !string.IsNullOrEmpty(ip))
                        {
                            YEasyInstaller.Comm.Log.d("Port-RichEdit20W:" + port);
                            int resultCode = SpyUtil.SendMessage(prtPort, SpyUtil.WM_SETTEXT, IntPtr.Zero, port.ToString());
                            SpyUtil.SendMessage(prtPort, SpyUtil.WM_LBUTTONDOWN, IntPtr.Zero, null);//鼠标按下按钮
                        }
                        else
                        {
                            YEasyInstaller.Comm.Log.d("Port-RichEdit20W Ptr is Zero");
                        }
                    }

                    if (!windPtr.Equals(IntPtr.Zero))
                    {
                        var ptrNext = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, "Button", "下一步(&N) >");

                        if (!ptrNext.Equals(IntPtr.Zero))
                        {
                            int temp = SpyUtil.GetWindowLong(ptrNext, SpyUtil.GWL_STYLE);
                            if ((temp & SpyUtil.WS_DISABLED) != 0)
                            {
                                //Console.WriteLine("[下一步] 不可用");
                            }
                            else
                            {
                                YEasyInstaller.Comm.Log.d("自动下一步");
                                SpyUtil.PrintEnter();
                                //SpyUtil.SendMessage(ptrNext, SpyUtil.WM_LBUTTONDOWN, IntPtr.Zero, null);//鼠标按下按钮
                            }
                        }
                        else
                        {
                            ptrNext = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, "Button", "完成(&F)");
                            if (!ptrNext.Equals(IntPtr.Zero))
                            {
                                YEasyInstaller.Comm.Log.d("完成,自动下一步");
                                SpyUtil.PrintEnter();
                            }
                            var ptrClose = SpyUtil.FindWindowEx(windPtr, IntPtr.Zero, "Button", "关闭(&C)");
                            if (!ptrClose.Equals(IntPtr.Zero))
                            {
                                YEasyInstaller.Comm.Log.d("自动关闭");
                                SpyUtil.PrintEnter();
                                //自动拷贝旧目录资源文件
                                if (args.AppName.Equals(CommParas.WebKey, StringComparison.OrdinalIgnoreCase) || args.AppName.Equals(CommParas.ApiKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    HostUtils.CopyResources(oldDir, lParam);
                                }

                                //自动配置Sokect服务 IP、接口域名
                                if (args.AppName.Equals(CommParas.SocketKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    AppSetting(lParam, ip, port, args);
                                }
                                //自动数据库连接字符串
                                AppConnSetting(backupDir, lParam, args);
                                new Thread(ShowSettingTip).Start();

                                noneCount = 0;
                                YEasyInstaller.Comm.Log.d("关闭,threadInstallerSettring.Abort()");
                                threadInstallerSettring.Abort();
                                break;
                            }
                        }
                    }

                    //是否空句柄，若15秒内检测不到句柄，自动退出监听线程
                    if (windPtr == null || IntPtr.Zero == windPtr)
                    {
                        YEasyInstaller.Comm.Log.d("windPtr is Empty, noneCount: " + noneCount);
                        noneCount++;
                        if (noneCount >= 100)
                        {
                            //15秒后，退出句柄监听
                            new Thread(ShowSettingTip).Start();
                            noneCount = 0;
                            YEasyInstaller.Comm.Log.d("Install complated or Dialog exit,threadInstallerSettring.Abort()");
                            threadInstallerSettring.Abort();
                            break;
                        }
                    }
                }
            }
        }

        private void AppSetting(string lParam, string ip, int port, AppModel args)
        {
            try
            {
                if (string.IsNullOrEmpty(lParam) || !Directory.Exists(lParam)) return;
                DirectoryInfo directoryInfo = new DirectoryInfo(lParam);
                var fileList = directoryInfo.GetFiles("*.config");
                if (fileList != null && fileList.Length > 0)
                {
                    var fi = fileList[0];
                    ParamSetting.config_file = fi.FullName;
                    if (!string.IsNullOrEmpty(ip))
                    {
                        ParamSetting.SetAppSettingsValue("IP", ip);
                        YEasyInstaller.Comm.Log.i(args.displayName + "IP配置成功！");
                    }
                    if (port > 0)
                    {
                        ParamSetting.SetAppSettingsValue("Port", port.ToString());
                        YEasyInstaller.Comm.Log.i(args.displayName + "Port配置成功！");
                    }
                    if (!string.IsNullOrEmpty(Api_Host))
                    {
                        ParamSetting.SetAppSettingsValue("WebApi", Api_Host);
                        YEasyInstaller.Comm.Log.i(args.displayName + "Api_Host配置成功！");
                    }
                }
            }
            catch (Exception ex)
            {
                YEasyInstaller.Comm.Log.i(ex);
            }
        }

        /// <summary>
        /// 自动配置数据库-从旧版本中读取或新创建数据库，设置到新安装版本中
        /// </summary>
        /// <param name="srcDir">旧版本目录</param>
        /// <param name="lParam">新版本目录</param>
        /// <param name="args"></param>
        private void AppConnSetting(string srcDir, string lParam, AppModel args)
        {
            try
            {
                if (string.IsNullOrEmpty(srcDir) || !Directory.Exists(srcDir))//新安装软件
                {
                    if (dbConn && !string.IsNullOrEmpty(CreateDB.connectionStr))//如果有新创建数据库或升级数据库
                    {
                        SetDBConnection(CreateDB.connectionStr.Replace("Connect Timeout=36000;", "").Replace("Connect Timeout=15000;", ""), lParam, args.displayName);
                    }
                }
                else  //升级软件
                {
                    DirectoryInfo srcInfo = new DirectoryInfo(srcDir);
                    var srcList = srcInfo.GetFiles("*.config");
                    if (srcList == null || srcList.Length == 0) return;

                    string connString = string.Empty;
                    var srcFile = srcList[0];
                    ParamSetting.config_file = srcFile.FullName;
                    connString = ParamSetting.GetAppSettingsValue("ConnectionString");

                    SetDBConnection(connString, lParam, args.displayName);
                }
            }
            catch (Exception ex)
            {
                YEasyInstaller.Comm.Log.i(ex);
            }
        }

        /// <summary>
        /// 数据库连接字符串写入程序配置
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="appDir"></param>
        /// <param name="appName"></param>
        private void SetDBConnection(string connString, string appDir, string appName)
        {
            if (string.IsNullOrEmpty(appDir) || !Directory.Exists(appDir)) return;
            DirectoryInfo directoryInfo = new DirectoryInfo(appDir);
            var fileList = directoryInfo.GetFiles("*.config");
            if (fileList != null && fileList.Length > 0)
            {
                var fi = fileList[0];
                ParamSetting.config_file = fi.FullName;
                if (!string.IsNullOrEmpty(connString))
                {
                    ParamSetting.SetAppSettingsValue("ConnectionString", connString);
                    YEasyInstaller.Comm.Log.i(appName + "ConnectionString配置成功！");
                    if (InstallList != null)
                        InstallList.Remove(appName);
                }
            }
            //dbConn
        }

        /// <summary>
        /// 获取安装盘名称
        /// </summary>
        /// <returns></returns>
        private string GetDiskName()
        {
            string diskName = "C";
            if (localDiskList != null && localDiskList.Count > 0)
            {
                int i = 0;
                foreach (string key in localDiskList.Keys)
                {
                    diskName = key.Substring(0, 1);
                    if (i >= 1)
                        break;
                    i++;
                }
            }
            return diskName;
        }

        /// <summary>
        /// 提示更新数据库配置
        /// </summary>
        private void ShowSettingTip()
        {
            if (InstallList == null || InstallList.Count <= 0)
            {
                return;
            }
            if (lblSettingTip.InvokeRequired)
            {
                Action actionDelegate = () =>
                {
                    lblSettingTip.Visible = true;
                    YEasyInstaller.Comm.Log.d("Visible = true");
                };
                this.Invoke(actionDelegate);
            }
            else
            {
                lblSettingTip.Visible = true;
                YEasyInstaller.Comm.Log.d("Visible = true");
            }
        }
    }
}
