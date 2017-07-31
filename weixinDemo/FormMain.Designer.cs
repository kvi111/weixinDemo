namespace weixinDemo
{
    partial class FormMain
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
            this.listBoxContact = new System.Windows.Forms.ListBox();
            this.textBoxHistory = new System.Windows.Forms.TextBox();
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblNickName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxContact
            // 
            this.listBoxContact.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listBoxContact.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.listBoxContact.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxContact.FormattingEnabled = true;
            this.listBoxContact.ItemHeight = 12;
            this.listBoxContact.Location = new System.Drawing.Point(2, 1);
            this.listBoxContact.Name = "listBoxContact";
            this.listBoxContact.Size = new System.Drawing.Size(221, 696);
            this.listBoxContact.Sorted = true;
            this.listBoxContact.TabIndex = 0;
            this.listBoxContact.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBoxContact_MeasureItem);
            this.listBoxContact.SelectedIndexChanged += new System.EventHandler(this.listBoxContact_SelectedIndexChanged);
            // 
            // textBoxHistory
            // 
            this.textBoxHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHistory.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.textBoxHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxHistory.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBoxHistory.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxHistory.ForeColor = System.Drawing.Color.Gray;
            this.textBoxHistory.Location = new System.Drawing.Point(229, 30);
            this.textBoxHistory.Multiline = true;
            this.textBoxHistory.Name = "textBoxHistory";
            this.textBoxHistory.ReadOnly = true;
            this.textBoxHistory.Size = new System.Drawing.Size(759, 392);
            this.textBoxHistory.TabIndex = 1;
            // 
            // textBoxInput
            // 
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInput.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxInput.Location = new System.Drawing.Point(229, 428);
            this.textBoxInput.Multiline = true;
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(759, 273);
            this.textBoxInput.TabIndex = 2;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BackColor = System.Drawing.Color.White;
            this.btnSend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSend.Location = new System.Drawing.Point(897, 664);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(79, 28);
            this.btnSend.TabIndex = 3;
            this.btnSend.Text = "发 送(S)";
            this.btnSend.UseVisualStyleBackColor = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblNickName
            // 
            this.lblNickName.AutoSize = true;
            this.lblNickName.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNickName.Location = new System.Drawing.Point(229, 9);
            this.lblNickName.Name = "lblNickName";
            this.lblNickName.Size = new System.Drawing.Size(21, 20);
            this.lblNickName.TabIndex = 4;
            this.lblNickName.Text = "   ";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ClientSize = new System.Drawing.Size(988, 704);
            this.Controls.Add(this.lblNickName);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.textBoxInput);
            this.Controls.Add(this.textBoxHistory);
            this.Controls.Add(this.listBoxContact);
            this.Name = "FormMain";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxContact;
        private System.Windows.Forms.TextBox textBoxHistory;
        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblNickName;
    }
}