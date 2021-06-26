using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace MailSorter
{
    class Mail
    {
        public string Email { get; set; }
        public string Pass { get; set; }
        public Mail()
        {

        }
        public Mail(string template)
        {
            string[] res = template.Split(':');
            if (res.Length >= 2)
            {
                string pass = res[1];
                for (int j = 2; j < res.Length; j++)
                {
                    pass += ":" + res[j];
                }
                Email = res[0];
                Pass = pass;
            }
            else
            {
                Email = template;
            }
        }
        public Mail(string email, string pass)
        {
            this.Email = email;
            this.Pass = pass;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Email);
            sb.Append(":");
            sb.Append(Pass);
            return sb.ToString();
        }
    }
    class Program
    {
        private static void mailsFromText(string[] mails_str, out List<Mail> mails)
        {
            mails = new List<Mail>();
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
        }
        static void Main(string[] args)
        {
            string path;
            if (args != null && args.Length == 1)
                if (args[0] == "/?")
                {
                    Console.WriteLine("This util sorts txt lines, where line`s template:\nemail_name@domain_name:email_password\nTxt is sorted by domain_name alphabetically.\nA path argument can be passed to util, otherwise path will be asked during util work.");
                    return;
                }
                else
                    path = args[0];
            else
            {
                Console.WriteLine("Enter path to file with emails: ");
                path = Console.ReadLine();
            }
            path = path.Trim('"');
            if (!File.Exists(path))
            {
                Console.Write("No such file found, path: ");
                Console.WriteLine(path);
                Console.ReadLine();
                return;
            }
            FileInfo f = new FileInfo(path);
            string save_path = f.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(path) + "_sorted";
            Directory.CreateDirectory(save_path);
            try
            {
                List<string> filters = GetFilters();
                using (StreamReader reader = new StreamReader(path))
                {
                    int i = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Mail tmp = new Mail(line);
                        string country = tmp.Email.Split('@').Last().Split('.').Last();
                        if(CheckDomain(filters, country))
                        {
                            try
                            {
                                File.AppendAllText(save_path + "\\" + country + ".txt", line + "\n");
                                Console.Write(++i);
                                Console.CursorLeft = 0;
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(save_path + "\\" + "other.text", line + "\n");
                                Console.WriteLine("Error in line " + i + "(" + line + "):" + ex.Message);
                                Console.Write(++i);
                                Console.CursorLeft = 0;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Sort dir is here: " + save_path);
            Console.ReadLine();
        }
        private static List<string> GetFilters()
        {
            List<string> filters = new List<string>();
            Console.WriteLine("Enter filters using 'Enter' in end put '/0' to end input or to select all, if at beginning:");
            string str;
            while ((str=Console.ReadLine())!="/0")
            {
                filters.Add(str);
            }
            return filters;
        }
        private static bool CheckDomain(List<string> filters, string domain)
        {
            if (filters.Count == 0)
                return true;
            return filters.Find(x => x == domain) != null;
        }
    }
}
