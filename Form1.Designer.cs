namespace yysls_token
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            btnToggle = new Button();
            lblStatus = new Label();
            txtToken = new TextBox();
            btnCopy = new Button();
            lblAuthor = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(217, 197, 138);
            lblTitle.Location = new Point(0, 15);
            lblTitle.Margin = new Padding(2, 0, 2, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(436, 34);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "燕云Token获取小工具";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnToggle
            // 
            btnToggle.BackColor = Color.FromArgb(46, 59, 78);
            btnToggle.Cursor = Cursors.Hand;
            btnToggle.FlatAppearance.BorderColor = Color.FromArgb(82, 100, 128);
            btnToggle.FlatStyle = FlatStyle.Flat;
            btnToggle.Font = new Font("Microsoft YaHei UI", 11.25F, FontStyle.Bold);
            btnToggle.ForeColor = Color.White;
            btnToggle.Location = new Point(140, 62);
            btnToggle.Margin = new Padding(2, 3, 2, 3);
            btnToggle.Name = "btnToggle";
            btnToggle.Size = new Size(156, 47);
            btnToggle.TabIndex = 1;
            btnToggle.Text = "开始捕获";
            btnToggle.UseVisualStyleBackColor = false;
            btnToggle.Click += BtnToggle_Click;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Microsoft YaHei UI", 9F);
            lblStatus.ForeColor = Color.FromArgb(184, 190, 201);
            lblStatus.Location = new Point(31, 128);
            lblStatus.Margin = new Padding(2, 0, 2, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(373, 94);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "正在检测 PC 版微信进程…\r\n检测到微信后将自动设置系统代理、启动捕获并拉起微信小程序。";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtToken
            // 
            txtToken.BackColor = Color.FromArgb(14, 17, 22);
            txtToken.BorderStyle = BorderStyle.FixedSingle;
            txtToken.Font = new Font("Consolas", 10.5F);
            txtToken.ForeColor = Color.FromArgb(130, 224, 168);
            txtToken.Location = new Point(31, 234);
            txtToken.Margin = new Padding(2, 3, 2, 3);
            txtToken.Multiline = true;
            txtToken.Name = "txtToken";
            txtToken.ReadOnly = true;
            txtToken.ScrollBars = ScrollBars.Vertical;
            txtToken.Size = new Size(374, 74);
            txtToken.TabIndex = 3;
            txtToken.Visible = false;
            txtToken.MouseClick += TxtToken_MouseClick;
            // 
            // btnCopy
            // 
            btnCopy.BackColor = Color.FromArgb(47, 93, 70);
            btnCopy.Cursor = Cursors.Hand;
            btnCopy.FlatAppearance.BorderColor = Color.FromArgb(74, 138, 102);
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
            btnCopy.ForeColor = Color.White;
            btnCopy.Location = new Point(156, 321);
            btnCopy.Margin = new Padding(2, 3, 2, 3);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(124, 42);
            btnCopy.TabIndex = 4;
            btnCopy.Text = "复制 Token";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Visible = false;
            btnCopy.Click += BtnCopy_Click;
            // 
            // lblAuthor
            // 
            lblAuthor.Font = new Font("Microsoft YaHei UI", 8F);
            lblAuthor.ForeColor = Color.FromArgb(120, 126, 138);
            lblAuthor.Location = new Point(0, 373);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(436, 20);
            lblAuthor.TabIndex = 5;
            lblAuthor.Text = "destiny.cool";
            lblAuthor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(20, 22, 27);
            ClientSize = new Size(436, 397);
            Controls.Add(lblAuthor);
            Controls.Add(btnCopy);
            Controls.Add(txtToken);
            Controls.Add(lblStatus);
            Controls.Add(btnToggle);
            Controls.Add(lblTitle);
            Font = new Font("Microsoft YaHei UI", 9F);
            ForeColor = Color.FromArgb(230, 230, 230);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2, 3, 2, 3);
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "燕云Token获取小工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnToggle;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Label lblAuthor;
    }
}
