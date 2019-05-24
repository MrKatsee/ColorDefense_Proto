using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public GameObject menu;
    public GameObject menu_Host;
    public GameObject menu_Client;
    public GameObject menu_Lobby;

    public void MenuToHost()
    {
        menu.SetActive(false);
        menu_Host.SetActive(true);

        PlayManager.Instance.pNum = PlayerNum.P1;
        ServerManager.Instance.Init_Server();
        ClientManager.Instance.ConnectToServer();
    }

    public void MenuToConnect()
    {
        menu.SetActive(false);
        menu_Client.SetActive(true);

        PlayManager.Instance.pNum = PlayerNum.P2;
    }

    public void ConnectButton()
    {
        InputField input = menu_Client.transform.GetChild(0).Find("InputField_Host").GetComponent<InputField>();
        string host = input.text;
        input.text = string.Empty;

        ClientManager.Instance.ConnectToServer(host);
    }

    public void ConnectToLobby()
    {
        if (PlayManager.Instance.pNum == PlayerNum.P1)
            return;

        menu_Client.SetActive(false);
        menu_Lobby.SetActive(true);
    }

    public void Host_StartGameButton()
    {
        ClientManager.Instance.SendMsg("Type:NOTICE&Value:GAMESTART");
    }

    public void StartGame()
    {

        menu_Host.SetActive(false);
        menu_Lobby.SetActive(false);

        PlayManager.Instance.gameStart = true;
    }

    public void SetText_PlayerConnected(PlayerNum pNum)
    {
        //Server에서 메세지를 받아 이 함수를 콜하도록 하자 -> UI에 접속된 거 표시되도록Te
        Text text;
        string determiner = string.Empty;

        if (PlayManager.Instance.pNum == PlayerNum.P1)
        {
            determiner = "_Host";
        }
        else
        {
            determiner = "_Lobby";
        }

        switch (pNum)
        {
            case PlayerNum.P1:
                text = GameObject.Find("ConnectionCheck_P1" + determiner).GetComponent<Text>();
                text.text = "P1_Connected";
                break;
            case PlayerNum.P2:
                text = GameObject.Find("ConnectionCheck_P2"+ determiner).GetComponent<Text>();
                text.text = "P2_Connected";
                break;
        }
    }



}
