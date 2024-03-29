using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SG_Server_Project
{
    class Program
    {
        private const string serverIP = "127.0.0.1";
        private const short serverPort = 9722;

        private const int BUFFER_SIZE = 1024;

        static void Main(string[] args)
        {
            Console.Title = "VSG SG Server";

            TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), serverPort);

            NetworkStream stream;

            listener.Start();

            Log("서버 시작됨.");

            // 클라이언트 기다리기
            TcpClient client = listener.AcceptTcpClient();

            Log("클라이언트 연결됨.");

            stream = client.GetStream();

            while (client.Connected)
            {
                byte[] buffer = new byte[BUFFER_SIZE];

                stream.Read(buffer, 0, BUFFER_SIZE);

                string data = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                Log(data);

                if (data.Equals(string.Empty))
                {
                    data = null;
                }

                if (buffer != null)
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private static void Log(string msg)
        {
            Console.WriteLine($"[Log] {DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")} {msg}");
        }
    }
}
