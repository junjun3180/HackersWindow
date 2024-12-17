using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window_Close : MonoBehaviour
{
    void Update()
    {
        
    }
    public void Close()
    {
        transform.gameObject.SetActive(false);
    }
}
