using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPServer
{
    class Program
    {
        public static List<Packet> packetList = new List<Packet>();
        public static Mutex mutex=new Mutex();
        static void Main(string[] args)
        {
            Server server = new Server();
            HandleDataClass hdc = new HandleDataClass();
           
            //start server Thread
            Thread serverThread = new Thread(() => server.listen());
            serverThread.Start();
            //start Handler Thread
            

            Thread serverThread2 = new Thread(() => server.readData());
            serverThread2.Start();
          


            
            Thread serverThread3 = new Thread(() => server.sendData());
            serverThread3.Start();
            
            
            Thread dataHandlerThread = new Thread(() => hdc.SubscribeToEvent(server));
            dataHandlerThread.Start();

            //Server 2

            

            //Do other things
            //while (true)
            //{
            //    Thread.Sleep(100);
            //}
        }
    }
}
