using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace TestFor
{
    public partial class Form2 : Form
    {
        string AllCookie = "";
        public Form2()
        {
            InitializeComponent();
            var height = 1000;
            var width = 500;
            webView21.MinimumSize = new(height, width);
            webView21.MaximumSize = new(height, width);
            this.MinimumSize = webView21.MinimumSize;
            this.MaximumSize = webView21.MaximumSize;

        }

        private void Form2_Load(object sender, EventArgs e)
        {

            webView21.Source = new Uri("https://passport.bilibili.com/login");
            timer1.Interval = 5000;
        }

        private async void GetStart()
        {
            List<Microsoft.Web.WebView2.Core.CoreWebView2Cookie> Cookies = null;
            if (webView21?.CoreWebView2?.CookieManager != null)
            {
                Cookies = await webView21.CoreWebView2.CookieManager.GetCookiesAsync("https://passport.bilibili.com/login");

            }
            else
            {
                return;
            }

            if (Cookies != null)
            {
                foreach (var Cookie in Cookies)
                {

                    string Cook = Cookie.Value;
                    string Name = Cookie.Name;
                    if (Name.IndexOf("bili_jct") != -1)
                    {
                        if (AllCookie == "")
                        {
                            AllCookie = "bili_jct=" + Cookie.Value + ";";

                        }
                        else
                        {
                            AllCookie = AllCookie + "bili_jct=" + Cookie.Value;//bili_jct=114514;SESSDATA=114154
                        }
                    }
                    else if (Name.IndexOf("SESSDATA") != -1)
                    {
                        if (AllCookie == "")
                        {
                            AllCookie = "SESSDATA=" + Cookie.Value + ";";

                        }
                        else
                        {
                            AllCookie = AllCookie + "SESSDATA=" + Cookie.Value;
                        }
                    }
                }
            }
            if (AllCookie != "")
            {
                File.WriteAllText(Directory.GetCurrentDirectory()+"\\Cookies",AllCookie);
                this.Text = "登录成功,即将跳转...";
                this.Text = "sucessful";
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetStart();
        }
    }
}
