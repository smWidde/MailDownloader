using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread th = new Thread(()=> { 
                for(int i=0; i<20; i++)
                {
                    Thread.Sleep(500);
                    Console.WriteLine(i);
                }
            });
            Thread th2 = new Thread(() => {
                for (int i = 30; i < 60; i++)
                {
                    Thread.Sleep(500);
                    Console.WriteLine(i);
                }
            });
            th.Start();
            th2.Start();
            th.Join();
            th2.Join();
            for (int i = 20; i < 40; i++)
            {
                Thread.Sleep(500);
                Console.WriteLine(i);
            }
        }
    }
}
