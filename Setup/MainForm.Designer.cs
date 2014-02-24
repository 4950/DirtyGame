namespace Setup
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
            this.DirBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Shorcut = new System.Windows.Forms.CheckBox();
            this.InstallBtn = new System.Windows.Forms.Button();
            this.ExitBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.StatusBar = new System.Windows.Forms.ProgressBar();
            this.StatusLbl = new System.Windows.Forms.Label();
            this.Browse = new System.Windows.Forms.FolderBrowserDialog();
            this.BrowseBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.LaunchBtn = new System.Windows.Forms.Button();
            this.Work = new System.ComponentModel.BackgroundWorker();
            this.UninstBtn = new System.Windows.Forms.Button();
            this.StartMenu = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DirBox
            // 
            this.DirBox.Location = new System.Drawing.Point(12, 68);
            this.DirBox.Name = "DirBox";
            this.DirBox.Size = new System.Drawing.Size(227, 20);
            this.DirBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Installation Directory:";
            // 
            // Shorcut
            // 
            this.Shorcut.AutoSize = true;
            this.Shorcut.Checked = true;
            this.Shorcut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Shorcut.Location = new System.Drawing.Point(12, 94);
            this.Shorcut.Name = "Shorcut";
            this.Shorcut.Size = new System.Drawing.Size(109, 17);
            this.Shorcut.TabIndex = 2;
            this.Shorcut.Text = "Desktop Shortcut";
            this.Shorcut.UseVisualStyleBackColor = true;
            // 
            // InstallBtn
            // 
            this.InstallBtn.Location = new System.Drawing.Point(12, 197);
            this.InstallBtn.Name = "InstallBtn";
            this.InstallBtn.Size = new System.Drawing.Size(75, 23);
            this.InstallBtn.TabIndex = 3;
            this.InstallBtn.Text = "Install";
            this.InstallBtn.UseVisualStyleBackColor = true;
            this.InstallBtn.Click += new System.EventHandler(this.InstallBtn_Click);
            // 
            // ExitBtn
            // 
            this.ExitBtn.Location = new System.Drawing.Point(197, 197);
            this.ExitBtn.Name = "ExitBtn";
            this.ExitBtn.Size = new System.Drawing.Size(75, 23);
            this.ExitBtn.TabIndex = 4;
            this.ExitBtn.Text = "Exit";
            this.ExitBtn.UseVisualStyleBackColor = true;
            this.ExitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(66, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Dirty Setup";
            // 
            // StatusBar
            // 
            this.StatusBar.Location = new System.Drawing.Point(12, 168);
            this.StatusBar.Maximum = 1000;
            this.StatusBar.Name = "StatusBar";
            this.StatusBar.Size = new System.Drawing.Size(260, 23);
            this.StatusBar.TabIndex = 6;
            // 
            // StatusLbl
            // 
            this.StatusLbl.AutoSize = true;
            this.StatusLbl.Location = new System.Drawing.Point(9, 152);
            this.StatusLbl.Name = "StatusLbl";
            this.StatusLbl.Size = new System.Drawing.Size(0, 13);
            this.StatusLbl.TabIndex = 7;
            // 
            // Browse
            // 
            this.Browse.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // BrowseBtn
            // 
            this.BrowseBtn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseBtn.Location = new System.Drawing.Point(245, 66);
            this.BrowseBtn.Name = "BrowseBtn";
            this.BrowseBtn.Size = new System.Drawing.Size(27, 23);
            this.BrowseBtn.TabIndex = 8;
            this.BrowseBtn.Text = "...";
            this.BrowseBtn.UseVisualStyleBackColor = true;
            this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Progress:";
            // 
            // LaunchBtn
            // 
            this.LaunchBtn.Location = new System.Drawing.Point(104, 226);
            this.LaunchBtn.Name = "LaunchBtn";
            this.LaunchBtn.Size = new System.Drawing.Size(75, 23);
            this.LaunchBtn.TabIndex = 10;
            this.LaunchBtn.Text = "Launch";
            this.LaunchBtn.UseVisualStyleBackColor = true;
            this.LaunchBtn.Visible = false;
            this.LaunchBtn.Click += new System.EventHandler(this.LaunchBtn_Click);
            // 
            // Work
            // 
            this.Work.WorkerReportsProgress = true;
            this.Work.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Work_DoWork);
            this.Work.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.Work_ProgressChanged);
            // 
            // UninstBtn
            // 
            this.UninstBtn.Location = new System.Drawing.Point(104, 197);
            this.UninstBtn.Name = "UninstBtn";
            this.UninstBtn.Size = new System.Drawing.Size(75, 23);
            this.UninstBtn.TabIndex = 11;
            this.UninstBtn.Text = "Uninstall";
            this.UninstBtn.Visible = false;
            this.UninstBtn.Click += new System.EventHandler(this.UninstBtn_Click);
            // 
            // StartMenu
            // 
            this.StartMenu.AutoSize = true;
            this.StartMenu.Checked = true;
            this.StartMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.StartMenu.Location = new System.Drawing.Point(12, 117);
            this.StartMenu.Name = "StartMenu";
            this.StartMenu.Size = new System.Drawing.Size(78, 17);
            this.StartMenu.TabIndex = 12;
            this.StartMenu.Text = "Start Menu";
            this.StartMenu.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 258);
            this.Controls.Add(this.StartMenu);
            this.Controls.Add(this.UninstBtn);
            this.Controls.Add(this.LaunchBtn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BrowseBtn);
            this.Controls.Add(this.StatusLbl);
            this.Controls.Add(this.StatusBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ExitBtn);
            this.Controls.Add(this.InstallBtn);
            this.Controls.Add(this.Shorcut);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DirBox);
            this.Name = "MainForm";
            this.Text = "Dirty Install";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DirBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Shorcut;
        private System.Windows.Forms.Button InstallBtn;
        private System.Windows.Forms.Button ExitBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar StatusBar;
        private System.Windows.Forms.Label StatusLbl;
        private System.Windows.Forms.FolderBrowserDialog Browse;
        private System.Windows.Forms.Button BrowseBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button LaunchBtn;
        private System.ComponentModel.BackgroundWorker Work;
        private System.Windows.Forms.Button UninstBtn;
        private System.Windows.Forms.CheckBox StartMenu;
    }
}

