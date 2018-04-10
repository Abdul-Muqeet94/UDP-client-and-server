using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace chattingServer1
{
    //State object for reading client
    public class StateObject
    {
        //client socket.
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        //receive buffer
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
    public class AsynchronousSocketListener
    {

        //Socket List
        static Socket listerSocket;
        static Socket handlerSocket;
        //Thread Signal
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public AsynchronousSocketListener()
        {
        }
        public static void StartListening()
        {
            //Data buffer for incoming data.
            byte[] bytes = new Byte[1024];
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[3];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            // Create a TCP/IP socket.
            Socket listner = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {

                listner.Bind(localEndPoint);
                listner.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();


                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for connection.....");
                    //Thread newWork = new Thread(() => listner.BeginAccept(new AsyncCallback(AcceptCallback), listner));
                    listner.BeginAccept(new AsyncCallback(AcceptCallback), listner);




                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            listerSocket = (Socket)ar.AsyncState;
            handlerSocket = listerSocket.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handlerSocket;
            //Thread newThread = new Thread(() => handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            //      new AsyncCallback(ReadCallback), state));
            handlerSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read   
                // more data.  
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the   
                    // client. Display it on the console.  
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    //Thread newThread = new Thread(() => Send(handler, content));
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    //   Thread newThread = new Thread(() => handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    //new AsyncCallback(ReadCallback), state));
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        private static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding. 
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device. 
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object. 
                Socket handler = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.  
                int bytesSend = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSend);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }


}
