using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideStats : MonoBehaviour
{
    public static bool isHidden = false; //скрыть буквы
    void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            isHidden = !isHidden;
            Debug.Log("H");
        }
    }
}
