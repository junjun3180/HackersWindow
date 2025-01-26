using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Definition
    
    public int itemScore;
    private GameManager gameManager;
    private StatusManager statusManager;
    private ItemManager itemManager;
    public ItemType itemType;
    public string ItemName;
    [TextArea] public string ItemInfomation;
    public int ItemSize;
    public bool IsUsable = true;
    public bool IsDeletable = true;
    private bool isPickedUp = false;

    Rigidbody2D rb = null;

    // 아이템 흡수 효과
    private Transform playerTransform;
    private bool isTracking = false;
    public bool isDroped = false;

    #endregion

    #region Default Function

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        itemManager = ItemManager.Instance;
        rb = GetComponent<Rigidbody2D>();
    }
     
    // Update is called once per frame
    void Update()
    {
        if (isTracking && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= statusManager.GetDistance)
            {
                // 흡수 범위 이내: 아이템 획득
                ItemTypeToFun();
            }
            else if (distanceToPlayer > statusManager.MaxDistance)
            {
                // 최대 거리 이상: 추적 중단
                isTracking = false;
            }
            else
            {
                // 추적: 플레이어 추적
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                transform.position += directionToPlayer * statusManager.AbsorptionSpeed * Time.deltaTime;
            }
        }
    }

    #endregion

    #region Collider

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 용량 부족한 경우 or (현재 체력 + 힐 아이템 회복량 > 맥스 체력)
            if (statusManager.MaxStorage - statusManager.CurrentStorage < ItemSize
                || 
                    (
                    (itemType == ItemType.Heal || itemType == ItemType.HPFull)
                    && 
                    (statusManager.HPisFull || (int)statusManager.MaxHp - (int)statusManager.CurrentHp == 0)
                    )
               )
            {
                isTracking = false;
                if (rb != null)
                {
                    Vector2 pushDirection = (transform.position - collision.transform.position).normalized;

                    // Add a small force to move the item
                    float DropForce = statusManager.DropForce;
                    rb.drag = statusManager.DragForce;
                    rb.AddForce(pushDirection * DropForce);
                    StartCoroutine(StopAfterDelay(0.5f));
                }
                else
                {
                    Debug.LogError("rb is null");
                }
            }
            else
            {
                // 흡수 효과 
                playerTransform = collision.transform;
                if(isDroped == false)
                    isTracking = true;
            }
        }
    }

    #endregion

    #region Get Item 

    private void ItemTypeToFun()
    {
        // 아이템 기능
        switch (itemType)
        {
            case ItemType.Coin1:
            case ItemType.Coin5:
            case ItemType.Coin10:
            case ItemType.Coin15:
            case ItemType.Coin100:
                CoinItem();
                break;
            case ItemType.Key:
            case ItemType.ExpansionKit_1:
            case ItemType.ExpansionKit_2:
            case ItemType.ExpansionKit_3:
                AddItem();
                break;
            case ItemType.CardPack:
                CardPackItem();
                break;
            case ItemType.ForcedDeletion:
                ForcedDeletionItem();
                break;
            case ItemType.ProgramRemove:
                ProgramRemoveItem();
                break;
            case ItemType.ProgramRecycle:
                AddItem();
                break;
            case ItemType.Card_Clover:
            case ItemType.Card_Dia:
            case ItemType.Card_Spade:
            case ItemType.Card_Hearth:
            case ItemType.Ticket_BlackShop:
            case ItemType.Ticket_Down:
            case ItemType.Ticket_Shop:
            case ItemType.Ticket_Random:
            case ItemType.Ticket_Special:
            case ItemType.Ticket_Boss:
                AddItem();
                break;
            case ItemType.Heal:
                HealItem();
                break;
            case ItemType.TemHp:
                TemHpItem();
                break;
            case ItemType.Shiled:
                ShiledItem();
                break;
            case ItemType.Spark:
                SparkItem();
                break;
            case ItemType.HPFull:
                HPFullItem();
                break;

        }
    }

    private void AddItem()
    {
        if (itemManager != null)
        {
            if (itemManager.AddItem(this))
            {
                Destroy(gameObject);
                isPickedUp = true;
            }
            else
                Debug.Log("Do not add item");
        }
        else
        {
            Debug.Log("ItemManager is not find");
        }
    }

    #endregion

    #region Item Effect 

    // Item Usage Effect Section
    private void CoinItem()
    {
        // Debug.Log("Item CoinItem");
        statusManager.CoinUp(itemScore);
        if (itemManager != null)
        {
            if (itemManager.AddItem(this))
            {
                Destroy(gameObject);
                isPickedUp = true;
            }
            else
                Debug.Log("Do not add item");
        }
        else
        {
            Debug.Log("ItemManager is not find");
        }
    }

    private void CardPackItem()
    {
        AddItem();
    }

    private void ForcedDeletionItem()
    {
        AddItem();
    }

    private void ProgramRemoveItem()
    {
        AddItem();
        Debug.Log("제거툴 기능 구현 안되어 있음");
    }

    private void ProgramRecycleItem()
    {
        AddItem();
        Debug.Log("프로그램 재활용 기능 구현 안되어 있음");
    }

    private void HealItem()
    {
        if(statusManager != null)
        {
            statusManager.Heal(itemScore);
            Destroy(gameObject);
        }
    }

    private void TemHpItem()
    {
        if (statusManager != null)
        {
            statusManager.TemHpUp(itemScore);
            Destroy(gameObject);
        }
    }

    private void ShiledItem()
    {
        if (statusManager != null)
        {
            statusManager.ShieldHpUp(itemScore);
            Destroy(gameObject);
        }
    }

    private void SparkItem()
    {
        if (statusManager != null)
        {
            statusManager.ElectUp(itemScore);
            Destroy(gameObject);
        }
    }

    private void HPFullItem()
    {
        if (statusManager != null)
        {
            statusManager.HPisFull = true;
            itemScore = (int)statusManager.MaxHp - (int)statusManager.CurrentHp;
            statusManager.Heal(itemScore);
            Destroy(gameObject);
        }
    }

    #endregion

    #region Coroutine

    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        rb.velocity = Vector2.zero;
    }

    #endregion
}
