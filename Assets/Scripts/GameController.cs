using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class GameController : MonoBehaviour
{
    public GameObject[] clothes; // 떨어질 옷 프리팹 배열
    private float maxWidth; // 화면 너비 범위
    public float spawnRate = 1.0f; // 옷 생성 초기 간격
    public float fallSpeed = 2.05f; // 옷이 떨어지는 초기 속도
    public float difficultyIncreaseRate = 1.05f; // 난이도 증가율 (생성 간격 감소율)
    public float fallSpeedIncrement = 0.115f; // 옷 떨어지는 속도 증가량
    public int winScore; // 승리 조건 점수
    private int clothesCollected = 0; // 받은 옷의 개수
    private bool gameRunning = true; // 게임 진행 상태 플래그
    public static GameController instance; // 싱글톤 인스턴스

    void Awake()
    {
        instance = this; // 싱글톤 인스턴스 설정
    }

    void Start()
    {
        // 화면 너비 계산
        Vector3 screenPos = new Vector3(Screen.width, 0, 0);
        Vector3 moveWidth = Camera.main.ScreenToWorldPoint(screenPos);
        float maxClothWidth = 0f;

        foreach (GameObject cloth in clothes)
        {
            float clothWidth = cloth.GetComponent<Renderer>().bounds.extents.x;
            if (clothWidth > maxClothWidth)
            {
                maxClothWidth = clothWidth;
            }
        }

        maxWidth = moveWidth.x - maxClothWidth;

        // 옷 생성 코루틴 시작
        StartCoroutine(SpawnClothes());
    }

    IEnumerator SpawnClothes()
    {
        while (gameRunning)
        {
            yield return new WaitForSeconds(spawnRate);

            float posX = Random.Range(-maxWidth, maxWidth);
            Vector3 spawnPosition = new Vector3(posX, transform.position.y, 0);
            int randomClothIndex = Random.Range(0, clothes.Length);

            GameObject newCloth = Instantiate(clothes[randomClothIndex], spawnPosition, Quaternion.identity);

            // 랜덤 회전 추가 (회전 범위 축소)
            float randomRotation = Random.Range(-10f, 10f); // -10도에서 10도 사이의 랜덤 각도
            newCloth.transform.Rotate(0, 0, randomRotation);

            // 옷 떨어지는 동작 추가
            StartCoroutine(FallCloth(newCloth));

            // 난이도 증가
            spawnRate = Mathf.Max(0.2f, spawnRate / difficultyIncreaseRate); // 생성 간격 감소 (최소 0.2초)
            fallSpeed += fallSpeedIncrement; // 떨어지는 속도 증가
        }
    }

    IEnumerator FallCloth(GameObject cloth)
    {
        Rigidbody2D rb = cloth.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Rigidbody2D를 사용해 물리적인 떨어짐 구현
            rb.gravityScale = fallSpeed; // 떨어지는 속도를 fallSpeed에 비례하도록 설정
            rb.angularVelocity = Random.Range(-15f, 15f); // 랜덤 회전 속도 추가
        }
        else
        {
            // Rigidbody가 없을 경우 Transform을 사용해 아래로 이동
            while (cloth != null)
            {
                cloth.transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void CollectCloth()
    {
        clothesCollected++;
        Debug.Log("clothesCollected: " + clothesCollected);

        if (clothesCollected >= winScore)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameRunning = false; // 코루틴 종료
        Debug.Log("Game Over!");

        if (GooseNPCGameManager.Instance != null)
        {
            GooseNPCGameManager.Instance.SetGameCompleted(true);
        }

        SceneManager.LoadScene("BeautifulScene"); // GameOver라는 이름의 씬으로 전환
    }
}
