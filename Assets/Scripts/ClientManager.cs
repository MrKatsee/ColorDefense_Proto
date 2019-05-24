using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public enum PlayerNum { P1 = 1, P2, ERROR }

public class ClientManager : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    StreamReader reader;
    StreamWriter writer;
    bool connected = false;

    private static ClientManager instance = null;
    public static ClientManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void ConnectToServer()
    {
        if (connected)
            return;

        try
        {
            client = new TcpClient("127.0.0.1", 19900);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            connected = true;

            recvThread = new Thread(new ThreadStart(RecvMsg));
            recvThread.Start();


            SendMsg("Type:NOTICE&Value:CONNECTNOTICE");
        }
        catch (Exception e)
        {
            Debug.Log("ErrMsg : " + e);
        }

    }

    public void ConnectToServer(string host)
    {
        if (connected)
            return;

        try
        {
            client = new TcpClient(host, 19900);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            connected = true;


            recvThread = new Thread(new ThreadStart(RecvMsg));
            recvThread.Start();

            SendMsg("Type:NOTICE&Value:CONNECTNOTICE");
        }
        catch (Exception e)
        {
            Debug.Log("ErrMsg : " + e);
        }

    }

    Thread recvThread;

    void RecvMsg()
    {
        while (connected)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();

                Debug.Log(data);

                if (data != null)
                {
                    SyncEnqueue(data);
                }
            }
        }

    }

    Queue<string> msgQueue = new Queue<string>();

    public void Enqueue(string str)
    {
        msgQueue.Enqueue(str);
    }

    public void SyncEnqueue(string str)
    {
        lock (msgQueue)
        {
            msgQueue.Enqueue(str);
        }
    }

    public string Dequeue()
    {
        return msgQueue.Dequeue();
    }

    public string SyncDequeue()
    {
        string str;
        lock (msgQueue)
        {
            str = msgQueue.Dequeue();
        }
        return str;
    }

    public int GetCount()
    {
        int count;
        lock (msgQueue)
        {
            count = msgQueue.Count;
        }
        return count;
    }

    void Update()
    {

        int msgCount = GetCount();
        if (msgCount > 0)
        {
            for (int loop = 0; loop < msgCount; ++loop)
            {
                string data = SyncDequeue();
                Debug.Log(data);

                PlayerNum recvPlayer = PlayerNum.ERROR;
                string[] splitData = data.Split('&');
                string[] valueData;
                string[] playerData = splitData[0].Split(':');
                switch (playerData[1])
                {
                    case "P1":
                        recvPlayer = PlayerNum.P1;
                        break;
                    case "P2":
                        recvPlayer = PlayerNum.P2;
                        break;
                }
                string[] typeData = splitData[1].Split(':');
                switch (typeData[1])
                {
                    case "NOTICE":
                        valueData = splitData[2].Split(':');

                        switch (valueData[1])
                        {
                            case "CONNECTCOMPLETE":
                                UIManager.Instance.ConnectToLobby();
                                UIManager.Instance.SetText_PlayerConnected(recvPlayer);
                                PlayManager.Instance.SetPlayer(recvPlayer);
                                break;
                            case "GAMESTART":
                                UIManager.Instance.Host_StartGame();
                                break;
                        }
                        break;

                    case "COMMAND":
                        string[] commandTypeData = splitData[2].Split(':');

                        switch (commandTypeData[1])
                        {
                            case "MOVE":
                                valueData = splitData[3].Split(':');

                                string[] vec_String = valueData[1].Split(',');
                                Vector3 vec = new Vector3(float.Parse(vec_String[0]), float.Parse(vec_String[1]), float.Parse(vec_String[2]));

                                InputManager.Instance.CallMove(recvPlayer, vec);
                                break;
                        }
                        break;
                }
            }
        }
                
    }

    public void SendMsg(string msg)
    {
        if (!connected)
            return;

        string data = string.Format("Player:{0}&{1}", PlayManager.Instance.pNum.ToString(), msg);

        writer.WriteLine(data);
        writer.Flush();
    }
}