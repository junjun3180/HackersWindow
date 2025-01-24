using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRandomMovement : MonoBehaviour
{
    public GameObject[] fishes; // 움직일 오브젝트 배열
    public Transform leftWall; // 왼쪽 WALL
    public Transform rightWall; // 오른쪽 WALL
    public float speedMin = 1f; // 최소 속도
    public float speedMax = 5f; // 최대 속도

    private List<GameObject> movingFishes = new List<GameObject>(); // 현재 Moving 상태의 물고기들
    private const int MaxMovingFishes = 4; // 최대 Moving 상태의 물고기 수

    void Start()
    {
        // 초기 Moving 상태를 랜덤한 4개 물고기에 설정
        ActivateRandomFishes(MaxMovingFishes);
    }

    void Update()
    {
        for (int i = movingFishes.Count - 1; i >= 0; i--)
        {
            GameObject fish = movingFishes[i];
            if (fish != null)
            {
                MoveFish(fish);
            }
        }

        // 모든 물고기 움직임이 끝난 뒤 새 물고기 활성화
        while (movingFishes.Count < MaxMovingFishes)
        {
            ActivateRandomFishes(1);
        }
    }


    void MoveFish(GameObject fish)
    {
        FishController controller = fish.GetComponent<FishController>();
        if (controller != null && controller.IsMoving)
        {
            float speed = controller.Speed * Time.deltaTime;
            Vector3 moveDirection = controller.IsFlipped ? Vector3.right : Vector3.left;
            fish.transform.Translate(moveDirection * speed);

            // 벽 충돌 체크
            if (!controller.IsFlipped && fish.transform.position.x <= leftWall.position.x)
            {
                StopAndFlip(fish);
            }
            else if (controller.IsFlipped && fish.transform.position.x >= rightWall.position.x)
            {
                StopAndFlip(fish);
            }
        }
    }

    void StopAndFlip(GameObject fish)
    {
        FishController controller = fish.GetComponent<FishController>();
        if (controller != null)
        {
            controller.IsMoving = false;
            controller.IsFlipped = !controller.IsFlipped; // 방향 반전
            movingFishes.Remove(fish); // 리스트에서 제거
        }
    }

    void ActivateRandomFishes(int count)
    {
        if (fishes == null || fishes.Length == 0)
        {
            Debug.LogError("Fishes array is not assigned!");
            return;
        }

        List<GameObject> availableFishes = new List<GameObject>(fishes);
        availableFishes.RemoveAll(f => movingFishes.Contains(f));

        for (int i = 0; i < count && availableFishes.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableFishes.Count);
            GameObject selectedFish = availableFishes[randomIndex];
            availableFishes.RemoveAt(randomIndex);

            FishController controller = selectedFish.GetComponent<FishController>();
            if (controller != null)
            {
                controller.Speed = Random.Range(speedMin + controller.MinPlusSpeed, speedMax + controller.MaxPlusSpeed);
                controller.IsMoving = true;
                movingFishes.Add(selectedFish);
            }
        }
    }
}


