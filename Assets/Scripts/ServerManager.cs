using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum MsgType { NOTICE }

public class ServerManager : MonoBehaviour
{
    private static ServerManager instance = null;
    public static ServerManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null) instance = this;
    }

    List<Client> clients;

    TcpListener server;
    bool isServerOn;
    int port = 19900;

    void Update()
    {
        if (!isServerOn) return;

        try
        {
            foreach (var c in clients)
            {
                if (!IsConnected(c.client))
                {
                    c.client.Close();
                    clients.Remove(c);
                }
                else
                {
                    NetworkStream s = c.client.GetStream();
                    if (s.DataAvailable)
                    {
                        StreamReader reader = new StreamReader(s);

                        string data = reader.ReadLine();

                        if (data != null)
                        {
                            OnIncommingData(c.client, data);
                        }
                    }

                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("ErrMsg : " + e);
        }

    }

    //Player:P1&Type:Notice&Value:ConnectNotice

    void OnIncommingData(TcpClient c, string data)
    {
        PlayerNum recvPlayer = PlayerNum.ERROR;
        string[] splitData = data.Split('&');
        string[] valueData;
        string[] playerData = splitData[0].Split(':');
        switch(playerData[1])
        {
            case "P1":
                recvPlayer = PlayerNum.P1;
                break;
            case "P2":
                recvPlayer = PlayerNum.P2;
                break;
        }
        string[] typeData = splitData[1].Split(':');
        switch(typeData[1])
        {
            case "NOTICE":
                valueData = splitData[2].Split(':');

                switch(valueData[1])
                {
                    case "CONNECTNOTICE":
                        UIManager.Instance.SetText_PlayerConnected(recvPlayer);
                        PlayManager.Instance.SetPlayer(recvPlayer);
                        break;
                }
                break;

            case "COMMAND":
                string[] commandTypeData = splitData[2].Split(':');

                switch(commandTypeData[1])
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

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }

            return false;
        }

        catch
        {
            return false;
        }
    }

    public void Init_Server()
    {
        clients = new List<Client>();

        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        isServerOn = true;

        StartAccept();
    }

    void StartAccept()
    {
        if (isServerOn)
            server.BeginAcceptTcpClient(AcceptClient, server);
    }

    void AcceptClient(IAsyncResult ar)
    {
        TcpListener server = (TcpListener)ar.AsyncState;

        clients.Add(new Client(server.EndAcceptTcpClient(ar)));
        StartAccept();
    }
}

public class Client
{
    public TcpClient client;

    public Client(TcpClient _client)
    {
        client = _client;
    }


}