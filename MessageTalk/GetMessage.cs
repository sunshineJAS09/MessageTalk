using KCNLanzouDirectLink;
using RestSharp;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static MessageTalk.LanZouYun;
namespace MessageTalk
{
    public class All
    {
        public Aes aes = Aes.Create();
        public RestClient Client = new RestClient();
        public RestRequest Request = new RestRequest();
        public static string bilijct;
        public static string sessdata;
        public void Settings(RestSharp.Method methods, string Url, Dictionary<string, string> parts = null)
        {
            Request = new RestRequest(Url, methods);
            if (bilijct != null && sessdata != null)
            {
                Request.AddCookie("bili_jct", GetMessage.bilijct, "/", ".bilibili.com");
                Request.AddCookie("SESSDATA", GetMessage.sessdata, "/", ".bilibili.com");
                if (methods == Method.Get)//如果访问方式为Get
                {
                    Request.AddHeader(name: "user-agent", value: "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36 Edg/140.0.0.0");
                }
            }
            else
            {
                if (bilijct == null && sessdata != null)
                {
                    throw new Exception("bili_jct==null");
                }
                else if (bilijct == null && sessdata == null)
                {
                    throw new Exception("sessdata==null;bili_jct==null");

                }
                else if (bilijct != null && sessdata == null)
                {
                    throw new Exception("sessdata==null");
                }//防止bug
            }
            if (parts != null)
            {
                foreach (var part in parts)
                {
                    Request.AddParameter(part.Key, part.Value);
                }

            }

        }
        public string AesAdd(string text)
        {
            aes.IV = Encoding.UTF8.GetBytes("1234123412341234");
            aes.Key = Encoding.UTF8.GetBytes("1234123412341234");
            return Convert.ToBase64String(aes.CreateEncryptor().TransformFinalBlock(Encoding.UTF8.GetBytes(text), 0, Encoding.UTF8.GetBytes(text).Length));
        }
        public string AesRemove(string text)
        {
            byte[] bytes = Convert.FromBase64String(text);
            aes.IV = Encoding.UTF8.GetBytes("1234123412341234");
            aes.Key = Encoding.UTF8.GetBytes("1234123412341234");
            return Encoding.UTF8.GetString(aes.CreateDecryptor().TransformFinalBlock(bytes, 0, bytes.Length));
        }
    }
    public class GetMessage : All
    {

