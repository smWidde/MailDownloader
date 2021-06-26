using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MailDownloader.Logic
{
    enum State
    {
        Blocked,
        Requesting,
        Accepted,
        Downloading,
        Stopped
    }
    class TcpServer
    {
        public delegate void CollectionChanged();
        public event CollectionChanged CollectionChange;
        public class ClientObject
        {
            public TcpServer server;
            public TcpClient client;
            public string macAddress;
            private NetworkStream stream;
            public ClientObject(TcpClient tcpClient, TcpServer tcpServer)
            {
                client = tcpClient;
                server = tcpServer;
            }
            public void Process()
            {
                stream = null;
                try
                {
                    stream = client.GetStream();
                    byte[] data = new byte[256];
                    while (true)
                    {
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();
                        macAddress = message.Split(' ')[0];
                        string request = message.Split(' ')[1];
                        State st = State.Blocked;
                        if (request == "access")
                        {
                            st = State.Requesting;
                        }
                        else if (request == "download")
                        {
                            st = State.Downloading;
                        }
                        else if (request == "stop")
                        {
                            st = State.Stopped;
                        }
                        string msg = server.ChangeState(this, st);
                        SendMessage(msg);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                    if (client != null)
                        client.Close();
                }
            }
            public void SendMessage(string msg)
            {
                if (client.Connected)
                {
                    byte[] data = new byte[256];
                    data = Encoding.Unicode.GetBytes(msg);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
        IPAddress localAddr;
        int port;
        private TcpListener server;
        private string path = "ipconfig.txt";
        private Dictionary<ClientObject, State> clients;
        private Dictionary<string, State> macAddresses;
        private Thread th;
        private List<Thread> working_threads;
        public TcpServer()
        {
            clients = new Dictionary<ClientObject, State>();
            GetIp();
            macAddresses = new Dictionary<string, State>();
            working_threads = new List<Thread>();
            server = new TcpListener(localAddr, port);
            try
            {
                server.Start();
            }
            catch(Exception ex)
            {
                throw ex;
            }
            working_threads = new List<Thread>();
            th = new Thread(() =>
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    working_threads.Add(clientThread);
                    clientThread.Start();
                }
            });
            th.Start();
        }
        public void Stop()
        {
            server.Stop();
            Thread.Sleep(2000);
            th.Abort();
            foreach(var i in working_threads)
            {
                i.Abort();
            }
        }
        public void Block(string macAddress)
        {
            ClientObject obj = FindClient(macAddress);
            if (obj != null)
            {
                obj.SendMessage(ChangeState(obj, State.Blocked, true));
            }
        }
        public void Unblock(string macAddress)
        {
            State st = macAddresses[macAddress];
            if (st == State.Requesting || st == State.Blocked)
            {
                ClientObject obj = FindClient(macAddress);
                if (obj != null)
                {
                    obj.SendMessage(ChangeState(obj, State.Accepted, true));
                }
            }
        }
        private ClientObject FindClient(string macAddress)
        {
            foreach (var i in clients)
            {
                if (i.Key.macAddress == macAddress)
                    return i.Key;
            }
            return null;
        }
        private string ChangeState(ClientObject caller, State st, bool superior = false)
        {
            string macAddress = caller.macAddress;
            string msg = "OK";
            if (macAddresses.ContainsKey(macAddress))
            {
                State current = macAddresses[macAddress];
                switch (st)
                {
                    case State.Blocked:
                        msg = "Deny";
                        macAddresses[macAddress] = st;
                        break;
                    case State.Requesting:
                        {
                            if (current == State.Blocked || current == State.Requesting)
                            {
                                msg = "Deny";
                                macAddresses[macAddress] = st;
                            }
                            break;
                        }
                    case State.Accepted:
                        if (superior)
                        {
                            if (current == State.Blocked || current == State.Requesting)
                            {
                                msg = "OK";
                                macAddresses[macAddress] = st;
                            }
                        }
                        break;
                    case State.Downloading:
                        msg = "OK";
                        macAddresses[macAddress] = st;
                        break;
                    case State.Stopped:
                        msg = "OK";
                        macAddresses[macAddress] = st;
                        break;
                }
            }
            else
            {
                macAddresses.Add(macAddress, State.Blocked);
                msg = "Deny";
            }
            clients[caller] = macAddresses[macAddress];
            CollectionChange?.Invoke();
            return msg;
        }
        private void GetIp()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(
    Dns.GetHostEntry(string.Empty).AddressList,
    a => a.AddressFamily == AddressFamily.InterNetwork);
            localAddr = ipv4Addresses[0];
            if (ipv4Addresses.Length == 0)
                localAddr = IPAddress.Parse("127.0.0.1");
            else
                localAddr = ipv4Addresses[0];
            port = 8000;
            if (File.Exists(path))
            {
                string[] strs = File.ReadAllLines(path);
                if (strs.Length != 0)
                {
                    string[] dcs_str = strs[0].Split(':');
                    if (dcs_str.Length == 2)
                    {
                        string ip_s = dcs_str[0];
                        IPAddress tmp;
                        if (IPAddress.TryParse(ip_s, out tmp))
                        {
                            int ptmp;
                            if(int.TryParse(dcs_str[1], out ptmp))
                            {
                                localAddr = tmp;
                                port = ptmp;
                            }
                        }
                    }
                }
            }
        }
        public Dictionary<string, State> GetClients()
        {
            return macAddresses;
        }
        public string GetAddress()
        {
            return localAddr.ToString() + ":" + port;
        }
    }
}
