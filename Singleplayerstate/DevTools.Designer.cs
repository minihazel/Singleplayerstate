namespace Singleplayerstate
{
    partial class DevTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DevTools));
            this.listAvailableInstallations = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listAutostart = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSetAsAutostartOption = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listAvailableInstallations
            // 
            this.listAvailableInstallations.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.listAvailableInstallations.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listAvailableInstallations.Font = new System.Drawing.Font("Bender", 14F);
            this.listAvailableInstallations.ForeColor = System.Drawing.Color.LightGray;
            this.listAvailableInstallations.FormattingEnabled = true;
            this.listAvailableInstallations.ItemHeight = 21;
            this.listAvailableInstallations.Location = new System.Drawing.Point(12, 45);
            this.listAvailableInstallations.Name = "listAvailableInstallations";
            this.listAvailableInstallations.Size = new System.Drawing.Size(293, 233);
            this.listAvailableInstallations.TabIndex = 0;
            this.listAvailableInstallations.SelectedIndexChanged += new System.EventHandler(this.listAvailableInstallations_SelectedIndexChanged);
            this.listAvailableInstallations.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listAvailableInstallations_KeyDown);
            this.listAvailableInstallations.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listAvailableInstallations_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(296, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available installations";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // listAutostart
            // 
            this.listAutostart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.listAutostart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listAutostart.Font = new System.Drawing.Font("Bender", 14F);
            this.listAutostart.ForeColor = System.Drawing.Color.LightGray;
            this.listAutostart.FormattingEnabled = true;
            this.listAutostart.ItemHeight = 21;
            this.listAutostart.Items.AddRange(new object[] {
            "autostart=false",
            "N/A"});
            this.listAutostart.Location = new System.Drawing.Point(311, 45);
            this.listAutostart.Name = "listAutostart";
            this.listAutostart.Size = new System.Drawing.Size(293, 44);
            this.listAutostart.TabIndex = 2;
            this.listAutostart.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listAutostart_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(308, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(296, 33);
            this.label2.TabIndex = 1;
            this.label2.Text = "Autostart";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSetAsAutostartOption
            // 
            this.btnSetAsAutostartOption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnSetAsAutostartOption.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSetAsAutostartOption.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnSetAsAutostartOption.FlatAppearance.BorderSize = 2;
            this.btnSetAsAutostartOption.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnSetAsAutostartOption.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnSetAsAutostartOption.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetAsAutostartOption.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnSetAsAutostartOption.ForeColor = System.Drawing.Color.MediumSlateBlue;
            this.btnSetAsAutostartOption.Location = new System.Drawing.Point(12, 284);
            this.btnSetAsAutostartOption.Name = "btnSetAsAutostartOption";
            this.btnSetAsAutostartOption.Size = new System.Drawing.Size(293, 40);
            this.btnSetAsAutostartOption.TabIndex = 22;
            this.btnSetAsAutostartOption.Text = "Set as autostart option";
            this.btnSetAsAutostartOption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSetAsAutostartOption.UseVisualStyleBackColor = false;
            this.btnSetAsAutostartOption.Click += new System.EventHandler(this.btnSetAsAutostartOption_Click);
            // 
            // DevTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(965, 561);
            this.Controls.Add(this.btnSetAsAutostartOption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listAutostart);
            this.Controls.Add(this.listAvailableInstallations);
            this.Font = new System.Drawing.Font("Bender", 10F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DevTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Developer tools";
            this.Load += new System.EventHandler(this.DevTools_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listAvailableInstallations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listAutostart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSetAsAutostartOption;
    }
}