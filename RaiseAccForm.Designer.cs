namespace Tool_TikTok
{
	partial class RaiseAccForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RaiseAccForm));
			this.ckbTym = new System.Windows.Forms.CheckBox();
			this.ckbComment = new System.Windows.Forms.CheckBox();
			this.txtCommentContent = new System.Windows.Forms.RichTextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.txtTimeInteract = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.txtTimeInteract)).BeginInit();
			this.SuspendLayout();
			// 
			// ckbTym
			// 
			this.ckbTym.AutoSize = true;
			this.ckbTym.Location = new System.Drawing.Point(15, 60);
			this.ckbTym.Name = "ckbTym";
			this.ckbTym.Size = new System.Drawing.Size(46, 17);
			this.ckbTym.TabIndex = 1;
			this.ckbTym.Text = "Tym";
			this.ckbTym.UseVisualStyleBackColor = true;
			// 
			// ckbComment
			// 
			this.ckbComment.AutoSize = true;
			this.ckbComment.Location = new System.Drawing.Point(15, 121);
			this.ckbComment.Name = "ckbComment";
			this.ckbComment.Size = new System.Drawing.Size(70, 17);
			this.ckbComment.TabIndex = 2;
			this.ckbComment.Text = "Comment";
			this.ckbComment.UseVisualStyleBackColor = true;
			// 
			// txtCommentContent
			// 
			this.txtCommentContent.Location = new System.Drawing.Point(12, 179);
			this.txtCommentContent.Name = "txtCommentContent";
			this.txtCommentContent.Size = new System.Drawing.Size(279, 96);
			this.txtCommentContent.TabIndex = 4;
			this.txtCommentContent.Text = "";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(182, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Tổng Thời Gian Tương Tác Mỗi Acc:";
			// 
			// btnSave
			// 
			this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSave.Location = new System.Drawing.Point(57, 335);
			this.btnSave.Margin = new System.Windows.Forms.Padding(2);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(142, 48);
			this.btnSave.TabIndex = 6;
			this.btnSave.Text = "Lưu Cài Đặt";
			this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// txtTimeInteract
			// 
			this.txtTimeInteract.Location = new System.Drawing.Point(218, 20);
			this.txtTimeInteract.Name = "txtTimeInteract";
			this.txtTimeInteract.Size = new System.Drawing.Size(57, 20);
			this.txtTimeInteract.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(276, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(28, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "phút";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 153);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(121, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Định dạng cmt1|cmt2|....";
			// 
			// RaiseAccForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ClientSize = new System.Drawing.Size(454, 403);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtTimeInteract);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtCommentContent);
			this.Controls.Add(this.ckbComment);
			this.Controls.Add(this.ckbTym);
			this.Name = "RaiseAccForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "RaiseAccForm";
			this.Load += new System.EventHandler(this.RaiseAccForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.txtTimeInteract)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox ckbTym;
		private System.Windows.Forms.CheckBox ckbComment;
		private System.Windows.Forms.RichTextBox txtCommentContent;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.NumericUpDown txtTimeInteract;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
	}
}