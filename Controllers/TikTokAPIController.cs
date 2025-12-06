using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tool_TikTok.Helpers;
using Tool_TikTok.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Newtonsoft.Json.Linq;
using AngleSharp;
using Jint;
using System.Net.Http;

namespace Tool_TikTok.Controllers
{
    public class TikTokAPIController
    {
        public TikTokAPIController() { }
        public   ResultModel GetInfoTikTok(AccountModel account)
        {
            string body = "", refer = "";
            using (var rq = new HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                //Lấy url kênh
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
								sec-ch-ua: ""Google Chrome"";v=""117"", ""Not;A=Brand"";v=""8"", ""Chromium"";v=""117""
								sec-ch-ua-mobile: ?0
								sec-ch-ua-platform: ""Windows""
								Upgrade-Insecure-Requests: 1
								User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
								Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
								Sec-Fetch-Site: none
								Sec-Fetch-Mode: navigate
								Sec-Fetch-User: ?1
								Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get("https://www.tiktok.com/profile").ToString();
                    //body = rq.Get("https://www.tiktok.com/@test_tool_1103").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                if (refer == "https://www.tiktok.com/foryou?lang=en")
                {
                    FunctionHelper.EditValueColumn(account, "C_Url", "lỗi", true);
                    FunctionHelper.EditValueColumn(account, "C_Status", "Không login được!", true);
                    return ResultModel.Fail;
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Url", refer, true);
                }

                var chanelName = RegexHelper.GetValueFromGroup("\"nickName\":\"(.*?)\",", body);
                if (chanelName != "")
                {
                    account.C_ChanelName = chanelName;
                    FunctionHelper.EditValueColumn(account, "C_ChanelName", account.C_ChanelName, true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_ChanelName", "lỗi");
                }
                var _follower = RegexHelper.GetValueFromGroup("{\"followerCount\":(.*?),\"", body);
                if (_follower != "")
                {
                    FunctionHelper.EditValueColumnTypeInt(account, "C_Follower", FunctionHelper.ConvertToInt(_follower), true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Follower", "lỗi");
                }
                var videoCount = RegexHelper.GetValueFromGroup("\"videoCount\":(.*?),\"", body);
                if (videoCount != "")
                {
                    FunctionHelper.EditValueColumnTypeInt(account, "C_Video", FunctionHelper.ConvertToInt(videoCount), true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Video", "lỗi");
                }
				
				//Lấy ViewVideo
				FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
                                            sec-ch-ua: ""Chromium"";v=""124"", ""Google Chrome"";v=""124"", ""Not-A.Brand"";v=""99""
                                            sec-ch-ua-mobile: ?0
                                            User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36
                                            sec-ch-ua-platform: ""Windows""
                                            Accept: */*
                                            Sec-Fetch-Site: same-origin
                                            Sec-Fetch-Mode: cors
                                            Sec-Fetch-Dest: empty
                                            Referer: {refer}");
                try
                {
					//body = rq.Get(@"https://www.tiktok.com/creator-center/api/web/items?locale=vi-VN&aid=1988&priority_region=VN&region=VN&tz_name=Asia%2FSaigon&app_name=tiktok_creator_center&device_platform=web_pc&os=win&screen_width=1466&screen_height=825&browser_language=vi-VN&browser_platform=Win32&browser_name=Mozilla&browser_version=5.0+(Windows+NT+10.0%3B+Win64%3B+x64)+AppleWebKit%2F537.36+(KHTML,+like+Gecko)+Chrome%2F119.0.0.0+Safari%2F537.36&cursorPosition=0&limit=10").ToString();
					body = rq.Get(@"https://www.tiktok.com/api/post/item_list/?WebIdLastTime=1723446228&aid=1988&app_language=vi-VN&app_name=tiktok_web&browser_language=vi-VN&browser_name=Mozilla&browser_online=true&browser_platform=Win32&browser_version=5.0%20%28Windows%20NT%2010.0%3B%20Win64%3B%20x64%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F124.0.0.0%20Safari%2F537.36&channel=tiktok_web&cookie_enabled=true&count=35&coverFormat=2&cursor=0&data_collection_enabled=true&device_id=7402144828275181072&device_platform=web_pc&focus_state=true&from_page=user&history_len=3&is_fullscreen=false&is_page_visible=true&language=vi-VN&odinId=7269560210382898222&os=windows&priority_region=&referer=&region=VN&screen_height=1080&screen_width=1920&secUid=MS4wLjABAAAAO5cuc-lfywcm1f59EZ_3qqjPTYq3MFw0sKD5NvEB20u-6FeP2qSgHvAKuBLx0gHk&tz_name=Asia%2FBangkok&user_is_login=true&webcast_language=vi-VN&msToken=tG9XK1uIXyB0OTg_BoSNbbkuheFtLYLwdJ-HA1riqCIMEztNmSEowHPx_bpy8R4r0LlWWW-wMjte-d9bwSvT9VQNmjZ3fyl9ZMlJGzqddzl25dOzCeZnBS1MJTkCLykPKJ_0AiQ1-PkaY7pK5h6kfYPs&X-Bogus=DFSzswVOGKiANegttfaAt6rxLCmS&_signature=_02B4Z6wo00001DHCnWwAAIDCdGdwMokGYrQxwpnAAGrR55").ToString();
				}
                catch
                {

                }
                var matches = Regex.Matches(body, ",\"playCount\":\"(.*?)\",");
                try
                {
                    var C_Video1 = matches[0].Groups[1].Value;
                    if (C_Video1 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video1", int.Parse(matches[0].Groups[1].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video1", "lỗi");
                    }
                    var C_Video2 = matches[1].Groups[1].Value;
                    if (C_Video2 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video2", int.Parse(matches[1].Groups[1].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video2", "lỗi");
                    }

                    var C_Video3 = matches[2].Groups[1].Value;
                    if (C_Video3 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video3", int.Parse(matches[2].Groups[1].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video3", "lỗi");
                    }
                    var C_Video4 = matches[3].Groups[1].Value;
                    if (C_Video4 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video4", int.Parse(matches[3].Groups[1].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video4", "lỗi");
                    }
                    var C_Video5 = matches[4].Groups[1].Value;
                    if (C_Video5 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video5", int.Parse(matches[4].Groups[1].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video5", "lỗi");
                    }
                }
                catch
                {

                }

                //Lấy country
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
											Cache-Control: max-age=0
											sec-ch-ua: ""Chromium"";v=""118"", ""Google Chrome"";v=""118"", ""Not=A?Brand"";v=""99""
											sec-ch-ua-mobile: ?0
											sec-ch-ua-platform: ""Windows""
											Upgrade-Insecure-Requests: 1
											User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36
											Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
											Sec-Fetch-Site: none
											Sec-Fetch-Mode: navigate
											Sec-Fetch-User: ?1");
                try
                {
                    body = rq.Get("https://api.myip.com/").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                string country = RegexHelper.GetValueFromGroup("\"cc\":\"(.*?)\"", body);
                if (country != "")
                    FunctionHelper.EditValueColumn(account, "C_Country", country, true);
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Country", "lỗi");
                }
                return ResultModel.Success;
            }
        }


        public ResultModel GetInfoTikTokERR(AccountModel account)
        {
            string body = "", refer = "";
            using (var rq = new HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                FunctionHelper.SetCookieToRequestXnet(rq, account.C_Cookie);
                //Lấy url kênh
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
								sec-ch-ua: ""Google Chrome"";v=""117"", ""Not;A=Brand"";v=""8"", ""Chromium"";v=""117""
								sec-ch-ua-mobile: ?0
								sec-ch-ua-platform: ""Windows""
								Upgrade-Insecure-Requests: 1
								User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
								Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
								Sec-Fetch-Site: none
								Sec-Fetch-Mode: navigate
								Sec-Fetch-User: ?1
								Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get("https://www.tiktok.com/profile").ToString();
                    //body = rq.Get("https://www.tiktok.com/@lebong95").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    return ResultModel.Fail;
                }
                if (refer == "https://www.tiktok.com/foryou?lang=en")
                {
                    FunctionHelper.EditValueColumn(account, "C_Url", "lỗi", true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Url", refer, true);
                }

                var chanelName = RegexHelper.GetValueFromGroup("\"nickName\":\"(.*?)\",", body);
                if (chanelName != "")
                {
                    account.C_ChanelName = chanelName;
                    FunctionHelper.EditValueColumn(account, "C_ChanelName", account.C_ChanelName, true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_ChanelName", "lỗi");
                }
                //Lấy country
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
											Cache-Control: max-age=0
											sec-ch-ua: ""Chromium"";v=""118"", ""Google Chrome"";v=""118"", ""Not=A?Brand"";v=""99""
											sec-ch-ua-mobile: ?0
											sec-ch-ua-platform: ""Windows""
											Upgrade-Insecure-Requests: 1
											User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36
											Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
											Sec-Fetch-Site: none
											Sec-Fetch-Mode: navigate
											Sec-Fetch-User: ?1");
                try
                {
                    body = rq.Get("https://api.myip.com/").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                string country = RegexHelper.GetValueFromGroup("\"cc\":\"(.*?)\"", body);
                if (country != "")
                    FunctionHelper.EditValueColumn(account, "C_Country", country, true);
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Country", "lỗi");
                }
                //Lấy số view...
                rq.Cookies.Clear();
                rq.Proxy = null;
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
								sec-ch-ua: ""Google Chrome"";v=""117"", ""Not;A=Brand"";v=""8"", ""Chromium"";v=""117""
								sec-ch-ua-mobile: ?0
								sec-ch-ua-platform: ""Windows""
								Upgrade-Insecure-Requests: 1
								User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
								Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
								Sec-Fetch-Site: none
								Sec-Fetch-Mode: navigate
								Sec-Fetch-User: ?1
								Sec-Fetch-Dest: document");
                try
                {
                    //	account.C_Url = "https://www.tiktok.com/@jennamorton19736?lang=en";
                    body = rq.Get(account.C_Url).ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    try
                    {
                        body = rq.Response.ToString();
                        string err = RegexHelper.GetValueFromGroup("\"err_user\":\"(.*?)\",", body);
                        if (err.Contains("Couldn't find this account"))
                            return ResultModel.Suspended;
                    }
                    catch
                    {

                    }
                    return ResultModel.AnotherError;
                }
                string _follower = RegexHelper.GetValueFromGroup(":{\"followerCount\":(.*?),\"", body);
                if (_follower != "")
                {
                    FunctionHelper.EditValueColumnTypeInt(account, "C_Follower", FunctionHelper.ConvertToInt(_follower), true);
                }
                else
                {
                    FunctionHelper.EditValueColumn(account, "C_Follower", "lỗi");
                }
                string _like = RegexHelper.GetValueFromGroup(" data-e2e=\"likes-count\">(.*?)<", body);
                if (_like != "")
                {
                    //FunctionHelper.EditValueColumn(account, "C_Follower", _like, true);
                }
                //	var matches = Regex.Matches(body, "playCount\":(.*?),");
                var matches = Regex.Matches(body, "<strong data-e2e=\"video-views\" class=\"video-count (.*?)\">(.*?)<\\/strong>");
                try
                {
                    var C_Video1 = matches[0].Groups[2].Value;
                    if (C_Video1 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video1", int.Parse(matches[0].Groups[2].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video1", "lỗi");
                    }
                    var C_Video2 = matches[1].Groups[2].Value;
                    if (C_Video2 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video2", int.Parse(matches[1].Groups[2].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video2", "lỗi");
                    }

                    var C_Video3 = matches[2].Groups[2].Value;
                    if (C_Video3 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video3", int.Parse(matches[2].Groups[2].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video3", "lỗi");
                    }
                    var C_Video4 = matches[3].Groups[2].Value;
                    if (C_Video4 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video4", int.Parse(matches[3].Groups[2].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video4", "lỗi");
                    }
                    var C_Video5 = matches[4].Groups[2].Value;
                    if (C_Video5 != "")
                    {
                        FunctionHelper.EditValueColumnTypeInt(account, "C_Video5", int.Parse(matches[4].Groups[2].Value), true);
                    }
                    else
                    {
                        FunctionHelper.EditValueColumn(account, "C_Video5", "lỗi");
                    }
                }
                catch
                {

                }
                return ResultModel.Success;
            }
        }


		public bool Follow(AccountModel account)
        {
            string body = "", refer = "";
            using (var rq = new HttpRequest())
            {
                rq.AllowAutoRedirect = true;
                rq.KeepAlive = true;
                //rq.Proxy = FunctionHelper.ConvertToProxyClient(account.C_Proxy);
                FunctionHelper.SetCookieToRequestXnet(rq, "_ttp=28QABYZV78gMbLjFFLZ8BP2zo6j; tiktok_webapp_theme=light; d_ticket=0bf49b01eb511901c793ff5882638eeeb695c; _ga=GA1.1.1803972723.1675828520; _tt_enable_cookie=1; d_ticket_ads=03b004b45a64b759242096183847be10b695c; _ga_BZBQ2QHQSP=GS1.1.1676902275.2.1.1676903417.0.0.0; _abck=FAD39252E790E9D92E6AB2FD5C37749D~0~YAAQznBHG528WpyGAQAA0gR9uwn2/rJFKMhhQUE+Dqgzi9b3XDi9rwJjXBOxpE/EHZiAWoZiy3guWlo1o1RXkcAg/4txPr7wm9tXG/6Kz0b0QjWM0mAkXYR/7nCVionZO0zOSICp0HvduEIdVAkFGiap3Bl25ZGDnOl+aWeXe1lGQEXvbxUjOSb5u4NiwXwMuJ0kAr2hQx7++U+kRj978k/WNh7ls7FPW+NXmo5iCfbbhHng1Q/fatKg9V+U3446MFCsyrYes2v2pZeNsJH8z/kgy5hjWae13T0Dk5Peom0MEyzG43M4WqbaUvfN7ri1qiB5t2k1FPWXUAPtpX1PrbxrynXjc34ppYiiKziveaPjXlEYzZnPvbyL3kufYANsX11n/fTAJPnTDOl5doBLnsl2VFFKGm/i~-1~-1~-1; tt_chain_token=+S4z5aYdrl00irwpoIGezg==; uid_tt=036b777cd8b1a62a1be40030714d67c737d3480dd8db360dc4dda9d2a404b7de; uid_tt_ss=036b777cd8b1a62a1be40030714d67c737d3480dd8db360dc4dda9d2a404b7de; sid_tt=8d9d2a31870af903472980f5f59cd8ea; sessionid=8d9d2a31870af903472980f5f59cd8ea; sessionid_ss=8d9d2a31870af903472980f5f59cd8ea; store-country-code=vn; store-country-code-src=uid; tt-target-idc=alisg; tt-target-idc-sign=EF4H7BeDjsyg_I7ftNpqzGMeXy6BwTbl4unO6XANkt7CBrX3ehPVfl_5iIW49FH1C_skLo-mYPED6HZY933DQTd0-ctBibTljKZTaNt8pIxxawCPqB1r6wxTEn4foxPXiYG0f0lJp1UKNF45XFqEIIIOgBsFL7YsCPxzwiJrk36Xlgshbsjc_xB89yih-gpVzcjM9akDGaYlmkJyH-5eweLCu-6Ap4PjqyZdcgDEYasifBFcg8bZzGeGbFb2eh5Ke607Ttgh7jKz516OecgF9fotY-y8PexRbC6cBmjOFF4rtRmqHJRx7i2p-pfyj6RVHWu3NnvgGyBTtXOYM-F1VFALPSvY1xpU661mbUtmXbODoDyOw1T5BTsQNLbYoxx0Qqo-KOrCo-lJFMJRgOOLbgXzme_SxCoO45xhThvRenHBdnjD2VX8XoIQC8Pj4fc9YY2hEAmdZ8ZOBx8bbhJFxbr7zd79PxV9lay_3uLSZpMj7-q07QOSyGetHdV9GBhz; store-idc=alisg; sid_guard=8d9d2a31870af903472980f5f59cd8ea%7C1695302997%7C15552000%7CTue%2C+19-Mar-2024+13%3A29%3A57+GMT; sid_ucp_v1=1.0.0-KGI2ZjZhZDMzZjE3Nzk3ZjZlYTVmN2U4NTYwMGRmNGQwZDdjNGJmOTcKIAiCiM7Gt5vkxWEQ1YqxqAYYswsgDDCeya-MBjgBQOsHEAMaA3NnMSIgOGQ5ZDJhMzE4NzBhZjkwMzQ3Mjk4MGY1ZjU5Y2Q4ZWE; ssid_ucp_v1=1.0.0-KGI2ZjZhZDMzZjE3Nzk3ZjZlYTVmN2U4NTYwMGRmNGQwZDdjNGJmOTcKIAiCiM7Gt5vkxWEQ1YqxqAYYswsgDDCeya-MBjgBQOsHEAMaA3NnMSIgOGQ5ZDJhMzE4NzBhZjkwMzQ3Mjk4MGY1ZjU5Y2Q4ZWE; __tea_cache_tokens_1988={%22user_unique_id%22:%227028943790783268353%22%2C%22timestamp%22:1697346240634%2C%22_type_%22:%22default%22}; tt_csrf_token=UNgngwQo-WuY1BamCdpKgDfcLiMyWKKRh3Zs; csrf_session_id=d01f80e2f38855ddcfd0497c4a0495df; passport_fe_beating_status=false; perf_feed_cache={%22expireTimestamp%22:0%2C%22itemIds%22:[]}; s_v_web_id=verify_lo03nwif_nVxHLZp6_s7Qk_4377_9unI_hr6LP444zeTY; ttwid=1%7CKAZ2-ug7hJkgpDJR2cNTwVliB0W85egktcV3yyiLQAU%7C1697896413%7Cfe222512401fbd54150421213e5078db6990ff0570efa3809a4db2aa658adb65; odin_tt=11701de4aa658139aa4e785ac7f119a54344f24cb031a53f08b070dc1e39b2d8178c0411aa8a1578dbd3d98fee6cb07fe928f77c3329291635deda8289bc86f4ed25f48b3a4ef42f78e6909b1331eeb2; msToken=16OYLryrhm3hi507ZCro9rciLlAb6r7aknNUcPU1YKYLpgUvyS-pMqD-3DB7TLxSOVdki2R8HtYQS4g0eadgJ6cmwuO-bAk13EqKhBMwxClndMn17X5fE-UkkntVJ_hZR2aqqxrIp7iVGfehrl9_VeE=; msToken=BbI2-2avFhabmSY1q1hqNSIg3WmKOtY_s0RBQIu2AYT0Dkqf8IjYGY2Je8rqfnELPArKJKcQAg55npLKtKyzgqPz8X4NLjpbjE5hkFui4Qr2E8hJxBOHrcIyKV3x2HCON2qUafYn7bK-HiEQg7HIhko=");
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
								sec-ch-ua: ""Google Chrome"";v=""117"", ""Not;A=Brand"";v=""8"", ""Chromium"";v=""117""
								sec-ch-ua-mobile: ?0
								sec-ch-ua-platform: ""Windows""
								Upgrade-Insecure-Requests: 1
								User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36
								Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7
								Sec-Fetch-Site: none
								Sec-Fetch-Mode: navigate
								Sec-Fetch-User: ?1
								Sec-Fetch-Dest: document");
                try
                {
                    body = rq.Get("https://www.tiktok.com/@qnhulove").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    //return false;
                }
                var tt_csrf_token = RegexHelper.GetValueFromGroup("\"csrfToken\":\"(.*?)\",", body);
                var webIdCreatedTime = RegexHelper.GetValueFromGroup("\"webIdCreatedTime\":\"(.*?)\",", body);
                var aid = RegexHelper.GetValueFromGroup("\\?aid=(.*?)&amp;", body);
                var device_id = RegexHelper.GetValueFromGroup("\"wid\":\"(.*?)\",", body);
                var user_id = RegexHelper.GetValueFromGroup("\"authorId\":\"(.*?)\",", body);
                var sec_user_id = RegexHelper.GetValueFromGroup("\"authorSecId\":\"(.*?)\",", body);
                //get common...
                FunctionHelper.AddHeaderxNet(rq, @"Connection: keep-alive
												sec-ch-ua: ""Chromium"";v=""118"", ""Google Chrome"";v=""118"", ""Not=A?Brand"";v=""99""
												sec-ch-ua-mobile: ?0
												User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36
												sec-ch-ua-platform: ""Windows""
												Accept: */*
												Sec-Fetch-Site: cross-site
												Sec-Fetch-Mode: no-cors");
                try
                {
                    body = rq.Get("https://s20.tiktokcdn.com/tiktok/common/init.js?async").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {

                }
                var htc6j8njvn_f = RegexHelper.GetValueFromGroup(";e.detail.init\\(\"(.*?)\",", body);
                //var htc6j8njvn_a = RegexHelper.GetValueFromGroup("", body);
                var htc6j8njvn_c = RegexHelper.GetValueFromGroup("init\\.js\\?seed=(.*?)&", body);
                //

                //follow
                RequestParams pr = new RequestParams()
                {
                    ["WebIdLastTime"] = webIdCreatedTime,
                    ["action_type"] = "1",
                    ["aid"] = aid,
                    ["app_language"] = "vi-VN",
                    ["app_name"] = "tiktok_web",
                    ["browser_language"] = "vi-VN",
                    ["browser_name"] = "Mozilla",
                    ["browser_online"] = "true",
                    ["browser_platform"] = "Win32",
                    ["browser_version"] = "5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36",
                    ["channel"] = "tiktok_web",
                    ["channel_id"] = "0",
                    ["cookie_enabled"] = "true",
                    ["device_id"] = device_id,
                    ["device_platform"] = "web_pc",
                    ["focus_state"] = "true",
                    ["from"] = "18",
                    ["fromWeb"] = "1",
                    ["from_page"] = "user",
                    ["from_pre"] = "0",
                    ["history_len"] = "4",
                    ["is_fullscreen"] = "false",
                    ["is_page_visible"] = "true",
                    ["os"] = "windows",
                    ["priority_region"] = "",
                    ["referer"] = "",
                    ["region"] = "VN",
                    ["screen_height"] = "825",
                    ["screen_width"] = "1466",
                    ["sec_user_id"] = sec_user_id,
                    ["type"] = "1",
                    ["tz_name"] = "Asia/Saigon",
                    ["user_id"] = user_id,
                    ["webcast_language"] = "vi-VN",
                    ["msToken"] = "RUfkLR1e7HcuXyZXbc-z-9UIGojUsIT8HR1rskJ-Fmr1GbT1XjFI7jrLuUcPSCJumvDTYxhns6rDTXrCni8r1czLNZgbRTjyXnya0y6LImycSjUrlkUA9GbNsf61SiHTWFsYb-L6B06m9NZFpL6Vuw==",
                    ["X-Bogus"] = "DFSzswVueNUANxFHtYU5F09WcBru",
                    ["_signature"] = "_02B4Z6wo00001aYDfpwAAIDBpgN-n5Wu6U2mA3oAAAy02c",
                };
                FunctionHelper.AddHeaderxNet(rq, $@"Connection: keep-alive
												Content-Length: 0
												sec-ch-ua: ""Chromium"";v=""118"", ""Google Chrome"";v=""118"", ""Not=A?Brand"";v=""99""
												htc6j8njvn-b: hplpyl
												x-secsdk-csrf-token: 00010000000144b51efb5ef482f0e4bf90003808fd7c1ac3de2186e21f06897a5e7212a8da021790168318039fe0
												htc6j8njvn-d: ABaAhIDBCKGFgQCAAYIQgISigaIAwBGAzvpCzi_33wfpq50UP-o6PgAAAAAF6UpmAMcH0mwlmDJEg3DDI7v_KFk
												User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36
												sec-ch-ua-mobile: ?0
												htc6j8njvn-z: q
												content-type: application/x-www-form-urlencoded
												htc6j8njvn-c: {htc6j8njvn_c}
												htc6j8njvn-a: oD9vY1=zLIUy-K3MVr2CJoUdbxUzRdX3ZCYOj6Osw9yof=90CsvF8ggFdXjbD3IiXH_ClbFaTLJbFhQXz7Yf9hKhlinQfa5By0DK94KIS2RsO=ZCTKqEXfGXzgbK-GEx8xmUx2qpZANd60gT_FsXVKdv-JAcMTBxO0=KBcKvJrU4cT1IRRx2hjZfxpz79wo9-v4unaqL3Hl5XGxdOpidOTJQvzENWxdOeNn9fh47R_7WMh5e87YJNow2QLHelIpZRG37Q_x37I8XijNmyKtyURWKH_is8L471I9xLnB6X9mxNZBoimqbVNgPITNlU0HMl4C4y9Sr5KivX1vXutvZdSO9W2ttKMpuuyrcHCG1NVcVzjAvnn_=6G_leAoFuC52vH3=WT9FDPv=AAffChf0iK7huf92sJB7-GxKTgf7P1JYUOPYR=0oUC-z-nPdTDA2ysc-5CNo5Rl4BZrU4KqNuV9Mq_Q42ZGwT9m57y3UyLAbtpP7B7SWWH1mVLRKD7iOMzqJ18AlZD-BCemFW4aSZUsIyBVYxjtywj_ho7Ybx_Fd7JxYl1z2eSAEDga_9MGuCKVPmgVmtm91xol=FMJ7YVs=K8GjdSSEFBLY9t-JQNY=cEGgFV8C=ZvpCXI3NfI7Y2chwKW28nxOMXgoG9UAn65-D6LyDCB386etlI=pHqTuGGOVWi_yvIBhCBiqF_FGahsxFmNN4C1MAmFQAoA-IRpfmaew5Zvh3JS9jIQU54BoVjpEv0f3IMBupyEwqOdhAJvxRuod2aV1=OFN68p2s=JRpFJSY0I4pHhDVGnDyrG6FrSWIOT3o7TLCPjz0XTyzEa6fzqi4eE7Sylmw-gIWT6fr3HQKhT3sjKmvih3_bHGvJwd6ePnmZFNLdGMjg4yx67LVt0Ow-awpqGJlK_IiSLGYVhdrWy3Bwy328yiOE9Hfw341PrXTR6eojEa0o=ZvEyqexcnLaTJiTEBl91OhOQYFwvt4pD45jcf1T2u5s9MrwalPAXwhnxSuStsJPYyLYuj4Bsuq1z=q8DjwLcC0H7iWc8q_wPzCsOgQ8v42pVDCV3FqQxF8w8SpaHRVgQf=mUaEgmufr9V3W2wO=AM8b5fJN_v4V6RfKot9hrOEPKQeqHWPGWQ2e2=8yQ81hjcHyhFeUjtqvhAqBEmS9CODl_fgC--EDi43oOld9Jc=5QM8A9R1IbGFg5HLtAlw6fOzd0P98aqctX=Lz-ty-D597xXbHsQ3M6fA4F0SqAb-tqNbONZ_J3HxGWf1AuEyYJJ1n9ucAQpFSwQXgAuaL31ZlRZc3gy5puvLEWhWgKfnIlr8q8u8quB4-aFGzByQ9hS3MLXiHedBVP_pogK1hr3beEFtj9D1Fr8yVfK0N3pRuzRmBcMnGRZEtNnDBKNYOAqe3M2qX_b3m8JZZN9T4sM1PdHj678IhLSSfTRtqTmC1Ue30LE5lK_mx7FSdeGiSo3xCbfS9I-eGnlNxADzGi2Wnbfd=H9A4OzmLCOYhtagh3bBhfxgg4-s-xwP4GD7axuzr1SdivAI6QrXjSTwFZ7zGEiUq9KJLFtoy9jzg56j1Xf_6NzGd=LvdC15I9BjxULZcfEQT7KZH6lfCJl1TeB8mKh1fBbh0SBzC6e0NMT87NJZXqmnRqwqcGht-LO_SMOvVKwgNmM=nhcOMqA7wHAreD9=aehXJ3dv9CJ-oxAytJN0oTxvlNiGaAGPmJZ1KQUFTaH_cH0GD_1-7xwNlussuWMeeA6gGdalRTqF=B9Rf2WSdL5QVFo=leohVgOE=WpO-vEI2fSZQeooKnjyL7Y=FffT2sdXHMU5cIp-vZaBLymVUUYlwpjwYTf4z0c1RBGM5AEX93dPE=3OGPfvrYRJwj=6shwWXFLqZxrrKEpSUtMfM=jzOcYAJdGIEyDXXK3hheqt-3bXzoZm7JjqbgpEJU_cMc6Paey-WXLVITvzFMXXalyG4XcM3=vdlC=IQE8i4TsxHCvvo3QNQE_B0ddqHOTWRf-euYv8QUwLfZ3v7jBXz71m7xj79J-1xhSHwCJIfo9AE-9gRYwjl14vJ6NZW8ZZ--p0zq2GV34plxCmRmCrEIYuGGJHSXeKbBF1V=4GoAuuau6tdtoly15TedECL6EDOCps_2AMOiv0md78xysZilnoPL0oaJ4lwe5=BjMV-VoAUAQS=K_Ls12AwmQqGlhu7s5pCVKQI5CAjNnDqbKZu8GFpqgYhpU=Um5S0-2iDdiXoLQwWC=iEp6=ZXh0=cbXLdKmiJ16Jes=zl6vrn_9KjX=VFS1a=uqrPTwfTwaFLYPF-32hy=dh3PStWvDUJKdraWFTlz-JZ2xRxNhsoQwWI_xF=hA53bg4SUSfCICxT=Pg2Y-ecz-YPvV7ZZ52et99e85I1KQLcecjCIQuA9XVblf4q7P8QwBECAqfdm0C9AO_=d96FsdcKc=7adz-Q6A4ED9v2SC-6KrZVjGL1P7HN8-SvFmfeU1x3fjELXoHxRf-RPFPBSRDyMeVwVsAYIFSS27J2Gc=lRB9iYdBg_WaCp1msKiImz6GYDPUddVIVKDWRSltZK0yGvgo-Wcd4iqxs5CMwj3lGBJN4tJa-TA=dzCxb31aAj_bhrVb4q1iIu1tFqXnQX-pKYnEH-K0IE-gfJIJrWt7aLE4D-fOaywuPeMTCcathrRTrhK3aWvept4s3zVwNjmuVApVgbVVcQWGcS8yaYynHsDvKeGagPRTxZ8bQVpKdpJoBL2njpZ_WxzySDjTT0DP0ouF-9Jx324Y3wPs6TihTv3CzrBlHEf-UNBYYKdxwQXCBIR-noJOf1dfhMdybvXlyzjNCzhCSzDODf9=lf2oRoBzHtPcZQCFCMXvhFlrwZf0GI09KvtIEt6XOC_Q53wgqmQz2K5xuxC_oZTeiSovSqRI1Bl0eYOKe2etNbzvITDrcPS=0rWTqhJLirzQoAwi6JxC6yuJ32UQc-IhiBK0U6QwelRWW3n85W90bZr=BdCsUlK-irAvBP_j9DDyTZ6V6iUhTJd=6uCVVyTGi8wI4Jr15uzn439mbGgY5OZU4baStzO8Pp08g2r0VdSs4z8Z_o7Px3EJv8pMz9xliUjlP9286AIMu9ACYE1Jr6yQj-BQlSSeX=vKw9xTrcFMbVtlYyB8gFDl4nA2RCXHHmByLfRFJTtnMpJU5LV0E7IQoZlXHxX07Pd3WFNWvLghh=I4aPdtcSRXHKPU1jowac8q0MhOynHrOtjCf=aZFUeXWrC6UT-xat7X89BtvzWp_uKH5v6FTlJXNCEEd7Xhnp5tzdDFoe7Ctq9LP7h3ttNgziI0pJznIe5VjGaBGfcibgmKWZ_HeOLNu07NlW4QzSx5WG8MavWiEAM7=IC6WMuSzcG9oytLOK3OrSj7h0_DzMJEjyxNhwtPy6frYoP_cI6JMDauuwKrjocua3eo0s5L7fOnffrZzFZz1rw4ZXUQatdIaBBN0JBJX5plU1IlHb1pmbEdXsWSgSpF5dWsFu=d06CXFGDSwXehjfTGF14myK7Fw43YPXbBLZ25QFBJwT8h9qDOXB=W8pPIrKttrYy0WxFC-2gWzJC9IEjfxlNhvtJX7VoXxf=99LS4WROsLr8SDhjmHAZpqzZi6=D6Mr-UTymxzDq1jpynfsv_BM=225IpsLhzILTva07tefwU2hTovY7M=q8VHIQNOaJgoRKZLhe2L-u=35Zc2otH9AQ2TOidz4o3p01PB-fZ_Zw9o6Q81nuAFF7TwvKWzh=b4=vB5axu-FKC11xisfCXFpP=FudFWoDSPV=fCurla46_ZOhA8NDBTHsgL8K1GnATIBVTa1c1J7hNWeofppHXfR5r_De8bjZgf-HxetlYScW0UXK6at5YbEI4pp06aSzVNT4RiOSCxPvw0ClgPGgBqzra95siMeADspPmhF1YMo-mXa3ngHP2AO=xtgzjXFjCl_6tIXqdyGLZXjSjhvA0GhWdUxyanHLa4uTzvdmDPr1I2xRo=Pqn-fH5fJH13cF_DbPw=7ztN0vKd3gvqzEagz1ztA4o7CXpdL1=7r2K=3mfXxaU9P6xMIbnoAicqveqj6NfP4AyiFWCR5qP71VYFoPMWBAYHz6T6e5jRVz0nWfUhmNyfRGX0VI8BKEqGoJ_VHwE-y-ziWNl617yF7sF-J3fXgSBFYAdKLSg3lfgbpAt7U4Ju5UBHzwllVDVYAL2He1LUb1-XSjquRzQVg9rGwbEEZld7ocCBDrc7ioU8c-NROKfs3m9Z1MTfernxK-EMwMzOvdJmlD_PpcGTbwtr49p4C4FUDcAu3p-EyYr44TmZbdWad0oK2hiiE50PSR9rHHrBQhpEWZ8=V9lqDsmMc7x9w6mnqCE5mF6ZxdpwgrdwNh_w14=wUfO5rX_Hn0-Rpox=VmVi-ifqRHaKLAm6ZfmhW_D_OSMWAFu_FBUVDnINKUaBiaQs0ynzjIcrLjfPbfzvXtAYbI9ZyrwbQ428e7_Zu8PwctyIN1IsweTZqpzHI-bs5wDWvjm9pY6SsQ6YfXfF2C2hAV5OwbpZrdpoA7wiqpBq_=sGWBUKw=6FbuNX8X_=HsOavOw_i=PKwXFq97N8ODrITR82tARqTy5JHwTzeuS0rihX9O=d1L-_vfT9uEyrU=tqAdNw991ZNm1J3qWw83H_Et5NV8FR5vTQqT0=UJ87x_Z00KfaAi2ZsG9shVpAUsOPf95-IfMfRafZTXBZG-hlUtJss=4HTw7_sBsCbv9WyI2cHlfqRjnFzjJjcHg8ifjYZpZ7Gb5ENRaxzTfsi8yeXgLavHJCdVIwi00yxX21Zlg0pFaK8tP-PmNASEq2A-zu8ln_=QBEiPuuurog3JXwVLUqJd6YSsavAR_euHX=R0=PneS3I0cvXn0Z9h3UVnLnPNrWLgmP5SuhlDwKlY1uyQ3inTD9hsph2llMHu5PK1Z2yj0s7WFJFlYQWe762cmTAIP5igb=qNABO7PR4JhfNKOjUIt5g0HSWgfVxcVlaxp37FQXSqB=L70qAWd8--mJerWSIi=pEP7YHNFJR8QxOL6lSuqHPE=CIjH9g2IUWhO5J65gN=ZgxBDWAEg9ALLRlfLxZ1y=YKOtvhgdiL9t_Ob23RZfAs03XWZi2spibLPbVE3dsIHAdy-GtopaScQU1-VoCuSwNpOH8CCvD2txH4rfzPhZNRu98edBQImI9J_LiYKIEOyRcTw__lhfGUWIKYQonUKhEfm7FgwhGphVdHmrdXFZ6ldLB0epFncgQ0tbF1GftrNTA_HD5SsGlV=BIMDbH6Od5ULuWLU28MJoR3EIcmf9rMBX1RAW7e2XKI43IsXVFdf32ImbVv5P1oymSpJNfErZLPy9r99j-mFb9lTM-e4GzHbRsxO12UysmL_WQ8z1IXla6RAZdXRP1pEVXg4mOIVTAn69TPyOQYerW=oPZfKpYqaJKDhRiebJRCpnJ63JTGn=c8vx5tdJY-Ym37jLTIs2R3xgijGuly2JzBs-KjLfI=2_fBwrlr-LrbRiR6NylJ03NDai-PStESYDD84z-CG2RU5j0xRF15gCXy8RXcy
												htc6j8njvn-f: {htc6j8njvn_f}
												tt-csrf-token: {tt_csrf_token}
												sec-ch-ua-platform: ""Windows""
												Accept: */*
												Origin: https://www.tiktok.com
												Sec-Fetch-Site: same-origin
												Sec-Fetch-Mode: cors
												Sec-Fetch-Dest: empty
												Referer: {refer}");
                try
                {
                    body = rq.Post($"https://www.tiktok.com/api/commit/follow/user/?WebIdLastTime={webIdCreatedTime}&action_type=1&aid={aid}&app_language=vi-VN&app_name=tiktok_web&browser_language=vi-VN&browser_name=Mozilla&browser_online=true&browser_platform=Win32&browser_version=5.0%20%28Windows%20NT%2010.0%3B%20Win64%3B%20x64%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F118.0.0.0%20Safari%2F537.36&channel=tiktok_web&channel_id=0&cookie_enabled=true&device_id={device_id}&device_platform=web_pc&focus_state=true&from=18&fromWeb=1&from_page=user&from_pre=0&history_len=4&is_fullscreen=false&is_page_visible=true&os=windows&priority_region=&referer=&region=VN&screen_height=825&screen_width=1466&sec_user_id=MS4wLjABAAAAB7CEl3KDDgHho3beDviz-2Bg-2BL7FbrtMUVx6L6-kaSwL8S7t3bqirkdVGLZwoD&type=1&tz_name=Asia%2FSaigon&user_id={user_id}&webcast_language=vi-VN&msToken=RUfkLR1e7HcuXyZXbc-z-9UIGojUsIT8HR1rskJ-Fmr1GbT1XjFI7jrLuUcPSCJumvDTYxhns6rDTXrCni8r1czLNZgbRTjyXnya0y6LImycSjUrlkUA9GbNsf61SiHTWFsYb-L6B06m9NZFpL6Vuw==&X-Bogus=DFSzswVueNUANxFHtYU5F09WcBru&_signature=_02B4Z6wo00001aYDfpwAAIDBpgN-n5Wu6U2mA3oAAAy02c").ToString();
                    refer = rq.Address.AbsoluteUri;
                }
                catch
                {
                    //return false;
                }
            }

            return false;
        }
    }

}
