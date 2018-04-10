using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Commons;
using System.Timers;
using System.Threading;

namespace UDPServer
{
    class Server
    {
        UdpClient listner = new UdpClient(11000);
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 11000);
        public static byte[] returnAck = new byte[4];
        public static bool flag=false;
        public static List<ackModel> ackList = new List<ackModel>();
        public static System.Timers.Timer timer;
        public static List<Packet> packetList = new List<Packet>();
        public static Mutex mutex = new Mutex();
        public void listen()
        {

            timer = new System.Timers.Timer(6000);
            timer.Elapsed += new ElapsedEventHandler(timeEvent);
            timer.Enabled = true;
            
        }
        public void readData()
        {
            while (true)
            {
                byte[] data = listner.Receive(ref ep);
                //Trigger Event
                RaiseDataReceived(new RevceivedDataArgs(ep.Address, ep.Port, data));
                
            }
        }
        public void sendData()
        {
            while (true)
            {
                mutex.WaitOne();
                if (flag)
                {
                    sendAck();
                }
                mutex.ReleaseMutex();
            }
        }

        private void timeEvent(object sender, ElapsedEventArgs e)
        {
            //sendAck();
        }
        public  void sendAck()
        {
            if (ackList.Count > 0)
            {
                for (int i = 0; i < ackList.Count; i++)
                {
                    //listner.Connect(ep);
                    if(ackList[i]!=null)
                    listner.Send(ackList[i].ackbyte, ackList[i].ackbyte.Length, ep);
                    ackList.Remove(ackList[i]);
                }
            }
            
            
        }

        public delegate void DataReceived(object sender, RevceivedDataArgs args);
        public event DataReceived DataReceivedEvent;
        private void RaiseDataReceived(RevceivedDataArgs args)
        {
            if (DataReceivedEvent != null)
            {
                DataReceivedEvent(this, args);
            }
        }
        
    }
    public class RevceivedDataArgs
    {
        public IPAddress IpAddress { get; set; }
        public int port { get; set; }
        public byte[] receivedBytes;
        public RevceivedDataArgs(IPAddress ip,int port, byte[] data)
        {
            this.IpAddress = ip;
            this.port = port;
            this.receivedBytes = data;
        }

    }

    public class ackModel
    {
        public int ackInt { get; set; }
        public byte[] ackbyte { get; set; }
    }
}
