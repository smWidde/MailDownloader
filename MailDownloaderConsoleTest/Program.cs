using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//using S22.Imap;
using AE.Net.Mail;
//using Spire.Email.IMap;
//using MimeKit;
//using MailKit.Net.Imap;
namespace MailDownloaderConsoleTest
{
    class Mail
    {
        public string Login { get; set; }
        public string Pass { get; set; }
        public Mail() { }
        public Mail(string email, string pass)
        {
            this.Login = email;
            this.Pass = pass;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Login);
            sb.Append(":");
            sb.Append(Pass);
            return sb.ToString();
        }
    }
    class DomainConfig
    {
        public string Domain { get; set; }
        public ImapConfig Config { get; set; }
        public DomainConfig(string domain, ImapConfig config)
        {
            Domain = domain;
            Config = config;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Domain);
            sb.Append(":");
            sb.Append(Config);
            return sb.ToString();
        }

    }
    class ImapConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public ImapConfig() { }
        public ImapConfig(string server, int port, bool ssl)
        {
            Server = server;
            Port = port;
            SSL = ssl;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Server);
            sb.Append(":");
            sb.Append(Port);
            sb.Append(":");
            sb.Append(SSL);
            return sb.ToString();
        }
    }
    //class MailDownloader
    //{
    //    public string Path { get; set; }
    //    public long SizeFrom { get; set; }
    //    public long SizeTo { get; set; }
    //    public string[] Extensions { get; set; }
    //    public Mail Email { get; set; }
    //    ImapClient ic;
    //    public ImapConfig ImapConfig;
    //    public MailDownloader(ImapConfig imapConfig, Mail email, string path, long size_from, long size_to, string[] extensions)
    //    {
    //        Path = path;
    //        SizeFrom = size_from;
    //        SizeTo = size_to;
    //        Extensions = extensions;
    //        Email = email;
    //        ImapConfig = imapConfig;
    //        ic = new ImapClient(ImapConfig.Server, ImapConfig.Port, Email.Login, Email.Pass, AuthMethod.Login, imapConfig.SSL);
    //    }
    //    public MailDownloader(ImapConfig imapConfig, Mail email)
    //    {
    //        Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(Path);
    //        sb.Append("\\");
    //        sb.Append(DateTime.Now.ToString("MM-dd-yyyy_HH-mm"));
    //        sb.Append("\\");
    //        Path = sb.ToString();
    //        sb.Clear();
    //        SizeFrom = 0;
    //        SizeTo = 25 * 1024 * 1024;
    //        Extensions = new string[] { "*" };
    //        Email = email;
    //        ImapConfig = imapConfig;
    //        ic = new ImapClient(ImapConfig.Server, ImapConfig.Port, Email.Login, Email.Pass, AuthMethod.Login, imapConfig.SSL);
    //    }
    //    public void Download()
    //    {
    //        IEnumerable<string> mailboxes = ic.ListMailboxes();
    //        List<uint> ids = new List<uint>();
    //        foreach (var mailbox in mailboxes)
    //        {
    //            ic.DefaultMailbox = mailbox;
    //            ids.AddRange(ic.Search(SearchCondition.All()));
    //        }
    //        int threads_count = 2;
    //        int ids_count = ids.Count;
    //        int ids_worked = 0;
    //        List<Thread> threads = new List<Thread>();
    //        for (int m = 0; m < threads_count; m++)
    //        {
    //            threads.Add(new Thread(new ParameterizedThreadStart((object obj) =>
    //            {
    //                IEnumerable<uint> uids = (IEnumerable<uint>)obj;
    //                ImapClient ic = new ImapClient(ImapConfig.Server, ImapConfig.Port, Email.Login, Email.Pass, AuthMethod.Login, ImapConfig.SSL);
    //                foreach (var i in uids)
    //                {
    //                    ids_worked++;
    //                    int start = Environment.TickCount;
    //                    RetrieveAttachments(ic, i);
    //                    int end = Environment.TickCount;
    //                    Console.WriteLine(ids_worked + "/" + ids_count + " => " + i + " : " + (end - start));
    //                }
    //            })));
    //        }
    //        Console.WriteLine("messages_operated/all_messages => message_id : milliseconds_tooked_to_get_message");
    //        double k = (double)ids.Count / (double)threads_count;
    //        for (int i = 0; i < threads_count; i++)
    //        {
    //            threads[i].Start(ids.Skip((int)(i * k)).Take((int)k < k ? (int)k + 1 : (int)k));
    //        }
    //    }
    //    private void RetrieveAttachments(ImapClient ic, uint id)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        using (System.Net.Mail.MailMessage mm = ic.GetMessage(id))
    //        {
    //            foreach (var j in mm.Attachments)
    //            {
    //                if (j.ContentStream.Length >= SizeFrom && j.ContentStream.Length <= SizeTo)
    //                {
    //                    string file_name = DecodeMime(j.Name);
    //                    if (ExtensionMatch(file_name.Split('.').Last(), Extensions))
    //                    {
    //                        sb.Append(Path);
    //                        sb.Append(Email.Login);
    //                        sb.Append(" ");
    //                        sb.Append(file_name);
    //                        file_name = sb.ToString();
    //                        sb.Clear();
    //                        using (var fileStream = File.Create(file_name))
    //                        {
    //                            j.ContentStream.Seek(0, SeekOrigin.Begin);
    //                            j.ContentStream.CopyTo(fileStream);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    private void RetrieveAttachments(uint id)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        using (System.Net.Mail.MailMessage mm = ic.GetMessage(id))
    //        {
    //            foreach (var j in mm.Attachments)
    //            {
    //                if (j.ContentStream.Length >= SizeFrom && j.ContentStream.Length <= SizeTo)
    //                {
    //                    string file_name = DecodeMime(j.Name);
    //                    if (ExtensionMatch(file_name.Split('.').Last(), Extensions))
    //                    {
    //                        sb.Append(Path);
    //                        sb.Append(Email.Login);
    //                        sb.Append(" ");
    //                        sb.Append(file_name);
    //                        file_name = sb.ToString();
    //                        sb.Clear();
    //                        using (var fileStream = File.Create(file_name))
    //                        {
    //                            j.ContentStream.Seek(0, SeekOrigin.Begin);
    //                            j.ContentStream.CopyTo(fileStream);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    private string DecodeMime(string text)
    //    {
    //        Console.WriteLine(text);
    //        Regex regex = new Regex(@"=\?{1}(.+)\?{1}([B|Q])\?{1}(.+)\?{1}=");
    //        if (regex.IsMatch(text))
    //        {
    //            string res = "";
    //            foreach (string i in text.Split(' ', '\t').Select(x => x).Where(x => x != ""))
    //            {
    //                res += Encoding.UTF8.GetString(Convert.FromBase64String(i.Split('?')[3]));
    //            }
    //            return res;
    //        }
    //        return text;
    //    }
    //    private bool ExtensionMatch(string extension, string[] extensions_to_match)
    //    {
    //        extension = extension.ToLower();
    //        foreach (string i in extensions_to_match)
    //        {
    //            if (extension.Equals(i.ToLower()) || i == "*")
    //                return true;
    //        }
    //        return false;
    //    }

    //}
    //class MultiMailDownloadManager
    //{
    //    private string configs = "configs.txt";
    //    private string settings = "settings.txt";
    //    private string emails_path;
    //    private string output_path;
    //    private long size_from;
    //    private long size_to;
    //    private string[] extensions;
    //    public string EmailsPath
    //    {
    //        get { return emails_path; }
    //        set
    //        {
    //            emails_path = value;
    //            string[] mails_str = File.ReadAllLines(emails_path);
    //            mails = new List<Mail>();
    //            for (int i = 0; i < mails_str.Length; i++)
    //            {
    //                string[] res = mails_str[i].Split(':');
    //                mails.Add(new Mail(res[0], res[1]));
    //            }
    //        }
    //    }
    //    public string OutputPath { get { return output_path; } set { output_path = value; SaveSettings(); } }
    //    public long SizeFrom { get { return size_from; } set { size_from = value; SaveSettings(); } }
    //    public long SizeTo { get { return size_to; } set { size_to = value; SaveSettings(); } }
    //    public string[] Extensions { get { return extensions; } set { extensions = value; SaveSettings(); } }
    //    private List<DomainConfig> dcs;
    //    private List<Mail> mails;
    //    public MultiMailDownloadManager(string emails_path)
    //    {
    //        EmailsPath = emails_path;
    //        GetSettings();
    //        GetConfigs();
    //    }
    //    public MultiMailDownloadManager()
    //    {
    //        GetSettings();
    //        GetConfigs();
    //    }
    //    private void GetConfigs()
    //    {
    //        dcs = new List<DomainConfig>();
    //        if (File.Exists(configs))
    //        {
    //            string[] dcs_str = File.ReadAllLines(configs);
    //            for (int i = 0; i < dcs_str.Length; i++)
    //            {
    //                string[] res = dcs_str[i].Split(':');
    //                dcs.Add(new DomainConfig(res[0], new ImapConfig(res[1], int.Parse(res[2]), bool.Parse(res[3]))));
    //            }
    //        }
    //    }
    //    private void SaveConfigs()
    //    {
    //        using (StreamWriter sw = File.CreateText(configs))
    //        {
    //            foreach (DomainConfig i in dcs)
    //            {
    //                sw.WriteLine(i);
    //            }
    //        }
    //    }
    //    private void GetSettings()
    //    {
    //        if (File.Exists(settings))
    //        {
    //            string[] settings_strs = File.ReadAllLines(settings);
    //            output_path = settings_strs[0];
    //            size_from = long.Parse(settings_strs[1]);
    //            size_to = long.Parse(settings_strs[2]);
    //            Extensions = settings_strs[3].Split(':');
    //        }
    //        else
    //        {
    //            output_path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    //            size_from = 0;
    //            size_to = 25 * 1024 * 1024;
    //            extensions = new string[] { "*" };
    //            SaveSettings();
    //        }
    //    }
    //    private void SaveSettings()
    //    {
    //        using (StreamWriter sw = File.CreateText(settings))
    //        {
    //            sw.WriteLine(OutputPath);
    //            sw.WriteLine(SizeFrom);
    //            sw.WriteLine(SizeTo);
    //            string res = Extensions[0];
    //            for (int i = 1; i < Extensions.Length; i++)
    //            {
    //                res += ":" + Extensions[i];
    //            }
    //            sw.WriteLine(res);
    //        }
    //    }
    //    public void SetDownloadSettings(string output_path, long size_from, long size_to, string[] extensions)
    //    {
    //        this.output_path = output_path;
    //        this.size_from = size_from;
    //        this.size_to = size_to;
    //        this.extensions = extensions;
    //        SaveSettings();
    //    }
    //    public void StartDownloading()
    //    {
    //        if (mails.Count == 0)
    //            return;
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(OutputPath);
    //        sb.Append("\\");
    //        sb.Append(DateTime.Now.ToString("MM-dd-yyyy_HH-mm"));
    //        sb.Append("\\");
    //        string path = sb.ToString();
    //        Directory.CreateDirectory(path);
    //        foreach (var i in mails)
    //        {
    //            string domain = i.Login.Split('@')[1];
    //            ImapConfig ic = FindConfig(domain);
    //            if (ic == null)
    //            {
    //                Console.WriteLine(domain + " is unknown domain.");
    //                ic = NewConfig();
    //                dcs.Add(new DomainConfig(domain, ic));
    //                SaveConfigs();
    //            }
    //            MailDownloader md = new MailDownloader(ic, i, path, SizeFrom, SizeTo, Extensions);
    //            md.Download();
    //        }
    //    }
    //    private bool MailValid(Mail mail)
    //    {
    //        string domain = mail.Login.Split('@')[1];
    //        ImapConfig ic = FindConfig(domain);
    //        if (ic == null)
    //        {
    //            Console.WriteLine(domain + " is unknown domain.");
    //            ic = NewConfig();
    //            dcs.Add(new DomainConfig(domain, ic));
    //            SaveConfigs();
    //        }
    //        ImapClient client = new ImapClient(ic.Server, ic.Port, ic.SSL);
    //        try
    //        {
    //            client.Login(mail.Login, mail.Pass, AuthMethod.Login);
    //        }
    //        catch(InvalidCredentialsException)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //    public void StartDownloading(Mail mail)
    //    {
    //        if(!MailValid(mail))
    //        {
    //            return;
    //        }
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(OutputPath);
    //        sb.Append("\\");
    //        sb.Append(DateTime.Now.ToString("MM-dd-yyyy_HH-mm"));
    //        sb.Append("\\");
    //        string path = sb.ToString();
    //        Directory.CreateDirectory(path);
    //        string domain = mail.Login.Split('@')[1];
    //        ImapConfig ic = FindConfig(domain);
    //        if (ic == null)
    //        {
    //            Console.WriteLine(domain + " is unknown domain.");
    //            ic = NewConfig();
    //            dcs.Add(new DomainConfig(domain, ic));
    //            SaveConfigs();
    //        }
    //        MailDownloader md = new MailDownloader(ic, mail, path, SizeFrom, SizeTo, Extensions);
    //        md.Download();
    //    }
    //    private ImapConfig FindConfig(string domain)
    //    {
    //        var res = dcs.Find((x) => x.Domain == domain);
    //        if (res == null)
    //            return null;
    //        return res.Config;
    //    }
    //    private ImapConfig NewConfig()
    //    {
    //        Console.WriteLine("You need to enter imap setting for this domain");
    //        Console.Write("Enter imap server (e.g imap.gmail.com, imap.mail.yahoo.com):");
    //        string server = Console.ReadLine();
    //        Console.Write("Enter port (e.g 993, 1053):");
    //        int port = int.Parse(Console.ReadLine());
    //        Console.Write("Enter if SSL needed (true/false):");
    //        bool ssl = bool.Parse(Console.ReadLine());
    //        return new ImapConfig(server, port, ssl);
    //    }
    //}
    class Program
    {
        private static List<string> invalid_dcs;
        private static List<DomainConfig> dcs;
        private static string configs = "configs.txt";
        static void Main(string[] args)
        {
            Console.WriteLine("Path: ");
            string path = Console.ReadLine();
            List<Mail> mails;
            if (File.Exists(path))
            {
                mails = mailsFromText(File.ReadAllLines(path));
                GetConfigs();
                int index = 0;
                foreach (Mail i in mails)
                {
                    ImapConfig config = FindConfig(i.Login.Split('@')[1]);
                    ////AE.Net.Mail
                    try
                    {
                        ImapClient client = new ImapClient(config.Server, i.Login, i.Pass, AuthMethods.Login, config.Port, config.SSL);
                        client.Logout();
                        client.Login(i.Login, i.Pass);
                        List<Lazy<MailMessage>> mails2 = client.SearchMessages(SearchCondition.Answered()).ToList();
                        mails2.AddRange(client.SearchMessages(SearchCondition.Unanswered()).ToList());
                        foreach (Lazy<MailMessage> j in mails2)
                        {
                            foreach (Attachment k in j.Value.Attachments)
                            {
                                string name = DecodeMime(k.Filename);
                                File.WriteAllBytes("Mails\\" + i.Login + " " + name, k.GetData());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    ////Spire.Email.IMap
                    //try
                    //{
                    //    ImapClient client = new ImapClient(config.Server, config.Port, i.Login, i.Pass, false, ConnectionProtocols.Ssl);
                    //    client.Connect();
                    //    client.Login();
                    //    var messages = client.GetAllMessageHeaders();
                    //    foreach (ImapMessage j in messages)
                    //    {
                    //        foreach (var k in client.GetFullMessage(j.UniqueId).Attachments)
                    //        {
                    //            string name = DecodeMime(k.FileName);
                    //            using (var fileStream = File.Create(name))
                    //            {
                    //                k.Data.Seek(0, SeekOrigin.Begin);
                    //                k.Data.CopyTo(fileStream);
                    //            }
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    ////MailKit
                    //try
                    //{
                    //    ImapClient client = new ImapClient();
                    //    client.Connect(config.Server, config.Port, config.SSL);
                    //    client.Authenticate(i.Login, i.Pass);
                    //    var inbox = client.Inbox;
                    //    inbox.Open(MailKit.FolderAccess.ReadOnly);
                    //    int index2 = 0;
                    //    foreach(var j in inbox)
                    //    {
                    //        foreach(var k in j.Attachments)
                    //        {
                    //            var fileName = k.ContentDisposition?.FileName ?? k.ContentType.Name;
                    //            using (var stream = File.Create(fileName))
                    //            {
                    //                if (k is MessagePart)
                    //                {
                    //                    var rfc822 = (MessagePart)k;

                    //                    rfc822.Message.WriteTo(stream);
                    //                }
                    //                else
                    //                {
                    //                    var part = (MimePart)k;

                    //                    part.Content.DecodeTo(stream);
                    //                }
                    //            }
                    //        }
                    //        Console.WriteLine(index2+":"+index + "\\" + mails.Count);
                    //        index2++;
                    //    }
                    //}
                    //catch(Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    index++;
                    Console.WriteLine(index + "\\" + mails.Count);
                }
            }
            else
            {
                Main(args);
                return;
            }
            Console.ReadLine();
        }
        private static List<Mail> mailsFromText(string[] mails_str)
        {
            List<Mail> mails = new List<Mail>();
            for (int i = 0; i < mails_str.Length; i++)
            {
                string[] res = mails_str[i].Split(':');
                if (res.Length == 2)
                    mails.Add(new Mail(res[0], res[1]));
            }
            return mails;
        }
        private static void GetConfigs()
        {
            dcs = new List<DomainConfig>();
            invalid_dcs = new List<string>();
            if (File.Exists(configs))
            {
                string[] dcs_str = File.ReadAllLines(configs);
                for (int i = 0; i < dcs_str.Length; i++)
                {
                    string[] res = dcs_str[i].Split(':');
                    if (res.Length == 4)
                        dcs.Add(new DomainConfig(res[0], new ImapConfig(res[1], int.Parse(res[2]), bool.Parse(res[3]))));
                    else
                        invalid_dcs.Add(dcs_str[i]);
                }
            }
        }
        private static void SaveConfigs(List<DomainConfig> dcs, List<string> invalid_dcs)
        {
            using (StreamWriter sw = File.CreateText(configs))
            {
                foreach (DomainConfig i in dcs)
                {
                    sw.WriteLine(i);
                }
                foreach (string i in invalid_dcs)
                {
                    sw.WriteLine(i);
                }
            }
        }
        private static ImapConfig FindConfig(string domain)
        {
            var res = dcs.Find((x) => x.Domain == domain);
            if (res == null)
            {
                string res2 = invalid_dcs.Find((x) => x == domain);
                if (res2 != null)
                    return new ImapConfig();
                else
                    return NewConfig(domain);
            }
            return res.Config;
        }
        private static ImapConfig NewConfig(string domain)
        {
            Console.WriteLine("Server:");
            string server = Console.ReadLine();
            Console.WriteLine("Port:");
            int port = int.Parse(Console.ReadLine());
            Console.WriteLine("SSL:");
            bool ssl = bool.Parse(Console.ReadLine());
            return new ImapConfig(server, port, ssl);
        }
        private static byte[] DecodeQuotedPrintable(string str)
        {
            var result = new List<byte>();
            str = str.Replace("=\r\n", "");
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                switch (c)
                {
                    case '=':
                        var b = Convert.ToByte(str.Substring(i + 1, 2), 16);
                        result.Add(b);
                        i += 2;
                        break;
                    default:
                        result.Add((byte)c);
                        break;
                }
            }

            return result.ToArray();
        }
        private static string DecodeMime(string text)
        {
            Regex regex = new Regex(@"=\?{1}(.+)\?{1}([B|Q])\?{1}(.+)\?{1}=");
            if (regex.IsMatch(text))
            {
                string res = "";
                foreach (string i in text.Split(' ', '\t').Select(x => x).Where(x => x != ""))
                {
                    byte[] bytes;
                    if (i.Split('?')[2] == "B")
                    {
                        bytes = Convert.FromBase64String(i.Split('?')[3]);
                    }
                    else
                    {
                        bytes = DecodeQuotedPrintable(i.Split('?')[3]);
                    }
                    res += Encoding.GetEncoding(i.Split('?')[1]).GetString(bytes);
                }
                return res;
            }
            return text;
        }
    }
}
