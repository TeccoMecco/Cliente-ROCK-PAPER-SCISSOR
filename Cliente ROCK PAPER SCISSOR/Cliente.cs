using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;

namespace Client
{





    class ClientDemo
    {

        private StreamReader _sReader;
        private StreamWriter _sWriter;
        private TcpClient _client;
        private Boolean _isConnected;

        public ClientDemo(String ipAddress, int portNum)
        {
            _client = new TcpClient();
            _client.Connect(ipAddress, portNum);

            HandleCommunication();
        }

        public void HandleCommunication()
        {
            _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
            _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

            _isConnected = true;
            String sData = null;
            while (_isConnected)
            {
                Console.Write("> ");
                sData = Console.ReadLine();

                // write data and make sure to flush, or the buffer will continue to 
                // grow, and your data might not be sent when you want it, and will
                // only be sent once the buffer is filled.
                _sWriter.WriteLine(sData);
                _sWriter.Flush();

                Console.WriteLine("Do you want to receive response from server ?");

                // if you want to receive anything
                String sDataIncomming = _sReader.ReadLine();
                Console.WriteLine(sDataIncomming);

            }
        }

    }
}
