using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
    private GameObject[] bulletList;
    private Transform firePoint;
    public float attackSpeed;
    private float nextFireTime = 0f;
    public enum WeaponType
    {
        BasicWeapon
    }
    public WeaponType weaponType;
    // Start is called before the first frame update
    void Start()
    {
        weaponType = WeaponType.BasicWeapon;
        firePoint = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 클릭 감지
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 클릭이 UI에서 발생한 경우, 아무 작업도 하지 않음
                return;
            }

            TryFire();
        }
    }

    public void Fire()
    {
        // Debug.Log("Fire");
        PoolingManager poolingManager = PoolingManager.Instance;
        if (poolingManager.bulletPool.Count > 0)
        {
            Bullet bullet = poolingManager.bulletPool.Dequeue();

            // 총알의 발사 위치와 방향을 설정합니다.
            Vector3 firePosition = transform.position;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; // 2D 환경에서 z축을 0으로 설정
            Vector3 direction = (mousePosition - firePosition).normalized; // 방향 벡터를 정규화

            bullet.transform.position = firePosition;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            bullet.gameObject.SetActive(true);

            // Rigidbody2D를 사용하여 총알을 발사합니다.
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = direction * bullet.speed; // 일정한 속도로 발사
            }
        }
    }
    private void TryFire()
    {
        float attackInterval = 1f / StatusManager.Instance.AttackSpeed;

        if (Time.time >= nextFireTime)
        {
            Fire();
            SoundManager.Instance.PlayerFireSound();
            nextFireTime = Time.time + attackInterval; // 다음 공격 가능한 시간 업데이트
        }
    }

    public void SetAttackSpeed(float speed)
    {
        // Debug.Log("ChangeSpeed");
        attackSpeed += speed;

        if (attackSpeed < 0.1f)
        {
            attackSpeed = 0.1f;
        }

        // Debug.Log("Attack Speed set to: " + attackSpeed);
    }

}
