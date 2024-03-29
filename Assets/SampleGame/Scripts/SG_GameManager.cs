using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SG_GameManager : MonoBehaviour
{
    private const string serverIP = "127.0.0.1";
    private const short serverPort = 9722;

    private const int BUFFER_SIZE = 1024;

    // 네트워크 관련
    private TcpClient client;

    private NetworkStream stream;

    private void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }

    public void ConnectToServer()
    {
        client = new TcpClient();

        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        client.Connect(ipEndPoint);

        stream = client.GetStream();

        Thread receiveThread = new Thread(Receive);

        receiveThread.Start();
    }

    private void Receive()
    {
        if (client.Connected)
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            stream.Read(buffer, 0, BUFFER_SIZE);

            string data = Encoding.ASCII.GetString(buffer);

            Debug.Log(data);

            Receive();
        }
    }

    public void SendData(string data)
    {
        if (client.Connected)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
