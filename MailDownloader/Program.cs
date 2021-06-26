using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MailDownloader
{
    static class Program
    {
        private static Mutex mutex = null;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            const string appName = "MailDownloaderHost";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form = new Form1();
            Application.Run(form);
        }
    }
}
