using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace YEasyInstaller.Comm
{
    public class Log
    {
        public static void d(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void d(object msg)
        {
            Console.WriteLine(msg);
        }

        public static void i(string msg)
        {
            Console.WriteLine(msg);
            WriteFile(msg);
        }

        public static void i(object msg)
        {
            Console.WriteLine(msg);
            if (msg != null)
                WriteFile(msg.ToString());
        }

        public static void i(Exception ex)
        {
            Console.WriteLine(ex);
            if (ex != null)
                WriteFile("Exception Message:" + ex.Message + "\r\n   StackTrace:" + ex.StackTrace);
        }

        #region 日志文件
        private static void WriteFile(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            msg = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] " + msg;
            if (msg.LastIndexOf("\n") != 0)
            {
                msg = msg + "\n";
            }
            string path = Application.StartupPath + "\\log.txt";
            try
            {
                FileStream fs = File.Open(path, FileMode.OpenOrCreate);//表示在该路径下创建文件  
                //写入数据
                StreamWriter writer = new StreamWriter(fs);
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                fs.Seek(0, SeekOrigin.End);
                fs.Write(buffer, 0, buffer.Length);
                //刷新和关闭FileStream
                fs.Flush();//清除该流的所有缓冲区，使得所有缓冲的数据都被写入到基础设备
                fs.Close(); //关闭流对象，否则会出现错误，提示程序被占用(关闭当前流并释放与之关联的所有资源)
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        #endregion
    }
}
