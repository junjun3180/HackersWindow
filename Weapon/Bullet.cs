using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed; // 총알 속도
    public Rigidbody2D rb;
    private float lifetime;
    [SerializeField]
    private Material bulletMa;
    private PoolingManager pool;
    private StatusManager statusManager;
    [Header("총알 기본값")]
    [SerializeField]
    private float sizeX;
    [SerializeField]
    private float sizeY;
    [SerializeField]
    private float sizeZ;
    [SerializeField]
    private float colorA;
    void Start()
    {
        pool = PoolingManager.Instance;
        statusManager = StatusManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)
        {
            Vector2 currentDirection = rb.velocity.normalized;
            rb.velocity = currentDirection * statusManager.BulletSpeed;
            lifetime = statusManager.BulletMaximumRange / statusManager.BulletSpeed;
        }
    }

    private void OnEnable()
    {
        if (statusManager == null || rb == null)
        {
            statusManager = StatusManager.Instance;
            rb = GetComponent<Rigidbody2D>();
            if (rb == null || statusManager == null)
            {
                Debug.LogError("Bullet initialization error: StatusManager or Rigidbody2D is not set up.");
                return;
            }
        }

        // Debug.Log("Bullet activated with damage: " + statusManager.AttackPower);
        if (rb != null)
        {
            // 총알이 활성화될 때 방향을 설정합니다.
            rb.velocity = Vector2.zero; // 기존 속도를 리셋
            rb.angularVelocity = 0f; // 기존 각속도 리셋

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            // 발사 방향으로 힘을 적용
            rb.AddForce(direction * statusManager.BulletSpeed, ForceMode2D.Impulse);
        }
        lifetime = statusManager.BulletMaximumRange / statusManager.BulletSpeed;
        StartCoroutine(DisableAfterTime(lifetime));
    }

    // 비활성화 시 자동으로 큐에 추가
    public void OnDisable()
    {
        pool.bulletPool.Enqueue(this);
    }

    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false); // 비활성화
    }

    private void OnCollisionTrigger2D(Collision2D collision)
    {
        gameObject.SetActive(false); // 비활성화
    }
    public void SizeReset()
    {
        transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
        // 현재 색상 가져오기
        Color color = bulletMa.color;

        // 알파값을 설정한 값으로 변경
        color.a = colorA;

        // 색상 다시 설정
        bulletMa.color = color;
        
    }

}


