namespace STCT.DBInstall
{
    partial class DBinstall
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBinstall));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCreateData = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.Save_location = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDB_Pass = new System.Windows.Forms.TextBox();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.txtDB_user = new System.Windows.Forms.TextBox();
            this.txtDB_Name = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(748, 464);
            this.tabControl1.TabIndex = 101;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Controls.Add(this.btnTest);
            this.tabPage1.Controls.Add(this.Save_location);
            this.tabPage1.Controls.Add(this.txtPath);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtDB_Pass);
            this.tabPage1.Controls.Add(this.txtServerName);
            this.tabPage1.Controls.Add(this.txtDB_user);
            this.tabPage1.Controls.Add(this.txtDB_Name);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(740, 432);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "创建&更新数据库";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnUpgrade);
            this.groupBox2.ForeColor = System.Drawing.Color.Green;
            this.groupBox2.Location = new System.Drawing.Point(470, 330);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(202, 87);
            this.groupBox2.TabIndex = 107;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "已安装，更新旧库";
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Font = new System.Drawing.Font("宋体", 10F);
            this.btnUpgrade.ForeColor = System.Drawing.Color.Green;
            this.btnUpgrade.Location = new System.Drawing.Point(34, 31);
            this.btnUpgrade.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(135, 40);
            this.btnUpgrade.TabIndex = 105;
            this.btnUpgrade.Text = "升级数据库";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCreateData);
            this.groupBox1.Location = new System.Drawing.Point(218, 330);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(204, 87);
            this.groupBox1.TabIndex = 106;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "未安装，创建新库";
            // 
            // btnCreateData
            // 
            this.btnCreateData.Font = new System.Drawing.Font("宋体", 10F);
            this.btnCreateData.Location = new System.Drawing.Point(37, 31);
            this.btnCreateData.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateData.Name = "btnCreateData";
            this.btnCreateData.Size = new System.Drawing.Size(135, 40);
            this.btnCreateData.TabIndex = 103;
            this.btnCreateData.Text = "创建数据库";
            this.btnCreateData.UseVisualStyleBackColor = true;
            this.btnCreateData.Click += new System.EventHandler(this.btnCreateData_Click);
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("宋体", 10F);
            this.btnTest.Location = new System.Drawing.Point(57, 361);
            this.btnTest.Margin = new System.Windows.Forms.Padding(4);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(135, 40);
            this.btnTest.TabIndex = 102;
            this.btnTest.Text = "检测连接";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // Save_location
            // 
            this.Save_location.Font = new System.Drawing.Font("宋体", 10F);
            this.Save_location.Location = new System.Drawing.Point(562, 243);
            this.Save_location.Margin = new System.Windows.Forms.Padding(4);
            this.Save_location.Name = "Save_location";
            this.Save_location.Size = new System.Drawing.Size(110, 35);
            this.Save_location.TabIndex = 104;
            this.Save_location.Text = "存放位置";
            this.Save_location.UseVisualStyleBackColor = true;
            this.Save_location.Click += new System.EventHandler(this.Save_location_Click);
            // 
            // txtPath
            // 
            this.txtPath.Font = new System.Drawing.Font("宋体", 12F);
            this.txtPath.Location = new System.Drawing.Point(218, 243);
            this.txtPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(286, 35);
            this.txtPath.TabIndex = 104;
            this.txtPath.Text = "D:\\STDB";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.Location = new System.Drawing.Point(64, 247);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 24);
            this.label5.TabIndex = 103;
            this.label5.Text = "存放路径：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(57, 301);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(615, 10);
            this.progressBar1.TabIndex = 102;
            this.progressBar1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(88, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 24);
            this.label1.TabIndex = 99;
            this.label1.Text = "服务器：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(88, 143);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 99;
            this.label2.Text = "用户名：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(88, 87);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 24);
            this.label4.TabIndex = 99;
            this.label4.Text = "数据库：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(100, 195);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 24);
            this.label3.TabIndex = 99;
            this.label3.Text = "密 码：";
            // 
            // txtDB_Pass
            // 
            this.txtDB_Pass.Font = new System.Drawing.Font("宋体", 12F);
            this.txtDB_Pass.Location = new System.Drawing.Point(218, 189);
            this.txtDB_Pass.Margin = new System.Windows.Forms.Padding(4);
            this.txtDB_Pass.Name = "txtDB_Pass";
            this.txtDB_Pass.PasswordChar = '*';
            this.txtDB_Pass.Size = new System.Drawing.Size(286, 35);
            this.txtDB_Pass.TabIndex = 4;
            this.txtDB_Pass.Text = "1";
            // 
            // txtServerName
            // 
            this.txtServerName.Font = new System.Drawing.Font("宋体", 12F);
            this.txtServerName.Location = new System.Drawing.Point(218, 27);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(4);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(286, 35);
            this.txtServerName.TabIndex = 1;
            this.txtServerName.Text = "(local)";
            // 
            // txtDB_user
            // 
            this.txtDB_user.Font = new System.Drawing.Font("宋体", 12F);
            this.txtDB_user.Location = new System.Drawing.Point(218, 135);
            this.txtDB_user.Margin = new System.Windows.Forms.Padding(4);
            this.txtDB_user.Name = "txtDB_user";
            this.txtDB_user.Size = new System.Drawing.Size(286, 35);
            this.txtDB_user.TabIndex = 3;
            this.txtDB_user.Text = "sa";
            // 
            // txtDB_Name
            // 
            this.txtDB_Name.Font = new System.Drawing.Font("宋体", 12F);
            this.txtDB_Name.Location = new System.Drawing.Point(218, 81);
            this.txtDB_Name.Margin = new System.Windows.Forms.Padding(4);
            this.txtDB_Name.Name = "txtDB_Name";
            this.txtDB_Name.Size = new System.Drawing.Size(286, 35);
            this.txtDB_Name.TabIndex = 2;
            this.txtDB_Name.Text = "STDB";
            // 
            // DBinstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 464);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DBinstall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库安装";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDB_Pass;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.TextBox txtDB_user;
        private System.Windows.Forms.TextBox txtDB_Name;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnCreateData;
        private System.Windows.Forms.Button Save_location;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}