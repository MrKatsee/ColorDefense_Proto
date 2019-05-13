using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

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

    void OnIncommingData(TcpClient c, string data)
    {
        Debug.Log("Msg : " + data);
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