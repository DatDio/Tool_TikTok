using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_TikTok.Models
{
	public class AccountModel
	{
		[JsonIgnore]
		public ChromeDriver driver { get; set; } = null;
		[JsonIgnore]
		public FirefoxDriver firefoxDriver { get; set; } = null;
		public string C_Account { get; set; } = "";
		public string C_Email { get; set; } = "";
		public string C_PassEmail { get; set; } = "";
		public string C_PassTikTok { get; set; } = "";
		public string C_Url { get; set; } = "";
		public string C_Cookie { get; set; } = "";
		public string C_Country { get; set; } = "";
		public string C_ChanelName { get; set; } = "";
		public int C_Follower { get; set; } = 0;
		public string C_Topic { get; set; } = "";
		public string C_Folder { get; set; } = "";
		public int C_Video1 { get; set; } = 0;
		public int C_Video2 { get; set; } = 0;
		public int C_Video3 { get; set; } = 0;
		public int C_Video4 { get; set; } = 0;
		public int C_Video5 { get; set; } = 0;
		public int C_Video { get; set; } = 0;
		public int C_View { get; set; } = 0;
		public int C_Like { get; set; } = 0;
		public string C_UserAgent { get; set; } = "";
		public string C_GPMID { get; set; } = "";
		public string C_Proxy { get; set; } = "";
		public string C_Status { get; set; } = "";
		public string C_TypeBrowser { get; set; } = "";
		[JsonIgnore]
		public DataGridViewRow C_Row { get; set; }
	}
}
