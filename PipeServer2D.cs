using System.Collections;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class PipeServer2D : MonoBehaviour
{
    private NamedPipeServerStream server;
    private Thread serverThread;
    private bool serverRunning = true;
    public string predictedDirection = "";
    private string receivedDirection = "";
    private object lockObject = new object();
    public static PipeServer2D Instance;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        serverThread = new Thread(Run);
        serverThread.Start();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Run()
    {
        try
        {
            using (server = new NamedPipeServerStream("UnityMediaPipeHands", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                Debug.Log("PipeServer2D: Waiting for connection...");
                server.WaitForConnection();
                Debug.Log("PipeServer2D: Connected!");

                using (var br = new BinaryReader(server, Encoding.UTF8))
                {
                    while (serverRunning && server.IsConnected)
                    {
                        try
                        {
                            var len = (int)br.ReadUInt32();
                            var str = new string(br.ReadChars(len));
                            ProcessReceivedData(str);
                        }
                        catch (EndOfStreamException)
                        {
                            Debug.LogWarning("PipeServer2D: End of stream reached. Ignoring.");
                        }
                        catch (IOException e)
                        {
                            Debug.LogError("PipeServer2D: IOException - " + e.Message);
                        }
                    }
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError("PipeServer2D: IOException - " + e.Message);
        }
        finally
        {
            server?.Close();
            Debug.Log("PipeServer2D: Server closed.");
        }
    }

    void ProcessReceivedData(string data)
    {
        Debug.Log("PipeServer2D: Received Data - " + data);

        string[] lines = data.Split('\n');
        foreach (string line in lines)
        {
            string[] s = line.Split('|');
            if (s.Length < 3) continue;
            if (s[0] == "Right" && s[1] == "Predicted")
            {
                lock (lockObject)
                {
                    receivedDirection = s[2];
                }
                Debug.Log("PipeServer2D: Processed Gesture - " + receivedDirection);
            }
        }
    }

    private void Update()
    {
        try
        {
            lock (lockObject)
            {
                if (!string.IsNullOrEmpty(receivedDirection))
                {
                    predictedDirection = receivedDirection;
                    Debug.Log("PipeServer2D: Gesture detected - " + predictedDirection);
                    receivedDirection = "";
                }
            }
        }
        catch (EndOfStreamException)
        {
            Debug.LogWarning("PipeServer2D: End of stream reached during gesture detection. Ignoring.");
            receivedDirection = "";
        }
    }

    public void ResetGesture()
    {
        predictedDirection = "";
    }

    private void OnApplicationQuit()
    {
        serverRunning = false;

        if (server != null && server.IsConnected)
        {
            server.Disconnect();
            Debug.Log("PipeServer2D: Server disconnected.");
        }

        if (serverThread != null && serverThread.IsAlive)
        {
            if (!serverThread.Join(1000)) 
            {
                serverThread.Interrupt(); 
            }
            Debug.Log("PipeServer2D: Server thread stopped.");
        }
    }
}
