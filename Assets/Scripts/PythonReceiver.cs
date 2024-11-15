using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PythonReceiver : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    public GameObject blackPrefab;
    public GameObject whitePrefab;
    List<Vector3> blackPositions = new List<Vector3>();
    List<Vector3> whitePosition = new List<Vector3>();
    bool running;
    public Camera camera;
    public GameObject playField;
    private string gameChanger;


    private void Update()
    {
        Debug.Log(blackPositions);
        Debug.Log(whitePosition);
        //Focuses the camera in the middle of the playfield
        camera.transform.position = new Vector3(playField.transform.position.x, 300f, playField.transform.position.z);
        camera.transform.LookAt(playField.transform.position);

        //Checks if the string has been changed to a game name and loads the scene for that game
        //if(gameChanger != null)
        //{
        //    SceneManager.LoadScene(gameChanger);
        //}

        //2 checks to see if the Python script send any positions for either black or white pieces.
        if (blackPositions.Count >= 0)
        {
            //Loops through all the positions and instantiates a prefab on the position it's iterating over
            foreach (var black in blackPositions)
            {
                int i = 0;
                Instantiate(blackPrefab, blackPositions[i], Quaternion.identity);
                //Remove the position from the list so we don't accidently get multiple of the same position
                blackPositions.Remove(blackPositions[i]);
            }
        }
        if (whitePosition.Count >= 0)
        {
            foreach (var white in whitePosition)
            {
                int i = 0;
                Instantiate(whitePrefab, whitePosition[i], Quaternion.identity);
                whitePosition.Remove(whitePosition[i]);
            }
        }
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

        if (dataReceived == ("Makvaer"))
        {
            gameChanger = "Makvaer";
        }
        if (dataReceived == ("Gaasetavl"))
        {
            gameChanger = "Gaasetavl";
        }
        if (dataReceived == ("Hundefterhare"))
        {
            gameChanger = "Hundefterhare";
        }

        Debug.Log($"Received JSON: {dataReceived}");
        //Uses a dictionary to check for key-value pairs from the json string.
        //Enables the check of a string key to give the corresponding position.
        var receivedData = JsonConvert.DeserializeObject<Dictionary<string, List<float>>>(dataReceived);
        //Loop through all key value pairs and add the position to a list of that piece.
        foreach (var kvp in receivedData)
        {
            // Check if the key contains "black"
            if (kvp.Key.Contains("black"))
            {
                Vector3 position = new Vector3(kvp.Value[0], 0, kvp.Value[2]);
                blackPositions.Add(position);
            }
            // Check if the key contains "white"
            else if (kvp.Key.Contains("white"))
            {
                Vector3 position = new Vector3(kvp.Value[0], 0, kvp.Value[2]);
                whitePosition.Add(position);
            }
        }
        print("Received list of positions!");

        //Sends data back to Python if necessary at a later point
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes("Hey I got your message Python! Do You see this message?");
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
    }
}

