using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MailSorter
{
    class Mail
    {
        public string Email { get; set; }
        public string Pass { get; set; }
        public Mail()
        {

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
            string[] mails_str = File.ReadAllLines(path);
            List<Mail> mails = new List<Mail>();
            for (int i = 0; i < mails_str.Length; i++)
            {
                string[] res = mails_str[i].Split(':');
                if (res.Length != 2)
                {
                    Console.WriteLine("Wrong mail format");
                    Console.WriteLine("line " + i + ": " + mails_str[i]);
                    Console.ReadLine();
                    return;
                }
                mails.Add(new Mail(res[0], res[1]));
            }
            try
            {
                Mail[] sorted_mails = mails.OrderBy(x => x.Email.Split('@')[1]).ToArray();
                FileInfo f = new FileInfo(path);
                string save_path = f.DirectoryName + "\\" + Path.GetFileNameWithoutExtension(path) + "_sorted" + f.Extension;
                using (StreamWriter sw = File.CreateText(save_path))
                {
                    foreach (Mail i in sorted_mails)
                    {
                        sw.WriteLine(i);
                    }
                }
                Console.WriteLine("Sorted version is here: " + save_path);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.ActualValue);
                Console.ReadLine();
                return;
            }
            Console.ReadLine();
        }
    }
}
