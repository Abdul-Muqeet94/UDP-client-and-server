using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commons;
namespace UDPServer
{
    class HandleDataClass
    {
        public void SubscribeToEvent(Server server)
        {
            server.DataReceivedEvent += server_DataReceivedEvent;
        }
        void server_DataReceivedEvent(object sender, RevceivedDataArgs args)
        {
            Console.WriteLine("Receive message from [{0}:{1}]: in server \r\n",
                args.IpAddress.ToString(),args.port.ToString()
                );
            Server.mutex.WaitOne();
            byte[] buffer= args.receivedBytes;
            byte[] header=new byte[8];
            byte[] ack = new byte[4];

            System.Buffer.BlockCopy(buffer, 0, header, 0, 8);
            System.Buffer.BlockCopy(buffer, 4, ack, 0, 4);
            int value = BitConverter.ToInt32(header,0);
            byte[] body = new byte[value];
            System.Buffer.BlockCopy(buffer, header.Length, body, 0, value);
            string result = Encoding.ASCII.GetString(body);
            
            int ackValue = BitConverter.ToInt32(ack, 0);
            byte[] ackb = BitConverter.GetBytes(ackValue);
            Server.ackList.Add(new ackModel { ackInt=ackValue, ackbyte= ackb });
            var packet = new Packet();
            packet.setBodyByte(body);
            Server.packetList.Add(packet);
            if (Server.ackList.Count %5==0)
            {
                Server.flag = true;
            }
            Console.WriteLine("Message Received ack count {0} \n {1} \n {2} \n", Server.ackList.Count, result, ackValue);
            Server.mutex.ReleaseMutex();
            //Commons.Packet packet=new Commons.Packet();
            //packet.setReAck(ack);
            //Program.packetList.Add(packet);






        }
    }
}
