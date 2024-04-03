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
            this.messageContent = new System.Windows.Forms.RichTextBox();
            this.appImage = new System.Windows.Forms.PictureBox();
            this.messageTitle = new System.Windows.Forms.Label();
            this.btnCopyMessage = new System.Windows.Forms.Button();
            this.statusCopy = new System.Windows.Forms.Label();
            this.btnLevelOne = new System.Windows.Forms.Button();
            this.btnLevelTwo = new System.Windows.Forms.Button();
            this.btnLevelThree = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appImage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnCancel.FlatAppearance.BorderSize = 2;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Gray;
            this.btnCancel.Location = new System.Drawing.Point(528, 333);
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
            this.btnOK.Location = new System.Drawing.Point(363, 333);
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
            this.mainPanel.AutoScroll = true;
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.mainPanel.Controls.Add(this.messageContent);
            this.mainPanel.Controls.Add(this.appImage);
            this.mainPanel.Controls.Add(this.messageTitle);
            this.mainPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.mainPanel.Location = new System.Drawing.Point(1, 1);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(25, 25, 25, 15);
            this.mainPanel.Size = new System.Drawing.Size(698, 311);
            this.mainPanel.TabIndex = 8;
            this.mainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mainPanel_MouseDown);
            // 
            // messageContent
            // 
            this.messageContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(24)))), ((int)(((byte)(26)))));
            this.messageContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.messageContent.Font = new System.Drawing.Font("Bender", 14F);
            this.messageContent.ForeColor = System.Drawing.Color.Silver;
            this.messageContent.Location = new System.Drawing.Point(30, 70);
            this.messageContent.Name = "messageContent";
            this.messageContent.ReadOnly = true;
            this.messageContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.messageContent.Size = new System.Drawing.Size(638, 226);
            this.messageContent.TabIndex = 13;
            this.messageContent.Text = "Placeholder";
            this.messageContent.MouseUp += new System.Windows.Forms.MouseEventHandler(this.messageContent_MouseUp);
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
            this.messageTitle.Size = new System.Drawing.Size(607, 45);
            this.messageTitle.TabIndex = 10;
            this.messageTitle.Text = "PlaceholderTitle";
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
            this.btnCopyMessage.Location = new System.Drawing.Point(302, 343);
            this.btnCopyMessage.Name = "btnCopyMessage";
            this.btnCopyMessage.Size = new System.Drawing.Size(25, 25);
            this.btnCopyMessage.TabIndex = 9;
            this.btnCopyMessage.UseVisualStyleBackColor = false;
            this.btnCopyMessage.Click += new System.EventHandler(this.btnCopyMessage_Click);
            this.btnCopyMessage.MouseEnter += new System.EventHandler(this.btnCopyMessage_MouseEnter);
            this.btnCopyMessage.MouseLeave += new System.EventHandler(this.btnCopyMessage_MouseLeave);
            // 
            // statusCopy
            // 
            this.statusCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.statusCopy.AutoSize = true;
            this.statusCopy.Cursor = System.Windows.Forms.Cursors.Default;
            this.statusCopy.Font = new System.Drawing.Font("Bender", 11F);
            this.statusCopy.ForeColor = System.Drawing.Color.Gray;
            this.statusCopy.Location = new System.Drawing.Point(262, 347);
            this.statusCopy.Name = "statusCopy";
            this.statusCopy.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.statusCopy.Size = new System.Drawing.Size(34, 17);
            this.statusCopy.TabIndex = 11;
            this.statusCopy.Text = "✔️";
            this.statusCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.statusCopy.Visible = false;
            // 
            // btnLevelOne
            // 
            this.btnLevelOne.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLevelOne.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnLevelOne.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLevelOne.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnLevelOne.FlatAppearance.BorderSize = 2;
            this.btnLevelOne.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelOne.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelOne.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLevelOne.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnLevelOne.ForeColor = System.Drawing.Color.Gray;
            this.btnLevelOne.Location = new System.Drawing.Point(12, 333);
            this.btnLevelOne.Name = "btnLevelOne";
            this.btnLevelOne.Size = new System.Drawing.Size(100, 10);
            this.btnLevelOne.TabIndex = 12;
            this.btnLevelOne.UseVisualStyleBackColor = false;
            this.btnLevelOne.Visible = false;
            this.btnLevelOne.Click += new System.EventHandler(this.btnLevelOne_Click);
            // 
            // btnLevelTwo
            // 
            this.btnLevelTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLevelTwo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnLevelTwo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLevelTwo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnLevelTwo.FlatAppearance.BorderSize = 2;
            this.btnLevelTwo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelTwo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelTwo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLevelTwo.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnLevelTwo.ForeColor = System.Drawing.Color.Gray;
            this.btnLevelTwo.Location = new System.Drawing.Point(12, 350);
            this.btnLevelTwo.Name = "btnLevelTwo";
            this.btnLevelTwo.Size = new System.Drawing.Size(100, 10);
            this.btnLevelTwo.TabIndex = 13;
            this.btnLevelTwo.UseVisualStyleBackColor = false;
            this.btnLevelTwo.Visible = false;
            this.btnLevelTwo.Click += new System.EventHandler(this.btnLevelTwo_Click);
            // 
            // btnLevelThree
            // 
            this.btnLevelThree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLevelThree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.btnLevelThree.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLevelThree.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(182)))), ((int)(((byte)(178)))), ((int)(((byte)(158)))));
            this.btnLevelThree.FlatAppearance.BorderSize = 2;
            this.btnLevelThree.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelThree.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(31)))), ((int)(((byte)(31)))));
            this.btnLevelThree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLevelThree.Font = new System.Drawing.Font("Bender", 11F, System.Drawing.FontStyle.Bold);
            this.btnLevelThree.ForeColor = System.Drawing.Color.Gray;
            this.btnLevelThree.Location = new System.Drawing.Point(12, 368);
            this.btnLevelThree.Name = "btnLevelThree";
            this.btnLevelThree.Size = new System.Drawing.Size(100, 10);
            this.btnLevelThree.TabIndex = 14;
            this.btnLevelThree.UseVisualStyleBackColor = false;
            this.btnLevelThree.Visible = false;
            this.btnLevelThree.Click += new System.EventHandler(this.btnLevelThree_Click);
            // 
            // msgBoard
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(28)))), ((int)(((byte)(30)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(700, 400);
            this.Controls.Add(this.btnLevelThree);
            this.Controls.Add(this.btnLevelTwo);
            this.Controls.Add(this.btnLevelOne);
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
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(1, 1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "msgBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.msgBoard_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.msgBoard_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.msgBoard_MouseDown);
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button btnCopyMessage;
        public System.Windows.Forms.Label messageTitle;
        public System.Windows.Forms.Label statusCopy;
        private System.Windows.Forms.PictureBox appImage;
        private System.Windows.Forms.Button btnLevelOne;
        private System.Windows.Forms.Button btnLevelTwo;
        private System.Windows.Forms.Button btnLevelThree;
        public System.Windows.Forms.RichTextBox messageContent;
    }
}