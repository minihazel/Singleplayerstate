namespace Singleplayerstate
{
    partial class searchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(searchForm));
            this.panelSearchString = new System.Windows.Forms.Panel();
            this.txtSearchString = new System.Windows.Forms.TextBox();
            this.panelSearchString.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSearchString
            // 
            this.panelSearchString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.panelSearchString.Controls.Add(this.txtSearchString);
            this.panelSearchString.Location = new System.Drawing.Point(12, 12);
            this.panelSearchString.Name = "panelSearchString";
            this.panelSearchString.Size = new System.Drawing.Size(438, 40);
            this.panelSearchString.TabIndex = 20;
            // 
            // txtSearchString
            // 
            this.txtSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchString.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.txtSearchString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearchString.Font = new System.Drawing.Font("Bender", 14F, System.Drawing.FontStyle.Bold);
            this.txtSearchString.ForeColor = System.Drawing.Color.Gray;
            this.txtSearchString.Location = new System.Drawing.Point(2, 2);
            this.txtSearchString.Multiline = true;
            this.txtSearchString.Name = "txtSearchString";
            this.txtSearchString.Size = new System.Drawing.Size(434, 36);
            this.txtSearchString.TabIndex = 0;
            this.txtSearchString.Text = "Placeholder";
            this.txtSearchString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchString_KeyDown);
            // 
            // searchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(27)))), ((int)(((byte)(29)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(462, 64);
            this.Controls.Add(this.panelSearchString);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "searchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search workshop item";
            this.Load += new System.EventHandler(this.searchForm_Load);
            this.panelSearchString.ResumeLayout(false);
            this.panelSearchString.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSearchString;
        private System.Windows.Forms.TextBox txtSearchString;
    }
}