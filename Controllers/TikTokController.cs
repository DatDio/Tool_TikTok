using Leaf.xNet.Services.Captcha;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool_TikTok.Helpers;
using Tool_TikTok.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Tool_TikTok.Controllers
{
	public class TikTokController
	{
		AccountModel account;
		Random random;
		public TikTokController(AccountModel account)
		{
			this.account = account;
			random = new Random();
		}
		public ResultModel Login()
		{
			FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang login ...");
			try
			{
				account.driver.Url = "https://www.tiktok.com/404";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			int count = 0;
		reStartLogin:
			count++;
			SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"modal-close-inner-button\"]"), 2);
			Thread.Sleep(1000);
			if (!SeleniumHelper.Click(account.driver, By.Id("header-login-button"), 15))
			{
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang check acc lỗi không ...");
				Thread.Sleep(1000);
				try
				{
					var urlUpload = account.driver.Url = "https://www.tiktok.com/tiktokstudio/upload?from=upload";
					if (SeleniumHelper.UrlChange(account.driver, urlUpload, 10))
					{
						if (account.driver.Url == "https://www.tiktok.com/creator-center/ineligible")
							return ResultModel.ErorrAcc;
						else
							return ResultModel.Fail;
					}
					else
						return ResultModel.Success;
				}
				catch
				{
					return ResultModel.Success;
				}

			}
			Thread.Sleep(3000);
			//Giao diện có các hàng phương thức đăng nhập
			if (!SeleniumHelper.WaitElement(account.driver, By.CssSelector("button[data-list-item-value=\"email/username\"]"), 8))
			{
				if (SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"channel-item\"]"), count: 1))
				{
					Thread.Sleep(random.Next(1000, 3000));
					//Bấm sang đăng nhập bằng email

					SeleniumHelper.Click(account.driver, By.CssSelector("a[href=\"/login/phone-or-email/email\"]"));
					goto interface1;
				}
				else
				{
					return ResultModel.Fail;
				}
			}
			//Giao Diện có 2 login fb, gg ở dưới
			if (!SeleniumHelper.Click(account.driver, By.CssSelector("button[data-list-item-value=\"email/username\"]"), 10))
				return ResultModel.Fail;
			interface1:
			Thread.Sleep(random.Next(1000, 3000));
			FunctionHelper.EditValueColumn(account, "C_Status", "Nhập email ...");
			if (!SeleniumHelper.SendKeys(account.driver, By.Name("username"), account.C_Email))
			{
				if (account.driver.Url == "https://www.tiktok.com/foryou?lang=en")
					return ResultModel.Success;
			}
			Thread.Sleep(random.Next(1000, 3000));
			FunctionHelper.EditValueColumn(account, "C_Status", "Nhập password ...");
			SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"password\"]"), account.C_PassTikTok);
			//var url = account.driver.Url;
			FunctionHelper.EditValueColumn(account, "C_Status", "Bấm login ...");
			for (int i = 0; i < 15; i++)
			{
				SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"login-button\"]"), 1);
				while (SeleniumHelper.GetLenghtElement(account.driver, By.CssSelector("button[data-e2e=\"login-button\"]")) > 10)
				{
					if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 1))
					{
						goto Captcha;
					}
				}
				if (SeleniumHelper.GetTextElement(account.driver, By.CssSelector("span[role=\"status\"]")) == "Your account was currently suspended.")
				{
					return ResultModel.Suspended;
				}
			}
		Captcha:
			if (SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("captcha_verify_bar"), 30))
			{
				if (!SeleniumHelper.WaitElementHidden(account.driver, By.Id("header-login-button"), 10))
				{
					if (count == 3)
						return ResultModel.Fail;
					goto reStartLogin;
				}
				else
				{
					try
					{
						Thread.Sleep(1000);
						FunctionHelper.EditValueColumn(account, "C_Status", "Đang check acc lỗi không ...");
						var urlUpload = account.driver.Url = "https://www.tiktok.com/tiktokstudio/upload?from=upload";
						if (SeleniumHelper.UrlChange(account.driver, urlUpload, 10))
						{
							return ResultModel.ErorrAcc;
						}
						else
							return ResultModel.Success;
					}
					catch
					{
						return ResultModel.Fail;
					}
				}
			}
			else
			{
				if (count == 3)
					return ResultModel.Fail;
				SeleniumHelper.Click(account.driver, By.ClassName("captcha_verify_bar--close"));
				goto reStartLogin;
			}
		}
		public ResultModel LoginByCookie()
		{
			FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang login ...");
			try
			{
				account.driver.Url = "https://www.tiktok.com/messages";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			var cookieModels = JsonConvert.DeserializeObject<List<CookieModel>>(account.C_Cookie);

			foreach (var cookie in cookieModels)
			{
				var ck = new Cookie(cookie.name, cookie.value, cookie.domain, cookie.path, FunctionHelper.TimeStampToDate(cookie.expiry), cookie.secure, cookie.httpOnly, cookie.sameSite);

				account.driver.Manage().Cookies.AddCookie(ck);
			}
			//Debug.WriteLine(account.driver.Manage().Cookies.GetCookieNamed("name"));
			try
			{
				account.driver.Url = "https://www.tiktok.com/messages";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			if (account.driver.Url.StartsWith("https://www.tiktok.com/messages"))
			{
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang check acc lỗi không ...");
				Thread.Sleep(1000);
				try
				{
					var urlUpload = account.driver.Url = "https://www.tiktok.com/creator-center/upload?from=upload";
					if (SeleniumHelper.UrlChange(account.driver, urlUpload, 10))
					{
						if (account.driver.Url == "https://www.tiktok.com/creator-center/ineligible")
							return ResultModel.ErorrAcc;
						else
							return ResultModel.Fail;
					}
					else
						return ResultModel.Success;
				}
				catch
				{
					return ResultModel.Success;
				}
			}
			else
			{
				return ResultModel.Fail;
			}

			//if (!SeleniumHelper.Click(account.driver, By.Id("header-login-button")))
			//{
			//    FunctionHelper.EditValueColumn(account, "C_Status", "Đang check acc lỗi không ...");
			//    Thread.Sleep(1000);
			//    try
			//    {
			//        var urlUpload = account.driver.Url = "https://www.tiktok.com/creator-center/upload?from=upload";
			//        if (SeleniumHelper.UrlChange(account.driver, urlUpload, 10))
			//        {
			//            if (account.driver.Url == "https://www.tiktok.com/creator-center/ineligible")
			//                return ResultModel.ErorrAcc;
			//            else
			//                return ResultModel.Fail;
			//        }
			//        else
			//            return ResultModel.Success;
			//    }
			//    catch
			//    {
			//        return ResultModel.Success;
			//    }
			//}

		}
		public ResultModel UpVideo(string path, DateTime _dateOneThread, ref bool clickAnotherVideo, int i)
		{
			try
			{
				int count = 0;
			reStart:
				FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Đến trang upload video ...");
				if (!clickAnotherVideo)
				{
					try
					{
						//	account.driver.Url = "https://www.tiktok.com/";
						var urlUpload = account.driver.Url = "https://www.tiktok.com/tiktokstudio/upload?from=upload";
						try
						{
							account.driver.SwitchTo().Alert().Accept();
						}
						catch
						{

						}
						FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Đang check acc lỗi không ...");
						if (SeleniumHelper.UrlChange(account.driver, urlUpload, 10))
						{
							if (account.driver.Url == "https://www.tiktok.com/creator-center/ineligible")
								return ResultModel.ErorrAcc;
							else
								return ResultModel.NotLogin;
						}
					}
					catch
					{
						return ResultModel.WeakProxy;
					}
					try
					{
						account.driver.SwitchTo().Alert().Accept();
					}
					catch
					{

					}
					//SeleniumHelper.WaitElement(account.driver, By.CssSelector("iframe[data-tt=\"Upload_index_iframe\"]"), 60);
					//account.driver.SwitchTo().Frame(account.driver.FindElement(By.CssSelector("iframe[data-tt=\"Upload_index_iframe\"]")));
				}

				//Check supspend acc
				if (SeleniumHelper.WaitElement(account.driver, By.ClassName("main-text"), 5))
				{
					if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("main-text")) == "Feature unavailable")
						return ResultModel.Suspended;
				}
				FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Up video ...");
				if (!SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"file\"]"), path, 60))
				{
					count++;
					clickAnotherVideo = false;
					if (count == 2)
						return ResultModel.Fail;
					goto reStart;
				}


				//Kiểm tra xem đã sendkeys video dc chưa = nút selected file
				if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("file-select-button"), 5))
				{
					count++;
					clickAnotherVideo = false;
					if (count == 2)
						return ResultModel.Fail;
					goto reStart;
				}
				if (Form1._rdoUpVideoTimer)
				{
					//Đặt lịch
					if (!SeleniumHelper.Click(account.driver, By.CssSelector("input[value=\"schedule\"]"), 20))
						return ResultModel.Fail;
					if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[role=\"dialog\"]"), 10))
					{
						//if (account.driver.FindElement(By.ClassName("is-highlight")).GetAttribute("innerHTML") == "Allow")
						//{
						//	account.driver.FindElement(By.ClassName("is-highlight")).Click();
						//}
						SeleniumHelper.Click(account.driver, By.ClassName("TUXButton--primary"), 5, count: 1);
					}
					Thread.Sleep(random.Next(1000, 4000));
					//click ngày
					FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Chọn ngày ...");
					if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXTextInputCore-input"), count: 1))
						return ResultModel.Fail;
					Thread.Sleep(random.Next(1000, 4000));
					var elements = SeleniumHelper.FindElements(account.driver, By.ClassName("day"));
					foreach (var element in elements)
					{

						if (element.GetAttribute("innerHTML") == _dateOneThread.Day.ToString("D2"))
						{
							Thread.Sleep(random.Next(1000, 4000));
							account.driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
							element.Click();
							break;
						}
					}
					//click giờ,phút
					if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXTextInputCore-input")))
						return ResultModel.Fail;
					Thread.Sleep(random.Next(1000, 4000));
					//chọn giờ
					FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Chọn giờ ...");
					elements = SeleniumHelper.FindElements(account.driver, By.ClassName("tiktok-timepicker-left"));
					foreach (var element in elements)
					{
						var hour = _dateOneThread.Hour.ToString("D2");
						var texthour = element.GetAttribute("innerHTML");
						if (element.GetAttribute("innerHTML") == _dateOneThread.Hour.ToString("D2"))
						{
							account.driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
							element.Click();
							break;
						}
					}
					//chọn phút
					Thread.Sleep(random.Next(1000, 4000));
					if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXTextInputCore-input")))
						return ResultModel.Fail;
					Thread.Sleep(random.Next(1000, 4000));
					FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Chọn phút ...");
					elements = SeleniumHelper.FindElements(account.driver, By.ClassName("tiktok-timepicker-right"));
					foreach (var element in elements)
					{
						var hour = _dateOneThread.Minute.ToString("D2");
						var texthour = element.GetAttribute("innerHTML");
						if (element.GetAttribute("innerHTML") == _dateOneThread.Minute.ToString("D2"))
						{
							account.driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
							element.Click();
							break;
						}
					}
				}

				//	fileInput.SendKeys(path);
				if (!SeleniumHelper.WaitElement(account.driver, By.CssSelector("span[data-text=\"true\"]"), 300))
					return ResultModel.Fail;

				//Đợi upload 100%
				//for(int d = 0; d < 300; d++)
				//{
				//	if ((SeleniumHelper.GetTextElement(account.driver, By.ClassName("info-progress-num"))).Contains("100"))
				//	{
				//		break;
				//	}
				//	else
				//	{
				//		Thread.Sleep(1000);
				//	}
				//	if (d == 299)
				//	{
				//		return ResultModel.Fail;
				//	}
				//}

				//chọn quét bản quyền 
				Thread.Sleep(random.Next(1000, 4000));

				//Điền title video
				//SeleniumHelper.SendKeys(account.driver, By.CssSelector("span[data-text=\"true\"]"), Keys.Control + "A");
				//Thread.Sleep(1000);
				//SeleniumHelper.SendKeys(account.driver, By.CssSelector("span[data-text=\"true\"]"), Form1._titileVideo);
				//Thread.Sleep(3000);
				//Điền tag
				var tags = Form1._Tag.Split(',');
				FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Điền thẻ tag ...");
				foreach (var tag in tags)
				{
					SeleniumHelper.SendKeys(account.driver, By.CssSelector("span[data-text=\"true\"]"), $"#{tag}");
					Thread.Sleep(3000);
					SeleniumHelper.SendKeys(account.driver, By.CssSelector("span[data-text=\"true\"]"), Keys.Enter);
					Thread.Sleep(1000);
				}

				//bấm up video
				Thread.Sleep(random.Next(1000, 4000));
				if (!SeleniumHelper.WaitElement(account.driver, By.ClassName("success-info"), 400))
					return ResultModel.Fail;

				SeleniumHelper.Click(account.driver, By.ClassName("btn-post"), 5);

				//Bấm đăng khi lên lịch khác đăng luôn
				if (Form1._rdoUpVideoTimer)
				{
					if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXButton-label"), 5, 2))
						return ResultModel.Fail;
				}
				else
				{
					if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXButton-label"), 5, 2))
						return ResultModel.Fail;
				}

				FunctionHelper.EditValueColumn(account, "C_Status", $"[{i}] Đợi upload video ...");

				//Up video hẹn giờ
				if (Form1._rdoUpVideoTimer)
				{
					//if (SeleniumHelper.WaitElement(account.driver, By.ClassName("is-line"), 60))
					//{

					//	if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("is-highlight")) == "Continue")
					//	{
					//		SeleniumHelper.Click(account.driver, By.ClassName("is-highlight"));
					//	}

					//}
					if (SeleniumHelper.WaitElement(account.driver, By.ClassName("TUXModal"), 10))
					{
						if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("css-1z070dx"), 3) == "Continue")
						{
							if (!SeleniumHelper.Click(account.driver, By.ClassName("css-1z070dx"), 3))
								return ResultModel.Fail;
							clickAnotherVideo = true;
							return ResultModel.Success;
						}
						return ResultModel.Success;
					}

					// emphasis

					else if (SeleniumHelper.WaitElement(account.driver, By.ClassName("tiktok-modal__modal-wrapper"), 5))
					{

						if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("css-1z070dx"), 3) == "Upload another video")
						{
							if (!SeleniumHelper.Click(account.driver, By.ClassName("css-1z070dx"), 5, 3))
								return ResultModel.Fail;
							clickAnotherVideo = true;
							return ResultModel.Success;
						}
						return ResultModel.Success;
					}
				}
				//Up luôn ko đặt lịch
				else
				{
					if (SeleniumHelper.WaitElement(account.driver, By.ClassName("common-modal-confirm-modal"), 60))
					{

						if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("TUXButton-label"), 3) == "Upload another video")
						{
							if (!SeleniumHelper.Click(account.driver, By.ClassName("css-1z070dx"), 5, count: 5))
								return ResultModel.Fail;
							clickAnotherVideo = true;
							return ResultModel.Success;
						}
						else if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("TUXButton-label"), count: 3) == "Tải lên")
						{
							if (!SeleniumHelper.Click(account.driver, By.ClassName("TUXButton-label"), 5, count: 5))
								return ResultModel.Fail;
							clickAnotherVideo = true;
							return ResultModel.Success;
						}
					}
				}
			}
			catch
			{
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}
		public ResultModel RegTikTok()
		{
			FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang signup ...");
			try
			{
				account.driver.Url = "https://www.tiktok.com/signup/phone-or-email/email";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			FunctionHelper.EditValueColumn(account, "C_Status", "Chọn tháng ...");
			if (!SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[data-e2e=\"select-container\"]")))
			{
				if (account.driver.Url == "https://www.tiktok.com/foryou?lang=en")
				{
					return ResultModel.Success;
				}
				return ResultModel.Fail;
			}

			Thread.Sleep(random.Next(1000, 3000));
			for (int i = 0; i < 3; i++)
			{
				if (SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"select-container\"]"), 5))
					break;
				else
				{
					if (i == 2)
						return ResultModel.Fail;
					if (account.driver.Url == "https://www.tiktok.com/foryou?lang=en")
					{
						return ResultModel.Success;
					}
					else
					{
						account.driver.Navigate().Refresh();
					}
				}
			}
			Thread.Sleep(1000);
			if (!SeleniumHelper.Click(account.driver, By.Id($"Month-options-item-{random.Next(0, 12)}"), 10))
				return ResultModel.Fail;
			FunctionHelper.EditValueColumn(account, "C_Status", "Chọn ngày ...");
			Thread.Sleep(random.Next(1000, 2000));
			for (int i = 0; i < 3; i++)
			{
				if (!SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"select-container\"]"), 10, count: 1))
				{
					if (i == 2)
					{
						return ResultModel.Fail;
					}
					continue;
				}

				else
				{
					break;
				}
			}
			Thread.Sleep(2000);
			if (!SeleniumHelper.Click(account.driver, By.Id($"Day-options-item-{random.Next(0, 27)}"), 10))
				return ResultModel.Fail;
			FunctionHelper.EditValueColumn(account, "C_Status", "Chọn năm ...");
			Thread.Sleep(random.Next(1000, 2000));
			if (!SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"select-container\"]"), count: 2))
			{
				return ResultModel.Fail;
			}
			Thread.Sleep(1000);
			if (!SeleniumHelper.Click(account.driver, By.Id($"Year-options-item-{random.Next(34, 53)}")))
				return ResultModel.Fail;
			FunctionHelper.EditValueColumn(account, "C_Status", "Nhập email ...");
			Thread.Sleep(random.Next(1000, 2000));
			if (!SeleniumHelper.SendKeys(account.driver, By.Name("email"), account.C_Email))
				return ResultModel.Fail;
			FunctionHelper.EditValueColumn(account, "C_Status", "Nhập pass ...");
			Thread.Sleep(random.Next(1000, 2000));
			var password = FunctionHelper.GenerateRandomString(12) + "1@@";
			SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"password\"]"), password);
			//Bấm gửi mã
			FunctionHelper.EditValueColumn(account, "C_Status", "Bấm gửi code ...");
			SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]"));
			Thread.Sleep(random.Next(1000, 2000));
			int countReSendCode = 0;
			int regetCode = 0;
		ReSendCode:
			countReSendCode++;
			while (!account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Enabled)
			{
				Thread.Sleep(1000);
			}
			SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]"));
			while (!account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Enabled)
			{
				Thread.Sleep(1000);
				if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 2))
					break;
				//	var inner = account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Text;
				if (account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Text.EndsWith("s"))
					goto GetCode;
			}
			if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
			{
				for (int i = 0; i < 20; i++)
				{
					SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]"), 1);
					if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("a[href=\"/login/with-signed-email\"]"), 5))
					{
						//var loginstatus = Login();
						return ResultModel.AlreadyReged;
					}
					if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 2))
						break;
					while (!SeleniumHelper.GetEnableElement(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]")))
					{
						Thread.Sleep(1000);
						if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 1))
							goto Captcha;
						try
						{
							if (account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Text.EndsWith("s"))
								goto GetCode;
						}
						catch
						{

						}
					}
				}
			}
		Captcha:
			if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("captcha_verify_bar"), 30))
			{
				SeleniumHelper.Click(account.driver, By.ClassName("captcha_verify_bar--close"));
				if (countReSendCode == 3)
					return ResultModel.Fail;
				goto ReSendCode;
			}
			//TH captcha tắt và có gửi code
			while (!SeleniumHelper.GetEnableElement(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]")))
			{
				Thread.Sleep(1000);
				try
				{
					if (account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Text.EndsWith("s"))
						goto GetCode;
				}
				catch
				{

				}
			}
			if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
			{
				if(SeleniumHelper.WaitElement(account.driver, By.CssSelector("a[href=\"/login/with-signed-email\"]"), 5))
				{
					return ResultModel.AlreadyReged;
			
				}
				if (countReSendCode == 3)
					return ResultModel.Fail;
				goto ReSendCode;
			}
		GetCode:
			regetCode++;
			FunctionHelper.EditValueColumn(account, "C_Status", "Đang đợi code ...");
			var code = FunctionHelper.GetCode(account.C_Email, account.C_PassEmail);
			if (code == "die")
				return ResultModel.EmailLoginFail;
			if (code == "")
			{
				countReSendCode = 0;
				if (regetCode == 2)
					return ResultModel.CodeEmpty;
				goto ReSendCode;
			}

			SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"text\"]"), code, count: 1);
			SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"));
			var url = account.driver.Url;
			while (SeleniumHelper.GetLenghtElement(account.driver, By.CssSelector("button[type=\"submit\"]")) > 50)
			{
				Thread.Sleep(1000);
				if (SeleniumHelper.UrlChange(account.driver, url, 1))
				{
					account.C_PassTikTok = password;
					if (account.driver.Url == "https://www.tiktok.com/signup/create-username")
					{
						SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());
						if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
						{
							SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), Keys.Control + "a");
							SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());

						}
						url = "https://www.tiktok.com/signup/create-username";
						SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 5);
						SeleniumHelper.UrlChange(account.driver, url, 10);
						Thread.Sleep(1000);
						return ResultModel.Success;
					}
					else
					{
						return ResultModel.Fail;
					}
				}
			}
			if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
			{
				FunctionHelper.EditValueColumn(account, "C_Status", "Đang click submit ...");
				for (int i = 0; i < 15; i++)
				{
					//var lenght = account.driver.FindElement(By.CssSelector("button[type=\"submit\"]")).GetAttribute("innerHTML").Length;
					while (SeleniumHelper.GetLenghtElement(account.driver, By.CssSelector("button[type=\"submit\"]")) > 50)
					{
						Thread.Sleep(1000);
						if (SeleniumHelper.UrlChange(account.driver, url, 1))
						{
							if (account.driver.Url == "https://www.tiktok.com/signup/create-username")
							{
								account.C_PassTikTok = password;
								SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());
								if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
								{
									SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), Keys.Control + "a");
									SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());

								}
								url = "https://www.tiktok.com/signup/create-username";
								SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 5);
								SeleniumHelper.UrlChange(account.driver, url, 10);
								Thread.Sleep(1000);
								return ResultModel.Success;
							}
							else
							{
								return ResultModel.Fail;
							}
						}
					}
					SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 1);
				}
			}
			//check url 
			FunctionHelper.EditValueColumn(account, "C_Status", "Đang check url change ...");
			if (SeleniumHelper.UrlChange(account.driver, url))
			{

				if (account.driver.Url == "https://www.tiktok.com/signup/create-username")
				{
					account.C_PassTikTok = password;
					//FunctionHelper.EditValueColumn(account, "C_PassTikTok", password, true);
					SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());
					if (SeleniumHelper.WaitElement(account.driver, By.CssSelector("div[type=\"error\"]"), 5))
					{
						SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), Keys.Control + "a");
						SeleniumHelper.SendKeys(account.driver, By.Name("new-username"), FunctionHelper.GenerateUsername());

					}
					url = "https://www.tiktok.com/signup/create-username";
					SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 5);
					SeleniumHelper.UrlChange(account.driver, url, 10);
					Thread.Sleep(1000);
					return ResultModel.Success;
				}
				else
				{
					return ResultModel.Fail;
				}
			}
			else
			{
				return ResultModel.Fail;
			}
		}
		public ResultModel ChangePass()
		{
			int reStart = 0;
			int regetCode = 0;
			string url = "";
			try
			{
				url = account.driver.Url = "https://www.tiktok.com/logout";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			if (!SeleniumHelper.UrlChange(account.driver, url, 10))
			{
				return ResultModel.Fail;
			}

			url = "https://www.tiktok.com/login/email/forget-password";
			FunctionHelper.EditValueColumn(account, "C_Status", "Đến trang forget pass ...");
		reStart:
			reStart++;
			try
			{
				account.driver.Url = "https://www.tiktok.com/login/email/forget-password";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			if (SeleniumHelper.UrlChange(account.driver, url, 6))
			{
				return ResultModel.AlreadyLogin;
			}
			if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 2))
			{
				Thread.Sleep(10000);
				if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("captcha_verify_bar"), 2))
				{
					if (reStart == 2)
						return ResultModel.Fail;
					goto reStart;
				}
			}

			if (!SeleniumHelper.SendKeys(account.driver, By.Name("email"), account.C_Email))
				return ResultModel.Fail;
			regetCode:
			regetCode++;
			SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"send-code-button\"]"));
			if (SeleniumHelper.WaitElement(account.driver, By.ClassName("captcha_verify_bar"), 10))
			{
				Thread.Sleep(10000);
				if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("captcha_verify_bar"), 10))
				{
					if (reStart == 2)
						return ResultModel.Fail;
					regetCode = 0;
					goto reStart;
				}
			}
			FunctionHelper.EditValueColumn(account, "C_Status", "Đang đợi code ...");
			var code = FunctionHelper.GetCode(account.C_Email, account.C_PassEmail);
			if (code == "die")
				return ResultModel.EmailLoginFail;
			if (code == "")
			{
				if (regetCode == 2)
					return ResultModel.CodeEmpty;
				try
				{
					while (account.driver.FindElement(By.CssSelector("button[data-e2e=\"send-code-button\"]")).Text.EndsWith("s"))
					{
						Thread.Sleep(1000);
					}
				}
				catch
				{

				}
				goto regetCode;
			}
			if (!SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"text\"]"), code, 10, 1))
				return ResultModel.Fail;
			account.C_PassTikTok = FunctionHelper.GenerateRandomString(12) + "1@@";
			if (!SeleniumHelper.SendKeys(account.driver, By.CssSelector("input[type=\"password\"]"), account.C_PassTikTok, 10))
				return ResultModel.Fail;
			if (!SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 8))
				return ResultModel.Fail;
			Thread.Sleep(1000);
			if (!SeleniumHelper.Click(account.driver, By.CssSelector("button[type=\"submit\"]"), 8))
				return ResultModel.Fail;
			//https://www.tiktok.com/foryou?
			if (SeleniumHelper.UrlChange(account.driver, url))
			{
				return ResultModel.Success;
			}
			return ResultModel.Success;
		}
		public ResultModel Interact()
		{
			bool _tym = true;
			List<string> cmtContent = RaiseAccForm._commentContent.Split('|').ToList();
			try
			{
				account.driver.Url = "https://www.tiktok.com";
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			//if (!SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"feed-video\"]")))
			//    return ResultModel.Fail;
			int countVideo = 0;
			while (Form1._timeInteractForm1.Minute - DateTime.Now.Minute > 0)
			{
				Thread.Sleep(5000);
				if (RaiseAccForm._tym)
				{
					if (_tym)
					{
						SeleniumHelper.Click(account.driver, By.CssSelector("span[data-e2e=\"like-icon\"]"), count: countVideo);

					}
				}


				if (RaiseAccForm._comment)
				{
					SeleniumHelper.Click(account.driver, By.CssSelector("div[data-e2e=\"feed-video\"]"), count: countVideo);
					Thread.Sleep(random.Next(8000, 15000));
					SeleniumHelper.SendKeys(account.driver, By.CssSelector("br[data-text=\"true\"]"), cmtContent[random.Next(0, cmtContent.Count)]);

					Thread.Sleep(3000);
					SeleniumHelper.SendKeys(account.driver, By.CssSelector("span[data-text=\"true\"]"), Keys.Enter);
				}
				Thread.Sleep(random.Next(8000, 15000));
				SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"browse-close\"]"));
				//if (!SeleniumHelper.GetAttributeTym(account.driver))
				//{
				//    _tym = false;
				//}
				//Bấm icon xuống video dưới
				//SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"arrow-right\"]"));
				SeleniumHelper.Scroll(account.driver);
				countVideo++;
			}

			return ResultModel.Success;
		}
		public ResultModel Follow(string url)
		{
			try
			{
				account.driver.Url = url;
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			if (SeleniumHelper.Click(account.driver, By.CssSelector("button[data-e2e=\"follow-button\"]")))
			{
				//var test = account.driver.FindElement(By.ClassName("tiktok-j2qm2m-DivFollowIconContainer")).Displayed;
				Thread.Sleep(1000);
				return ResultModel.Success;
			}
			return ResultModel.Fail;
		}
		public ResultModel GetUrbanVPN(string country)
		{
			int indexCountry = 0;
			FunctionHelper.EditValueColumn(account, "C_Status", "Đang bật vpn ...");
			Thread.Sleep(2000);
			var c = account.driver.WindowHandles.Count();
			if (account.driver.WindowHandles.Count() > 1)
			{
				try
				{
					account.driver.SwitchTo().Window(account.driver.WindowHandles[1]);
					account.driver.Close();
					account.driver.SwitchTo().Window(account.driver.WindowHandles[0]);
				}
				catch
				{
					return ResultModel.Fail;
				}
			}
			account.driver.Url = "chrome-extension://eppiocemhmnlbhjplcgkofciiegomcon/popup/index.html#/main";
			if (!SeleniumHelper.WaitElement(account.driver, By.ClassName("play-button"), 60))
				return ResultModel.Fail;

			if (!SeleniumHelper.Click(account.driver, By.ClassName("play-button")))
				return ResultModel.Fail;
			Thread.Sleep(2000);
			if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("loader__spin"), 40))
				return ResultModel.Fail;
			SeleniumHelper.Click(account.driver, By.ClassName("select-location__icon--cross"), 5);
			//Japan 28
			//SeleniumHelper.Click(account.driver, By.ClassName("locations__item-name"), 5, 28);
			//var elements = SeleniumHelper.FindElements(account.driver, By.ClassName("locations__item"));
			//foreach (var element in elements)
			//{
			//	country = "Japan";
			//	var t = element.Text;

			//	if (element.Text == country)
			//	{
			//		Thread.Sleep(1000);
			//		account.driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
			//		element.Click();
			//		break;
			//	}
			//}
			var elements = account.driver.FindElements(By.ClassName("locations__item-name"));
			if (elements.Count() == 61)
			{
				if (country == "Japan")
				{
					indexCountry = 28;
				}
				else if (country == "United Kingdom (UK)")
				{
					indexCountry = 58;
				}
				else if (country == "United States (USA)")
				{
					indexCountry = 59;
				}
				else if (country == "Italy")
				{
					indexCountry = 27;
				}
			}
			else
			{
				if (country == "Japan")
				{
					indexCountry = 27;
				}
				else if (country == "United Kingdom (UK)")
				{
					indexCountry = 57;
				}
				else if (country == "United States (USA)")
				{
					indexCountry = 58;
				}
				else if (country == "Italy")
				{
					indexCountry = 26;
				}
			}
			try
			{
				var element = account.driver.FindElements(By.ClassName("locations__item-name"))[indexCountry];
				//country = "Japan";
				Thread.Sleep(1000);
				account.driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
				element.Click();
			}
			catch
			{
				return ResultModel.Fail;
			}
			if (!SeleniumHelper.WaitElementHidden(account.driver, By.ClassName("loader__spin"), 40))
				return ResultModel.Fail;
			Thread.Sleep(2000);
			if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("main-page__timer")).EndsWith("00"))
			{
				return ResultModel.Fail;
			}
			return ResultModel.Success;
		}
		public ResultModel GetInfoTikTok()
		{
			var url = "https://www.tiktok.com/profile";
			try
			{
				account.driver.Url = "https://www.tiktok.com/profile";
				if (SeleniumHelper.UrlChange(account.driver, url, 10))
				{
					if (account.driver.Url == "https://www.tiktok.com/foryou?lang=en")
						return ResultModel.NotLogin;
				}
				if (SeleniumHelper.WaitElement(account.driver, By.ClassName("emuynwa1"), 5))
				{
					if (SeleniumHelper.GetTextElement(account.driver, By.ClassName("emuynwa1")) == "Couldn't find this account")
						return ResultModel.Suspended;
				}
			}
			catch
			{
				return ResultModel.WeakProxy;
			}
			if (!SeleniumHelper.WaitElement(account.driver, By.CssSelector("h2[data-e2e=\"user-subtitle\"]"), 30))
				return ResultModel.Fail;
			try
			{
				FunctionHelper.EditValueColumn(account, "C_Url", account.driver.Url, true);
			}
			catch
			{

			}
			var nickName = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("h2[data-e2e=\"user-subtitle\"]"));
			if (nickName != "")
			{
				FunctionHelper.EditValueColumn(account, "C_ChanelName", nickName, true);
			}
			else
			{
				FunctionHelper.EditValueColumn(account, "C_ChanelName", "lỗi", true);
			}
			string _follower = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"followers-count\"]"));
			if (_follower != "")
			{
				FunctionHelper.EditValueColumnTypeInt(account, "C_Follower", FunctionHelper.ConvertToInt(_follower), true);
			}
			else
			{
				FunctionHelper.EditValueColumn(account, "C_Follower", "lỗi");
			}

			try
			{
				var C_Video1 = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"video-views\"]"));
				if (C_Video1 != "")
				{
					FunctionHelper.EditValueColumnTypeInt(account, "C_Video1", int.Parse(C_Video1), true);
				}
				else
				{
					FunctionHelper.EditValueColumn(account, "C_Video1", "lỗi");
				}
				var C_Video2 = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"video-views\"]"), count: 1);
				if (C_Video2 != "")
				{
					FunctionHelper.EditValueColumnTypeInt(account, "C_Video2", int.Parse(C_Video2), true);
				}
				else
				{
					FunctionHelper.EditValueColumn(account, "C_Video2", "lỗi");
				}

				var C_Video3 = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"video-views\"]"), count: 2);
				if (C_Video3 != "")
				{
					FunctionHelper.EditValueColumnTypeInt(account, "C_Video3", int.Parse(C_Video3), true);
				}
				else
				{
					FunctionHelper.EditValueColumn(account, "C_Video3", "lỗi");
				}
				var C_Video4 = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"video-views\"]"), count: 3);
				if (C_Video4 != "")
				{
					FunctionHelper.EditValueColumnTypeInt(account, "C_Video4", int.Parse(C_Video4), true);
				}
				else
				{
					FunctionHelper.EditValueColumn(account, "C_Video4", "lỗi");
				}
				var C_Video5 = SeleniumHelper.GetTextElement(account.driver, By.CssSelector("strong[data-e2e=\"video-views\"]"), count: 4);
				if (C_Video5 != "")
				{
					FunctionHelper.EditValueColumnTypeInt(account, "C_Video5", int.Parse(C_Video5), true);
				}
				else
				{
					FunctionHelper.EditValueColumn(account, "C_Video5", "lỗi");
				}
			}
			catch
			{

			}
			var allCookies = account.driver.Manage().Cookies.AllCookies;
			account.C_Cookie = JsonConvert.SerializeObject(allCookies);
			FunctionHelper.EditValueColumn(account, "C_Cookie", account.C_Cookie, true);
			return ResultModel.Success;
		}
	}
}
