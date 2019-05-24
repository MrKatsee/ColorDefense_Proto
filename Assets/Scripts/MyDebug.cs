using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyDebug : MonoBehaviour
{
    public static Text text;

    void Awake()
    {
        text = GameObject.Find("Debug_Text").GetComponent<Text>();
    }
    
    public static void Log(object o)
    {
        //text.text = o.ToString();
    }
}
