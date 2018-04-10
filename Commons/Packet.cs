using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons
{
    public class Packet
    {
        byte[] header;
        byte[] body;
        byte[] packet;
        byte[] ack;
        byte[] ReAck;
        bool ackReveived=false;
        public Packet()
        {
            header = null;
            body = null;
            ack = new byte[4];
            packet = null;
            ReAck = new byte[4];
        }
        public Packet(int hLength, int bLength)
        {
            header = new byte[hLength];
            body = new byte[bLength];
            ack = new byte[4];
            packet = new byte[hLength + bLength + 4];
            ReAck = new byte[4];
        }

        public void setHeader(int head)
        {
            header = BitConverter.GetBytes(head);
        }

        public void setBody(string message)
        {
            body = Encoding.ASCII.GetBytes(message);
        }
        public void setBodyByte(byte[] message)
        {
            body = message;
        }
        public void setAck(int count)
        {
            ack = BitConverter.GetBytes(count);
        }
        public void makePacket(int count)
        {
            setAck(count++);
            setLastSend(true);
            System.Buffer.BlockCopy(header, 0, packet, 0, header.Length);
            System.Buffer.BlockCopy(ack, 0, packet, header.Length , ack.Length);
            System.Buffer.BlockCopy(body, 0, packet, header.Length + ack.Length, body.Length);
            
        }
        public byte[] getPacket()
        {

            return packet;
        }
        public void setReAck(byte[] ackToSend)
        {
            int val=BitConverter.ToInt32(ackToSend, 0)+1;
            ReAck = BitConverter.GetBytes(val);
        }
        public byte[] getReAck()
        {
            return ReAck;
        }
        public byte[] getAck()
        {
            return ack;
        }
        public int getAckInt()
        {
            return BitConverter.ToInt32(ack, 0);
        }
        public void setLastSend(bool flag)
        {
            ackReveived = flag;
        }
        public bool getLastSend()
        {
            return ackReveived;
        }
    }
}
