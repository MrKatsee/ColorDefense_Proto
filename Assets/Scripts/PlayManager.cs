using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    private static PlayManager instance = null;
    public static PlayManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public PlayerNum pNum;
    public bool gameStart = false;

    public List<PlayerData> players = new List<PlayerData>();

    public void SetPlayer(PlayerNum _pNum)
    {
       foreach (var p in players)
        {
            if (p.pNum == _pNum)
            {
                p.gameObject.SetActive(true);
            }
        }
    }
}
