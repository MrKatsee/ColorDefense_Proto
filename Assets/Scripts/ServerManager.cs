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
                            OnIncommingData(c, data);
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

    void OnIncommingData(Client c, string data)
    {
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
                    case "CONNECTNOTICE":
                        c.pNum = recvPlayer;
                        
                        foreach (var cl in clients)
                        {
                            if (cl.pNum != recvPlayer)
                            {
                                UniCast(recvPlayer, cl.pNum, "Type:NOTICE&Value:CONNECTCOMPLETE");
                            }
                        }

                        BroadCast(recvPlayer, "Type:NOTICE&Value:CONNECTCOMPLETE");
                        break;
                        
                    case "GAMESTART":
                        BroadCast(recvPlayer, "Type:NOTICE&Value:GAMESTART");
                        break;
                }
                break;

            case "COMMAND":
                string[] commandTypeData = splitData[2].Split(':');

                switch (commandTypeData[1])
                {
                    case "MOVE":
                        BroadCast(recvPlayer, "Type:COMMAND&CommandType:MOVE&" + splitData[3]); ;
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

    void UniCast(PlayerNum recvPNum, PlayerNum pNum, string msg)
    {
        if (!isServerOn)
            return;

        StreamWriter writer = new StreamWriter(GetClientByPNum(recvPNum).client.GetStream());

        string data = string.Format("Player:{0}&{1}", pNum.ToString(), msg);

        writer.WriteLine(data);
        writer.Flush();
    }

    void BroadCast(PlayerNum pNum, string msg)
    {
        if (!isServerOn)
            return;

        foreach(var c in clients)
        {
            StreamWriter writer = new StreamWriter(c.client.GetStream());

            string data = string.Format("Player:{0}&{1}", pNum.ToString(), msg);

            Debug.Log(c.pNum.ToString() + " " + data);

            writer.WriteLine(data);
            writer.Flush();
        }
    }

    Client GetClientByPNum(PlayerNum pNum)
    {
        foreach(var c in clients)
        {
            if (c.pNum == pNum)
            {
                return c;
            }
        }

        throw new Exception($"No Client having pNum : {pNum.ToString()}");
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
    public PlayerNum pNum;

    public Client(TcpClient _client)
    {
        client = _client;
    }


}