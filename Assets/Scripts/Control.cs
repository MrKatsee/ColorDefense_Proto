using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public PlayerData pData;

    private void Start()
    {
        InputManager.Instance.onMove += new OnJoystickMove(Move);
    }

    float spd = 0.1f;

    private void Move(PlayerNum pNum, Vector3 _vec)
    {
        if (pNum != pData.pNum)
            return;

        Vector3 vec = _vec.normalized;

        transform.Translate(vec * spd);
    }
}
