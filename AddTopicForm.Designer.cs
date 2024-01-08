namespace Tool_TikTok
{
	partial class AddTopicForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTopicForm));
			this.txtTopicName = new System.Windows.Forms.TextBox();
			this.btnAddTopicName = new System.Windows.Forms.Button();
			this.label14 = new System.Windows.Forms.Label();
			this.btnCancelAddTopic = new System.Windows.Forms.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.SuspendLayout();
			// 
			// txtTopicName
			// 
			resources.ApplyResources(this.txtTopicName, "txtTopicName");
			this.txtTopicName.Name = "txtTopicName";
			// 
			// btnAddTopicName
			// 
			resources.ApplyResources(this.btnAddTopicName, "btnAddTopicName");
			this.btnAddTopicName.Name = "btnAddTopicName";
			this.btnAddTopicName.UseVisualStyleBackColor = true;
			this.btnAddTopicName.Click += new System.EventHandler(this.btnAddTopicName_Click);
			// 
			// label14
			// 
			resources.ApplyResources(this.label14, "label14");
			this.label14.Name = "label14";
			// 
			// btnCancelAddTopic
			// 
			resources.ApplyResources(this.btnCancelAddTopic, "btnCancelAddTopic");
			this.btnCancelAddTopic.Name = "btnCancelAddTopic";
			this.btnCancelAddTopic.UseVisualStyleBackColor = true;
			this.btnCancelAddTopic.Click += new System.EventHandler(this.btnCancelAddTopic_Click);
			// 
			// AddTopicForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnCancelAddTopic);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.btnAddTopicName);
			this.Controls.Add(this.txtTopicName);
			this.Name = "AddTopicForm";
			this.Load += new System.EventHandler(this.AddTopicForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtTopicName;
		private System.Windows.Forms.Button btnAddTopicName;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Button btnCancelAddTopic;
		private System.Windows.Forms.ColorDialog colorDialog1;
	}
}