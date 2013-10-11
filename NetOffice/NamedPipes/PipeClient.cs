using System;
using System.Collections.Generic;
using System.Text;

namespace NetOffice.NamedPipes
{
    internal class PipeClient
    {
        private string _pipeName = "NOTools.ConsoleMonitor.Shared.Server";

        public bool SendConsoleMessage(string message)
        {
            if (String.IsNullOrEmpty(message) || message.Length > 1023)
                return false;
            return SendRecieveString("CNSL?" + "[" + DateTime.Now.ToLongTimeString() + "]" + message);
        }

        public bool SendChannelMessage(string channel, string message)
        {
            if (String.IsNullOrEmpty(channel) || channel.IndexOf("?") < -1)
                throw new ArgumentException("channel can't empty und must be without '?' character");
            if (String.IsNullOrEmpty(message) || message.Length > 1023)
                return false;
            return SendRecieveString(channel + "?" + message);
        }

        private bool SendRecieveString(string any)
        {
            ClientPipeConnection clientConnection = null;
            try
            {
                clientConnection = new ClientPipeConnection(_pipeName, ".");
                if (!clientConnection.TryConnect())
                    return false;
                
                clientConnection.Write(any);
                string response = clientConnection.Read();
                clientConnection.Close();
                return true;
            }
            catch (Exception exception)
            {
                if (null != clientConnection)
                    clientConnection.Dispose();
                throw (exception);
            }
        }
    }
}
