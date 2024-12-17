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
        // telManger = TeleportManager.Instance;
    }

    // Update is called once per frame
 
    private void OnEnable()
    {
        // ui_4_LocalDisk.RoomUISet();
    }
    private void OnDisable()
    {
        // telManger.LocalDisckTel(telMap);
    }
    public void LocalDiskTel()
    {
        // ui_4_LocalDisk.SetWindowUI();
    }
}
