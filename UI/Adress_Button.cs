using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adress_Button : MonoBehaviour
{
    // MapGenerator mapGenerator;
    UI_4_LocalDisk ui_4_LocalDisk;

    // Start is called before the first frame update
    void Start()
    {
        // mapGenerator = FindObjectOfType<MapGenerator>();
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
    }

    public void AdressButton()
    {
        // Debug.Log("버튼눌림");
        int index = transform.GetSiblingIndex()-1;
        // int targetIndex = mapGenerator.mapList.IndexOf(ui_4_LocalDisk.adressList[index]);
        // .LocalDisckUISet(targetIndex);

    }
}