        public GetMessage(string bilijct, string sessdata)
        {
            All.bilijct = bilijct;
            All.sessdata = sessdata;
        }
        public bool iflogin()
        {
            var Client = new RestClient("https://api.bilibili.com");
            Settings(Method.Get, "/x/web-interface/nav");
            if (Client.Execute(Request).Content.IndexOf("-101") == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<string> GetMessages(string uid)
        {
            Settings(Method.Get, $"https://api.bilibili.com/x/polymer/web-dynamic/desktop/v1/feed/space?host_mid={uid}");
            List<string> Message = new List<string>() { };
            var data = Client.Execute(Request).Content.ToString();
            for (int i = 0; i < JsonDocument.Parse(data).RootElement.GetProperty("data").GetProperty("items").GetArrayLength(); i++)
            {
                Message.Add(JsonDocument.Parse(data).RootElement.GetProperty("data").GetProperty("items")[i].GetProperty("modules")[1].GetProperty("module_desc").GetProperty("text").ToString());
            }
            return Message;
        }
        public List<string> ReadMessages(string adi)
        {
            Settings(Method.Get, $"https://api.bilibili.com/x/polymer/web-dynamic/desktop/v1/feed/space?host_mid={adi}");
            List<string> Message = new List<string>() { };
            foreach (var i in JsonDocument.Parse(Client.Execute(Request).Content.ToString()).RootElement.GetProperty("data").GetProperty("items").EnumerateArray())
            {
                Message.Add(i.GetProperty("modules").GetProperty("module_desc").GetProperty("text").ToString());
            }
            return Message;
        }
    }
    public class ServerFunctions : All
    {
        public ServerFunctions(string bilijct, string sessdata)
        {
            All.bilijct = bilijct;
            All.sessdata = sessdata;
        }

        public string SendArticle(string text)
        {
            var client = new RestClient("https://api.vc.bilibili.com");
            var req = new RestRequest("/dynamic_svr/v1/dynamic_svr/create", Method.Post);
            req.AddParameter("dynamic_id", "0");
            req.AddParameter("type", "4");
            req.AddParameter("rid", "0");
            req.AddParameter("content", text);
            req.AddCookie("bili_jct", "144d8159fe6c7147fab2c60ffea00a39", "/", ".bilibili.com");
            req.AddCookie("SESSDATA", "501d50f3%2C1778774000%2C6dbb0%2Ab2CjBGB9J5WodbrggCPd1neWcEl0Pi1jcIxYktJNrY8zvPMDy1gl2waidrx_Sk-5smLbISVjQxTFN2YW8ycHhxYXU2WWF4VHM0TU8wWTNEeVI2NVgtZnp1MkFnam1VSldaZ2h3QmxsQS1ZRWtLWUk0ODRlZmctMDhNZlZIQjIyMElka0pFMndoNzdnIIEC", "/", ".bilibili.com");
            req.AddParameter("csrf_token", All.bilijct);
            req.AddParameter("csrf", All.bilijct);
            return client.Execute(req).Content.ToString();
        }

    }
    public class LanZouYun
    {
        public Byte[] LanZouYUnJX(string ID)
        {
            byte[] bytefile = null;
            string data = "";
            var Client = new RestClient("https://wwxa.lanzouu.com");
            var Req = new RestRequest("/"+ID, Method.Get);
            string html = "";
            Req.AddHeader("Referer", "https://wwxa.lanzouu.com/"+ID);
            Req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");
            Req.AddHeader("Accept", "application/json, text/javascript, */*");
            html = Client.Execute(Req).Content.ToString();
            foreach (var test in html.Split('\n'))
            {
                if (test.IndexOf("src") != -1)
                {
                    data = Regex.Split(test, "src")[1].Replace("frameborder=\"0\" scrolling=\"no\"></iframe>", "").Split('/')[1].Replace("\"", "");
                    if (data.IndexOf("fn") == -1)
                    {
                        continue;
                    }
                    break;
                }
            }
            Req = new RestRequest($"/{data}", Method.Get);
            Req.AddHeader("Referer", "https://wwxa.lanzouu.com/"+ID);
            Req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");
            Req.AddHeader("Accept", "application/json, text/javascript, */*");
            //MessageBox.Show(Client.Execute(Req).Content.ToString());
            string file = "";
            string ty = "";
            string sign = "";
            html = Client.Execute(Req).Content.ToString();
            foreach (var test in html.Split('\n'))
            {
                if (test.IndexOf("url : '/ajaxm.php?file=") != -1 && test.IndexOf("',//data///////") != -1)
                {
                    file = test.Split('=')[1].Split('\'')[0];
                    //break;
                }
                if (test.IndexOf("var wp_sign") != -1)
                {
                    sign = test.Split('=')[1].Replace("\'", "").Replace(";", "");
                }
                if (test.IndexOf("data : { 'action':'") != -1)
                {
                    foreach (var tos in test.Replace("data : { '", "").Split(','))
                    {
                        if (tos.Replace("\'", "").Replace(":", "=").Replace("}", "").Replace("//", "").Replace("/", "").Trim() == "")
                        {
                            continue;
                        }
                        if (tos.IndexOf("//'ves':1 }") != -1)
                        {
                            continue;
                        }
                        if (ty == "")
                        {
                            ty = tos.Replace("\'", "").Replace(":", "=").Replace("}", "").Replace("//", "").Replace("/", "");

                        }
                        else
                        {
                            if (tos.IndexOf("sign") == -1)
                            {
                                ty = ty + "&" + tos.Replace("\'", "").Replace(":", "=").Replace("}", "").Replace("//", "").Replace("/", "");

                            }
                            else
                            {
                                if (tos.IndexOf("websign") == -1 && tos.IndexOf("wp_sign") == -1)
                                {
                                    ty = ty + "&" + "sign=" + sign;
                                }
                                else
                                {
                                    ty = ty + "&" + tos.Replace("\'", "").Replace(":", "=").Replace("}", "").Replace("//", "").Replace("/", "");

                                }

                            }

                        }
                    }
                    ty = ty.Trim();
                }
            }

            string[] funclist = { "action", "websignkey", "sign", "websign", "kd", "ves" };
            Dictionary<string, string> func = new Dictionary<string, string>()
            {
                ["action"] = "",
                ["websignkey"] = "",
                ["sign"] = "",
                ["websign"] = "",
                ["kd"] = "",
                ["ves"] = ""
            };
            foreach (string t in ty.Split('&'))
            {
                var tp = t.Split('=');
                if (tp[1] == "wp_sign")
                {
                    continue;
                }
                foreach (var key in funclist)
                {

                    if (func.ContainsKey(tp[0]))
                    {
                        func[tp[0]] = tp[1];
                        break;
                    }
                }
            }
            var postData = new
            {
                action = func["action"],
                websignkey = func["websignkey"],
                sign = func["sign"],
                websign = func["websign"],
                kd = func["action"],
                ves = func["action"]
            };
            Client = new RestClient("https://wwxa.lanzouu.com");
            Req = new RestRequest($"/ajaxm.php?file={file} ", Method.Post);
            Req.AddObject(postData);
            Req.AddHeader("Referer", "https://wwxa.lanzouu.com/" + data);
            data = Client.Execute(Req).Content.ToString();
            data = JsonDocument.Parse(Client.Execute(Req).Content.ToString()).RootElement.GetProperty("url").ToString();
            var options = new RestClientOptions("https://developer-oss.lanrar.com")
            {
                FollowRedirects = false  // 关键：禁用自动重定向
            };

            Client = new RestClient(options);
            Req = new RestRequest("/file/" + data, Method.Get);
            Req.AddHeader("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            var headersData = Client.Execute(Req).Headers;
            foreach (var header in headersData)
            {
                if (header.ToString().IndexOf("Location") != -1)
                {
                    data = header.ToString().Replace(header.ToString().Split('=')[0], "");
                    data = data.Substring(1, data.Length - 1);
                    break;
                }
            }
            Client = new RestClient(data);
            Req = new RestRequest("", Method.Get);
            bytefile = Client.Execute(Req).RawBytes;
            return bytefile;
        }
    }
}

