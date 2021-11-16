using System;
using System.Windows.Forms;
using System.Xml;

namespace SocketServer
{
    public class ParamSetting
    {
        public static string config_file = Application.ExecutablePath + ".config";
        /// <summary>
        /// 读取配置文件的值(XML)
        /// </summary>
        /// <param name="configPath">配置文件名</param>
        /// <param name="key">配置项名</param>
        /// <returns></returns>
        public static string GetConfigValue(string configPath, string key)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //string configPath = "config.xml";
            xmlDoc.Load(configPath);
            XmlElement xElem = xmlDoc.SelectSingleNode("//appSettings/add[@key='" + key + "']") as XmlElement;
            if (xElem != null)
                return xElem.GetAttribute("value");
            else
                return string.Empty;
        }

        /// <summary>
        /// 获取应用程序配置文件参数值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAppSettingsValue(string name)
        {
            //string value = null;
            //try
            //{
            //    value = ConfigurationManager.AppSettings[name];
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.StackTrace);
            //}

            //return value;
            return GetConfigValue(config_file, name);
        }

        /// <summary>
        /// 设置应用程序配置文件值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetAppSettingsValue(string name, string value)
        {
            //try
            //{
            //    //获取Configuration对象
            //    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //    //写入<add>元素的Value
            //    config.AppSettings.Settings[name].Value = value;
            //    config.Save();
            //    //刷新，否则程序读取的还是之前的值（可能已装入内存）
            //    ConfigurationManager.RefreshSection("appSettings");
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.StackTrace);
            //}
            SaveConfig(config_file, name, value);
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <param name="_FilePath"></param>
        /// <param name="_Key_Value"></param>
        /// <param name="str"></param>
        public static bool SaveConfig(string configPath, string strkey, string strValue)
        {
            bool b = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                //获得配置文件的全路径
                string strFileName = configPath.Trim();
                doc.Load(strFileName);
                //找出名称为“add”的所有元素
                XmlNodeList nodes = doc.GetElementsByTagName("add");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的key属性
                    XmlAttribute att = nodes[i].Attributes["key"];
                    //根据元素的第一个属性来判断当前的元素是不是目标元素
                    if (att != null && att.Value == strkey)
                    {
                        //对目标元素中的第二个属性赋值
                        att = nodes[i].Attributes["value"];
                        att.Value = strValue;
                        break;
                    }
                }
                //保存上面的修改
                doc.Save(strFileName);
                b = true;
                //StringWriter sw = new StringWriter();
                //XmlTextWriter xw = new XmlTextWriter(sw);
                //// Save Xml Document to Text Writter.
                //doc.WriteTo(xw);
                //string contents = sw.ToString();
                //File.WriteAllText(configPath,contents);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存配置信息出错:" + ex.Message.ToString());
                YEasyInstaller.Comm.Log.i(ex);
            }
            return b;
        }
    }
}
