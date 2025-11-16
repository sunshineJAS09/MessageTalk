using RestSharp;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
                if(bilijct==null && sessdata != null)
                {
                    throw new Exception("bili_jct==null");
                }
                else if(bilijct==null && sessdata == null)
                {
                    throw new Exception("sessdata==null;bili_jct==null");

                }
                else if(bilijct!=null  && sessdata == null)
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
            GetMessage.sessdata = sessdata;
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
        void Client()
        {
            Dictionary<string, string> datas = new Dictionary<string, string>() { };
            var bili = new GetMessage("5938330e5c560726c5431c77852555bf", "2cd3ac70%2C1778502984%2Ca38a8%2Ab2CjCdxtaMeFINy5E6pIXkLu_yEbRotv1dtGu--FzrXmD-lJ_yutHOUh7zhPXx0dOrcXsSVkh5Qy1PaXdqM2xDMDVKTVJocjVBOFp2Wjh0Z0R6R05DWXN4dkF5ZHlLcnFPaXlLOXlpUlhTNkljaVVBMXhHNkd2UlNUYmZONmJ6RVYxTy14UXBCRGFnIIEC");
            var Datas = bili.GetMessages("604524574");

            foreach (var Data in Datas)
            {
                string text = "";
                int i = 0;
                for (i = 0; i < Data.Length; i++)
                {
                    if (Data[i] == 'n' || Data[i] == '&')// 1 n 2 3 4
                    {
                        text = Data.Substring(i, Data.Length);
                        break;
                    }
                }
                int Mathid = i;
                var IDS = text.Split(';');
                foreach (var ID in IDS)//获取数据
                {
                    var DataIDs = ID.Split(',');
                    var Name = DataIDs[0];
                    var Urls = DataIDs[1];
                    var Password = DataIDs[2];
                    datas.Add(Name, Urls + "," + Password);
                }
                foreach (var Name in datas.Keys)
                {
                    //待测试...

                }
                Mathid = Mathid - 1;
                if (Mathid == 0)
                {
                    break;
                }
            }
        }
    }
    public class ServerFunctions : All
    {
        public string SendArticle(string text)
        {
            Dictionary<string, string> Part = new Dictionary<string, string>() 
            {
                ["dynamic_id"] = "0",
                ["type"]="4",
                ["rid"]="0",
                ["content"] =text,
            };
            Settings(Method.Get, $"https://api.vc.bilibili.com/dynamic_svr/v1/dynamic_svr/create",Part);

            var Json = JsonDocument.Parse(Client.Execute(Request).Content.ToString()).RootElement;
            if (Json.GetProperty("message").ToString() != "") 
            {
                throw new Exception("CookieError");
            }
            return Json.GetProperty("data").GetProperty("dynamic_id").ToString();
        }
        void Servers()//待测试
        {
            var biliServer = new ServerFunctions();
            string[] FilePath = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\", "*.txt");
            Dictionary<string, string> Temp = new Dictionary<string, string>() { };
            List<string> TempTimes = new List<string>() { };
            Dictionary<string, string> TempTimesDictionary = new Dictionary<string, string>() { };
            foreach (var time in FilePath)
            {
                var math = time.Split(')');// ")" 为集     示例:6)炮打资产阶级
                TempTimes.Add(math[0]);
                TempTimesDictionary.Add(math[0], math[1]);
            }
            TempTimes.Sort();//从小到底大
            TempTimes.Reverse();//开始从最新的开始
            foreach (var time in TempTimes)
            {
                var Name = Path.GetFileName(TempTimesDictionary[time]);
                var data = File.ReadAllText(TempTimesDictionary[time]);//http ;114514
                var url = data.Split(';')[0];//https:  www     .com
                var password = data.Split(";")[1];//114514
                Temp.Add(biliServer.AesAdd(Name), url + ";" + password);//开始添加   
            }
            string TempText = "";
            int i = 0;//用于计数
            foreach (var informations in Temp)
            {
                var url = informations.Value.Split(';')[0];//http fddsfsd.com
                var password = informations.Value.Split(';')[1];//114514
                if (TempText.Length + (informations.Key + "," + url + "," + password + ";").Length < 1000)//当小于时
                {
                    TempText = TempText + informations.Key + "," + url + "," + password + ";";

                }
                else if (TempText.Length + (informations.Key + "," + url + "," + password + ";").Length > 1000)//当大于时
                {
                    i = i + 1;
                    biliServer.SendArticle(i + "&" + TempText);
                    TempText = "";
                    TempText = informations.Key + "," + url + "," + password + ";";
                }
            }
            if (TempText != "")
            {
                i = i + 1;
                biliServer.SendArticle(i + "n" + TempText);
                TempText = "";//结束
            }

        }
    }
    
}

