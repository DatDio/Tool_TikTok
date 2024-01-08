using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_TikTok.Models
{
	public class CookieModel
	{
		public string name { get; set; }
		public string value { get; set; }
		public string domain { get; set; }
		public string path { get; set; }
		public bool secure { get; set; }
		public bool httpOnly { get; set; }
		public string sameSite { get; set; }
		public int expiry { get; set; }
	}
}
