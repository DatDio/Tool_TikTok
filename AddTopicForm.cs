using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_TikTok.Controllers;

namespace Tool_TikTok
{
	public partial class AddTopicForm : Form
	{
		SqlController sqlController;
		
		public AddTopicForm()
		{
			InitializeComponent();
		}
		private void AddTopicForm_Load(object sender, EventArgs e)
		{
			sqlController = new SqlController();
		}
		private void btnAddTopicName_Click(object sender, EventArgs e)
		{
			sqlController.excuteSQL($"INSERT INTO tbl_topic(C_Topic) VALUES ('{txtTopicName.Text}')");
			MessageBox.Show($"Đã thêm chủ đề {txtTopicName.Text}");
			sqlController.LoadDataIntoComboBox();
			
			this.Close();
		}

		private void btnCancelAddTopic_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		
	}
}
