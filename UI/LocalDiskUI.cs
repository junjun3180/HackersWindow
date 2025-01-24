using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDiskUI : MonoBehaviour
{
    UI_4_LocalDisk ui_4_LocalDisk;
    public GameObject telMap;
    // TeleportManager telManger;
    // Start is called before the first frame update
    private void Awake()
    {
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
    }
 
    public void LocalDiskTel()
    {

    }
}
