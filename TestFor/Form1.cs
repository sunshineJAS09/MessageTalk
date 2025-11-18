using Ionic.Zip;
using MessageTalk;
using System.Diagnostics;
using System.Text;

namespace TestFor
{
    public partial class Form1 : Form
    {
        Form2 login = new Form2();
        string bilijct = "";
        string sessdata = "";
        string uid = "604524574";//这只是为了测试
        Dictionary<string, string> datas = new Dictionary<string, string>() { };
        public Form1()
        {
            InitializeComponent();

        }
        int Count = 0;


        void Client()
        {

            datas.Clear();
            listBox1.Items.Clear();
            var bili = new GetMessage(bilijct, sessdata);
            var Datas = bili.GetMessages(uid);
            int i = 0;
            string text = "";
            for (i = 0; i < Datas[0].Length; i++)//获取多少长度
            {
                if (Datas[0][i] == 'n' || Datas[0][i] == '&')// 1 n 2 3 4
                {
                    text = Datas[0].Substring(i + 1, Datas[0].Length - i - 1);
                    break;
                }
            }
            int Mathid = i;
            foreach (var Datass in Datas)
            {
                for (int y = 0; y < Datass.Length; y++)//获取多少长度
                {
                    if (Datass[y] == 'n' || Datass[y] == '&')// 1 n 2 3 4
                    {
                        text = Datass.Substring(y + 1, Datass.Length - y - 1);
                        break;
                    }
                }//取原始文本
                text = text.Substring(0, text.Length - 1);//去除;

                foreach (var s in text.Split(';'))
                {
                    var IDS = s.Split(',');
                    var Name = IDS[0];
                    var Urls = IDS[1];
                    string result = bili.AesRemove(Name);
                    for (int y = 0; y < result.Length; y++)
                    {
                        char word = result[y];
                        if (word == ')')
                        {
                            result = result.Substring(y + 1, result.Length - y - 1);
                            break;
                        }
                    }
                    datas.Add(result, Urls);


                    listBox1.Items.Add(result);



                }
                Mathid = Mathid - 1;//一次次来
                if (Mathid == 0)
                {
                    break;
                }



            }
        }




        private async void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Message") == false)
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Message");
            }
            var LanZouYun = new LanZouYun();
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString() + ".pdf"))
            {
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString() + ".zip", LanZouYun.LanZouYUnJX(datas[listBox1.SelectedItem.ToString()]));
                var zip = ZipFile.Read(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString() + ".zip");
                zip.Password = "mlmws";
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString());
                zip.ExtractAll(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString(), ExtractExistingFileAction.OverwriteSilently);
                string[] file = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString(),"*.pdf");
                File.Move(file[0], Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString() + ".pdf");
                File.Delete(file[0]);
                Directory.Delete(Directory.GetCurrentDirectory()+"\\"+"Message"+"\\"+ listBox1.SelectedItem.ToString());
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + "\\" + "Message" + "\\" + listBox1.SelectedItem.ToString() + ".pdf",
                UseShellExecute = true
            });


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] deletefile = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Message", "*.zip");
            foreach (var file in deletefile)
            {
                File.Delete(file);//日常清理缓存
            }
            timer1.Interval = 1000;
            if (File.Exists(Directory.GetCurrentDirectory() + "\\Cookies") == false)
            {
                login.ShowDialog();
            }
            else
            {
                Start();
            }

        }
        private void Start()
        {
            Count = 1;
            timer1.Interval = 60000;
            var Data = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Cookies").Split(';');
            foreach (var cookie in Data)
            {
                if (cookie.IndexOf("bili_jct") != -1)
                {
                    bilijct = cookie.Replace("bili_jct=", "");
                }
                else if (cookie.IndexOf("SESSDATA") != -1)
                {
                    sessdata = cookie.Replace("SESSDATA=", "");
                }
            }
            if (new GetMessage(bilijct, sessdata).iflogin() == false)
            {
                login.ShowDialog();
            }
            if (bilijct == "" || sessdata == "")
            {
                MessageBox.Show("出现未知错误");
                Application.Exit();
            }
            Client();

        }
        private void button1_Click_1(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (login.Text == "sucessful")
            {
                if (Count == 1)
                {
                    Client();
                }
                else if (Count == 0)
                {
                    login.Close();
                    Start();

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] deletefile = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Message", "*.zip");
            foreach (var file in deletefile)
            {
                File.Delete(file);//日常清理缓存
            }
        }
    }
}

