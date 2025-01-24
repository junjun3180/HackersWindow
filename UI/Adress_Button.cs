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
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
    }

    public void AdressButton()
    {
        int index = transform.GetSiblingIndex()-1;
    }
}
