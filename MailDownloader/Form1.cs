using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MailDownloader.Logic;
using Microsoft.WindowsAPICodePack.Dialogs;
using Org.Mentalis.Network.ProxySocket;

namespace MailDownloader
{
    public partial class Form1 : Form
    {
        private MultiMailDownloadManager mmdm;
        private List<Mail> mails;
        private List<ProxyConfig> proxies;
        private Thread download;
        private TcpServer server;
        private Dictionary<string, State> list;
        public Form1()
        {
            Logger.IsAutosave = true;
            try
            {
                server = new TcpServer();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
                return;
            }
            list = server.GetClients();
            ConfigManager.DomainNotFound += Manager_DomainNotFound;
            mmdm = new MultiMailDownloadManager();
            InitializeComponent();
            mmdm.OnUpdateStatus += delegate
            {
                this.Invoke((MethodInvoker)delegate
                {
                    int all, left, valid, invalid, questionable;
                    mmdm.GetDownloadStats(out valid, out invalid, out left, out questionable);
                    all = valid + invalid + left + questionable;
                    accounts_tb.Text = all.ToString();
                    invalid_tb.Text = invalid.ToString();
                    valid_tb.Text = valid.ToString();
                    left_tb.Text = left.ToString();
                    files_tb.Text = mmdm.DownloadedFileCount.ToString();
                });
            };
            mmdm.OnStoppedDownloading += delegate
            {
                this.Invoke((MethodInvoker)delegate
                {
                    left_btn2.Enabled = invalid_btn.Enabled = valid_btn.Enabled = true;
                });
            };
            server.CollectionChange += delegate
            {
                this.Invoke((MethodInvoker)delegate
                {
                    macAddresses.Items.Clear();
                    list = server.GetClients();
                    foreach (var i in list)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(i.Key);
                        string action = "";
                        switch (i.Value)
                        {
                            case State.Accepted:
                                action = "Простаивает";
                                break;
                            case State.Blocked:
                                action = "Простаивает (заблокирован)";
                                break;
                            case State.Requesting:
                                action = "Запрашивает подключение";
                                break;
                            case State.Downloading:
                                action = "Скачивает";
                                break;
                            case State.Stopped:
                                action = "Перестал скачивать";
                                break;
                        }
                        sb.Append(" - ");
                        sb.Append(action);
                        macAddresses.Items.Add(sb.ToString());
                    }
                });
            };
            address_tb.Text = server.GetAddress();
        }
        private ServerConfig Manager_DomainNotFound(string domain)
        {
            Form3 form = new Form3(domain);
            DialogResult dr = form.ShowDialog();
            if (dr == DialogResult.No)
            {
                return new ServerConfig();
            }
            else if (dr == DialogResult.OK)
            {
                return new ServerConfig(form.server, form.port, form.ssl);
            }
            else
            {
                return null;
            }
        }
        private void settings_btn_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2(mmdm.settings);
            form.ShowDialog();
        }
        private List<Mail> mailsFromText(string[] mails_str)
        {
            List<Mail> mails = new List<Mail>();
            for (int i = 0; i < mails_str.Length; i++)
            {
                string[] res = mails_str[i].Split(':');
                if (res.Length >= 2)
                {
                    string pass = res[1];
                    for (int j = 2; j < res.Length; j++)
                    {
                        pass += ":" + res[j];
                    }
                    mails.Add(new Mail(res[0], pass));
                }
                else
                {
                    mails.Add(new Mail(mails_str[i], null));
                }
            }
            return mails;
        }
        private void mail_pass_buffer_Click(object sender, EventArgs e)
        {
            string text = Clipboard.GetText();
            string[] mails_str = text.Split('\n');
            mails = mailsFromText(mails_str);
            mail_pass_buffer.BackColor = Color.LimeGreen;
            mail_pass_file.BackColor = settings_btn.BackColor;
        }
        private ProxyTypes GetProxy()
        {
            ProxyTypes res = ProxyTypes.None;
            if (proxy_type1.Checked)
                res = ProxyTypes.Socks4;
            else if (proxy_type2.Checked)
                res = ProxyTypes.Socks5;
            return res;
        }
        private void start_btn_Click(object sender, EventArgs e)
        {
            int? threads = parseThreadsCount();
            if (threads == null)
                return;
            if (mails != null)
            {
                if (proxies != null && proxies?.Count != 0)
                {
                    DialogResult dr = MessageBox.Show("Использовать прокси?", "", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        ProxyTypes res = GetProxy();
                        if (res != ProxyTypes.None)
                            new ProxyConfigManager(proxies, res);
                        else
                        {
                            MessageBox.Show("Не выбран тип прокси");
                            return;
                        }
                    }
                    else
                    {
                        ConfigManager.SummonBase();
                    }
                }
                mail_pass_buffer.BackColor = settings_btn.BackColor;
                mail_pass_file.BackColor = settings_btn.BackColor;
                download = new Thread(() =>
                {
                    mmdm.StartDownloading(mails, threads.Value);
                });
                download.Start();
            }
            else
            {
                MessageBox.Show("mail:pass коллекция не выбрана");
                return;
            }
            left_btn2.Enabled = invalid_btn.Enabled = valid_btn.Enabled = false;
            files_btn.Enabled = true;
        }
        private int? parseThreadsCount()
        {
            int res;
            if (int.TryParse(threads_tb.Text, out res))
                return res;
            MessageBox.Show("Неправильный формат числа");
            return null;
        }
        private void mail_pass_file_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = mmdm.GetCreatedPath();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                mails = mailsFromText(System.IO.File.ReadAllLines(dialog.FileName));
                mail_pass_buffer.BackColor = settings_btn.BackColor;
                mail_pass_file.BackColor = Color.LimeGreen;
            }
        }
        private void left_btn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Проверено писем\\всего: " + mmdm.GetDownloadProgress());
        }
        private void stop_btn_Click(object sender, EventArgs e)
        {
            mmdm.StopDownloading();
            left_btn2.Enabled = invalid_btn.Enabled = valid_btn.Enabled = true;
            mmdm.StopDownloading();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mmdm != null)
                stop_btn_Click(null, null);
            if (server != null)
                server.Stop();
            ConfigManager.DomainNotFound -= Manager_DomainNotFound;
            Logger.IsAutosave = false;
        }
        private void filter_btn_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", mmdm.FilterPath);
        }
        private void files_btn_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", mmdm.GetCreatedPath());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (macAddresses.SelectedItem != null)
            {
                string address = macAddresses.SelectedItem.ToString().Split(' ')[0];
                server.Block(address);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (macAddresses.SelectedItem != null)
            {
                string address = macAddresses.SelectedItem.ToString().Split(' ')[0];
                server.Unblock(address);
            }
        }
        private List<ProxyConfig> proxiesFromText(string[] strs)
        {
            List<ProxyConfig> proxies = new List<ProxyConfig>();
            for (int i = 0; i < strs.Length; i++)
            {
                string[] res = strs[i].Split(':');
                if (res.Length >= 2)
                {
                    System.Net.IPAddress ip;
                    if (System.Net.IPAddress.TryParse(res[0], out ip))
                    {
                        int port;
                        if (int.TryParse(res[1], out port))
                            proxies.Add(new ProxyConfig(ip, port));
                    }
                }
            }
            return proxies;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = mmdm.GetCreatedPath();
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                proxies = proxiesFromText(System.IO.File.ReadAllLines(dialog.FileName));
                proxy_type_group.Enabled = true;
                button3.BackColor = Color.LimeGreen;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
