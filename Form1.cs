using MediaInfo.DotNetWrapper.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Tool_TikTok.Controllers;
using Tool_TikTok.Helpers;
using Tool_TikTok.Models;
using Tool_TikTok.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tool_TikTok
{
	public partial class Form1 : Form
	{
		public static DataGridView tblMain;
		public static System.Windows.Forms.ComboBox _cbbTopic;
		public static int CurrentWidth, CurrentHeight, _numberThread, _threadRunning, _success, _fail, _maxVideo;
		public static double Scale;
		public static bool _Login, _UpVideo, stop,
							finish, _Statistical, _Follow,
							_rdoChrome, _rdoFirefox,
							_RegTikTok, _RegTikTokTMProxy,
							_Interact, _LoginByCookie,
							_rdoProxyThuong, _rdoTMProxy,
							_rdoUpVideoTimer, _rdoUpVideoNotTimer,
							_changePass;
		public static object lockChrome, lockProxy, lockTopic, lockfolder, lockDatabase;
		public static decimal _TimeSleepFrom, _TimeSleepTo;
		public static string _TimerHours, _TimerMinutes, _Tag, ApiGPM, _urlToFollow;
		public static DateTime _TimerDate, _date, _timeInteractForm1;
		public static SqlController sqlController;
		public static List<string> _proxyList,
			_folderVideo, _listVideoUsing,
			_listTopic, _listApiKey, _listUserName;
		List<DataGridViewRow> rowsChecked;
		public static Random random;
		Stopwatch stopwatch;
		public Form1()
		{
			InitializeComponent();
			tblMain = dataGridView1;
			_cbbTopic = cbbTopic;
			_TimerDate = DateTime.Today;
		}
		private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
				return;
			if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
			{
				dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = ""; // Đặt giá trị của ô thành chuỗi rỗng nếu nó là null
			}
		}
		private void Form1_Load(object sender, EventArgs e)
		{
			_listUserName = new List<string>(Resources.name.Replace("\r", "").Split('\n'));
			_proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
			lblcountProxy.Text = _proxyList.Count.ToString();
			_listApiKey = new List<string>(File.ReadAllLines("input/ApiKeyTMProxy.txt"));
			lblApiKeyTMProxy.Text = _listApiKey.Count.ToString();
			var dvgType = dataGridView1.GetType();
			var pi = dvgType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
			pi.SetValue(dataGridView1, true, null);
			dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
			rdoTimer.Checked = Settings.Default.rdoTimer;
			rdoNotTimer.Checked = Settings.Default.rdoNotTimer;
			random = new Random();
			rdoFireFox.Checked = Settings.Default.rdoFireFox;
			rdoChrome.Checked = Settings.Default.rdoChrome;
			ckbChangePass.Checked = Settings.Default.ckbChangePass;
			ckbLogin.Checked = Settings.Default.ckbLogin;
			rdoProxyThuong.Checked = Settings.Default.rdoProxyThuong;
			rdoTMProxy.Checked = Settings.Default.rdoTMProxy;
			ckbUpVideo.Checked = Settings.Default.ckbUpVideo;
			ckbStatistical.Checked = Settings.Default.ckbStatistical;
			ckbFollow.Checked = Settings.Default.ckbFollow;
			ckbRegTikTok.Checked = Settings.Default.ckbRegTikTok;
			ckbInteract.Checked = Settings.Default.ckbInteract;
			ckbLoginByCookie.Checked = Settings.Default.ckbLoginByCookie;
			txtTimeSleepFrom.Value = Settings.Default.txtTimeSleepFrom;
			txtTimeSleepTo.Value = Settings.Default.txtTimeSleepTo;
			txtScale.Text = Settings.Default.txtScale;
			txtAPIGPM.Text = Settings.Default.txtAPIGPM;
			txtLinkToBuffFollow.Text = Settings.Default.txtLinkToBuffFollow;
			RaiseAccForm._timeInteract = Settings.Default.txtTimeInteract;
			RaiseAccForm._commentContent = Settings.Default.txtCommentContent;
			RaiseAccForm._tym = Settings.Default.ckbTym;
			RaiseAccForm._comment = Settings.Default.ckbComment;
			//txtTimerDate.Value = Settings.Default.txtTimerDate;
			if (Settings.Default.txtTimerDate != DateTime.MinValue) // Kiểm tra giá trị mặc định
			{
				txtTimerDate.Value = Settings.Default.txtTimerDate;
			}
			else
			{
				txtTimerDate.Value = DateTime.Now; // Đặt giá trị mặc định là thời gian hiện tại
			}
			txtNumberThread.Text = Settings.Default.txtNumberThread;
			txtMaxVideo.Text = Settings.Default.txtMaxVideo;
			txtTimerHours.Text = Settings.Default.txtTimerHours;
			txtTimerMinutes.Text = Settings.Default.txtTimerMinutes;
			txtTag.Text = Settings.Default.txtTag;
			if (!Directory.Exists("input"))
			{
				Directory.CreateDirectory("input");
			}
			if (!Directory.Exists("Video"))
			{
				Directory.CreateDirectory("Video");
			}
			if (!File.Exists("input/Proxy.txt"))
			{
				File.Create("input/Proxy.txt").Close();
			}
			if (!File.Exists("input/Hotmail.txt"))
			{
				File.Create("input/Hotmail.txt").Close();
			}
			if (!File.Exists("input/ApiKeyTMProxy.txt"))
			{
				File.Create("input/ApiKeyTMProxy.txt").Close();
			}
			sqlController = new SqlController();
			sqlController.createTable("CREATE TABLE IF NOT EXISTS tbl_topic (C_Topic TEXT PRIMARY KEY)");
			sqlController.excuteSQL("INSERT INTO tbl_topic(C_Topic) VALUES('All')");
			sqlController.excuteSQL("INSERT INTO tbl_topic(C_Topic) VALUES('Reg Acc')");
			sqlController.createTable("CREATE TABLE IF NOT EXISTS tbl_accounts (C_Email TEXT PRIMARY KEY, C_Account TEXT, C_Status TEXT, C_Proxy TEXT, C_Follower INTEGER, C_Video INTEGER, C_UserAgent TEXT,C_GPMID TEXT)");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Topic TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Cookie TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Url TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Country TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_ChanelName TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_PassEmail TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_PassTikTok TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Video1 INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Video2 INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Video3 INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Video4 INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Video5 INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_View INTEGER");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_Folder TEXT");
			sqlController.excuteSQL("ALTER TABLE tbl_accounts ADD C_TypeBrowser TEXT");
			//sqlController.ReloadData();
			sqlController.LoadDataIntoComboBox();
			lockChrome = new object();
			lockProxy = new object();
			lockTopic = new object();
			lockfolder = new object();
			lockDatabase = new object();
			//ApiGPM = txtAPIGPM.Text;
			//Scale = double.Parse(txtScale.Text);
		}
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			string searchText = txtSearch.Text;
			if (!string.IsNullOrEmpty(searchText))
			{
				// Sử dụng LINQ để tìm kiếm các hàng chứa nội dung cần tìm
				var query = from row in tblMain.Rows.Cast<DataGridViewRow>()
							where row.Cells.Cast<DataGridViewCell>()
									   .Any(cell => cell.Value != null && cell.Value.ToString().Contains(searchText))
							select row;

				// Hiển thị kết quả tìm kiếm trong DataGridView
				tblMain.Rows.Cast<DataGridViewRow>().ToList().ForEach(row => row.Visible = query.Contains(row));
			}
			else
			{
				// Nếu không có nội dung tìm kiếm, hiển thị tất cả các hàng.
				tblMain.Rows.Cast<DataGridViewRow>().ToList().ForEach(row => row.Visible = true);
			}

		}
		#region Folder Manager
		private void btnLoadFolder_Click(object sender, EventArgs e)
		{
			sqlController.ReloadDataTopic(cbbTopic.Text);
		}

		private void btnAddFolder_Click(object sender, EventArgs e)
		{
			AddTopicForm addTopicForm = new AddTopicForm();
			addTopicForm.ShowDialog();
			//sqlController.createTable($"INSERT INTO tbl_topic(C_Topic) VALUES ()");

			menuSwitchFolder.DropDownItems.Clear();
			foreach (var line in _listTopic.Where(line => line != cbbTopic.Text))
			{
				menuSwitchFolder.DropDownItems.Add(line, null, OnClick);
			}
		}
		private void cbbTopic_SelectedValueChanged(object sender, EventArgs e)
		{
			string selectedItem = cbbTopic.SelectedItem.ToString();
			//MessageBox.Show("Bạn đã chọn: " + selectedItem);
			if (cbbTopic.Text == "All")
				sqlController.ReloadData();
			else
			{
				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
			menuSwitchFolder.DropDownItems.Clear();
			foreach (var line in _listTopic.Where(line => line != cbbTopic.Text))
			{
				menuSwitchFolder.DropDownItems.Add(line, null, OnClick);
			}
			lblSumRow.Text = tblMain.RowCount.ToString();
		}
		private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
		{
			var grid = sender as DataGridView;
			var rowIdx = (e.RowIndex + 1).ToString();
			var centerFormat = new StringFormat()
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
			e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
		}
		private void OnClick(object sender, EventArgs e)
		{
			var item = (ToolStripItem)sender;
			var rows = FunctionHelper.GetRowSelected(tblMain);
			var list = new List<AccountModel>();
			for (int i = 0; i < rows.Count; i++)
			{
				var account = FunctionHelper.ConvertRowToModel(rows[i]);
				if (account != null)
				{
					account.C_Topic = item.Text;
					list.Add(account);
					FunctionHelper.EditValueColumn(account, "C_Status", $"Đã chuyển data đến chủ đề {item.Text}");
				}
			}

			sqlController.BulkUpdate(list);
		}
		private void btnDeleteFolder_Click(object sender, EventArgs e)
		{
			var status = MessageBox.Show($"Bạn có chắc muốn xóa chủ đề và dữ liệu trong chủ đề {cbbTopic.Text} này? ", "Cảnh báo!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.excuteSQL($"DELETE FROM tbl_topic WHERE C_Topic='{cbbTopic.Text}'");
				sqlController.excuteSQL($"DELETE FROM tbl_accounts WHERE C_Topic='{cbbTopic.Text}'");
				sqlController.LoadDataIntoComboBox();
				tblMain.Rows.Clear();
				cbbTopic.Text = "";
			}

			menuSwitchFolder.DropDownItems.Clear();
			foreach (var line in _listTopic.Where(line => line != cbbTopic.Text))
			{
				menuSwitchFolder.DropDownItems.Add(line, null, OnClick);
			}
		}
		#endregion
		private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.Value == null)
			{
				e.Value = "";
			}
		}
		private void btnSelectFolder_Click(object sender, EventArgs e)
		{
			Process.Start(Path.GetFullPath("Video"));
		}
		#region Menu
		private void mởTrìnhDuyệtToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < tblMain.Rows.Count; i++)
			{
				tblMain.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}
			var rows = FunctionHelper.GetRowSelected(tblMain);
			foreach (var row in rows)
			{
				row.DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
				Thread thread = new Thread(new ParameterizedThreadStart(OneThreadOpenChrome)) { IsBackground = true };
				thread.Start(row);
			}

		}
		private void OneThreadOpenChrome(object data)
		{
			DataGridViewRow row = (DataGridViewRow)data;
			string position = BrowserController.GetNewPosition(800, 800, Scale);
			_proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
			int countPerform = 0;
			string tokenTM = "";
			var account = FunctionHelper.ConvertRowToModel(row);
			if (account != null)
			{
			reStart:
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
				BrowserController browserController = new BrowserController(account);
				try
				{
					if (account.C_Proxy == "")
					{
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								return;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									return;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
					}

					account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					FunctionHelper.EditValueColumn(account, "C_Status", account.C_Status);

				}
				catch
				{
					if (_rdoTMProxy)
					{
						FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
						account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
						if (account.C_Proxy == "")
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
							return;
						}

						FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
					}
					else if (_rdoProxyThuong)
					{
						lock (lockProxy)
						{
							if (_proxyList.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								return;
							}
							account.C_Proxy = _proxyList[0];
							_proxyList.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _proxyList);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
					}
					countPerform++;
					if (countPerform <= 2)
					{
						goto reStart;

					}
					else
					{
						browserController.CloseChrome();
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						return;
					}

				}
				if (account.driver == null)
				{
					if (_rdoTMProxy)
					{
						FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
						account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
						if (account.C_Proxy == "")
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
							return;
						}

						FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
					}
					else if (_rdoProxyThuong)
					{
						lock (lockProxy)
						{
							if (_proxyList.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								return;
							}
							account.C_Proxy = _proxyList[0];
							_proxyList.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _proxyList);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
					}
					countPerform++;
					if (countPerform <= 2)
					{
						goto reStart;

					}
					else
					{
						browserController.CloseChrome();
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
					}
				}
			}
		}
		private void mởThưMụcVideoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var rows = FunctionHelper.GetRowSelected(tblMain);
			for (int i = 0; i < rows.Count; i++)
			{
				try
				{
					accountModel.Add(new AccountModel
					{
						C_Folder = rows[i].Cells["C_Folder"].Value == null ? "" : rows[i].Cells["C_Folder"].Value.ToString(),

					});
					Process.Start(Path.GetFullPath("Video/" + accountModel[i].C_Folder));
				}
				catch
				{

				}
			}
		}
		private void btnApiKeyTMProxy_Click(object sender, EventArgs e)
		{
			Process.Start(Path.GetFullPath("input/ApiKeyTMProxy.txt"));
		}
		private void chọnCácDòngBôiĐenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var row = FunctionHelper.GetRowSelected(tblMain);
			for (int i = 0; i < tblMain.Rows.Count; i++)
			{
				if ((bool)tblMain.Rows[i].Cells["C_Check"].Value == false)
					tblMain.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}

			for (int i = 0; i < row.Count; i++)
			{
				if ((bool)row[i].Cells["C_Check"].Value == true)
				{
					row[i].Cells["C_Check"].Value = false;
					row[i].DefaultCellStyle.ForeColor = Color.Black;
				}
				else
				{
					row[i].Cells["C_Check"].Value = true;
					row[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
				}
			}
		}
		private void chọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var rows = tblMain.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_Check"].Value = true;
				rows[i].DefaultCellStyle.ForeColor = Settings.Default.colorOrange;
			}
		}
		private void bỏChọnTấtCảToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var rows = tblMain.Rows;
			for (int i = 0; i < rows.Count; i++)
			{
				rows[i].Cells["C_Check"].Value = false;
				rows[i].DefaultCellStyle.ForeColor = Color.Black;
			}
		}
		private void xóaDữLiệuTrongÔToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var item = FunctionHelper.GetRowSelected(tblMain);
			var selectedCells = tblMain.SelectedCells;
			var status = MessageBox.Show("Bạn có chắc muốn xóa các ô này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				foreach (DataGridViewCell cell in selectedCells)
				{
					cell.Value = "";
				}
				lưuToolStripMenuItem_Click(sender, e);
			}

		}
		//Xóa dòng và trong GPM
		private void xóaDòngVàCảTrongGPMToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ApiGPM = txtAPIGPM.Text;
			List<AccountModel> accountModel = new List<AccountModel>();
			GPMLoginAPI apiGPM = new GPMLoginAPI(ApiGPM);
			string profileGPM = "";
			var item = FunctionHelper.GetRowSelected(tblMain);
			for (int i = 0; i < item.Count; i++)
			{
				accountModel.Add(new AccountModel
				{
					C_Email = item[i].Cells["C_Email"].Value.ToString(),

				});
			}
			var status = MessageBox.Show("Bạn có chắc muốn xóa các dòng này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.BulkDelete(accountModel);
				MessageBox.Show("Đã xóa!");
				for (int i = 0; i < item.Count; i++)
				{
					item[i].DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					profileGPM = item[i].Cells["C_GPMID"].Value.ToString();
					apiGPM.Delete(profileGPM);
					item[i].Cells["C_Status"].Value = "Đã Xóa";
				}
			}
		}
		private void xóaToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var item = FunctionHelper.GetRowSelected(tblMain);
			for (int i = 0; i < item.Count; i++)
			{
				accountModel.Add(new AccountModel
				{
					C_Email = item[i].Cells["C_Email"].Value.ToString()
				});
			}
			var status = MessageBox.Show("Bạn có chắc muốn xóa các dòng này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (status == DialogResult.Yes)
			{
				sqlController.BulkDelete(accountModel);
				MessageBox.Show("Đã xóa!");
				for (int i = 0; i < item.Count; i++)
				{
					item[i].DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					item[i].Cells["C_Status"].Value = "Đã Xóa";
				}
			}
		}
		private void lưuToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			for (int i = 0; i < tblMain.Rows.Count; i++)
			{
				//DataGridViewRow row = tblMain.Rows[i];
				//var proxy = tblMain.Rows[i].Cells["C_Proxy"].Value.ToString();
				try
				{
					var account = FunctionHelper.ConvertRowToModel(tblMain.Rows[i]);
					if (account != null)
					{
						accountModel.Add(account);
					}
				}
				catch
				{

				}
			}
			sqlController.BulkUpdate(accountModel);
			MessageBox.Show("Đã Lưu!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		private void copyFullDòngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var rows = FunctionHelper.GetRowSelected(tblMain);
			for (int i = 0; i < rows.Count; i++)
			{
				try
				{
					var account = FunctionHelper.ConvertRowToModel(tblMain.Rows[i]);
					if (account != null)
					{
						accountModel.Add(account);
						FunctionHelper.EditValueColumn(account, "C_Status", "Đã copy!");
					}

				}
				catch
				{

				}
			}
			//var acc = JsonConvert.SerializeObject(accountModel);
			Clipboard.SetText(JsonConvert.SerializeObject(accountModel));
		}
		private void thêmFullDòngToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				var accountModel = JsonConvert.DeserializeObject<List<AccountModel>>(Clipboard.GetText());
				if (accountModel.Count > 0)
				{
					sqlController.BulkInsert(accountModel);
					sqlController.ReloadDataTopic(cbbTopic.Text);
				}
			}
			catch
			{

			}
		}
		private void tảiLạiToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (cbbTopic.Text == "All")
				sqlController.ReloadData();
			else
			{
				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
			lblSumRow.Text = tblMain.RowCount.ToString();
		}

		//Email|PassEmail|Username|PassTikTok
		private void emailPassMailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					//var le = item.Length;
					if (item.Length == 4)
					{
						accountModel.Add(new AccountModel
						{
							C_Account = items[i].Trim(),
							C_Email = item[0].Trim(),
							C_PassEmail = item[1].Trim(),
							C_PassTikTok = item[3].Trim(),
							C_Topic = cbbTopic.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
					//Directory.CreateDirectory(Path.GetFullPath("Video" + "\\" + item[0].Trim()));
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
		}
		//Thêm Mail|Passmail|PassTiktok|Cookie
		private void mailPassmaiPasstiktokCookieToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 4)
					{
						accountModel.Add(new AccountModel
						{
							C_Account = items[i].Trim(),
							C_Email = item[0].Trim(),
							C_PassEmail = item[1].Trim(),
							C_PassTikTok = item[2].Trim(),
							C_Cookie = item[3].Trim(),
							C_Topic = cbbTopic.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
					//Directory.CreateDirectory(Path.GetFullPath("Video" + "\\" + item[0].Trim()));
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
		}
		//Thêm Mail|Passmail|PassTiktok
		private void emailPassmaiPasstiktokToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 3)
					{
						accountModel.Add(new AccountModel
						{
							C_Account = items[i].Trim(),
							C_Email = item[0].Trim(),
							C_PassEmail = item[1].Trim(),
							C_PassTikTok = item[2].Trim(),
							C_Topic = cbbTopic.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
					//Directory.CreateDirectory(Path.GetFullPath("Video" + "\\" + item[0].Trim()));
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
		}
		//Thêm Email|PassEmail
		private void emailPassEmailToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<AccountModel> accountModel = new List<AccountModel>();
			var items = Clipboard.GetText().Replace("\r", "").Split('\n').ToList();
			for (int i = 0; i < items.Count; i++)
			{
				try
				{
					var item = items[i].Split('|');
					if (item.Length == 2)
					{
						accountModel.Add(new AccountModel
						{
							C_Account = items[i].Trim(),
							C_Email = item[0].Trim(),
							C_PassEmail = item[1].Trim(),
							C_Topic = cbbTopic.Text
						});
					}
					else
					{
						MessageBox.Show("Không đúng định dạng!");
						break;
					}
				}
				catch
				{
				}
			}
			if (accountModel.Count > 0)
			{
				sqlController.BulkInsert(accountModel);

				sqlController.ReloadDataTopic(cbbTopic.Text);
			}
		}
		#endregion
		#region Interact
		private void btnRaiseAcc_Click(object sender, EventArgs e)
		{
			RaiseAccForm raiseAccForm = new RaiseAccForm();
			raiseAccForm.ShowDialog();
		}
		#endregion
		private void btnOpenProxy_Click(object sender, EventArgs e)
		{
			Process.Start(Path.GetFullPath("input/Proxy.txt"));
		}
		private void btnStop_Click(object sender, EventArgs e)
		{
			stop = true;
			finish = true;

			btnStop.Enabled = false;
		}
		private void btnCloseAllChrome_Click(object sender, EventArgs e)
		{
			var Processs = new List<Process>();
			Processs.AddRange(Process.GetProcessesByName("chromedriver"));
			Processs.AddRange(Process.GetProcessesByName("chrome"));
			foreach (var pr in Processs)
			{
				try
				{
					pr.Kill();
				}
				catch
				{

				}
			}
		}
		private void btnSave_Click(object sender, EventArgs e)
		{
			Settings.Default.ckbLogin = ckbLogin.Checked;
			Settings.Default.ckbUpVideo = ckbUpVideo.Checked;
			Settings.Default.ckbChangePass = ckbChangePass.Checked;
			Settings.Default.rdoProxyThuong = rdoProxyThuong.Checked;
			Settings.Default.rdoTimer = rdoTimer.Checked;
			Settings.Default.rdoNotTimer = rdoNotTimer.Checked;
			Settings.Default.rdoTMProxy = rdoTMProxy.Checked;
			Settings.Default.rdoFireFox = rdoFireFox.Checked;
			Settings.Default.rdoChrome = rdoChrome.Checked;
			Settings.Default.ckbStatistical = ckbStatistical.Checked;
			Settings.Default.ckbFollow = ckbFollow.Checked;
			Settings.Default.ckbRegTikTok = ckbRegTikTok.Checked;
			Settings.Default.ckbInteract = ckbInteract.Checked;
			Settings.Default.ckbLoginByCookie = ckbLoginByCookie.Checked;
			Settings.Default.txtTimeSleepFrom = txtTimeSleepFrom.Value;
			Settings.Default.txtTimeSleepTo = txtTimeSleepTo.Value;
			Settings.Default.txtTimerDate = txtTimerDate.Value;
			Settings.Default.txtNumberThread = txtNumberThread.Text;
			Settings.Default.txtMaxVideo = txtMaxVideo.Text;
			Settings.Default.txtAPIGPM = txtAPIGPM.Text;
			Settings.Default.txtLinkToBuffFollow = txtLinkToBuffFollow.Text;
			Settings.Default.txtScale = txtScale.Text;
			Settings.Default.txtTimerHours = txtTimerHours.Text;
			Settings.Default.txtTimerMinutes = txtTimerMinutes.Text;
			Settings.Default.txtTag = txtTag.Text;
			Settings.Default.Save();
			_rdoProxyThuong = rdoProxyThuong.Checked;
			_rdoTMProxy = rdoTMProxy.Checked;
			_rdoChrome = rdoChrome.Checked;
			_rdoFirefox = rdoFireFox.Checked;
			_rdoUpVideoNotTimer = rdoNotTimer.Checked;
			_rdoUpVideoTimer = rdoTimer.Checked;
			_Login = ckbLogin.Checked;
			_UpVideo = ckbUpVideo.Checked;
			_Statistical = ckbStatistical.Checked;
			_Follow = ckbFollow.Checked;
			_RegTikTok = ckbRegTikTok.Checked;
			_changePass = ckbChangePass.Checked;
			_Interact = ckbInteract.Checked;
			_LoginByCookie = ckbLoginByCookie.Checked;
			_TimeSleepFrom = txtTimeSleepFrom.Value;
			_TimeSleepTo = txtTimeSleepTo.Value;
			_TimerDate = txtTimerDate.Value;
			_TimerHours = txtTimerHours.Text;
			_TimerMinutes = txtTimerMinutes.Text;
			_maxVideo = int.Parse(txtMaxVideo.Text);
			_date = _TimerDate.Date.AddHours(int.Parse(_TimerHours)).AddMinutes(int.Parse(_TimerMinutes));

			_Tag = txtTag.Text;
			_urlToFollow = txtLinkToBuffFollow.Text;
			if (txtAPIGPM.Text == "")
				MessageBox.Show("Vui lòng điền key GPM!");
			else
			{
				ApiGPM = txtAPIGPM.Text;
			}
			Scale = double.Parse(txtScale.Text);
			_proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
			lblcountProxy.Text = _proxyList.Count.ToString();
			_listApiKey = new List<string>(File.ReadAllLines("input/ApiKeyTMProxy.txt"));
			lblApiKeyTMProxy.Text = _listApiKey.Count.ToString();
		}
		private void btnStart_Click(object sender, EventArgs e)
		{
			btnSave_Click(sender, e);
			stop = false;
			_success = 0;
			_fail = 0;
			CurrentWidth = 0;
			CurrentHeight = 0;
			_numberThread = int.Parse(txtNumberThread.Text);
			_threadRunning = _numberThread;
			lblthreadRunning.Text = _threadRunning.ToString();
			lblSuccess.Text = _success.ToString();
			lblFail.Text = _fail.ToString();
			rowsChecked = FunctionHelper.GetRowChecked(tblMain);
			_folderVideo = new List<string>(Directory.GetDirectories(Path.GetFullPath("Video")));
			//_listVideoUsing = new List<string>();
			for (int i = 0; i < rowsChecked.Count; i++)
			{
				rowsChecked[i].Cells["C_Status"].Value = "";
			}
			if (rowsChecked.Count == 0 || _numberThread == 0)
				return;

			for (int i = 0; i < _numberThread; i++)
			{
				Thread thread = new Thread(OneThread) { IsBackground = true };
				thread.Start();
			}
			stopwatch = new Stopwatch();
			stopwatch.Start();
			timer1.Start();
			btnStart.Enabled = false;
			btnStop.Enabled = true;

		}
		private void OneThread()
		{
			string position = BrowserController.GetNewPosition(800, 800, Scale);
			AccountModel account = null;
			TikTokController tikTokController = null;
			TikTokAPIController tikTokAPIController = null;
			BrowserController browserController = null;
			DataGridViewRow row;
			bool success = false;
			DateTime _dateOneThread = _date;
			int countPerform = 0;
			string tokenTM = "";
			lock (lockProxy)
			{
				if (_rdoTMProxy)
				{
					if (_listApiKey.Count == 0)
					{
						goto decreThread;
					}
					tokenTM = _listApiKey[0];
					_listApiKey.RemoveAt(0);
				}
			}
			while (!stop)
			{
				lock (rowsChecked)
				{
					if (rowsChecked.Count == 0)
						break;
					row = rowsChecked[0];
					rowsChecked.RemoveAt(0);
				}
				try
				{
					account = new AccountModel()
					{
						C_Account = row.Cells["C_Account"].Value.ToString(),
						C_Email = row.Cells["C_Email"].Value.ToString(),
						C_PassEmail = row.Cells["C_PassEmail"].Value.ToString(),
						C_PassTikTok = row.Cells["C_PassTikTok"].Value.ToString(),
						C_Url = row.Cells["C_Url"].Value.ToString(),
						C_Cookie = row.Cells["C_Cookie"].Value.ToString(),
						C_Country = row.Cells["C_Country"].Value.ToString(),
						C_Follower = int.Parse(row.Cells["C_Follower"].Value.ToString()),
						C_Topic = row.Cells["C_Topic"].Value.ToString(),
						C_Folder = row.Cells["C_Folder"].Value.ToString(),
						C_Video1 = int.Parse(row.Cells["C_Video1"].Value.ToString()),
						C_Video2 = int.Parse(row.Cells["C_Video2"].Value.ToString()),
						C_Video3 = int.Parse(row.Cells["C_Video3"].Value.ToString()),
						C_Video4 = int.Parse(row.Cells["C_Video4"].Value.ToString()),
						C_Video5 = int.Parse(row.Cells["C_Video5"].Value.ToString()),
						C_Video = int.Parse(row.Cells["C_Video"].Value.ToString()),
						C_View = int.Parse(row.Cells["C_View"].Value.ToString()),
						C_Status = row.Cells["C_Status"].Value.ToString(),
						C_UserAgent = row.Cells["C_UserAgent"].Value.ToString(),
						C_GPMID = row.Cells["C_GPMID"].Value.ToString(),
						C_Proxy = row.Cells["C_Proxy"].Value.ToString(),
						C_Row = row
					};
				}
				catch
				{
					success = false;
					FunctionHelper.EditValueColumn(account, "C_Status", "Vui lòng thử lại!", true);
					goto finish;
				}

				if (account.C_Proxy == "")
				{
					if (_rdoProxyThuong)
					{
						lock (lockProxy)
						{
							if (_proxyList.Count == 0)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
								break;
							}
							account.C_Proxy = _proxyList[0];
							_proxyList.RemoveAt(0);
							File.WriteAllLines("input/Proxy.txt", _proxyList);
							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
					}
				}
				if (_rdoTMProxy)
				{
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
					account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
					if (account.C_Proxy == "")
					{
						FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
						goto finish;
					}

					FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy);
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
				}
				browserController = new BrowserController(account);
				tikTokController = new TikTokController(account);

			reStartLogin:
				if (_Login)
				{
					//account.C_Proxy = "";
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");

					account.firefoxDriver = browserController.OpenChromeGpmV3FireFox(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					try
					{
						account.firefoxDriver = browserController.OpenChromeGpmV3FireFox(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
						if (_rdoFirefox)
						{
							account.driver.Manage().Window.Size = new System.Drawing.Size(800, 800);
							account.driver.Manage().Window.Position = new System.Drawing.Point(int.Parse(position.Split(',')[0].Trim()), int.Parse(position.Split(',')[1].Trim()));
						}
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLogin;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLogin;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu login ...");
					var status = tikTokController.Login();

					if (status == ResultModel.Success)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						//FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Login ok!", true);
						browserController.CloseChrome();
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Login thất bại", true);
						goto finish;
					}
					else if (status == ResultModel.ErorrAcc)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						//FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Acc lỗi ineligible!", true);
						goto finish;
					}
					else if (status == ResultModel.WeakProxy)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLogin;
						}
					}
				}
			reStartLoginByCookie:
				if (_LoginByCookie)
				{
					//account.C_Proxy = "";
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
					try
					{
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLoginByCookie;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLoginByCookie;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu login ...");
					var status = tikTokController.LoginByCookie();

					if (status == ResultModel.Success)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						//FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Login ok!", true);
						//browserController.CloseChrome();
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Login thất bại", true);
						goto finish;
					}
					else if (status == ResultModel.ErorrAcc)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						//FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Acc lỗi ineligible!", true);
						goto finish;
					}
					else if (status == ResultModel.WeakProxy)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Proxy quá yếu,đã đổi proxy!", true);
						//row.Cells["C_Check"].Value = false;
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartLogin;
						}
					}
				}
			reStartUpVideo:
				if (_UpVideo)
				{
					if (account.C_Folder == "")
					{
						account.C_Folder = FunctionHelper.GetFolderNotUsing(account.C_Email);
						if (account.C_Folder == "")
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Đã hết folder video!", true);
							goto finish;
						}
						FunctionHelper.EditValueColumn(account, "C_Folder", account.C_Folder, true);
					}
					//account.C_Proxy = "";
					try
					{
						FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...", true);
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "Mozilla/5.0 (iPhone; CPU iPhone OS 16_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/20G75 Safari/604.1", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi,đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartUpVideo;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartUpVideo;
						}
					}
					bool clickAnotherVideo = false;
					for (int i = 1; i <= _maxVideo; i++)
					{
						var file = FunctionHelper.GetVideoFolder(account.C_Folder);
						if (file == "")
						{
							success = true;
							FunctionHelper.EditValueColumn(account, "C_Status", $"Folder đã hết video!", true);
							goto finish;
						}
						var duration = FunctionHelper.GetDuration(file);
						if (duration >= 10)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Video quá 10 phút!", true);
							File.Move(file, file.Replace(account.C_Folder + "\\", account.C_Folder + "\\ERORR_"));
							i--;
							continue;
						}
						FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Bắt đầu đăng video ...");
						var status = tikTokController.UpVideo(file, _dateOneThread, ref clickAnotherVideo, i);
						_dateOneThread = _dateOneThread.AddMinutes(FunctionHelper.RandomMinutes((int)_TimeSleepFrom, (int)_TimeSleepTo));
						if (status == ResultModel.Success)
						{
							//if (i == 1)
							//{
							//    var allCookies = account.driver.Manage().Cookies.AllCookies;
							//    account.C_Cookie = JsonConvert.SerializeObject(allCookies);
							//    FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
							//}
							success = true;
							FunctionHelper.EditValueColumn(account, "C_Folder", account.C_Email, true);
							File.Move(file, file.Replace(account.C_Folder + "\\", account.C_Folder + "\\DONE_"));
							FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}]Đăng video thành công!", true);

						}
						else if (status == ResultModel.Fail)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}]Đăng video thất bại!", true);

						}
						else if (status == ResultModel.ErorrAcc)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Acc lỗi ineligible!", true);
							goto finish;
						}
						else if (status == ResultModel.Suspended)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Suspend acc!", true);
							goto finish;
						}
						else if (status == ResultModel.NotLogin)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Chưa login!", true);
							goto finish;
						}
						else if (status == ResultModel.WeakProxy)
						{
							success = false;
							if (_rdoTMProxy)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
								account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
								if (account.C_Proxy == "")
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
									goto finish;
								}

								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
								FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
							}
							else if (_rdoProxyThuong)
							{
								lock (lockProxy)
								{
									if (_proxyList.Count == 0)
									{
										FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
										break;
									}
									account.C_Proxy = _proxyList[0];
									_proxyList.RemoveAt(0);
									File.WriteAllLines("input/Proxy.txt", _proxyList);
									FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
								}
							}
							FunctionHelper.EditValueColumn(account, "C_Status", $"Proxy quá yếu, đã đổi proxy!", true);
							countPerform++;
							if (countPerform == 2)
							{
								goto finish;
							}
							else
							{
								browserController.CloseChrome();
								goto reStartUpVideo;
							}
						}
					}
					browserController.CloseChrome();
				}

				if (_Statistical)
				{
					tikTokAPIController = new TikTokAPIController();

					FunctionHelper.EditValueColumn(account, "C_Status", "Đang thống kê ...");
					var status = tikTokAPIController.GetInfoTikTok(account);
					if (status == ResultModel.Success)
					{
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Thống kê xong!", true);
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Thống kê có lỗi!", true);
						goto finish;
					}
					else if (status == ResultModel.Suspended)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Suspended account!", true);
						goto finish;
					}
					else if (status == ResultModel.AnotherError)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Lỗi khác!", true);
						goto finish;
					}
					else if (status == ResultModel.NotLogin)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Chưa login!", true);
						goto finish;
					}
				}

			reStartRegTikTok:
				if (_RegTikTok)
				{
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
					try
					{
						//account.C_Proxy = "";
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartRegTikTok;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartRegTikTok;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu reg ...");
					var status = tikTokController.RegTikTok();
					if (status == ResultModel.Success)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie);
						FunctionHelper.EditValueColumn(account, "C_PassTikTok", account.C_PassTikTok);
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Reg ok!", true);
						browserController.CloseChrome();
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Reg thất bại", true);
						goto finish;
					}
					else if (status == ResultModel.EmailLoginFail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Reg thất bại:login email fail", true);
						goto finish;
					}
					else if (status == ResultModel.CodeEmpty)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Reg thất bại: ko có code", true);
						goto finish;
					}
					else if (status == ResultModel.WeakProxy)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Proxy quá yếu,đã đổi proxy!", true);
						//row.Cells["C_Check"].Value = false;
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartRegTikTok;
						}
					}
				}

			reStartChangePass:
				if (_changePass)
				{
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
					try
					{
						//account.C_Proxy = "";
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartChangePass;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy 2 lần!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartChangePass;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu change pass ...");
					var status = tikTokController.ChangePass();
					if (status == ResultModel.Success)
					{
						var allCookies = account.driver.Manage().Cookies.AllCookies;
						account.C_Cookie = JsonConvert.SerializeObject(allCookies);
						FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie);
						FunctionHelper.EditValueColumn(account, "C_PassTikTok", account.C_PassTikTok);
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Change pass ok!", true);
						browserController.CloseChrome();
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Change pass thất bại", true);
						goto finish;
					}
					else if (status == ResultModel.EmailLoginFail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Change pass thất bại:login email fail", true);
						goto finish;
					}
					else if (status == ResultModel.CodeEmpty)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Change pass thất bại: ko có code", true);
						goto finish;
					}
					else if (status == ResultModel.AlreadyLogin)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Chưa log out để đổi pass dc!", true);
						goto finish;
					}
					else if (status == ResultModel.WeakProxy)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Proxy quá yếu,đã đổi proxy!", true);
						//row.Cells["C_Check"].Value = false;
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartChangePass;
						}
					}
				}
			reStartFollow:
				if (_Follow)
				{
					if (_urlToFollow == "")
					{
						MessageBox.Show("Chưa điền link kênh!");
						break;
					}
					//account.C_Proxy = "";
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
					try
					{
						account.driver = browserController.OpenChromeGpm(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartFollow;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartFollow;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu follow ...");

					var urls = _urlToFollow.Split('\n');

					foreach (var url in urls)
					{
						var status = tikTokController.Follow(url);

						if (status == ResultModel.Success)
						{
							success = true;
							FunctionHelper.EditValueColumn(account, "C_Status", "follow ok!", true);

						}
						else if (status == ResultModel.Fail)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Follow thất bại", true);

						}
						else if (status == ResultModel.ErorrAcc)
						{
							success = false;
							FunctionHelper.EditValueColumn(account, "C_Status", "Acc lỗi nhả follow!", true);
							goto finish;
						}
						else if (status == ResultModel.WeakProxy)
						{
							success = false;
							if (_rdoTMProxy)
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
								account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
								if (account.C_Proxy == "")
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
									goto finish;
								}

								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
								FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
							}
							else if (_rdoProxyThuong)
							{
								lock (lockProxy)
								{
									if (_proxyList.Count == 0)
									{
										FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
										break;
									}
									account.C_Proxy = _proxyList[0];
									_proxyList.RemoveAt(0);
									File.WriteAllLines("input/Proxy.txt", _proxyList);
									FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
								}
							}
							FunctionHelper.EditValueColumn(account, "C_Status", "Proxy quá yếu,đã đổi proxy!", true);
							//row.Cells["C_Check"].Value = false;
							countPerform++;
							if (countPerform == 2)
							{
								goto finish;
							}
							else
							{
								browserController.CloseChrome();
								goto reStartFollow;
							}
						}
					}
				}

			reStartInteract:
				if (_Interact)
				{
					_timeInteractForm1 = DateTime.Now;
					_timeInteractForm1 = _timeInteractForm1.AddMinutes((Double)RaiseAccForm._timeInteract);

					//var time = _timeInteractForm1;
					//account.C_Proxy = "";
					FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
					try
					{
						account.driver = browserController.OpenChromeGpmV3(ApiGPM, account.C_GPMID, account.C_Email, "", Scale, account.C_Proxy, position: position);
					}
					catch
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartInteract;
						}
					}
					if (account.driver == null)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Mở GPM lỗi, đã đổi proxy!", true);
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartInteract;
						}
					}
					FunctionHelper.EditValueColumn(account, "C_Status", "Bắt đầu tương tác ...");
					var status = tikTokController.Interact();

					if (status == ResultModel.Success)
					{
						FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
						success = true;
						FunctionHelper.EditValueColumn(account, "C_Status", "Tương tác xong!", true);
						browserController.CloseChrome();
					}
					else if (status == ResultModel.Fail)
					{
						success = false;
						FunctionHelper.EditValueColumn(account, "C_Status", "Có lỗi xảy ra!", true);
						goto finish;
					}
					else if (status == ResultModel.WeakProxy)
					{
						success = false;
						if (_rdoTMProxy)
						{
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang lấy proxy ...");
							account.C_Proxy = TMProxyHelper.GetNewProxy(tokenTM);
							if (account.C_Proxy == "")
							{
								FunctionHelper.EditValueColumn(account, "C_Status", "Lấy proxy lỗi");
								goto finish;
							}

							FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							FunctionHelper.EditValueColumn(account, "C_Status", "Đang mở trình duyệt ...");
						}
						else if (_rdoProxyThuong)
						{
							lock (lockProxy)
							{
								if (_proxyList.Count == 0)
								{
									FunctionHelper.EditValueColumn(account, "C_Status", "Hết Proxy", true);
									break;
								}
								account.C_Proxy = _proxyList[0];
								_proxyList.RemoveAt(0);
								File.WriteAllLines("input/Proxy.txt", _proxyList);
								FunctionHelper.EditValueColumn(account, "C_Proxy", account.C_Proxy, true);
							}
						}
						FunctionHelper.EditValueColumn(account, "C_Status", "Proxy quá yếu,đã đổi proxy!", true);
						//row.Cells["C_Check"].Value = false;
						countPerform++;
						if (countPerform == 2)
						{
							goto finish;
						}
						else
						{
							browserController.CloseChrome();
							goto reStartInteract;
						}
					}
				}
			finish:
				try
				{
					browserController.CloseChrome();
				}
				catch
				{

				}

				if (success)
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorGreeen;
					Interlocked.Increment(ref _success);
					Invoke((MethodInvoker)delegate ()
					{
						lblSuccess.Text = _success.ToString();
					});
				}
				else
				{
					account.C_Row.DefaultCellStyle.ForeColor = Settings.Default.colorRed;
					Interlocked.Increment(ref _fail);
					Invoke((MethodInvoker)delegate ()
					{
						lblFail.Text = _fail.ToString();
					});
				}
				account.C_Row.Cells["C_Check"].Value = false;
			}
		//browserController.CloseChrome();
		decreThread:
			Interlocked.Decrement(ref _threadRunning);
			Invoke((MethodInvoker)delegate ()
			{
				lblthreadRunning.Text = _threadRunning.ToString();
			});
			if (_threadRunning == 0)
			{
				Invoke(new MethodInvoker(() =>
				{
					_proxyList = new List<string>(File.ReadAllLines("input/Proxy.txt"));
					lblcountProxy.Text = _proxyList.Count.ToString();
					btnStop.Enabled = false;
					btnStart.Enabled = true;
					timer1.Stop();
				}));

				MessageBox.Show("Tool đã dừng");
			}
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			lblTimeRunning.Text = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
		}
	}
}
