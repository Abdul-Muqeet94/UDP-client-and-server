using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static UDPClient.Client;

namespace UDPClient
{
    class HandleDataClass
    {

        public int readerCounter = 0;
        public void SubscribeToEvent(Client client)
        {
            client.dataReceivedEvent += client_DataReceivedEvent;
        }
        void client_DataReceivedEvent(object sender, ReceivedDataArgs args)
        {
            Console.WriteLine("Receive message from [{0}:{1}]:\r\n",
                args.IpAddress.ToString(), args.port.ToString()
                );
            Client.mutexx.WaitOne();

            byte[] buffer = args.receivedBytes;
            readerCounter++;
            int val = BitConverter.ToInt32(buffer, 0);

            if (Client.packetList.Where(c => c.getAckInt() == val).FirstOrDefault() != null)
            {
                Client.packetList.Where(c => c.getAckInt() == val).FirstOrDefault().setLastSend(true);
                
            }
            if (readerCounter % 5 == 0)
            {
                Client.flag = true;
            }

            Console.WriteLine("Message Received in Client {0} and counter is {1} \n", val, readerCounter);
            Client.mutexx.ReleaseMutex();


        }
    }
}
