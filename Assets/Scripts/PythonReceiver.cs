using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PythonReceiver : MonoBehaviour
{
    public static PythonReceiver Instance;
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
    private string gameChanger;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Makes this object persist across scene changes
        }
        else
        {
            Destroy(gameObject);  // Destroys any duplicate instances
        }
    }

    private void Update()
    {
        // Check if the string has been changed to a game name and loads the scene for that game
        if (gameChanger != null)
        {
            SceneManager.LoadScene(gameChanger);
            gameChanger = null;  // Clear the gameChanger after switching scenes
        }

        // Check and instantiate black pieces
        if (blackPositions.Count > 0)
        {
            foreach (var black in blackPositions)
            {
                GameObject spawnedBlack = Instantiate(blackPrefab, black, Quaternion.identity);
                DontDestroyOnLoad(spawnedBlack);  // Make sure the spawned black piece persists
            }
            blackPositions.Clear();  // Clear the list after instantiation to avoid duplicates
        }

        // Check and instantiate white pieces
        if (whitePosition.Count > 0)
        {
            foreach (var white in whitePosition)
            {
                GameObject spawnedWhite = Instantiate(whitePrefab, white, Quaternion.identity);
                DontDestroyOnLoad(spawnedWhite);  // Make sure the spawned white piece persists
            }
            whitePosition.Clear();  // Clear the list after instantiation to avoid duplicates
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

