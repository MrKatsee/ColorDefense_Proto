using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public delegate void OnJoystickMove(PlayerNum _pNum, Vector3 _vec);

public class InputManager : MonoBehaviour
{
    private static InputManager instance = null;
    public static InputManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        joystick = FindObjectOfType<Joystick>();
    }

    public event OnJoystickMove onMove;

    Joystick joystick;

    private void Update()
    {
        if (!(joystick.Horizontal == 0 && joystick.Vertical == 0))
           SendMove(new Vector3(joystick.Horizontal, 0, joystick.Vertical));
    }

    void SendMove(Vector3 vec)
    {
        if (!PlayManager.Instance.gameStart)
            return;

        string valueData = string.Format("{0},{1},{2}", vec.x.ToString(), vec.y.ToString(), vec.z.ToString());

        string data = string.Format("Type:COMMAND&CommandType:MOVE&Value:{0}", valueData);
        ClientManager.Instance.SendMsg(data);
    }

    public void CallMove(PlayerNum _pNum, Vector3 _vec)
    {
        onMove(_pNum, _vec);
    }
}