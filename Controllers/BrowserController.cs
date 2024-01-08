using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tool_TikTok.Helpers;
using Tool_TikTok.Models;

namespace Tool_TikTok.Controllers
{
	public class BrowserController
	{
		ChromeDriver driver;
		FirefoxDriver firefoxDriver;
		AccountModel account;
		//public string createdProfileId;
		private GPMLoginAPI api;
		private GPMLoginAPIV3 apiV3;
		public BrowserController(AccountModel account)
		{
			this.account = account;
		}
		public ChromeDriver OpenChromeGpmV3(string apiGpm, string createdProfileId, string name, string useragent = "", double scale = 0.7, string proxy = "", bool hideBrowser = false, string position = "0,0")
		{

			apiV3 = new GPMLoginAPIV3(apiGpm);
			if (createdProfileId == "")
			{
				createdProfileId = apiV3.Create(name);
				if (createdProfileId != null)
				{
					account.C_GPMID = createdProfileId;
					account.C_TypeBrowser = Form1._rdoChrome ? "chromium" : "firefox";
					FunctionHelper.EditValueColumn(account, "C_TypeBrowser", createdProfileId);
					FunctionHelper.EditValueColumn(account, "C_GPMID", createdProfileId, true);
				}
			}

			//if (string.IsNullOrEmpty(position))
			//{
			//	position = GetNewPosition(800, 800, scale);
			//}

			var arg = $"--window-position={position} --window-size=800,800 --force-device-scale-factor={scale}";

			//var arg = $"win_scale=0.5$win_pos=300,300$win_size=400,400";

			if (useragent != "")
			{
				arg += $" --user-agent=\"{useragent}\"";
			}

			if (hideBrowser)
			{
				arg += $" --headless";
			}

			apiV3.Update(name, createdProfileId, proxy);
			var startedResult = apiV3.Start(createdProfileId, arg);
			//apiV3.Close(createdProfileId);
			var browserLocation = RegexHelper.GetValueFromGroup("\"browser_location\":\"(.*?)\",", startedResult);
			var seleniumRemoteDebugAddress = RegexHelper.GetValueFromGroup("\"remote_debugging_address\":\"(.*?)\",", startedResult);
			var gpmDriverPath = RegexHelper.GetValueFromGroup("\"driver_path\":\"(.*?)\"", startedResult);

			if (gpmDriverPath == "")
			{
				//createdProfileId = "";
				//goto CreateProfile;
				return null;
			}

			var gpmDriverFileInfo = new FileInfo(gpmDriverPath);

			var service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions
			{
				BinaryLocation = browserLocation,
				DebuggerAddress = seleniumRemoteDebugAddress
			};

			driver = new ChromeDriver(service, options);

			return driver;
		}
		public FirefoxDriver OpenChromeGpmV3FireFox(string apiGpm, string createdProfileId, string name, string useragent = "", double scale = 0.7, string proxy = "", bool hideBrowser = false, string position = "0,0")
		{
			apiV3 = new GPMLoginAPIV3(apiGpm);
			if (createdProfileId == "")
			{
				createdProfileId = apiV3.Create(name);
				if (createdProfileId != null)
				{
					account.C_GPMID = createdProfileId;
					account.C_TypeBrowser = "firefox";
					FunctionHelper.EditValueColumn(account, "C_TypeBrowser", createdProfileId);
					FunctionHelper.EditValueColumn(account, "C_GPMID", createdProfileId, true);
				}
			}

			//if (string.IsNullOrEmpty(position))
			//{
			//	position = GetNewPosition(800, 800, scale);
			//}

			var arg = $"--window-position={position} --window-size=400,400 --force-device-scale-factor={scale}";

			//var arg = $"win_scale=0.5$win_pos=300,300$win_size=400,400";

			if (useragent != "")
			{
				arg += $" --user-agent=\"{useragent}\"";
			}

			if (hideBrowser)
			{
				arg += $" --headless";
			}

			apiV3.Update(name, createdProfileId, proxy);
			var startedResult = apiV3.Start(createdProfileId, arg);
			//apiV3.Close(createdProfileId);
			//var browserLocation = RegexHelper.GetValueFromGroup("\"browser_location\":\"(.*?)\",", startedResult).Replace("\\\\","\\");
			var browserLocation = "C:\\Users\\ADMIN\\AppData\\Local\\Programs\\GPMLogin\\gpm_browser\\gpm_browser_firefox_core_119\\firefox.exe";
			var seleniumRemoteDebugAddress = RegexHelper.GetValueFromGroup("\"remote_debugging_address\":\"(.*?)\",", startedResult).Replace("\\\\", "\\");
			//var gpmDriverPath = RegexHelper.GetValueFromGroup("\"driver_path\":\"(.*?)\"", startedResult).Replace("\\\\", "\\");
			var gpmDriverPath = "C:\\Users\\ADMIN\\AppData\\Local\\Programs\\GPMLogin\\gpm_browser\\gpm_browser_firefox_core_119\\gpmdriver.exe";
			if (gpmDriverPath == "")
			{
				//createdProfileId = "";
				//goto CreateProfile;
				return null;
			}

			var gpmDriverFileInfo = new FileInfo(gpmDriverPath);

			var service = FirefoxDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
			service.HideCommandPromptWindow = true;
			//service.DriverServicePath = gpmDriverPath;
			service.Host = "127.0.0.1";
			service.Port = int.Parse(seleniumRemoteDebugAddress.Split(':')[1]);
			var options = new FirefoxOptions
			{
				BinaryLocation = browserLocation,

				//LogLevel = FirefoxDriverLogLevel.Debug,
				EnableDevToolsProtocol = true,
				UseWebSocketUrl= true,
				//DebuggerAddress = seleniumRemoteDebugAddress

			};
			// Thêm cấu hình để kích hoạt Remote Debugging
			options.AddAdditionalOption("devtools.debugger.remote-enabled", true);
			options.AddAdditionalOption("devtools.debugger.prompt-connection", false);
			options.UseWebSocketUrl = false;
			//options.AddAdditionalOption("moz:debuggerAddress", seleniumRemoteDebugAddress);
			//options.AddArgument($"--remote-debugging-port={seleniumRemoteDebugAddress.Split(':')[1]}");
			firefoxDriver = new FirefoxDriver(service, options);

			return firefoxDriver;
		}
		public ChromeDriver OpenChromeGpm(string apiGpm, string createdProfileId, string name, string useragent = "", double scale = 0.7, string proxy = "", bool hideBrowser = false, string position = "0,0")
		{
			//this.createdProfileId = "";

			api = new GPMLoginAPI(apiGpm);

			if (createdProfileId == "")
			{
				var createdResult = api.Create(name, isNoiseCanvas: true);

				if (createdResult != null)
				{
					var status = Convert.ToBoolean(createdResult["status"]);
					if (status)
					{
						createdProfileId = Convert.ToString(createdResult["profile_id"]);
						account.C_GPMID = createdProfileId;
						FunctionHelper.EditValueColumn(account, "C_GPMID", createdProfileId, true);
					}

				}

				//this.createdProfileId = createdProfileId;
				//Console.WriteLine("Created profile ID: " + createdProfileId);
			}

			//if (string.IsNullOrEmpty(position))
			//{
			//	position = GetNewPosition(800, 800, scale);
			//}

			var arg = $"--window-position={position} --window-size=800,800 --force-device-scale-factor={scale} ";

			if (useragent != "")
			{
				arg += $" --user-agent=\"{useragent}\"";
			}

			if (hideBrowser)
			{
				arg += $" --headless";
			}

			api.UpdateProxy(createdProfileId, proxy.Replace("http://", ""));

			var startedResult = api.Start(createdProfileId, null, arg);

			var browserLocation = Convert.ToString(startedResult["browser_location"]);
			var seleniumRemoteDebugAddress = Convert.ToString(startedResult["selenium_remote_debug_address"]);
			var gpmDriverPath = Convert.ToString(startedResult["selenium_driver_location"]);

			if (gpmDriverPath == "")
			{
				//createdProfileId = "";
				//goto CreateProfile;
				return null;
			}

			var gpmDriverFileInfo = new FileInfo(gpmDriverPath);

			var service = ChromeDriverService.CreateDefaultService(gpmDriverFileInfo.DirectoryName, gpmDriverFileInfo.Name);
			service.HideCommandPromptWindow = true;
			var options = new ChromeOptions
			{
				BinaryLocation = browserLocation,
				DebuggerAddress = seleniumRemoteDebugAddress
			};

			driver = new ChromeDriver(service, options);

			return driver;
		}
		public ChromeDriver OpenChrome(string userAgent = "", double scale = 0.5, string position = "0,0")
		{
			if (string.IsNullOrEmpty(position))
			{
				position = GetNewPosition(600, 800, scale);
			}

			var chromeDriverService = ChromeDriverService.CreateDefaultService();
			chromeDriverService.HideCommandPromptWindow = true;
			var chromeOption = new ChromeOptions();
			chromeOption.AddArgument("--user-data-dir=" + Path.GetFullPath("profile/" + account.C_Email));

			if (account.C_UserAgent != "")
			{
				chromeOption.AddArgument("--user-agent=" + account.C_UserAgent);
			}
			if (userAgent != "")
				chromeOption.AddArgument($"--user-agent={userAgent}");
			chromeOption.AddArgument("--disable-web-security");
			chromeOption.AddArgument("--disable-rtc-smoothness-algorithm");
			chromeOption.AddArgument("--disable-webrtc-hw-decoding");
			chromeOption.AddArgument("--disable-webrtc-hw-encoding");
			chromeOption.AddArgument("--disable-webrtc-multiple-routes");
			chromeOption.AddArgument("--disable-webrtc-hw-vp8-encoding");
			chromeOption.AddArgument("--enforce-webrtc-ip-permission-check");
			chromeOption.AddArgument("--force-webrtc-ip-handling-policy");
			chromeOption.AddArgument("ignore-certificate-errors");
			chromeOption.AddArgument("disable-infobars");
			chromeOption.AddArgument("mute-audio");
			chromeOption.AddArgument("--disable-popup-blocking");
			chromeOption.AddArgument("--disable-plugins");
			chromeOption.AddArgument($"--force-device-scale-factor={scale}");
			chromeOption.AddArgument("--no-sandbox");
			chromeOption.AddArgument("--log-level=3");
			chromeOption.AddArgument("test-type=browser");
			chromeOption.AddExcludedArgument("enable-automation");
			chromeOption.AddUserProfilePreference("useAutomationExtension", false);
			chromeOption.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
			chromeOption.AddArgument("disable-blink-features=AutomationControlled");
			chromeOption.AddArgument("--window-size=600,800");
			chromeOption.AddArgument($"--window-position={position}");

			if (!string.IsNullOrEmpty(account.C_Proxy))
			{
				chromeOption.AddArgument("--proxy-server=" + account.C_Proxy);
			}

			driver = new ChromeDriver(chromeDriverService, chromeOption);
			driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 10, 0);

			return driver;
		}
		public static string GetNewPosition(int w, int h, double scale = 1)
		{
			var current_size = "";

			lock (Form1.lockChrome)
			{
				current_size = $"{Form1.CurrentWidth},{Form1.CurrentHeight}";

				Form1.CurrentWidth += w;

				if (Form1.CurrentWidth + w >= Screen.PrimaryScreen.Bounds.Width / scale)
				{
					Form1.CurrentWidth = 0;
					Form1.CurrentHeight += h;
				}

				if (Form1.CurrentHeight + h >= Screen.PrimaryScreen.Bounds.Height / scale)
				{
					Form1.CurrentWidth = 0;
					Form1.CurrentHeight = 0;
				}
			}

			return current_size;
		}
		public void CloseChrome()
		{
			try
			{
				var windowHandles = driver.WindowHandles;
				foreach (var handle in windowHandles)
				{
					driver.SwitchTo().Window(handle);
					driver.Close();
				}

				driver.Quit();
			}
			catch { }
			try
			{
				api.Stop(account.C_GPMID);
			}
			catch
			{

			}
		}
	}
}
