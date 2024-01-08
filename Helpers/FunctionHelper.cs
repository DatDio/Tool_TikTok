using Leaf.xNet;
using MediaInfo.DotNetWrapper.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_TikTok.Controllers;
using Tool_TikTok.Models;
using MediaInfo.DotNetWrapper;
using WMPLib;
using System.Text.RegularExpressions;
using System.Threading;
using MailKit;
using MailKit.Search;

namespace Tool_TikTok.Helpers
{
    public class FunctionHelper
    {
        public static string GenerateRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] randomString = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomString[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomString);
        }
        public static List<int> GetRowIndexSelected(DataGridView dataGridView, bool sort_ascending = false)
        {
            // Biến selectedcells để lưu các ô đã chọn trong datagridview
            var selectedCells = dataGridView.SelectedCells;

            // Tạo một danh sách các chỉ mục dòng cần xóa
            List<int> rowsToRemove = new List<int>();

            // Lặp qua các ô được chọn để lấy index dòng 
            foreach (DataGridViewCell cell in selectedCells)
            {
                if (cell.RowIndex == 0)
                {
                    continue;
                }
                //Nếu trong list rowstoremove không chứa dòng đó thì thêm chỉ số dòng đó vào list
                if (!rowsToRemove.Contains(cell.RowIndex))
                {
                    rowsToRemove.Add(cell.RowIndex);
                }
            }

            //Sắp xếp các dòng trong list
            if (sort_ascending)
                rowsToRemove.Sort((x, y) => x.CompareTo(y)); //tăng dần
            else
                rowsToRemove.Sort((x, y) => y.CompareTo(x)); //giảm dần

            return rowsToRemove;
        }
        public static List<DataGridViewRow> GetRowSelected(DataGridView dataGridView, bool sort_ascending = false)
        {
            // Biến selectedcells để lưu các ô đã chọn trong datagridview
            var selectedCells = dataGridView.SelectedCells;

            // Tạo một danh sách các chỉ mục dòng cần xóa
            List<int> rowsToRemove = new List<int>();
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            // Lặp qua các ô được chọn để lấy index dòng 
            foreach (DataGridViewCell cell in selectedCells)
            {
                //Nếu trong list rowstoremove không chứa dòng đó thì thêm chỉ số dòng đó vào list
                if (!rowsToRemove.Contains(cell.RowIndex))
                {
                    rowsToRemove.Add(cell.RowIndex);
                }
            }

            //Sắp xếp các dòng trong list
            if (sort_ascending)
                rowsToRemove.Sort((x, y) => x.CompareTo(y)); //tăng dần
            else
                rowsToRemove.Sort((x, y) => y.CompareTo(x)); //giảm dần

            // từ danh sách các chỉ số dòng, lấy ra được danh sách các dòng
            for (int i = 0; i < rowsToRemove.Count; i++)
            {
                rows.Add(dataGridView.Rows[rowsToRemove[i]]);
            }

            return rows;
        }
        public static List<DataGridViewRow> GetRowChecked(DataGridView dataGridView)
        {
            List<DataGridViewRow> rows_checked = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                try
                {
                    if (row.Cells["C_Check"].Value == null)
                        continue;

                    if ((bool)row.Cells["C_Check"].Value == true)
                        rows_checked.Add(row);
                }
                catch { }
            }

            return rows_checked;
        }
        public static void EditValueColumn(AccountModel account, string column, string content, bool update = false)
        {
            try
            {
                account.C_Row.Cells[column].Value = content;
            }
            catch
            {
            }

            if (update)
            {
                SetPropertyValue(account, column, content);
                lock (Form1.lockDatabase)
                {
                    Form1.sqlController.Update(account);
                }
            }
        }
        public static void EditValueColumnTypeInt(AccountModel account, string column, int content, bool update = false)
        {
            try
            {
                account.C_Row.Cells[column].Value = content;
            }
            catch
            {
            }

            if (update)
            {
                SetPropertyValue(account, column, content);
                lock (Form1.lockDatabase)
                {
                    Form1.sqlController.Update(account);
                }
            }
        }
        public static void SetPropertyValue(AccountModel account, string propertyName, object value)
        {
            PropertyInfo propertyInfo = account.GetType().GetProperty(propertyName);

            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                propertyInfo.SetValue(account, value);
            }
        }
        public static int RandomMinutes(int min, int max)
        {
            Random random = new Random();
            int randomNumber;
            do
            {
                // Sinh một số ngẫu nhiên trong khoảng từ 5 đến 50
                randomNumber = random.Next(min, max + 1);
            } while (randomNumber % 5 != 0);
            return randomNumber;
        }
        public static string GetVideoFolder(string folder)
        {
            try
            {
                var files = Directory.GetFiles(Path.GetFullPath("Video/" + folder));
                foreach (var file in files)
                {
                    if (!file.Contains("DONE_") && !file.Contains("ERORR_"))
                    {
                        return file;
                    }
                }
            }
            catch
            {
                MessageBox.Show($"Không tìm thấy thư mục {folder}");
            }
            //foreach(var file in files)
            //{
            //	if (!file.Contains("DONE_") && !Form1._listVideoUsing.Contains(file))
            //	{
            //		Form1._listVideoUsing.Add(file);
            //		return file;
            //	}
            //}
            return "";
        }
        public static string GetFolderNotUsing(string email)
        {
            lock (Form1.lockfolder)
            {
                var folders = Directory.GetDirectories(Path.GetFullPath("Video"));
                foreach (var folder in folders)
                {
                    if (!folder.Contains("@"))
                    {
                        try
                        {
                            Directory.Move(Path.GetFullPath(folder), Path.GetFullPath("Video\\" + email));
                            return email;
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return "";
        }
        public static void AddHeaderxNet(HttpRequest rq, string Header)
        {
            var header = Header.Replace("\r", "").Split('\n');
            foreach (var line in header)
            {
                try
                {
                    var arr = line.Split(":".ToCharArray(), 2);
                    rq.AddHeader(arr[0].Trim(), arr[1].Trim());
                }
                catch { }
            }
        }
        public static void SetCookieToRequestXnet(HttpRequest rq, string cookie)
        {
            try
            {
                var cookieModels = JsonConvert.DeserializeObject<List<CookieModel>>(cookie);
                rq.Cookies = new CookieStorage();
                foreach (var ck in cookieModels)
                {
                    try
                    {
                        rq.Cookies.Add(new System.Net.Cookie(ck.name, ck.value, ck.path, ck.domain));
                    }
                    catch { }
                }
            }
            catch
            {

            }
        }
        public static ProxyClient ConvertToProxyClient(string proxy)
        {
            if (string.IsNullOrEmpty(proxy))
                return null;

            if (proxy.Contains("socks5://"))
            {
                proxy = proxy.Replace("socks5://", "");
                var proxies = proxy.Split(':');
                if (proxies.Length > 2)
                {
                    return new Socks5ProxyClient(proxies[0], int.Parse(proxies[1]), proxies[2], proxies[3]);
                }
                else
                {
                    return new Socks5ProxyClient(proxies[0], int.Parse(proxies[1]));
                }
            }
            else
            {
                proxy = proxy.Replace("http://", "");
                var proxies = proxy.Split(':');
                if (proxies.Length > 2)
                {
                    return new HttpProxyClient(proxies[0], int.Parse(proxies[1]), proxies[2], proxies[3]);
                }
                else
                {
                    return new HttpProxyClient(proxies[0], int.Parse(proxies[1]));
                }
            }

        }
        public static int ConvertToInt(string input)
        {
            if (input.Contains("K"))
            {
                input = input.Replace("K", "");
                input = input.Replace(".", "");
                //var output = double.Parse(input);
                return int.Parse(input) * 100;
            }
            else if (input.Contains("M"))
            {
                input = input.Replace("M", "");
                input = input.Replace(".", "");
                //var output = double.Parse(input) ;
                return int.Parse(input) * 100000;
            }
            return int.Parse(input);
        }
        public static Double GetDuration(String file)
        {
            WindowsMediaPlayer wmp = new WindowsMediaPlayer();
            IWMPMedia mediainfo = wmp.newMedia(file);
            return mediainfo.duration / 60;
        }
        public static AccountModel ConvertRowToModel(DataGridViewRow row)
        {
            try
            {
                return new AccountModel
                {
                    C_Account = row.Cells["C_Account"].Value.ToString() ?? "",
                    C_Email = row.Cells["C_Email"].Value.ToString() ?? "",
                    C_PassEmail = row.Cells["C_PassEmail"].Value.ToString() ?? "",
                    C_PassTikTok = row.Cells["C_PassTikTok"].Value.ToString() ?? "",
                    C_Url = row.Cells["C_Url"].Value.ToString() ?? "",
                    C_Cookie = row.Cells["C_Cookie"].Value.ToString() ?? "",
                    C_Country = row.Cells["C_Country"].Value.ToString() ?? "",
                    C_ChanelName = row.Cells["C_ChanelName"].Value.ToString() ?? "",
                    C_Follower = int.Parse(row.Cells["C_Follower"].Value.ToString()),
                    C_Topic = row.Cells["C_Topic"].Value.ToString() ?? "",
                    C_Folder = row.Cells["C_Folder"].Value.ToString() ?? "",
                    C_Video1 = int.Parse(row.Cells["C_Video1"].Value.ToString()),
                    C_Video2 = int.Parse(row.Cells["C_Video2"].Value.ToString()),
                    C_Video3 = int.Parse(row.Cells["C_Video3"].Value.ToString()),
                    C_Video4 = int.Parse(row.Cells["C_Video4"].Value.ToString()),
                    C_Video5 = int.Parse(row.Cells["C_Video5"].Value.ToString()),
                    C_Video = int.Parse(row.Cells["C_Video"].Value.ToString()),
                    C_View = int.Parse(row.Cells["C_View"].Value.ToString()),
                    C_Status = row.Cells["C_Status"].Value.ToString() ?? "",
                    C_UserAgent = row.Cells["C_UserAgent"].Value.ToString() ?? "",
                    C_GPMID = row.Cells["C_GPMID"].Value.ToString() ?? "",
                    C_Proxy = row.Cells["C_Proxy"].Value.ToString() ?? "",
                    C_Row = row
                };
            }
            catch
            {
                return null;
            }

        }
        public static string GetCode(string email, string pass)
        {
            string code = "", to = email, passOld = pass;

            for (var i = 0; i < 10; i++)
            {
                Thread.Sleep(5000);

                try
                {
                    using (var client = new MailKit.Net.Imap.ImapClient())
                    {
                        client.Connect("outlook.office365.com", 993);
                        try
                        {
                            client.Authenticate(email, pass);
                        }
                        catch
                        {
                            return "die";
                        }

                        //đọc thư trong inbox
                        client.Inbox.Open(FolderAccess.ReadWrite);
                        var uids = client.Inbox.Search(
                            SearchQuery.SentSince(DateTime.Now.AddHours(-24))
                                .And(SearchQuery.NotSeen)
                                .And(SearchQuery.FromContains("noreply@account.tiktok.com"))
                                .And(SearchQuery.ToContains(to.ToLower())));

                        foreach (var uid in uids)
                        {
                            var message = client.Inbox.GetMessage(uid);
                            var text = message.HtmlBody;
                            var match = Regex.Match(message.HtmlBody, ">(.*?)<\\/p>\r\n      <p style=\"margin-bottom:20px;\">Verification codes expire after 48 hours.<\\/p>");
                            if (match.Length == 0)
                            {
                                match = Regex.Match(message.HtmlBody, "<\\/p>\r\n      <p style=\"margin-bottom:20px;color: rgb\\(22,24,35\\);font-weight: bold;\">(.*?)<\\/p>");
                            }
                            if (match.Success)
                            {
                                code = match.Groups[1].Value;
                                //	continue;
                            }

                            try
                            {
                                client.Inbox.SetFlags(uid, MessageFlags.Deleted, true);
                                client.Inbox.Expunge(new List<UniqueId> { uid });
                            }
                            catch
                            {
                                //
                            }
                        }

                        if (string.IsNullOrEmpty(code))
                        {
                            foreach (var subfolder in client.GetFolders(client.PersonalNamespaces[0]))
                            {
                                try
                                {
                                    client.GetFolder(subfolder.FullName).Open(FolderAccess.ReadWrite);

                                    uids = client.GetFolder(subfolder.FullName).Search(SearchQuery.NotSeen
                                        .And(SearchQuery.SentSince(DateTime.Now.AddHours(-24)))
                                        .And(SearchQuery.FromContains("noreply@account.tiktok.com"))
                                        .And(SearchQuery.ToContains(to.ToLower())));

                                    foreach (var uid in uids)
                                    {
                                        var message = client.GetFolder(subfolder.FullName).GetMessage(uid);
                                        var match = Regex.Match(message.HtmlBody, ">(.*?)<\\/p>\r\n      <p style=\"margin-bottom:20px;\">Verification codes expire after 48 hours.<\\/p>");
                                        if (match.Length == 0)
                                        {
                                            match = Regex.Match(message.HtmlBody, "<\\/p>\r\n      <p style=\"margin-bottom:20px;color: rgb\\(22,24,35\\);font-weight: bold;\">(.*?)<\\/p>");
                                        }
                                        if (match.Success)
                                        {
                                            code = match.Groups[1].Value;
                                            //	continue;
                                        }
                                        try
                                        {
                                            client.GetFolder(subfolder.FullName).SetFlags(uid, MessageFlags.Deleted, true);
                                            client.GetFolder(subfolder.FullName).Expunge(new List<UniqueId> { uid });
                                        }
                                        catch
                                        {
                                            //
                                        }
                                    }
                                }
                                catch
                                {
                                    //Console.Out.WriteLine(e.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message);
                    if (e.Message.ToLower().Contains("no login failed"))
                    {
                        return "die";
                    }
                }
                if (!string.IsNullOrEmpty(code))
                {
                    break;
                }
            }
            Console.WriteLine("code = " + code);
            return code;
        }
        public static DateTime TimeStampToDate(int epoch)
        {
            if (epoch == 0)
            {
                return DateTime.UtcNow.AddDays(10);
            }
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
        }
        public static string GenerateUsername()
        {
            return Form1._listUserName[Form1.random.Next(Form1._listUserName.Count)] + Form1._listUserName[Form1.random.Next(Form1._listUserName.Count)] + Form1.random.Next(1000, 9999999);
        }
    }
}

