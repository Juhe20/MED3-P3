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
    public GameObject dogPrefab;
    public GameObject harePrefab;
    List<Vector3> dogPositions = new List<Vector3>();
    List<Vector3> harePosition = new List<Vector3>();
    bool running;
    public Camera camera;
    public GameObject playField;


    private void Update()
    {
        if(dogPositions.Count >= 0)
        {
            foreach (var dog in dogPositions)
            {
                int i = 0;
                Instantiate(dogPrefab, dogPositions[i], Quaternion.identity);
                dogPositions.Remove(dogPositions[i]);
            }
        }
        if(harePosition.Count >= 0)
        {
            foreach(var hare in harePosition) 
            {
                int i = 0;
                Instantiate(harePrefab, harePosition[i], Quaternion.identity);
                harePosition.Remove(harePosition[i]);
            }
        }
        camera.transform.position = new Vector3(playField.transform.position.x, 300f, playField.transform.position.z);
        camera.transform.LookAt(playField.transform.position);

    }

    private void Start()
    {
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    //Network stuff for Python to work with Unity
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
        //Reads the message from the Python script
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        //Checks if the data read from above is a non empty string.
        if (!string.IsNullOrEmpty(dataReceived))
        {
            Debug.Log($"Received JSON: {dataReceived}");
            //Uses a dictionary to check for key-value pairs from the json string.
            //Enables the check of a string key to give the corresponding position.
            var receivedData = JsonConvert.DeserializeObject<Dictionary<string, List<float>>>(dataReceived);

            //Loop through all key value pairs and add the position to a list of that animal.
            foreach (var kvp in receivedData)
            {
                // Check if the key contains "dog"
                if (kvp.Key.Contains("dog"))
                {
                    Vector3 position = new Vector3(kvp.Value[0], 0, kvp.Value[1]);
                    dogPositions.Add(position); // Optional: Store position in a list if needed
                }
                else if (kvp.Key.Contains("hare"))
                {
                    Vector3 position = new Vector3(kvp.Value[0], 0, kvp.Value[1]);
                    harePosition.Add(position); // Optional: Store position in a list if needed
                }
            }
            print("Received list of positions!");

            //Sends data back to Python if necessary at a later point
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this message?");
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }
    }
}
