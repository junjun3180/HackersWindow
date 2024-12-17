using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : MonoBehaviour
{
    public GameObject[] sprites; // 스크롤할 스프라이트 배열
    public float speed; // 이동 속도
    public float startPositionX = 93f; // 스프라이트가 이쪽까지 도달하면 텔레포트 시작
    public float resetPositionX = -93f; // 스프라이트를 맨 왼쪽으로 이동할 위치 (화면 밖)

    public GameObject Parents;

    private FolderNode ParentsPos;

    public float Teleposition;
    public float Resetposition;

    void Start()
    {
        ParentsPos = Parents.GetComponent<FolderNode>();
        
    }
    private void Update()
    {

        ParentsPos.nowPosition = Parents.transform.position.x;

        Teleposition = (startPositionX + ParentsPos.nowPosition);
        Resetposition = (resetPositionX + ParentsPos.nowPosition);


        foreach (GameObject sprite in sprites)
        {
            // 오른쪽으로 이동
            sprite.transform.Translate(Vector3.right * speed * Time.deltaTime);

            // 스프라이트가 화면 오른쪽 끝에 도달했을 때
            if (sprite.transform.position.x >= Teleposition)
            {
                Debug.Log(Teleposition);
                // 스프라이트를 왼쪽 끝으로 이동
                Vector3 newPosition = sprite.transform.position;
                newPosition.x = Resetposition;
                Debug.Log(newPosition.x);
                sprite.transform.position = newPosition;
                Debug.Log(sprite.transform.position);
            }
        }
    }
}
