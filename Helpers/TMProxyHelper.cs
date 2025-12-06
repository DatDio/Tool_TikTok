using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool_TikTok.Helpers
{
	public class TMProxyHelper
	{
		public static string GetCurrentProxy(string api)
		{
			using (var rq = new HttpRequest())
			{

				var body = rq.Post("https://tmproxy.com/api/proxy/get-current-proxy", $"{{ \"api_key\": \"{api}\" }}", "application/json").ToString();
				var proxy = RegexHelper.GetValueFromGroup("\"https\":\"(.*?)\"", body);
				return proxy;
			}

		}

		public static string GetNewProxy(string api)
		{
			using (var rq = new HttpRequest())
			{

				var body = rq.Post("https://tmproxy.com/api/proxy/get-new-proxy", $"{{ \"api_key\": \"{api}\" }}", "application/json").ToString();
				var proxy = RegexHelper.GetValueFromGroup("\"https\":\"(.*?)\"", body);
				if (proxy != "")
				{
					return proxy;
				}
				else
				{
					body = rq.Post("https://tmproxy.com/api/proxy/get-current-proxy", $"{{ \"api_key\": \"{api}\" }}", "application/json").ToString();
					proxy = RegexHelper.GetValueFromGroup("\"https\":\"(.*?)\"", body);
					if (proxy != "")
					{
						return proxy;
					}
					return "";
				}
			}
		}
	}
}
