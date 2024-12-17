using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    private static PoolingManager instance = null;
    public GameObject[] bulletList;
    public GameObject[] monsterList;
    public GameObject bulletBox;
    public int bulletCount;
    public int monsterCount = 50;
    public Queue<Bullet> bulletPool = new Queue<Bullet>();
    public Transform shotPoint;


    public static PoolingManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 싱글톤 인스턴스가 없으면 새로운 인스턴스를 찾거나 생성
                instance = FindObjectOfType<PoolingManager>();

                if (instance == null)
                {
                    GameObject singleton = new GameObject("PoolingManager");
                    instance = singleton.AddComponent<PoolingManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return instance;
        }
    }
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

    void Start()
    {
        BulletMake(0);
    }

    
    public void BulletMake(int bulletNum)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            // 총알 프리팹을 생성
            GameObject bulletPrefab = bulletList[bulletNum];
            GameObject bulletObject = Instantiate(bulletPrefab);

            // 그룹에 넣음
            bulletObject.transform.SetParent(bulletBox.transform, false);

            // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
            bulletObject.transform.SetAsLastSibling();
        }
    }
    public void ReMakeBullet(int Bulletindex)
    {
        foreach (Transform child in bulletBox.transform)
        {
            if (child != null && child.gameObject != null)
            {
                Destroy(child.gameObject); // 자식 오브젝트 삭제 (총알 제거)
            }
        }
        // 기존 총알 제거
        while (bulletPool.Count > 0)
        {
            Bullet bullet = bulletPool.Dequeue(); // 큐에서 총알을 꺼냄
            if (bullet != null && bullet.gameObject != null)
            {
                Destroy(bullet.gameObject); // 게임 오브젝트 삭제
            }
        }

        // 큐 비워짐, 총알 새로 생성
        BulletMake(Bulletindex); // `currentBulletNum`은 새로 만들 총알의 번호
    }
    private void OnDisable()
    {
        for(int i = 0; i < bulletList.Length; i++)
        {
            bulletList[i].GetComponent<Bullet>().SizeReset();
        }
    }
}

