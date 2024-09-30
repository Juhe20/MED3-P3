using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class PythonReceiver : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    public GameObject dog1;
    public GameObject dog2;
    public GameObject dog3;
    public GameObject hare;

    Vector3 blackposition1 = Vector3.zero;
    Vector3 blackposition2 = Vector3.zero;
    Vector3 blackposition3 = Vector3.zero;
    Vector3 whiteposition = Vector3.zero;

    bool running;

    private void Update()
    {
        dog1.transform.position = blackposition1;
        dog2.transform.position = blackposition2;
        dog3.transform.position = blackposition3;
        hare.transform.position = whiteposition;
    }

    private void Start()
    {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    private void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        running = true;
        while (running)
        {
            SendAndReceiveData();
        }
        listener.Stop();
    }


    private void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (!string.IsNullOrEmpty(dataReceived))
        {
            Debug.Log($"Received JSON: {dataReceived}");
            var receivedData = JsonConvert.DeserializeObject<Dictionary<string, List<float>>>(dataReceived);

            if (receivedData.ContainsKey("blackposition1"))
                blackposition1 = new Vector3(receivedData["blackposition1"][0], 0, receivedData["blackposition1"][1]);

            if (receivedData.ContainsKey("blackposition2"))
                blackposition2 = new Vector3(receivedData["blackposition2"][0], 0, receivedData["blackposition2"][1]);

            if (receivedData.ContainsKey("blackposition3"))
                blackposition3 = new Vector3(receivedData["blackposition3"][0], 0, receivedData["blackposition3"][1]);

            if (receivedData.ContainsKey("whiteposition1"))
                whiteposition = new Vector3(receivedData["whiteposition1"][0], 0, receivedData["whiteposition1"][1]);

            print("Received data and updated the object!");
            Debug.Log($"checking blackposition1 {blackposition1}");


            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this message?");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
    }
}
