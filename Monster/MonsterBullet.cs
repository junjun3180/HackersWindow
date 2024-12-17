using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    public float lifeTime = 5f;  // 총알이 파괴되기까지의 시간
    public int damage = 10;      // 총알의 공격력
    public MonsterBase.MonsterType monstertype;

    //한번의 충돌로 두번 효과 발생하는거 방지용
    private bool hasHit = false;



    // Start is called before the first frame update
    void Start()
    {
        // 일정 시간이 지나면 총알 파괴
        Destroy(gameObject, lifeTime);
    }



    // 충돌 처리
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>();

        if (hitInfo.gameObject.CompareTag("Player"))
        {
            if (hasHit) return; // 이미 충돌했으면 더 이상 처리하지 않음
            hasHit = true;
            StatusManager statusManager = StatusManager.Instance;
            if (statusManager != null)
            {
                statusManager.TakeDamage(damage, monstertype);
            }

            Destroy(gameObject);
        }
    }
}
