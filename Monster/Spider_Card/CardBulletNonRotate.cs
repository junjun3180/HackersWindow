using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBulletNonRotate : MonoBehaviour
{
    #region Variable Element

    public float BulletPower = 1;

    #endregion

    #region Trigger Event

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StatusManager.Instance.TakeDamage(BulletPower, MonsterType.M_CardPack);
            gameObject.SetActive(false); // ÆÄ±«
        }
    }

    #endregion
}
