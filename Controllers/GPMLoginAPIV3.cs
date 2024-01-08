using Leaf.xNet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Tool_TikTok.Helpers;

namespace Tool_TikTok.Controllers
{
	public class GPMLoginAPIV3
	{
		private string _apiUrl;
		public GPMLoginAPIV3(string apiUrl)
		{
			if (apiUrl.EndsWith("/"))
				apiUrl = apiUrl.Substring(0, apiUrl.Length - 1);
			_apiUrl = apiUrl;
		}
		public string Create(string name)
		{
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.KeepAlive = true;
				string browser = Form1._rdoChrome ? "chromium" : "firefox";

				//rq.AddHeader("Content-Type", "application/json");
				try
				{
					var content = $@"{{
    ""profile_name"" : ""{name}"",
    ""browser_core"": ""{browser}"",
 ""browser_type"": ""{browser}""
}}";

					string body = rq.Post($"{_apiUrl}/api/v3/profiles/create", content, "application/json").ToString();
					var status = RegexHelper.GetValueFromGroup("\"success\": (.*?),", body);
					if (status == "true")
					{
						return RegexHelper.GetValueFromGroup("\"id\":\"(.*?)\"", body);
					}
				}
				catch
				{
					var body = rq.Response.ToString();
				}
				return null;
			}
		}
		public bool Update(string name, string proID, string proxy)
		{
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.KeepAlive = true;
				rq.AddHeader("Content-Type", "application/json");
				try
				{
					var content = $@"{{
    ""profile_name"" : ""{name}"",
    ""raw_proxy"" : ""{proxy}"",
    ""note"": """",
    ""color"": ""COLOR_HEX"",
    ""user_agent"": ""auto""
}}";

					string body = rq.Post($"{_apiUrl}/api/v3/profiles/update/{proID}", content, "application/json").ToString();
					var status = RegexHelper.GetValueFromGroup("\"success\": (.*?),", body);
					if (status == "true")
					{
						return true;
					}
				}
				catch
				{
					var body = rq.Response.ToString();
				}
				return false;
			}
		}
		public string Start(string proID, string arguments)
		{
			//arguments = "-width 400 -height 600";
			//arguments = "win_scale=0.8&win_pos=300,300";

			using (var rq = new HttpRequest())
			{
				try
				{
					string body = rq.Get($@"{_apiUrl}/api/v3/profiles/start/{proID}?addination_args={arguments}").ToString();
					var status = RegexHelper.GetValueFromGroup("\"success\": (.*?),", body);
					if (status == "true")
					{
						return body;
					}
				}
				catch
				{
					var body = rq.Response.ToString();
				}
				return null;

			}
		}
		public bool Close(string proID)
		{
			using (var rq = new HttpRequest())
			{
				rq.AllowAutoRedirect = true;
				rq.KeepAlive = true;
				try
				{
					string body = rq.Get($"{_apiUrl}/api/v3/profiles/close/{proID}").ToString();
					var status = RegexHelper.GetValueFromGroup("\"success\": (.*?),", body);
					if (status == "true")
					{
						return true;
					}
				}
				catch
				{
					var body = rq.Response.ToString();
				}
				return false;
			}
		}
		public bool Delete(string proID)
		{
			using (var rq = new HttpRequest())
			{
				try
				{
					string body = rq.Get($"{_apiUrl}/api/v3/profiles/delete/{proID}").ToString();
					var status = RegexHelper.GetValueFromGroup("\"success\": (.*?),", body);
					if (status == "true")
					{
						return true;
					}
				}
				catch
				{
					var body = rq.Response.ToString();
				}
				return false;
			}
		}
		private string httpRequest(string url)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					using (Stream stream = response.GetResponseStream())
					{
						using (StreamReader reader = new StreamReader(stream))
						{
							return reader.ReadToEnd();
						}
					}
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
