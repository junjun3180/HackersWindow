using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBullet : MonoBehaviour
{
    #region Variable Element

    public float rotationSpeed = 1000000f;  // 회전 속도
    public float BulletPower = 1;

    #endregion

    #region Default Function

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region Trigger Event

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StatusManager.Instance.TakeDamage(BulletPower, MonsterType.M_CardPack);
            gameObject.SetActive(false); // 파괴
        }
    }

    #endregion
}
