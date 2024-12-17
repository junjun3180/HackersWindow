using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special_bullet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            
        }
    }
}
