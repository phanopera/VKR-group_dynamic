using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupNum : MonoBehaviour
{
    //ÎÏÈÑÀÍÈÅ ÍÀÇÂÀÍÈß ÃĞÓÏÏÛ
    void Start()
    {
        string parentName = transform.parent.name.Substring(7);
        GetComponent<TextMesh>().text = parentName + "";
    }

}
