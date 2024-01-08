using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_TikTok.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tool_TikTok.Controllers
{
    public class SqlController
    {
        private SQLiteConnection _con;
        private string tableName;
        public SqlController(string pathTableName = "input/Account.sqllite")
        {
            _con = new SQLiteConnection();
            tableName = Path.GetFullPath(pathTableName);
            createConnection();

        }
        public void createConnection()
        {
            _con.ConnectionString = $"Data Source={tableName};Version=3;";
            _con.Open();
        }
        public void closeConnection()
        {
            _con.Close();
        }
        public void createTable(string sql)
        {
            if (!File.Exists(tableName))
            {

                SQLiteConnection.CreateFile(tableName);
            }
            try
            {
                var command = new SQLiteCommand(sql, _con);
                command.ExecuteNonQuery();
            }
            catch { }
        }
        public void excuteSQL(string sql)
        {
            try
            {
                var command = new SQLiteCommand(sql, _con);
                command.ExecuteNonQuery();
            }
            catch { }
        }
        public void BulkDelete(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"DELETE FROM tbl_accounts WHERE C_Email=@C_Email";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                //
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            catch
            {
                //
            }
        }
        public void BulkInsert(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"INSERT INTO tbl_accounts(C_Email, C_Account, C_Status, C_Proxy, C_Follower, C_Video, C_UserAgent,C_GPMID,C_Topic,C_Folder,C_Cookie,C_Url,C_PassEmail,C_PassTikTok,C_Country,C_ChanelName,C_Video1,C_Video2,C_Video3,C_Video4,C_Video5,C_View,C_TypeBrowser) VALUES(@C_Email,@C_Account, @C_Status, @C_Proxy, @C_Follower, @C_Video, @C_UserAgent,@C_GPMID,@C_Topic,@C_Folder,@C_Cookie,@C_Url,@C_PassEmail,@C_PassTikTok,@C_Country,@C_ChanelName,@C_Video1,@C_Video2,@C_Video3,@C_Video4,@C_Video5,@C_View,@C_TypeBrowser)";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                            command.Parameters.Add(new SQLiteParameter("@C_Account", account.C_Account));
                            command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                            command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                            command.Parameters.Add(new SQLiteParameter("@C_Follower", account.C_Follower));
                            command.Parameters.Add(new SQLiteParameter("@C_Video", account.C_Video));
                            command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));
                            command.Parameters.Add(new SQLiteParameter("@C_GPMID", account.C_GPMID));
                            command.Parameters.Add(new SQLiteParameter("@C_Topic", account.C_Topic));
                            command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                            command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                            command.Parameters.Add(new SQLiteParameter("@C_Url", account.C_Url));
                            command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                            command.Parameters.Add(new SQLiteParameter("@C_PassTikTok", account.C_PassTikTok));
                            command.Parameters.Add(new SQLiteParameter("@C_Country", account.C_Country));
                            command.Parameters.Add(new SQLiteParameter("@C_ChanelName", account.C_ChanelName));
                            command.Parameters.Add(new SQLiteParameter("@C_Video1", account.C_Video1));
                            command.Parameters.Add(new SQLiteParameter("@C_Video2", account.C_Video2));
                            command.Parameters.Add(new SQLiteParameter("@C_Video3", account.C_Video3));
                            command.Parameters.Add(new SQLiteParameter("@C_Video4", account.C_Video4));
                            command.Parameters.Add(new SQLiteParameter("@C_Video5", account.C_Video5));
                            command.Parameters.Add(new SQLiteParameter("@C_View", account.C_View));
							command.Parameters.Add(new SQLiteParameter("@C_TypeBrowser", account.C_TypeBrowser));
							try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch(SQLiteException ex)
                            {
                                if (ex.Message.Contains("UNIQUE constraint failed"))
                                {
                                    MessageBox.Show("Tài khoản này đã tồn tại!");
                                }
                                else
                                {
                                    MessageBox.Show("Có lỗi xảy ra!");
                                }
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            catch
            {
                //
            }
        }
        public void BulkUpdate(List<AccountModel> accountDtos)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {
                    for (var i = 0; i < accountDtos.Count; i++)
                    {
                        var account = accountDtos[i];
                        using (SQLiteCommand command = new SQLiteCommand(_con))
                        {
                            command.CommandText = $"UPDATE  tbl_accounts SET C_Email = @C_Email, C_Account=@C_Account, C_Status= @C_Status, C_Proxy = @C_Proxy, C_Follower =@C_Follower, C_Video = @C_Video, C_UserAgent= @C_UserAgent, C_GPMID =@C_GPMID, C_Topic =@C_Topic,C_Folder = @C_Folder,C_Cookie=@C_Cookie, C_Url =@C_Url,C_PassEmail=@C_PassEmail,C_PassTikTok=@C_PassTikTok,C_Country=@C_Country,C_ChanelName=@C_ChanelName,C_Video1=@C_Video1,C_Video2 =@C_Video2,C_Video3=@C_Video3,C_Video4=@C_Video4,C_Video5=@C_Video5,C_View=@C_View,C_TypeBrowser=@C_TypeBrowser WHERE C_Email = @C_Email";
                            command.CommandType = CommandType.Text;
                            command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                            command.Parameters.Add(new SQLiteParameter("@C_Account", account.C_Account));
                            command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                            command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                            command.Parameters.Add(new SQLiteParameter("@C_Follower", account.C_Follower));
                            command.Parameters.Add(new SQLiteParameter("@C_Video", account.C_Video));
                            command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));
                            command.Parameters.Add(new SQLiteParameter("@C_GPMID", account.C_GPMID));
                            command.Parameters.Add(new SQLiteParameter("@C_Topic", account.C_Topic));
                            command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                            command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                            command.Parameters.Add(new SQLiteParameter("@C_Url", account.C_Url));
                            command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                            command.Parameters.Add(new SQLiteParameter("@C_PassTikTok", account.C_PassTikTok));
                            command.Parameters.Add(new SQLiteParameter("@C_Country", account.C_Country));
                            command.Parameters.Add(new SQLiteParameter("@C_ChanelName", account.C_ChanelName));
                            command.Parameters.Add(new SQLiteParameter("@C_Video1", account.C_Video1));
                            command.Parameters.Add(new SQLiteParameter("@C_Video2", account.C_Video2));
                            command.Parameters.Add(new SQLiteParameter("@C_Video3", account.C_Video3));
                            command.Parameters.Add(new SQLiteParameter("@C_Video4", account.C_Video4));
                            command.Parameters.Add(new SQLiteParameter("@C_Video5", account.C_Video5));
                            command.Parameters.Add(new SQLiteParameter("@C_View", account.C_View));
							command.Parameters.Add(new SQLiteParameter("@C_TypeBrowser", account.C_TypeBrowser));
							try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch
                            {
                                //
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
            catch
            {
                //
            }
        }
        public void Update(AccountModel account)
        {
            try
            {
                using (SQLiteTransaction transaction = _con.BeginTransaction())
                {

                    using (SQLiteCommand command = new SQLiteCommand(_con))
                    {
                        command.CommandText = $"UPDATE  tbl_accounts SET C_Email = @C_Email, C_Account=@C_Account, C_Status= @C_Status, C_Proxy = @C_Proxy, C_Follower =@C_Follower, C_Video = @C_Video, C_UserAgent= @C_UserAgent, C_GPMID =@C_GPMID, C_Topic =@C_Topic,C_Folder = @C_Folder,C_Cookie=@C_Cookie, C_Url =@C_Url,C_PassEmail=@C_PassEmail,C_PassTikTok=@C_PassTikTok,C_Country=@C_Country,C_ChanelName=@C_ChanelName,C_Video1=@C_Video1,C_Video2 =@C_Video2,C_Video3=@C_Video3,C_Video4=@C_Video4,C_Video5=@C_Video5,C_View=@C_View,C_TypeBrowser=@C_TypeBrowser WHERE C_Email = @C_Email";
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new SQLiteParameter("@C_Email", account.C_Email));
                        command.Parameters.Add(new SQLiteParameter("@C_Account", account.C_Account));
                        command.Parameters.Add(new SQLiteParameter("@C_Status", account.C_Status));
                        command.Parameters.Add(new SQLiteParameter("@C_Proxy", account.C_Proxy));
                        command.Parameters.Add(new SQLiteParameter("@C_Follower", account.C_Follower));
                        command.Parameters.Add(new SQLiteParameter("@C_Video", account.C_Video));
                        command.Parameters.Add(new SQLiteParameter("@C_UserAgent", account.C_UserAgent));
                        command.Parameters.Add(new SQLiteParameter("@C_GPMID", account.C_GPMID));
                        command.Parameters.Add(new SQLiteParameter("@C_Topic", account.C_Topic));
                        command.Parameters.Add(new SQLiteParameter("@C_Folder", account.C_Folder));
                        command.Parameters.Add(new SQLiteParameter("@C_Cookie", account.C_Cookie));
                        command.Parameters.Add(new SQLiteParameter("@C_Url", account.C_Url));
                        command.Parameters.Add(new SQLiteParameter("@C_PassEmail", account.C_PassEmail));
                        command.Parameters.Add(new SQLiteParameter("@C_PassTikTok", account.C_PassTikTok));
                        command.Parameters.Add(new SQLiteParameter("@C_Country", account.C_Country));
                        command.Parameters.Add(new SQLiteParameter("@C_ChanelName", account.C_ChanelName));
                        command.Parameters.Add(new SQLiteParameter("@C_Video1", account.C_Video1));
                        command.Parameters.Add(new SQLiteParameter("@C_Video2", account.C_Video2));
                        command.Parameters.Add(new SQLiteParameter("@C_Video3", account.C_Video3));
                        command.Parameters.Add(new SQLiteParameter("@C_Video4", account.C_Video4));
                        command.Parameters.Add(new SQLiteParameter("@C_Video5", account.C_Video5));
                        command.Parameters.Add(new SQLiteParameter("@C_View", account.C_View));
						command.Parameters.Add(new SQLiteParameter("@C_TypeBrowser", account.C_TypeBrowser));
						try
						{
                            command.ExecuteNonQuery();
                        }
                        catch
                        {
                            //
                        }


                        transaction.Commit();
                    }
                }
            }
            catch
            {
                //
            }
        }
        public DataSet Select(string sql)
        {
            //"SELECT stt, uid from tbl_accounts"
            var ds = new DataSet();
            try
            {
                //createConection();
            }
            catch
            {
                //
            }

            try
            {
                var da = new SQLiteDataAdapter(sql, _con);
                da.Fill(ds);
                //closeConnection();
            }
            catch
            {
                //
            }

            return ds;
        }
        public void ReloadData()
        {
            try
            {
                var table = Select("SELECT * FROM tbl_accounts").Tables[0];

                List<DataGridViewRow> rows = new List<DataGridViewRow>();

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    int stt = 0;
                    var row = table.Rows[i];
                    DataGridViewRow row1 = new DataGridViewRow();
                    row1.CreateCells(Form1.tblMain);
                    row1.Cells[stt++].Value = false;
                    row1.Cells[stt++].Value = row["C_Account"].ToString();
                    row1.Cells[stt++].Value = row["C_Email"].ToString();
                    row1.Cells[stt++].Value = row["C_PassEmail"].ToString();
                    row1.Cells[stt++].Value = row["C_PassTikTok"].ToString();
                    row1.Cells[stt++].Value = row["C_ChanelName"].ToString();
                    row1.Cells[stt++].Value = row["C_Cookie"].ToString();
                    row1.Cells[stt++].Value = row["C_Country"].ToString();
                    row1.Cells[stt++].Value = row["C_Url"].ToString();
                    row1.Cells[stt++].Value = row["C_Follower"].ToString() == "" ? 0 : int.Parse(row["C_Follower"].ToString());
                    row1.Cells[stt++].Value = row["C_Topic"].ToString();
                    row1.Cells[stt++].Value = row["C_Folder"].ToString();
                    row1.Cells[stt++].Value = row["C_Status"].ToString();
                    row1.Cells[stt++].Value = row["C_Video1"].ToString() == "" ? 0 : int.Parse(row["C_Video1"].ToString());
                    row1.Cells[stt++].Value = row["C_Video2"].ToString() == "" ? 0 : int.Parse(row["C_Video2"].ToString());
                    row1.Cells[stt++].Value = row["C_Video3"].ToString() == "" ? 0 : int.Parse(row["C_Video3"].ToString());
                    row1.Cells[stt++].Value = row["C_Video4"].ToString() == "" ? 0 : int.Parse(row["C_Video4"].ToString());
                    row1.Cells[stt++].Value = row["C_Video5"].ToString() == "" ? 0 : int.Parse(row["C_Video5"].ToString());
                    row1.Cells[stt++].Value = row["C_Video"].ToString() == "" ? 0 : int.Parse(row["C_Video"].ToString());
                    row1.Cells[stt++].Value = row["C_View"].ToString() == "" ? 0 : int.Parse(row["C_View"].ToString());
                    row1.Cells[stt++].Value = row["C_UserAgent"].ToString();
                    row1.Cells[stt++].Value = row["C_GPMID"].ToString();
                    row1.Cells[stt++].Value = row["C_Proxy"].ToString();
					row1.Cells[stt++].Value = row["C_TypeBrowser"].ToString();
					rows.Add(row1);
                }

                Form1.tblMain.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblMain.Rows.Clear();

                    Form1.tblMain.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblMain.ColumnHeadersVisible = false;
                    Form1.tblMain.RowHeadersVisible = false;
                    Form1.tblMain.Rows.AddRange(rows.ToArray());
                    Form1.tblMain.ColumnHeadersVisible = true;
                    Form1.tblMain.RowHeadersVisible = true;
                }));
            }
            catch
            {
                //
            }
        }
        public void ReloadDataTopic(string topic)
        {
            try
            {
                var table = Select($"SELECT * FROM tbl_accounts WHERE C_Topic = '{topic}'").Tables[0];

                List<DataGridViewRow> rows = new List<DataGridViewRow>();

                for (var i = 0; i < table.Rows.Count; i++)
                {
                    int stt = 0;
                    var row = table.Rows[i];
                    DataGridViewRow row1 = new DataGridViewRow();
                    row1.CreateCells(Form1.tblMain);
                    row1.Cells[stt++].Value = false;
                    row1.Cells[stt++].Value = row["C_Account"].ToString();
                    row1.Cells[stt++].Value = row["C_Email"].ToString();
                    row1.Cells[stt++].Value = row["C_PassEmail"].ToString();
                    row1.Cells[stt++].Value = row["C_PassTikTok"].ToString();
                    row1.Cells[stt++].Value = row["C_ChanelName"].ToString();
                    row1.Cells[stt++].Value = row["C_Cookie"].ToString();
                    row1.Cells[stt++].Value = row["C_Country"].ToString();
                    row1.Cells[stt++].Value = row["C_Url"].ToString();
                    row1.Cells[stt++].Value = row["C_Follower"].ToString() == "" ? 0 : int.Parse(row["C_Follower"].ToString());
                    row1.Cells[stt++].Value = row["C_Topic"].ToString();
                    row1.Cells[stt++].Value = row["C_Folder"].ToString();
                    row1.Cells[stt++].Value = row["C_Status"].ToString();
                    row1.Cells[stt++].Value = row["C_Video1"].ToString() == "" ? 0 : int.Parse(row["C_Video1"].ToString());
                    row1.Cells[stt++].Value = row["C_Video2"].ToString() == "" ? 0 : int.Parse(row["C_Video2"].ToString());
                    row1.Cells[stt++].Value = row["C_Video3"].ToString() == "" ? 0 : int.Parse(row["C_Video3"].ToString());
                    row1.Cells[stt++].Value = row["C_Video4"].ToString() == "" ? 0 : int.Parse(row["C_Video4"].ToString());
                    row1.Cells[stt++].Value = row["C_Video5"].ToString() == "" ? 0 : int.Parse(row["C_Video5"].ToString());
                    row1.Cells[stt++].Value = row["C_Video"].ToString() == "" ? 0 : int.Parse(row["C_Video"].ToString());
                    row1.Cells[stt++].Value = row["C_View"].ToString() == "" ? 0 : int.Parse(row["C_View"].ToString());
                    row1.Cells[stt++].Value = row["C_UserAgent"].ToString();
                    row1.Cells[stt++].Value = row["C_GPMID"].ToString();
                    row1.Cells[stt++].Value = row["C_Proxy"].ToString();
                    rows.Add(row1);
                }

                Form1.tblMain.Invoke(new MethodInvoker(delegate
                {
                    Form1.tblMain.Rows.Clear();

                    Form1.tblMain.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                    Form1.tblMain.ColumnHeadersVisible = false;
                    Form1.tblMain.RowHeadersVisible = false;
                    Form1.tblMain.Rows.AddRange(rows.ToArray());
                    Form1.tblMain.ColumnHeadersVisible = true;
                    Form1.tblMain.RowHeadersVisible = true;
                }));
            }
            catch
            {
                //
            }
        }
        public void LoadDataIntoComboBox()
        {
            try
            {
                Form1._listTopic = new List<string>();
                var table = Select("SELECT * FROM tbl_topic").Tables[0];
                Form1._cbbTopic.Items.Clear();
                //Form1._cbbTopic.DisplayMember = table.Columns["C_Topic"].ToString();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Form1._cbbTopic.Items.Add(table.Rows[i]["C_Topic"].ToString());
                    Form1._listTopic.Add(table.Rows[i]["C_Topic"].ToString());
                }
            }
            catch { }
        }
    }

}
