using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void MenuToHost()
    {
        menu.SetActive(false);
        menu_Host.SetActive(true);

        PlayManager.Instance.pNum = PlayerNum.P1;
        ServerManager.Instance.Init_Server();
        ClientManager.Instance.ConnectToServer();
    }

    public void SetText_PlayerConnected()
    {
        //Server에서 메세지를 받아 이 함수를 콜하도록 하자 -> UI에 접속된 거 표시되도록
    }

}
