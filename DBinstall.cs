using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace STCT.DBInstall
{
    public partial class DBinstall : Form
    {
        CreateDB db = new CreateDB();

        public DBinstall()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            txtDB_Name.Text = ConfigurationManager.AppSettings["DatabaseName"];
            txtPath.Text = ConfigurationManager.AppSettings["FilePath"];

            InitAppList();
        }


        #region 验证安装数据库信息
        private bool CheckDataBase()
        {
            //STBlock.STPublic.Dialog dialog = new STBlock.STPublic.Dialog();
            if (this.txtServerName.Text == "")
            {
                MessageBox.Show("服务器地址不允许为空！");
                this.txtServerName.Focus();
                return false;
            }
            if (this.txtDB_Name.Text == "")
            {
                MessageBox.Show("数据库名称不允许为空！");
                this.txtDB_Name.Focus();
                return false;
            }
            if (this.txtDB_user.Text == "")
            {
                MessageBox.Show("数据库登录用户名不允许为空！");
                this.txtDB_user.Focus();
                return false;
            }
            //if (this.txtDB_Pass.Text == "")
            //{
            //    MessageBox.Show("数据库登录密码不允许为空！");
            //    this.txtDB_Pass.Focus();
            //    return false;
            //}
            return true;
        }
        #endregion

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (!CheckDataBase())
            {
                return;
            }

            string connStr = "server=" + this.txtServerName.Text.Trim() + ";database=master;uid="
                + this.txtDB_user.Text.Trim() + ";pwd=" + this.txtDB_Pass.Text + ";";
            if (db.IsConnection(connStr))
            {
                connStr = "server=" + this.txtServerName.Text.Trim() + ";database=" + txtDB_Name.Text.Trim() + ";uid="
                    + this.txtDB_user.Text.Trim() + ";pwd=" + this.txtDB_Pass.Text + ";";
                if (db.IsConnection(connStr + "Connect Timeout=36000;"))
                {
                    CreateDB.connectionStr = connStr;
                    SetAppSettingsValue("ConnectionString", connStr);
                    SetAppSettingsValue("DatabaseName", this.txtDB_Name.Text.Trim());
                    MessageBox.Show("测试连接" + this.txtDB_Name.Text.Trim() + "库成功，您可以开始升级数据库！");
                }
                else
                {
                    MessageBox.Show("测试连接master主库成功，您可以开始创建数据库！");
                }
            }
            else
            {
                MessageBox.Show("数据库连接失败！请确认您输入的连接参数是否正确！");
            }
        }

        #region 安装数据库
        private void btnCreateData_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定是创建数据库吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.progressBar1.Visible = true;
                this.progressBar1.Value = 30;

                if (!CheckDataBase())
                {
                    this.progressBar1.Visible = false;
                    this.progressBar1.Value = 0;
                    return;
                }

                db.SetConnection(this.txtServerName.Text, "master", this.txtDB_user.Text, this.txtDB_Pass.Text);
                if (!db.IsConnection())
                {
                    MessageBox.Show("数据库连接失败！请确认你输入的连接参数是否有错误！");
                    this.progressBar1.Visible = false;
                    this.progressBar1.Value = 0;
                    return;
                }
                if (db.IsDBExist(this.txtDB_Name.Text))
                {
                    this.progressBar1.Visible = false;
                    this.progressBar1.Value = 0;
                    MessageBox.Show("数据库“" + this.txtDB_Name.Text + "”存在，请重新输入要创建的数据库名！");
                    return;
                }
                #region 获取当前路径
                try
                {
                    //判断当前目录是否存在,不存在并自动创建
                    string path = this.txtPath.Text.Trim() + "\\";
                    //string path = System.Windows.Forms.Application.StartupPath + "\\Data\\";
                    path = path.Replace("\\", @"\");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    this.progressBar1.Value = 40;

                    #endregion
                    this.btnCreateData.Enabled = false;
                    this.progressBar1.Value = 60;

                    object obj = path + "*" + path + "*";
                    CheckForIllegalCrossThreadCalls = false;
                    Thread thread = new Thread(new ParameterizedThreadStart(setupDataBase));
                    thread.Start(obj);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("创建失败！ 失败原因为：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    YEasyInstaller.Comm.Log.i(ex);
                }
            }
        }
        #endregion

        private void setupDataBase(object obj1)
        {
            try
            {
                this.progressBar1.Value = 70;
                string[] obj = obj1.ToString().Split('*');
                string path1 = obj[0];
                string path2 = obj[1];
                string message = "";
                bool flag = this.db.AddDBTable(this.txtDB_Name.Text, path1, path2, ref message);
                this.progressBar1.Value = 90;
                if (flag)
                {
                    this.progressBar1.Value = 100;
                    string connStr = CreateDB.connectionStr.Replace("database=master", "database=" + this.txtDB_Name.Text.Trim());
                    connStr = connStr.Replace("Connect Timeout=36000;", "");
                    SetAppSettingsValue("ConnectionString", connStr);
                    SetAppSettingsValue("DatabaseName", this.txtDB_Name.Text.Trim());

                    MessageBox.Show("创建成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("创建失败，失败原因为：" + message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    YEasyInstaller.Comm.Log.i(message);
                }

                this.btnCreateData.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "  ------------  " + ex.StackTrace);
                YEasyInstaller.Comm.Log.i(ex);
            }
        }

        //数据库存放位置
        private void Save_location_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择Txt所在文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show("文件夹路径不能为空");
                    //System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                else
                {
                    this.txtPath.Text = dialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 获取应用程序配置文件参数值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAppSettingsValue(string name)
        {
            string value = null;
            try
            {
                value = ConfigurationManager.AppSettings[name];
            }
            catch (Exception ex)
            {
                //TODO: 
                MessageBox.Show(ex.StackTrace);
            }

            //return value;
            return value;
        }

        /// <summary>
        /// 设置应用程序配置文件值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAppSettingsValue(string name, string value)
        {
            try
            {
                //获取Configuration对象
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //写入<add>元素的Value
                config.AppSettings.Settings[name].Value = value;
                config.Save();
                //刷新，否则程序读取的还是之前的值（可能已装入内存）
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        /// <summary>
        /// 测试数据库是否连接成功
        /// </summary>
        /// <param name="SqlString">连接字符串</param>
        /// <param name="msg">返回错误信息</param>
        /// <param name="isConfigAdress">是否使用config 连接字符串</param>
        /// <param name="millisecond">多少秒 终止访问(默认2000)</param>
        /// <returns></returns>
        public bool IsConnection(string SqlString, out string msg, int millisecond = 2000)
        {
            System.Data.SqlClient.SqlConnection SqlConect = new System.Data.SqlClient.SqlConnection(SqlString);
            bool IsCanConnectioned = false;
            string Error = "";
            string msg_temp = "";

            Thread thread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    SqlConect.Open();
                    IsCanConnectioned = true;
                    SqlConect.Close();
                    msg_temp = "1";
                }
                catch (Exception e)
                {
                    Error = "数据库服务器连接错误,原因:" + e.Message + " 请务必确认IP地址,数据库名称,账户,密码是否正确填写";
                    IsCanConnectioned = false;
                    msg_temp = "1";
                }

            }));
            thread.Start();

            #region 延时

            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < millisecond)//毫秒
            {
                if (msg_temp == "1")
                {
                    break;
                };
            }

            #endregion

            if (ConnectionState.Open == SqlConect.State)
            {
                IsCanConnectioned = true;
            }
            else
            {
                SqlConect.Close();
                Error = (Error == "") ? "数据库连接失败,可能是数据库没有正确配置,也可能是数据库设置问题" : Error;
            }
            msg = Error;
            return IsCanConnectioned;
        }

        List<AppModel> appModelList;

        private void InitAppList()
        {
            try
            {
                string path = Application.StartupPath + "\\Resources\\AppList.json";
                if (File.Exists(path))
                {
                    string strJson = File.ReadAllText(path);
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    this.appModelList = javaScriptSerializer.Deserialize<List<AppModel>>(strJson);
                    //this.appModelList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AppModel>>(strJson);
                }
            }
            catch { }
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            db.SetConnection(this.txtServerName.Text, "master", this.txtDB_user.Text, this.txtDB_Pass.Text);
            if (!db.IsConnection())
            {
                MessageBox.Show("数据库连接失败！请确认你输入的连接参数是否有错误！");
                this.progressBar1.Visible = false;
                this.progressBar1.Value = 0;
                return;
            }
            if (!db.IsDBExist(this.txtDB_Name.Text))
            {
                this.progressBar1.Visible = false;
                this.progressBar1.Value = 0;
                MessageBox.Show("数据库“" + this.txtDB_Name.Text.Trim() + "”不存在，请重新输入要更新的数据库名！");
                return;
            }
            string connStr = CreateDB.connectionStr.Replace("database=master", "database=" + this.txtDB_Name.Text.Trim());
            CreateDB.connectionStr = connStr;
            connStr = connStr.Replace("Connect Timeout=36000;", "");
            SetAppSettingsValue("ConnectionString", connStr);
            SetAppSettingsValue("DatabaseName", this.txtDB_Name.Text.Trim());

            if (MessageBox.Show("确定是更新数据库吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    Version dbVersion = new Version("1.0.0");
                    if (!CheckDBVersion(ref dbVersion))
                    {
                        this.progressBar1.Visible = false;
                        return;
                    }
                    this.progressBar1.Value = 0;
                    //TODO 先备份数据库
                    BackupDB();
                    this.progressBar1.Value = 30;
                    string dir = Application.StartupPath + "\\Resources\\SQLUpgrade\\";
                    bool flag = true;
                    //更新SQLUpgrade目录数据库脚本
                    if (Directory.Exists(dir))
                    {
                        DirectoryInfo folder = new DirectoryInfo(dir);
                        foreach (FileInfo file in folder.GetFiles())
                        {
                            //Console.WriteLine(file.FullName); //获取每个文件的所有信息
                            //file.CopyTo(modelList[idx].Person_No + file.Extension);
                            string verString = file.Name.Replace("SQLUpgrade_", "").Replace(".sql", "");
                            Version ver = new Version(verString);
                            if (dbVersion < ver)
                            {
                                flag = this.db.ExecuteSqlFile(CreateDB.connectionStr, file.FullName, this.txtDB_Name.Text.Trim());

                                this.progressBar1.Value = this.progressBar1.Value + 5;
                                YEasyInstaller.Comm.Log.i("db版本" + verString + ",更新结果" + flag);
                            }
                        }
                    }
                    this.progressBar1.Value = 90;
                    if (flag)
                    {
                        this.progressBar1.Value = 100;
                        MessageBox.Show("更新成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("更新失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        YEasyInstaller.Comm.Log.i("更新失败");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新失败：" + ex.Message + "。\r\n请联系技术人员，说明当前错误信息，协助处理数据库升级。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    YEasyInstaller.Comm.Log.i(ex);
                }
            }
        }

        private bool CheckDBVersion(ref Version dbVersion)
        {
            if (this.appModelList != null)
            {
                var dbModel = this.appModelList.Find(q => q.AppName == YEasyInstaller.Comm.CommParas.DBKey);
                if (dbModel != null)
                {
                    var strNew = dbModel.DisplayVersion;
                    string version = db.GetDBVersion(this.txtDB_Name.Text.Trim());
                    if (string.IsNullOrEmpty(version))
                        version = "1.0.0";
                    else
                    {
                        version = version.Replace(".2020", ".20");
                        if (version.Length > 12)
                            version = version.Substring(0, 12);
                    }

                    dbVersion = new Version(version);
                    Version newVer = new Version(strNew);
                    if (dbVersion >= newVer)
                    {
                        if (MessageBox.Show("当前数据库版本已经是最新的,重复更新可能导致数据库问题，是否要继续更新？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void BackupDB()
        {
            string dir = txtPath.Text.Trim();
            if (!Directory.Exists(dir))
            {
                dir = GetDiskName() + @":\Database\";
                Directory.CreateDirectory(dir);
            }
            db.BackupDatabase(txtDB_Name.Text.Trim(), dir, txtDB_Name.Text.Trim() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak");
            Console.WriteLine("数据库备份完成，目录：" + dir);
        }

        /// <summary>
        /// 获取安装盘名称
        /// </summary>
        /// <returns></returns>
        private string GetDiskName()
        {
            string diskName = "C";
            try
            {
                var localDiskList = YEasyInstaller.DiskUtil.GetLocalDiskList();
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
            }
            catch { }

            return diskName;
        }

    }
}