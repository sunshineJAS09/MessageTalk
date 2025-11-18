using MessageTalk;
using RestSharp;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TestFor
{
    public partial class Form1 : Form
    {
        string bilijct = "144d8159fe6c7147fab2c60ffea00a39";
        string sessdata = "501d50f3%2C1778774000%2C6dbb0%2Ab2CjBGB9J5WodbrggCPd1neWcEl0Pi1jcIxYktJNrY8zvPMDy1gl2waidrx_Sk-5smLbISVjQxTFN2YW8ycHhxYXU2WWF4VHM0TU8wWTNEeVI2NVgtZnp1MkFnam1VSldaZ2h3QmxsQS1ZRWtLWUk0ODRlZmctMDhNZlZIQjIyMElka0pFMndoNzdnIIEC";
        string uid = "604524574";
        Dictionary<string, string> datas = new Dictionary<string, string>() { };
        public Form1()
        {
            InitializeComponent();
            
        }


        void Servers()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Pages"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Pages");
            }
            var biliServer = new ServerFunctions(bilijct, sessdata);
            string[] FilePath = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Pages", "*.txt");
            Dictionary<string, string> Temp = new Dictionary<string, string>() { };
            List<string> TempTimes = new List<string>() { };
            Dictionary<string, string> TempTimesDictionary = new Dictionary<string, string>() { };
            foreach (var time in FilePath)
            {
                var math = Path.GetFileName(time).Replace(".txt", "").Split(')');// ")" 为集     示例:6)炮打资产阶级
                TempTimes.Add(math[0]);
                TempTimesDictionary.Add(math[0], time);
            }
            TempTimes.Sort();//从小到底大
            TempTimes.Reverse();//开始从最新的开始
            foreach (var time in TempTimes)
            {
                var Name = Path.GetFileName(TempTimesDictionary[time].Replace(".txt", ""));
                var data = File.ReadAllText(TempTimesDictionary[time]).Split('/')[File.ReadAllText(TempTimesDictionary[time]).Split('/').Length - 1];//http ;114514
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

            var bili = new GetMessage(bilijct, sessdata);
            var Datas = bili.GetMessages(uid);

            foreach (var Data in Datas)
            {
                string text = "";
                int i = 0;
                for (i = 0; i < Data.Length; i++)
                {
                    if (Data[i] == 'n' || Data[i] == '&')// 1 n 2 3 4
                    {
                        text = Data.Substring(i + 1, Data.Length - i - 1);
                        break;
                    }
                }
                int Mathid = i;
                var IDS = text.Split(',');

                var Name = IDS[0];
                var Urls = IDS[1];
                string result = bili.AesRemove(Name);
                for (int y = 0;y<result.Length;y++)
                {
                    char word = result[y];
                    if (word == ')')
                    {
                        result = result.Substring(y+1,result.Length-y-1);
                        break;
                    }
                }
                datas.Add(result, Urls);

                foreach (var Names in datas.Keys)
                {
                    listBox1.Items.Add(Names);
                }
                Mathid = Mathid - 1;
                if (Mathid == 0)
                {
                    break;
                }
            }
        }




        private async void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            var LanZouYun = new LanZouYun();
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + listBox1.SelectedItem.ToString() + ".pdf", LanZouYun.LanZouYUnJX(datas[listBox1.SelectedItem.ToString()]));
            //File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + listBox1.SelectedItem.ToString() + ".pdf", LanZouYun.LanZouYUnJX(listBox1.SelectedItem.ToString()));
            Process.Start(new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + "\\" + listBox1.SelectedItem.ToString() + ".pdf",
                UseShellExecute = true
            });//暂时测试
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //button1.Visible = false;
            Client();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Servers();
        }
    }
}

