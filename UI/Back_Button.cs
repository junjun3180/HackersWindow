using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_Button : MonoBehaviour
{
    UI_4_LocalDisk ui_4_LocalDisk;
    //MapGenerator mapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
        //mapGenerator = FindObjectOfType<MapGenerator>();
    }

    // Update is called once per frame
   

    public void BackButton()
    {
        //if (ui_4_LocalDisk.adressList.Count > 1)
        //{
        //    Map targetMap = ui_4_LocalDisk.adressList[ui_4_LocalDisk.adressList.Count - 2];
        //    //int targetIndex = mapGenerator.mapList.IndexOf(targetMap);
        //    //ui_4_LocalDisk.LocalDisckUISet(targetIndex);
        //}
    }
}
