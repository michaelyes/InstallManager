using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace YEasyInstaller
{
    public class IISUtil
    {

        public static bool ExistSqlServerService(string tem)
        {
            bool ExistFlag = false;
            ServiceController[] service = ServiceController.GetServices();
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].ServiceName.ToString() == tem)
                {
                    ExistFlag = true;
                }
            }
            return ExistFlag;
        }

        public static bool CheckIIS(string destVersion)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey ver = rk.OpenSubKey(@"SOFTWARE\Microsoft\InetStp");
                int majorVersion = Convert.ToInt32(ver.GetValue("majorversion"));
                int minorVersion = Convert.ToInt32(ver.GetValue("minorversion"));
                Version versionStr = new Version(majorVersion + "." + minorVersion);
                //System.Windows.Forms.MessageBox.Show(versionStr.ToString());
                if (versionStr >= new Version(destVersion))
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool checkSQLServer(string destVersion)
        {
            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            RegistryKey sub = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server");
            object keyInst = null;
            if (sub != null)
                keyInst = sub.GetValue("InstalledInstances");

            if (keyInst != null)
            {
                try
                {
                    Version SQLVer = null;
                    foreach (string str in (string[])keyInst)
                    {
                        RegistryKey subSQL = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL");
                        object keySQL = subSQL.GetValue(str);
                        RegistryKey subVer = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\" + (string)keySQL + @"\Setup");
                        object keyVer = subVer.GetValue("Version");
                        //get version numer
                        SQLVer = new Version((string)keyVer);
                    }
                    //SQL Version should >= 12.1.4100.1
                    Version tagVer = new Version(destVersion);
                    if (SQLVer >= tagVer)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

            }
            else
                return false;
        }
    }
}
