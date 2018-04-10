using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace UDPClient
{
    public class Client
    {
        public static UdpClient c = new UdpClient();
        public IPEndPoint ep = new IPEndPoint(IPAddress.Parse("10.104.12.203"), 11000);
        public static List<Packet> packetList;
        static int count = 0;
        public static bool flag = false;
        public static int seqToSend = 0;
        List<int> seqNo = new List<int>();
        public static Mutex mutex = new Mutex();
        public static int ackSend = 0;
        public static System.Timers.Timer timer;
        public static Mutex mutexx = new Mutex();
        public Client()
        {
            packetList = new List<Packet>();
        }
        public void connect()
        {
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += new ElapsedEventHandler(timerElapsed);
            c.Connect(ep);

            resetPacketList();
            //sendMessage();
            //ReadPackets();
            timer.Enabled = true;
            Console.ReadLine();
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            resendPacket();
        }

        public void ReadPackets()
        {
            while (true)
            {
                //mutex.WaitOne();
                byte[] dataReveived = c.Receive(ref ep);
                RaiseDataReceived(new ReceivedDataArgs(ep.Address, ep.Port, dataReveived));

                //resend paket if any 
                resendPacket();
                //if (flag)
                //{
                //  resetPacketList();
                //}

                //mutex.ReleaseMutex();
                //Console.WriteLine("Received Ack {0} ", Encoding.ASCII.GetString(dataReveived));
                //Console.ReadLine();
            }

        }
        public void resetPacketList()
        {
            packetList = new List<Packet>();
        }
        private static void resendPacket()
        {
            mutexx.WaitOne();
            var paketss = packetList;
            var packets = (paketss.Count() < 0) ? null : paketss.Where(aa => aa.getLastSend() == false).ToArray();
            if (packets != null)
            {
                if (packets.Length == 0) flag = true;
                for (int i = 0; i < packets.Length; i++)
                {
                    c.Send(packets[i].getPacket(), packets[i].getPacket().Length);
                }
            }
            mutexx.ReleaseMutex();

        }

        public void sendMessage()
        {
            //if (seqToSend == 0)
            //{
            //    for (int i=0; i<packetList.Length;i++)
            //    {
            while (true)
            {
                if (flag || count == 0)
                {
                    mutexx.WaitOne();

                    for (int i = 0; i < 5; i++)
                    {
                        string msg = "Hello this a data to send " + count;
                        int header = msg.Length;
                        Packet packet = new Packet(4, header);
                        packet.setHeader(header);
                        packet.setBody(msg);
                        packet.makePacket(count++);
                        packetList.Add(packet);
                        c.Send(packet.getPacket(), packet.getPacket().Length);
                        packet.setLastSend(true);
                        flag = false;
                    }
                    mutexx.ReleaseMutex();


                }
            }

            //    }
            //}
            //else
            //{
            //    var packet = packetList.Where(c => c.getAckInt() == seqToSend).FirstOrDefault();
            //    c.Send(packet.getPacket(), packet.getPacket().Length);
            //    packet.setLastSend(true);
            //    packetList[packetList.IndexOf(packet) - 1].setLastSend(false);
            //}


        }

        public delegate void DataReceived(object sender, ReceivedDataArgs args);
        public event DataReceived dataReceivedEvent;

        private void RaiseDataReceived(ReceivedDataArgs args)
        {
            if (dataReceivedEvent != null)
            {
                dataReceivedEvent(this, args);
            }
        }

        public class ReceivedDataArgs
        {
            public IPAddress IpAddress { get; set; }
            public int port { get; set; }
            public byte[] receivedBytes;
            public ReceivedDataArgs(IPAddress ip, int port, byte[] data)
            {
                this.IpAddress = ip;
                this.port = port;
                this.receivedBytes = data;
            }
        }


    }



    public class PacketWrapper
    {
        public Packet packet { get; set; }
        public int seqNo { get; set; }
    }
}
