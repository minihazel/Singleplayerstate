namespace Singleplayerstate
{
    partial class msgBoard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(msgBoard));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.messageContent = new System.Windows.Forms.Label();
            this.btnCopyMessage = new System.Windows.Forms.Button();
            this.messageTitle = new System.Windows.Forms.Label();
            this.statusCopy = new System.Windows.Forms.Label();
            this.appImage = new System.Windows.Forms.PictureBox();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnCancel.FlatAppearance.BorderSize = 2;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(428, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 45);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnOK.FlatAppearance.BorderSize = 2;
            this.btnOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnOK.ForeColor = System.Drawing.Color.Gray;
            this.btnOK.Location = new System.Drawing.Point(253, 233);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(150, 45);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.mainPanel.Controls.Add(this.appImage);
            this.mainPanel.Controls.Add(this.messageTitle);
            this.mainPanel.Controls.Add(this.messageContent);
            this.mainPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.mainPanel.Location = new System.Drawing.Point(1, 1);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(25, 25, 25, 15);
            this.mainPanel.Size = new System.Drawing.Size(598, 211);
            this.mainPanel.TabIndex = 8;
            this.mainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // messageContent
            // 
            this.messageContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageContent.Cursor = System.Windows.Forms.Cursors.Default;
            this.messageContent.Font = new System.Drawing.Font("Bender", 14F);
            this.messageContent.ForeColor = System.Drawing.Color.Silver;
            this.messageContent.Location = new System.Drawing.Point(28, 70);
            this.messageContent.Name = "messageContent";
            this.messageContent.Size = new System.Drawing.Size(542, 126);
            this.messageContent.TabIndex = 9;
            this.messageContent.Text = "PlaceholderText";
            this.messageContent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.messageContent_MouseDown);
            // 
            // btnCopyMessage
            // 
            this.btnCopyMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnCopyMessage.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCopyMessage.BackgroundImage")));
            this.btnCopyMessage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCopyMessage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCopyMessage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnCopyMessage.FlatAppearance.BorderSize = 0;
            this.btnCopyMessage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCopyMessage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCopyMessage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyMessage.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnCopyMessage.ForeColor = System.Drawing.Color.Gray;
            this.btnCopyMessage.Location = new System.Drawing.Point(191, 243);
            this.btnCopyMessage.Name = "btnCopyMessage";
            this.btnCopyMessage.Size = new System.Drawing.Size(25, 25);
            this.btnCopyMessage.TabIndex = 9;
            this.btnCopyMessage.UseVisualStyleBackColor = false;
            this.btnCopyMessage.Click += new System.EventHandler(this.btnCopyMessage_Click);
            this.btnCopyMessage.MouseEnter += new System.EventHandler(this.btnCopyMessage_MouseEnter);
            this.btnCopyMessage.MouseLeave += new System.EventHandler(this.btnCopyMessage_MouseLeave);
            // 
            // messageTitle
            // 
            this.messageTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.messageTitle.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.messageTitle.ForeColor = System.Drawing.Color.Gray;
            this.messageTitle.Location = new System.Drawing.Point(63, 25);
            this.messageTitle.Name = "messageTitle";
            this.messageTitle.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.messageTitle.Size = new System.Drawing.Size(507, 45);
            this.messageTitle.TabIndex = 10;
            this.messageTitle.Text = "PlaceholderTitle";
            // 
            // statusCopy
            // 
            this.statusCopy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusCopy.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusCopy.Font = new System.Drawing.Font("Bender", 11F);
            this.statusCopy.ForeColor = System.Drawing.Color.Gray;
            this.statusCopy.Location = new System.Drawing.Point(12, 243);
            this.statusCopy.Name = "statusCopy";
            this.statusCopy.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.statusCopy.Size = new System.Drawing.Size(173, 25);
            this.statusCopy.TabIndex = 11;
            this.statusCopy.Text = "✔️";
            this.statusCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusCopy.Visible = false;
            // 
            // appImage
            // 
            this.appImage.Location = new System.Drawing.Point(32, 25);
            this.appImage.Name = "appImage";
            this.appImage.Size = new System.Drawing.Size(25, 25);
            this.appImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.appImage.TabIndex = 12;
            this.appImage.TabStop = false;
            // 
            // msgBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(28)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(600, 300);
            this.Controls.Add(this.statusCopy);
            this.Controls.Add(this.btnCopyMessage);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Bender", 12F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(1, 1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "msgBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "msgBoard";
            this.Load += new System.EventHandler(this.msgBoard_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.msgBoard_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.msgBoard_MouseDown);
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel mainPanel;
        public System.Windows.Forms.Label messageContent;
        private System.Windows.Forms.Button btnCopyMessage;
        public System.Windows.Forms.Label messageTitle;
        public System.Windows.Forms.Label statusCopy;
        private System.Windows.Forms.PictureBox appImage;
    }
}