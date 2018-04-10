using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace UDPClient
{
    class Program
    {
        public static Mutex mutex = new Mutex();
        static void Main(string[] args)
        {
           
                
                Client client = new Client();
                HandleDataClass handle = new HandleDataClass();
               

                
                Thread clientThread = new Thread(() => client.connect());
                clientThread.Start();
               



            
            Thread clientSendThread = new Thread(() => client.sendMessage());
            clientSendThread.Start();
            

           
            Thread clientReadThread = new Thread(() => client.ReadPackets());
            clientReadThread.Start();
           

            Thread listen = new Thread(() => handle.SubscribeToEvent(client));
                listen.Start();

            while (true)
            {
                Thread.Sleep(3000);
            }


        }

    }
}
