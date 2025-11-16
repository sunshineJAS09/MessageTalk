using MessageTalk;

namespace TestFor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Client();
            Servers();
        }
        void Servers()
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
        void Client()
        {
            Dictionary<string,string> datas = new Dictionary<string, string>() { };
            var bili = new GetMessage("5938330e5c560726c5431c77852555bf", "2cd3ac70%2C1778502984%2Ca38a8%2Ab2CjCdxtaMeFINy5E6pIXkLu_yEbRotv1dtGu--FzrXmD-lJ_yutHOUh7zhPXx0dOrcXsSVkh5Qy1PaXdqM2xDMDVKTVJocjVBOFp2Wjh0Z0R6R05DWXN4dkF5ZHlLcnFPaXlLOXlpUlhTNkljaVVBMXhHNkd2UlNUYmZONmJ6RVYxTy14UXBCRGFnIIEC");
            var Datas = bili.GetMessages("604524574");
            
            foreach (var Data in Datas)
            {
                string text = "";
                int i = 0;
                for (i = 0; i < Data.Length; i++)
                {
                    if (Data[i] == 'n' || Data[i]=='&')// 1 n 2 3 4
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
                    datas.Add(Name,Urls+","+Password);
                }
                foreach (var Name in datas.Keys)
                {
                    listBox1.Items.Add(Name);

                }
                Mathid = Mathid - 1;
                if (Mathid == 0) 
                {
                    break;
                }
            }
        }
    }
}
