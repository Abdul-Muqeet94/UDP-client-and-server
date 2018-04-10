using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chattingServer1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                chattingServer1.AsynchronousSocketListener.StartListening();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
