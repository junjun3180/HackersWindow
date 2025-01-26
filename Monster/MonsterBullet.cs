using UnityEngine;

public class MonsterBullet : MonoBehaviour
{
    #region Variable Element

    public float lifeTime = 5f;
    public int damage = 10;      
    public MonsterType monstertype;
    private bool hasHit = false; // First Hit Flag

    #endregion

    #region Defualt Function

    void Start()
    {
        // Life Time Destroy
        Destroy(gameObject, lifeTime);
    }

    #endregion

    #region Trigger Event

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Player player = hitInfo.GetComponent<Player>();

        if (hitInfo.gameObject.CompareTag("Player"))
        {
            if (hasHit) return;

            hasHit = true;
            StatusManager statusManager = StatusManager.Instance;
            if (statusManager != null)
            {
                statusManager.TakeDamage(damage, monstertype);
            }

            Destroy(gameObject);
        }
    }

    #endregion
}
