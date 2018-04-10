using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;

namespace ChattingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            List<AsynchronousClient> client = new List<ChattingClient.AsynchronousClient>();
            try { 
            for (int i = 0; i < 330000; i++)
            {
               
                client.Add(new AsynchronousClient());
                client[i].StartClient(i);
                    
                        
                    
            }
            }
            catch(SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
            //Thread [] thread = new Thread[2];
            //for(int i = 0; i < 2; i++)
            //{
            //    thread[i] = new Thread(() => ChattingClient.AsynchronousClient.StartClient());
            //    thread[i].Start();
            //    thread[i].Join();
            //}
            Console.ReadLine();
        }
    }
}
