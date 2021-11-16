using System;
using System.Configuration;
using System.Windows.Forms;

namespace STCT.DBInstall
{
    public partial class FrmConnStringSetting : Form
    {
        public FrmConnStringSetting()
        {
            InitializeComponent();
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            btnOK.Click += BtnOK_Click;
            btnExit.Click += (ss, ee) => 
            {
                this.Close();
            };

            #region 数据库设置

            string[] Datebaseconfig = null;
            try
            {
                Datebaseconfig = GetAppSettingsValue("ConnectionString").Split(';');
            }
            catch (Exception ex)
            {
                Datebaseconfig = new string[4] { "a=", "b=", "c=", "d=" };
            }

            txtServerName.Text = Datebaseconfig[0].Substring(Datebaseconfig[0].IndexOf("=") + 1);
            txtDB_Name.Text = Datebaseconfig[1].Substring(Datebaseconfig[1].IndexOf("=") + 1);
            txtDB_user.Text = Datebaseconfig[2].Substring(Datebaseconfig[2].IndexOf("=") + 1);
            txtDB_Pass.Text = Datebaseconfig[3].Substring(Datebaseconfig[3].IndexOf("=") + 1);

            #endregion
        }

        public string connectionStr = string.Empty;

        private void BtnOK_Click(object sender, EventArgs e)
        {
            connectionStr = "server=" + txtServerName.Text.Trim() + ";database=" + txtDB_Name.Text.Trim()
                + ";uid=" + txtDB_user.Text.Trim() + ";pwd=" + txtDB_Pass.Text.Trim() + ";";

            CreateDB.connectionStr = connectionStr;
            if (new CreateDB().IsConnection())
            {
                SetAppSettingsValue("ConnectionString", connectionStr);
                SetAppSettingsValue("DatabaseName", this.txtDB_Name.Text.Trim());
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if (MessageBox.Show("当前数据库配置测试连接不通，可能有误的。确定是否配置？", "警告", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    this.Close();
                    this.DialogResult = DialogResult.OK;
                }
            }
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

    }
}
