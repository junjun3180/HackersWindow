using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public Player player;
    public float StartTime;
    // private MapGenerator mg;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    
    void Start()
    {
        ResetPlayTime();
        // GameObject mapGeneratorObject = GameObject.Find("MapGenerator");

        // if (mapGeneratorObject != null)
        {
            // mg = mapGeneratorObject.GetComponent<MapGenerator>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetPlayTime()
    {
        StartTime = Time.time;
    }

    //public void ReStartGame()
    //{
    //    if (mg != null)
    //    {
    //        mg.RestMap();
    //    }
    //    else
    //    {
    //        Debug.Log("MapGenerator is not find");
    //    }
    //}

    public Player GetPlayer()
    {
        if(player != null)
            return player;
        else 
            return null;
    }
}
