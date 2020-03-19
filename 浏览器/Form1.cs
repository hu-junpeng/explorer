using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

namespace 浏览器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("User32.DLL")]
        public static extern int SendMessage(IntPtr hWnd,uint Msg,int wParam,string Iparam);
        [DllImport("User32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,IntPtr hwndChildAfter,string lpszWindow,string str);
        public const uint WM_SETTEXT = 0x00C;
        private void Open_Click(object sender, EventArgs e)
        {
            openPage();
        }
        private void openPage()
        {
            if (textBox1.Text.Length > 0)
            {
                webBrowser2.Navigate(textBox1.Text.Trim());
            }
            else
            {
                MessageBox.Show("请正确输入你要访问的网址！！包括http://");
            }
        }

        private void webBrowser2_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            if (webBrowser2.Document.ActiveElement != null)
            {
                webBrowser2.Navigate(webBrowser2.Document.ActiveElement.GetAttribute("href"));
                textBox1.Text =webBrowser2.Document.ActiveElement.GetAttribute("href");
            }
        }

        private void webBrowser2_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (webBrowser2.CanGoBack)
            {
                toolStrip1.Items[0].Enabled = true;
            }
            else
                toolStrip1.Items[0].Enabled = false;
            if(webBrowser2.CanGoForward)
            {
                toolStrip1.Items[1].Enabled = true;
            }
            else
                toolStrip1.Items[1].Enabled = false;
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            { 
                if(e.ClickedItem.Name=="tsbBack")
                {
                    webBrowser2.GoBack();
                }
                if (e.ClickedItem.Name=="tsbForward")
                {
                    webBrowser2.GoForward();
                }
                if (e.ClickedItem.Name == "tsbRefresh")
                {
                    webBrowser2.Refresh();
                }
                if(e.ClickedItem.Name=="tsbHome")
                {
                    webBrowser2.GoHome();
                }
                if(e.ClickedItem.Name=="tsbStop")
                {
                    webBrowser2.Stop();
                }
                if(e.ClickedItem.Name=="tsbExit")
                {
                    if (MessageBox.Show("确认退出浏览器吗？", "退出对话框", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        Application.Exit();
                    }
                }
                if (e.ClickedItem.Name == "tsbViewSource")
                {
                    WebRequest wrq = WebRequest.Create(textBox1.Text);
                    WebResponse wrs = wrq.GetResponse();
                    StreamReader sr = new StreamReader(wrs.GetResponseStream(),Encoding.Default);
                    string code = "";
                    while(sr.ReadLine()!=null)
                    {
                        code += sr.ReadLine();
                    }
                    System.Diagnostics.Process pro = new System.Diagnostics.Process();
                    pro.StartInfo.FileName = "notepad.exe";
                    pro.StartInfo.RedirectStandardInput = true;
                    pro.StartInfo.RedirectStandardOutput = true;
                    pro.StartInfo.UseShellExecute = false;
                    pro.Start();
                    if(pro!=null)
                    {
                        while(pro.MainWindowHandle==IntPtr.Zero)
                        {
                            pro.Refresh();
                        }
                        IntPtr vHandle = FindWindowEx(pro.MainWindowHandle,IntPtr.Zero,"Edit",null);
                        SendMessage(vHandle,WM_SETTEXT,0,code);
                    }
                }
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message.ToString()); }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            if (c == 13) openPage();
        }
    }
}
