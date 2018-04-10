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
           
                mutex.WaitOne();
                Client client = new Client();
                HandleDataClass handle = new HandleDataClass();
                mutex.ReleaseMutex();

                mutex.WaitOne();
                Thread clientThread = new Thread(() => client.connect());
                clientThread.Start();
                mutex.ReleaseMutex();



            mutex.WaitOne();
            Thread clientSendThread = new Thread(() => client.sendMessage());
            clientSendThread.Start();
            mutex.ReleaseMutex();

            mutex.WaitOne();
            Thread clientReadThread = new Thread(() => client.ReadPackets());
            clientReadThread.Start();
            mutex.ReleaseMutex();

            Thread listen = new Thread(() => handle.SubscribeToEvent(client));
                listen.Start();
            
            


        }
        
    }
}
