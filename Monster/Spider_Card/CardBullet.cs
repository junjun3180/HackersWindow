using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBullet : MonoBehaviour
{
    public float rotationSpeed = 1000000f;  // 회전 속도
    public float BulletPower = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StatusManager.Instance.TakeDamage(BulletPower, MonsterBase.MonsterType.M_CardPack);
            gameObject.SetActive(false); // 파괴
        }
    }
}
