using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        const int PORT_NUMBER = 7777;

        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void FlushRoom()
        {
            Room.Push(() => Room.Flush());
            JobTimer.Instance.Push(FlushRoom, 100);
        }

        static void UpdateClient()
        {
            Room.Push(() => Room.CheckSession());
            Room.Push(() => Room.Broadcast(new S_CheckConnect().Write()));
            JobTimer.Instance.Push(UpdateClient, 500);
        }

        static void Main(string[] args)
        {
            // DNS 활용
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

            _listener.init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            FlushRoom();
            UpdateClient();
            while (true)
            {
                JobTimer.Instance.Flush();
            }
        }
    }
}
