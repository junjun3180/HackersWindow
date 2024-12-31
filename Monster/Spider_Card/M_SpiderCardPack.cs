using System.Collections;
using UnityEngine;

public class M_SpiderCardPack : MonsterBase
{
    public Transform SpawnPoint;
    public float AttackCoolTime;

    // ÇÁ¸®ÆÕ ¸®½ºÆ® 
    public GameObject SpiderPrefab1;
    public GameObject SpiderPrefab2;
    public GameObject SpiderPrefab3;
    public GameObject SpiderPrefab4;

    protected override void Start()
    {
        GameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MAnimator = GetComponentInChildren<Animator>();
        folderManager = FolderManager.Instance;
        StartCoroutine(MonsterRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator MonsterRoutine()
    {
        while (true)
        {
            yield return AttackPreparation();
            yield return null;
        }

    }

    public override IEnumerator AttackPreparation()
    {
        int SpiderColor = Random.Range(0, 4); // 0, 1 Èò»ö. 2, 3 »¡°­»ö
        switch (SpiderColor)
        {
            case 0:
            case 1:
                MAnimator.SetTrigger("White_Spawn");
                break;
            case 2:
            case 3:
                MAnimator.SetTrigger("Red_Spawn");
                break;
        }

        // Animation Loading
        yield return new WaitForSeconds(10.6f);

        switch (SpiderColor)
        {
            case 0:
                Instantiate(SpiderPrefab1, SpawnPoint.position, SpawnPoint.rotation);
                break;
            case 1:
                Instantiate(SpiderPrefab2, SpawnPoint.position, SpawnPoint.rotation);
                break;
            case 2:
                Instantiate(SpiderPrefab3, SpawnPoint.position, SpawnPoint.rotation);
                break;
            case 3:
                Instantiate(SpiderPrefab4, SpawnPoint.position, SpawnPoint.rotation);
                break;
        }

        yield return new WaitForSeconds(AttackCoolTime);
    }
}
