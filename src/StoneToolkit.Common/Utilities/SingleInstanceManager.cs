using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace StoneToolkit.Common.Utilities
{
    public class SingleInstanceManager
    {
        public delegate void NextInstanceHandler(string[] args);
        public event NextInstanceHandler OnNextInstance = null;

        private static System.Threading.Mutex _mutex = null;
        private string _identifier = null;
#pragma warning disable 414
        private SingleInstanceManager _instance = null;
#pragma warning restore 414

        public SingleInstanceManager(string identifier)
        {
            if (identifier == null || identifier.Trim().Length == 0)
                throw new Exception("Invalid guid parameter");

            this._identifier = identifier;
        }

        public bool InitInstance(string[] args)
        {
            bool ok;
            _mutex = new System.Threading.Mutex(true, _identifier, out ok);
            if (!ok)
            {
                //mutex already exixts
                SendToServer(args);
                return false;
            }

            //first instance
            BeginWaitForConnection();

            return true;
        }

        private void SendToServer(string[] args)
        {
            //Serialize args
            XmlSerializer serializer = new XmlSerializer(typeof(string[]));
            MemoryStream memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, args);
            byte[] bytes = memoryStream.ToArray();

            try
            {
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", _identifier, PipeDirection.Out);
                pipeClient.Connect(5000);
                pipeClient.Write(bytes, 0, bytes.Length);
                pipeClient.Flush();
                pipeClient.Close();
            }
            catch (TimeoutException)
            {
            }
        }

        private void BeginWaitForConnection()
        {
            NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(_identifier, PipeDirection.In, 1,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous);

            ServerAsyncInfo async = new ServerAsyncInfo();
            async.Server = pipeServer;

            IAsyncResult result = pipeServer.BeginWaitForConnection(Connection, async);
        }

        private void Connection(IAsyncResult result)
        {
            ServerAsyncInfo async = (ServerAsyncInfo)result.AsyncState;

            try
            {
                async.Server.EndWaitForConnection(result);

                //Process the connection if the server was successfully connected.
                if (async.Server.IsConnected)
                {
                    //Read the message from the secondary instance
                    MemoryStream memoryStream = new MemoryStream();
                    byte[] buffer = new byte[8192];
                    do
                    {
                        int lastRead = async.Server.Read(buffer, 0, buffer.Length);
                        memoryStream.Write(buffer, 0, lastRead);
                    }
                    while (!async.Server.IsMessageComplete);

                    XmlSerializer serializer = new XmlSerializer(typeof(string[]));
                    memoryStream.Position = 0;
                    string[] args = (string[])serializer.Deserialize(memoryStream);

                    //Let the event handler process the message.
                    if (OnNextInstance != null)
                        OnNextInstance(args);
                }

                async.Server.Close();
                BeginWaitForConnection();
            }
            catch (Exception m)
            {
            }
        }

        private struct ServerAsyncInfo
        {
            public NamedPipeServerStream Server;
        }
    }
}
