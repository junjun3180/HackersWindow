using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalDiskUIPortalPanel : MonoBehaviour,IPointerClickHandler
{
    float clickTime = 0;
    UI_4_LocalDisk ui_4_LocalDisk;


    // Start is called before the first frame update
    void Start()
    {
       //  mapGenerator = FindObjectOfType<MapGenerator>();
        ui_4_LocalDisk = UI_4_LocalDisk.Instance;
    }

    void OnMouseDoubleClick()
    {

    }   

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - clickTime) < 0.3f)
        {

        }
        else
        {
            clickTime = Time.time;
        }
    }
}
