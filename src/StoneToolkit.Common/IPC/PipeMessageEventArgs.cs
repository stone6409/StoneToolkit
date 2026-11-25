using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;

namespace StoneToolkit.Common.IPC
{
    public class PipeMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public PipeStream PipeStream { get; }

        public PipeMessageEventArgs(string message, PipeStream pipeServer)
        {
            Message = message;
            PipeStream = pipeServer;
        }
    }
}
