using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace YEasyInstaller
{
    public class DiskUtil
    {
        /// <summary>
        /// 获取本地硬盘
        /// </summary>
        /// <returns></returns>
        public static SortedDictionary<string, DriveInfo> GetLocalDiskList()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            SortedDictionary<string, DriveInfo> list = new SortedDictionary<string, DriveInfo>();
            for (int i = allDrives.Length - 1; i >= 0; i--)
            {
                DriveInfo d = allDrives[i];
                if (d.IsReady == true && d.DriveType == DriveType.Fixed)
                {
                    //本地硬盘
                    list.Add(d.Name, d);
                }
                else
                {
                }
            }
            //list.TryGetValue(1,out value);

            return list;
        }

        [DllImport("kernel32")]
        public static extern uint GetDriveType(string lpRootPathName);
        public static void GetDisk2()
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string drive in drives)
            {
                //Determine icon to display by drive
                switch (GetDriveType(drive))
                {
                    case 2:
                        Console.WriteLine("软盘");
                        break;
                    case 3:
                        Console.WriteLine("硬盘");
                        break;
                    case 4:
                        Console.WriteLine("网络驱动器");
                        break;
                    case 5:
                        Console.WriteLine("光驱驱动器");
                        break;
                    default:
                        Console.WriteLine("");
                        break;
                }
            }
        }

    }
}
