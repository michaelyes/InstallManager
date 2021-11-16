using System;
using System.Windows.Forms;

namespace YEasyInstaller
{
    public partial class FrmChooseIP : Form
    {
        public string IP = string.Empty;
        public int Port = 0;

        public FrmChooseIP()
        {
            InitializeComponent();
        }

        public FrmChooseIP(int Port)
        {
            InitializeComponent();
            
            this.Port = Port;
            txtPort.Text = this.Port.ToString();
            txtPort.ReadOnly = true;
            //if (HostUtils.IsPortOccuped(this.Port))
            //{
            //    txtPort.ReadOnly = false;
            //}
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            btnExit.Click += BtnExit_Click;
            btnOK.Click += BtnOK_Click;
            lblModifyPort.Click += LblModifyPort_Click;
            btnClose.Click += BtnClose_Click;

            cbxIPList.DataSource = HostUtils.IPActiveAddressList();
            cbxIPList.DisplayMember = "IP";
            cbxIPList.ValueMember = "IsDhcp";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.Abort;
        }

        private void LblModifyPort_Click(object sender, EventArgs e)
        {
            txtPort.ReadOnly = false;
            txtPort.SelectAll();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (!(bool)cbxIPList.SelectedValue)
            {
                if(!int.TryParse(txtPort.Text.Trim(), out Port) || Port <= 0)
                {
                    MessageBox.Show("端口号错误，请重新输入！");
                    return;
                }
                IP = cbxIPList.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("请选择静态IP");
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定忽略绑定IP，直接安装软件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                IP = "localhost";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
