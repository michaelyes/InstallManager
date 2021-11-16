using Microsoft.Win32;
using SocketServer;
using STCT.DBInstall;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

namespace YEasyInstaller
{
    public class HostUtils
    {
        public static Boolean IsPortOccuped(int port)
        {
            IPGlobalProperties iproperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = iproperties.GetActiveTcpListeners();
            foreach (var item in ipEndPoints)
            {
                if (item.Port == port)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取IP列表
        /// </summary>
        /// <returns></returns>
        public static List<IPAddressEntity> IPActiveAddressList()
        {
            var IPAddressCollection = new List<IPAddressEntity>(0);
            var NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in NetworkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var IPProperties = networkInterface.GetIPProperties();
                    var UnicastAddresses = IPProperties.UnicastAddresses;
                    if (UnicastAddresses.Count > 0)
                    {
                        foreach (var Unicast in UnicastAddresses)
                        {
                            if (Unicast.Address.AddressFamily == AddressFamily.InterNetwork && !Unicast.Address.ToString().Equals("127.0.0.1"))
                            {
                                if (!IPAddressCollection.Exists(q => q.IP.Equals(Unicast.Address.ToString())))
                                {
                                    var IPModel = new IPAddressEntity();
                                    IPModel.IP = Unicast.Address.ToString();
                                    IPModel.IsDhcp = IPProperties.GetIPv4Properties() != null && IPProperties.GetIPv4Properties().IsDhcpEnabled;
                                    IPAddressCollection.Add(IPModel);
                                }
                            }
                        }
                    }
                }
            }

            return IPAddressCollection;
        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>本机IP地址</returns>
        public static List<string> GetLocalIP(ref string ip)
        {
            List<string> ipList = new List<string>();
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipList.Add(IpEntry.AddressList[i].ToString());
                        if (string.IsNullOrEmpty(ip))
                        {
                            ip = IpEntry.AddressList[i].ToString();
                        }
                    }
                }

