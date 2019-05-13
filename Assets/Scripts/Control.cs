using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.onMove += new OnJoystickMove(Move);
    }

    float spd = 0.1f;

    private void Move(Vector3 _vec)
    {
        Vector3 vec = _vec.normalized;

        transform.Translate(vec * spd);
    }
}
