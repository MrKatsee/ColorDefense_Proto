using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnJoystickMove(Vector3 _vec);

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
        onMove(new Vector3(joystick.Horizontal, 0, joystick.Vertical));
    }

}