                return ipList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取所有已经安装的程序
        /// </summary>
        /// <param name="reg"></param>
        /// <returns>程序名称,安装路径</returns> 
        public static List<InstalledModel> GetProgramAndPath(List<AppModel> appModelList)
        {
            var reg = new string[] {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            string tempType = null;
            RegistryKey currentKey = null;
            var installedModelList = new List<InstalledModel>();
            foreach (var item222 in reg)
            {
                object displayName = null, uninstallString = null, installLocation = null,
                    releaseType = null, DisplayVersion = null, InstallDate = null;
                RegistryKey pregkey = Registry.LocalMachine.OpenSubKey(item222);//获取指定路径下的键 
                if (pregkey == null) continue;

                foreach (string item in pregkey.GetSubKeyNames())               //循环所有子键
                {
                    currentKey = pregkey.OpenSubKey(item);
                    if (currentKey == null) continue;

                    displayName = currentKey.GetValue("DisplayName");           //获取显示名称
                    DisplayVersion = currentKey.GetValue("DisplayVersion");
                    InstallDate = currentKey.GetValue("InstallDate");
                    installLocation = currentKey.GetValue("InstallLocation");   //获取安装路径
                    uninstallString = currentKey.GetValue("UninstallString");   //获取卸载字符串路径
                    releaseType = currentKey.GetValue("ReleaseType");           //发行类型,值是Security Update为安全更新,Update为更新
                    bool isSecurityUpdate = false;
                    if (releaseType != null)
                    {
                        tempType = releaseType.ToString();
                        if (tempType == "Security Update" || tempType == "Update")
                        {
                            isSecurityUpdate = true;
                        }
                    }
                    if (!isSecurityUpdate && displayName != null && uninstallString != null
                        && appModelList.Exists(q => q.displayName == Obj2String(displayName)))
                    {
                        if (!installedModelList.Exists(q => q.displayName.Equals(Obj2String(displayName))))
                        {
                            var model = new InstalledModel()
                            {
                                displayName = Obj2String(displayName),
                                DisplayVersion = Obj2String(DisplayVersion),
                                InstallDate = Obj2String(InstallDate),
                                installLocation = Obj2String(installLocation),
                                releaseType = Obj2String(releaseType),
                                uninstallString = Obj2String(uninstallString)
                            };
                            installedModelList.Add(model);
                        }
                    }
                }
            }

            if (installedModelList != null)
            {
                RegistryKey pregkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths");//获取指定路径下的键 
                if (pregkey == null) return installedModelList;

                foreach (var model in installedModelList)
                {
                    string code = model.uninstallString.Replace("MsiExec.exe /I", "");
                    code = code.Replace("MsiExec.exe /X", "");
                    model.ProductCode = code;
                    var app = appModelList.Find(q => q.displayName == model.displayName);
                    foreach (string item in pregkey.GetSubKeyNames())               //循环所有子键
                    {
                        currentKey = pregkey.OpenSubKey(app.DirName);
                        if (currentKey == null) continue;

                        var dir = currentKey.GetValue(app.DirKeyName);
                        model.AppDir = Obj2String(dir);
                    }
                }
            }

            return installedModelList;
        }

        private static string Obj2String(object value)
        {
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// 人脸图片文件拷贝
        /// </summary>
        /// <param name="oldDir">旧安装目录</param>
        /// <param name="targetDir">新安装目录</param>
        public static void CopyResources(string oldDir, string targetDir)
        {
            Console.WriteLine("旧安装目录：" + oldDir);
            Console.WriteLine("新安装目录：" + targetDir);
            if (string.IsNullOrEmpty(oldDir) || string.IsNullOrEmpty(targetDir) || targetDir.Equals(oldDir)
               || !Directory.Exists(oldDir + "UserFace"))
                return;

            string faceDir = targetDir + @"UserFace\";
            if (!Directory.Exists(faceDir))
            {
                Directory.CreateDirectory(faceDir);
            }

            int idx = 0;
            DirectoryInfo folder = new DirectoryInfo(oldDir + "UserFace");//获取旧目录信息人脸图片
            Console.WriteLine("开始复制人脸图片...");
            foreach (FileInfo file in folder.GetFiles())
            {
                if (!File.Exists(faceDir + file.Name))
                {
                    file.CopyTo(faceDir + file.Name, false);
                    idx++;
                }
            }
            Console.WriteLine("复制完成，共" + idx + "张人脸图片");

            //复制旧版本资源文件
            if (Directory.Exists(oldDir + "Upload"))
            {
                CopyDirectory(targetDir + "Upload", oldDir + "Upload");
            }
        }

        /// <summary>
        /// 备份旧安装目录
        /// </summary>
        /// <param name="oldDir">旧安装目录</param>
        /// <param name="installedModel">旧安装程序信息</param>
        public static string BackupOld(string oldDir, InstalledModel installedModel)
        {
            string backupDir = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(oldDir) && !Directory.Exists(oldDir)) return backupDir;

                backupDir = oldDir.Substring(0, oldDir.Length - 1) + ".old_" + installedModel.DisplayVersion + "/";
                //备份旧安装目录文件，备份目录名称：原目录名称+“.old/”+ “年月日/”
                if (Directory.Exists(backupDir))
                {
                    //Directory.CreateDirectory(backupDir);
                    Directory.Delete(backupDir);//删除旧备份文件
                }
                CopyDirectory(oldDir, backupDir);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return backupDir;
        }

        /// <summary>
        /// 目录复制
        /// </summary>
        /// <param name="oldDir">源目录</param>
        /// <param name="targetDir">目标目录</param>
        public static void CopyDirectory(string oldDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            DirectoryInfo directoryInfo_old = new DirectoryInfo(oldDir);
            DirectoryInfo directoryInfo_new = new DirectoryInfo(targetDir);
            FileInfo[] files = directoryInfo_old.GetFiles();
            foreach(FileInfo file in files)
            {
                File.Copy(file.FullName, directoryInfo_new.FullName + @"\" + file.Name, true);
            }

            DirectoryInfo[] directoryInfos = directoryInfo_old.GetDirectories();
            foreach (DirectoryInfo directoryInfo in directoryInfos)
            {
                if (directoryInfo.Name.Equals("log", StringComparison.OrdinalIgnoreCase)) continue;//跳过log文件目录
                CopyDirectory(directoryInfo.FullName, directoryInfo_new.FullName + @"\" + directoryInfo.Name + @"\");
            }
        }


        /// <summary>
        /// 检查 IIS 版本
        /// </summary>
        /// <returns></returns>
        public static bool CheckIIS()
        {
            string iisVersion = ParamSetting.GetConfigValue(Application.ExecutablePath + ".config", "IISVersion");
            if (IISUtil.ExistSqlServerService("W3SVC"))
            {
                //Response.Write("IIS已经存在了");
                if (!IISUtil.CheckIIS(iisVersion))
                {
                    if (MessageBox.Show("IIS版本过低，请先升级至IIS" + iisVersion + "或以上版本。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                //Response.Write("IIS没有安装");
                if (MessageBox.Show("IIS未安装，请先安装IIS" + iisVersion + "或以上版本。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
