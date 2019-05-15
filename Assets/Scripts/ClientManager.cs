using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
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

            SendMsg("Type:NOTICE&Value:CONNECTNOTICE");
        }
        catch (Exception e)
        {
            Debug.Log("ErrMsg : " + e);
        }

    }

    void Update()
    {
        if (!connected)
        {
            return;
        }

        if (stream.DataAvailable)
        {
            string data = reader.ReadLine();

            if (data != null)
            {
                OnIncommingData(data);
            }
        }
    }

    void OnIncommingData(string data)
    {
        Debug.Log("Msg : " + data);
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