namespace Singleplayerstate
{
    partial class EditLocalhost
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditLocalhost));
            this.panelIPString = new System.Windows.Forms.Panel();
            this.txtIPString = new System.Windows.Forms.TextBox();
            this.titleNotice = new System.Windows.Forms.Label();
            this.panelIPString.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIPString
            // 
            this.panelIPString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelIPString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.panelIPString.Controls.Add(this.txtIPString);
            this.panelIPString.Location = new System.Drawing.Point(12, 12);
            this.panelIPString.Name = "panelIPString";
            this.panelIPString.Size = new System.Drawing.Size(438, 40);
            this.panelIPString.TabIndex = 21;
            // 
            // txtIPString
            // 
            this.txtIPString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIPString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.txtIPString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPString.Font = new System.Drawing.Font("Bender", 14F, System.Drawing.FontStyle.Bold);
            this.txtIPString.ForeColor = System.Drawing.Color.Gray;
            this.txtIPString.Location = new System.Drawing.Point(2, 2);
            this.txtIPString.Multiline = true;
            this.txtIPString.Name = "txtIPString";
            this.txtIPString.Size = new System.Drawing.Size(434, 36);
            this.txtIPString.TabIndex = 0;
            this.txtIPString.Text = "Placeholder";
            this.txtIPString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtIPString_KeyDown);
            // 
            // titleNotice
            // 
            this.titleNotice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleNotice.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.titleNotice.ForeColor = System.Drawing.Color.Gray;
            this.titleNotice.Location = new System.Drawing.Point(12, 57);
            this.titleNotice.Name = "titleNotice";
            this.titleNotice.Size = new System.Drawing.Size(438, 20);
            this.titleNotice.TabIndex = 31;
            this.titleNotice.Text = "Type \'reset\' or close the window to cancel";
            this.titleNotice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // EditLocalhost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(462, 86);
            this.Controls.Add(this.titleNotice);
            this.Controls.Add(this.panelIPString);
            this.Font = new System.Drawing.Font("Bahnschrift Light", 10F);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditLocalhost";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Edit IP address parameter";
            this.Load += new System.EventHandler(this.EditLocalhost_Load);
            this.panelIPString.ResumeLayout(false);
            this.panelIPString.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelIPString;
        private System.Windows.Forms.TextBox txtIPString;
        private System.Windows.Forms.Label titleNotice;
    }
}