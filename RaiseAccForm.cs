using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_TikTok.Properties;

namespace Tool_TikTok
{
	public partial class RaiseAccForm : Form
	{
		public static bool _surfing, _tym, _comment;
		public static string _commentContent;
		public static decimal _timeInteract;

		public RaiseAccForm()
		{
			InitializeComponent();

		}
		private void RaiseAccForm_Load(object sender, EventArgs e)
		{
			ckbTym.Checked = Settings.Default.ckbTym;
			ckbComment.Checked = Settings.Default.ckbComment;
			txtCommentContent.Text = Settings.Default.txtCommentContent;
			txtTimeInteract.Value = Settings.Default.txtTimeInteract;
			_tym = ckbTym.Checked;
			_comment = ckbComment.Checked;
			_commentContent = txtCommentContent.Text;
			_timeInteract = txtTimeInteract.Value;
		}
		public  void btnSave_Click(object sender, EventArgs e)
		{
			Settings.Default.ckbTym = ckbTym.Checked;
			Settings.Default.ckbComment = ckbComment.Checked;
			Settings.Default.txtCommentContent = txtCommentContent.Text;
			Settings.Default.txtTimeInteract = txtTimeInteract.Value;
			Settings.Default.Save();
			_tym = ckbTym.Checked;
			_comment = ckbComment.Checked;
			_commentContent = txtCommentContent.Text;
			_timeInteract = txtTimeInteract.Value;
		}
	}
}

