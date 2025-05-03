namespace Singleplayerstate
{
    partial class AdjustFikaSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdjustFikaSettings));
            this.titleNotice = new System.Windows.Forms.Label();
            this.panelHostIP = new System.Windows.Forms.Panel();
            this.txtIPString = new System.Windows.Forms.TextBox();
            this.ponelHostPort = new System.Windows.Forms.Panel();
            this.txtPortString = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancelAction = new System.Windows.Forms.Button();
            this.btnConfirmSettings = new System.Windows.Forms.Button();
            this.panelHostIP.SuspendLayout();
            this.ponelHostPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleNotice
            // 
            this.titleNotice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleNotice.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.titleNotice.ForeColor = System.Drawing.Color.Gray;
            this.titleNotice.Location = new System.Drawing.Point(12, 25);
            this.titleNotice.Name = "titleNotice";
            this.titleNotice.Size = new System.Drawing.Size(420, 20);
            this.titleNotice.TabIndex = 32;
            this.titleNotice.Text = "Fika host IP address";
            this.titleNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelHostIP
            // 
            this.panelHostIP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelHostIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.panelHostIP.Controls.Add(this.txtIPString);
            this.panelHostIP.Location = new System.Drawing.Point(15, 48);
            this.panelHostIP.Name = "panelHostIP";
            this.panelHostIP.Size = new System.Drawing.Size(417, 40);
            this.panelHostIP.TabIndex = 33;
            // 
            // txtIPString
            // 
            this.txtIPString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIPString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.txtIPString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPString.Font = new System.Drawing.Font("Bender", 14F, System.Drawing.FontStyle.Bold);
            this.txtIPString.ForeColor = System.Drawing.Color.DarkGray;
            this.txtIPString.Location = new System.Drawing.Point(2, 2);
            this.txtIPString.Multiline = true;
            this.txtIPString.Name = "txtIPString";
            this.txtIPString.Size = new System.Drawing.Size(413, 36);
            this.txtIPString.TabIndex = 0;
            this.txtIPString.Text = "Placeholder";
            this.txtIPString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPString_KeyDown);
            // 
            // ponelHostPort
            // 
            this.ponelHostPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ponelHostPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.ponelHostPort.Controls.Add(this.txtPortString);
            this.ponelHostPort.Location = new System.Drawing.Point(17, 123);
            this.ponelHostPort.Name = "ponelHostPort";
            this.ponelHostPort.Size = new System.Drawing.Size(417, 40);
            this.ponelHostPort.TabIndex = 35;
            // 
            // txtPortString
            // 
            this.txtPortString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPortString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.txtPortString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPortString.Font = new System.Drawing.Font("Bender", 14F, System.Drawing.FontStyle.Bold);
            this.txtPortString.ForeColor = System.Drawing.Color.DarkGray;
            this.txtPortString.Location = new System.Drawing.Point(2, 2);
            this.txtPortString.Multiline = true;
            this.txtPortString.Name = "txtPortString";
            this.txtPortString.Size = new System.Drawing.Size(413, 36);
            this.txtPortString.TabIndex = 0;
            this.txtPortString.Text = "Placeholder";
            this.txtPortString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPortString_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(14, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(420, 20);
            this.label1.TabIndex = 34;
            this.label1.Text = "Fika host port (DON\'T CHANGE UNLESS TOLD TO)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCancelAction
            // 
            this.btnCancelAction.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnCancelAction.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelAction.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnCancelAction.FlatAppearance.BorderSize = 2;
            this.btnCancelAction.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancelAction.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancelAction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelAction.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancelAction.ForeColor = System.Drawing.Color.Brown;
            this.btnCancelAction.Location = new System.Drawing.Point(254, 207);
            this.btnCancelAction.Name = "btnCancelAction";
            this.btnCancelAction.Size = new System.Drawing.Size(180, 40);
            this.btnCancelAction.TabIndex = 36;
            this.btnCancelAction.Text = "Cancel";
            this.btnCancelAction.UseVisualStyleBackColor = false;
            this.btnCancelAction.Click += new System.EventHandler(this.btnCancelAction_Click);
            // 
            // btnConfirmSettings
            // 
            this.btnConfirmSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnConfirmSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnConfirmSettings.FlatAppearance.BorderSize = 2;
            this.btnConfirmSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnConfirmSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnConfirmSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmSettings.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfirmSettings.ForeColor = System.Drawing.Color.SeaGreen;
            this.btnConfirmSettings.Location = new System.Drawing.Point(68, 207);
            this.btnConfirmSettings.Name = "btnConfirmSettings";
            this.btnConfirmSettings.Size = new System.Drawing.Size(180, 40);
            this.btnConfirmSettings.TabIndex = 37;
            this.btnConfirmSettings.Text = "Apply settings";
            this.btnConfirmSettings.UseVisualStyleBackColor = false;
            this.btnConfirmSettings.Click += new System.EventHandler(this.btnConfirmSettings_Click);
            // 
            // AdjustFikaSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(462, 259);
            this.Controls.Add(this.btnConfirmSettings);
            this.Controls.Add(this.btnCancelAction);
            this.Controls.Add(this.ponelHostPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelHostIP);
            this.Controls.Add(this.titleNotice);
            this.Font = new System.Drawing.Font("Bahnschrift Light", 10F);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdjustFikaSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adjust Fika host connection settings";
            this.Load += new System.EventHandler(this.AdjustFikaSettings_Load);
            this.panelHostIP.ResumeLayout(false);
            this.panelHostIP.PerformLayout();
            this.ponelHostPort.ResumeLayout(false);
            this.ponelHostPort.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label titleNotice;
        private System.Windows.Forms.Panel panelHostIP;
        private System.Windows.Forms.TextBox txtIPString;
        private System.Windows.Forms.Panel ponelHostPort;
        private System.Windows.Forms.TextBox txtPortString;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancelAction;
        private System.Windows.Forms.Button btnConfirmSettings;
    }
}