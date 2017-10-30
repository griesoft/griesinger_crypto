namespace GriesingerCrypto
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.servername_txt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.username_txt = new System.Windows.Forms.TextBox();
            this.password_txt = new System.Windows.Forms.TextBox();
            this.file_radiobtn = new System.Windows.Forms.RadioButton();
            this.folder_radiobtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.findresource_btn = new System.Windows.Forms.Button();
            this.resourcePath_txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.encrypt_btn = new System.Windows.Forms.Button();
            this.decrypt_btn = new System.Windows.Forms.Button();
            this.createaccount_btn = new System.Windows.Forms.Button();
            this.output_txt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cryptoProgressBar = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // servername_txt
            // 
            resources.ApplyResources(this.servername_txt, "servername_txt");
            this.servername_txt.Name = "servername_txt";
            this.servername_txt.TextChanged += new System.EventHandler(this.servername_txt_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // username_txt
            // 
            resources.ApplyResources(this.username_txt, "username_txt");
            this.username_txt.Name = "username_txt";
            this.username_txt.TextChanged += new System.EventHandler(this.username_txt_TextChanged);
            // 
            // password_txt
            // 
            resources.ApplyResources(this.password_txt, "password_txt");
            this.password_txt.Name = "password_txt";
            this.password_txt.UseSystemPasswordChar = true;
            this.password_txt.TextChanged += new System.EventHandler(this.password_txt_TextChanged);
            // 
            // file_radiobtn
            // 
            resources.ApplyResources(this.file_radiobtn, "file_radiobtn");
            this.file_radiobtn.Checked = true;
            this.file_radiobtn.Name = "file_radiobtn";
            this.file_radiobtn.TabStop = true;
            this.file_radiobtn.UseVisualStyleBackColor = true;
            // 
            // folder_radiobtn
            // 
            resources.ApplyResources(this.folder_radiobtn, "folder_radiobtn");
            this.folder_radiobtn.Name = "folder_radiobtn";
            this.folder_radiobtn.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.folder_radiobtn);
            this.groupBox1.Controls.Add(this.file_radiobtn);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // findresource_btn
            // 
            resources.ApplyResources(this.findresource_btn, "findresource_btn");
            this.findresource_btn.Name = "findresource_btn";
            this.findresource_btn.UseVisualStyleBackColor = true;
            this.findresource_btn.Click += new System.EventHandler(this.findresource_btn_Click);
            // 
            // resourcePath_txt
            // 
            resources.ApplyResources(this.resourcePath_txt, "resourcePath_txt");
            this.resourcePath_txt.Name = "resourcePath_txt";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // encrypt_btn
            // 
            resources.ApplyResources(this.encrypt_btn, "encrypt_btn");
            this.encrypt_btn.Name = "encrypt_btn";
            this.encrypt_btn.UseVisualStyleBackColor = true;
            this.encrypt_btn.Click += new System.EventHandler(this.encrypt_btn_Click);
            // 
            // decrypt_btn
            // 
            resources.ApplyResources(this.decrypt_btn, "decrypt_btn");
            this.decrypt_btn.Name = "decrypt_btn";
            this.decrypt_btn.UseVisualStyleBackColor = true;
            this.decrypt_btn.Click += new System.EventHandler(this.decrypt_btn_Click);
            // 
            // createaccount_btn
            // 
            resources.ApplyResources(this.createaccount_btn, "createaccount_btn");
            this.createaccount_btn.Name = "createaccount_btn";
            this.createaccount_btn.UseVisualStyleBackColor = true;
            this.createaccount_btn.Click += new System.EventHandler(this.createaccount_btn_Click);
            // 
            // output_txt
            // 
            resources.ApplyResources(this.output_txt, "output_txt");
            this.output_txt.Name = "output_txt";
            this.output_txt.ReadOnly = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cryptoProgressBar
            // 
            resources.ApplyResources(this.cryptoProgressBar, "cryptoProgressBar");
            this.cryptoProgressBar.Name = "cryptoProgressBar";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cryptoProgressBar);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.output_txt);
            this.Controls.Add(this.createaccount_btn);
            this.Controls.Add(this.decrypt_btn);
            this.Controls.Add(this.encrypt_btn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.resourcePath_txt);
            this.Controls.Add(this.findresource_btn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.password_txt);
            this.Controls.Add(this.username_txt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.servername_txt);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox servername_txt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox username_txt;
        private System.Windows.Forms.TextBox password_txt;
        private System.Windows.Forms.RadioButton file_radiobtn;
        private System.Windows.Forms.RadioButton folder_radiobtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button findresource_btn;
        private System.Windows.Forms.TextBox resourcePath_txt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button encrypt_btn;
        private System.Windows.Forms.Button decrypt_btn;
        private System.Windows.Forms.Button createaccount_btn;
        private System.Windows.Forms.TextBox output_txt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar cryptoProgressBar;
    }
}