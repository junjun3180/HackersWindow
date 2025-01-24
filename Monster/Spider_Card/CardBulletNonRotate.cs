using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBulletNonRotate : MonoBehaviour
{
    public float BulletPower = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StatusManager.Instance.TakeDamage(BulletPower, MonsterBase.MonsterType.M_CardPack);
            gameObject.SetActive(false); // ÆÄ±«
        }
    }
}
