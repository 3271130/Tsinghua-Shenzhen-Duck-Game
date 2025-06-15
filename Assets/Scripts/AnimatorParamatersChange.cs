using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리 네임스페이스 추가
using UnityEngine.UI; // UI 네임스페이스 추가 (UI 기능 사용 시)

namespace FiveRabbitsDemo
{
    public class RabbitController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 15f; // 이동 속도 조절
        public bool isDead = false; // 사망 여부

        [Header("UI Elements")]
        public GameObject pressQPrompt; // "Press Q to Enter" 텍스트 오브젝트

        private Animator m_animator;
        private Rigidbody rb; // 3D 게임용 Rigidbody

        private Vector3 movement;

        // 현재 가까이 있는 NPC
        private NPCInteraction currentNPC;

        void Start()
        {
            // Animator 컴포넌트 가져오기
            m_animator = GetComponent<Animator>();

            // Rigidbody 컴포넌트 가져오기 또는 추가
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true; // 중력 활성화 (필요 시)
            }

            // Rigidbody 설정 최적화
            rb.constraints = RigidbodyConstraints.FreezeRotation; // 회전 잠금
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 충돌 감지 모드 설정

            // UI 프롬프트 비활성화
            if (pressQPrompt != null)
            {
                pressQPrompt.SetActive(false);
            }
        }

        void Update()
        {
            if (!isDead)
            {
                // 방향키 입력 감지
                float horizontal = Input.GetAxisRaw("Horizontal"); // 좌우 입력: -1, 0, 1
                float vertical = Input.GetAxisRaw("Vertical");     // 상하 입력: -1, 0, 1

                movement = new Vector3(horizontal, 0, vertical).normalized;

                // 애니메이터 파라미터 설정
                if (movement != Vector3.zero)
                {
                    m_animator.SetInteger("AnimIndex", 1); // Run 애니메이션

                    // 이동 방향에 따라 캐릭터 회전
                    Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 720 * Time.deltaTime);
                }
                else
                {
                    m_animator.SetInteger("AnimIndex", 0); // Idle 애니메이션
                }

                // Q 키 프롬프트 표시 여부 설정
                if (pressQPrompt != null)
                {
                    pressQPrompt.SetActive(currentNPC != null);
                }

                // 근접 NPC가 있을 때 Q 키 입력 감지
                if (currentNPC != null && Input.GetKeyDown(KeyCode.Q))
                {
                    string sceneToLoad = currentNPC.targetSceneName;
                    if (!string.IsNullOrEmpty(sceneToLoad))
                    {
                        SceneManager.LoadScene(sceneToLoad);
                    }
                    else
                    {
                        Debug.LogWarning("현재 NPC에 할당된 씬 이름이 설정되지 않았습니다.");
                    }
                }
            }

            // 사망 이벤트 처리 (예시: 스페이스바를 누르면 사망)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Die();
            }
        }

        void FixedUpdate()
        {
            if (!isDead)
            {
                // Rigidbody를 사용한 물리 기반 이동
                Vector3 newPosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }

        void Die()
        {
            isDead = true;
            m_animator.SetInteger("AnimIndex", 2); // Dead 애니메이션
            // 추가로 Dead 상태에서의 동작을 설정할 수 있습니다.
        }

        // Trigger Enter 이벤트: NPC 근접 시
        void OnTriggerEnter(Collider other)
        {
            NPCInteraction npc = other.GetComponent<NPCInteraction>();
            if (npc != null)
            {
                currentNPC = npc;
                Debug.Log("근처 NPC 감지: " + npc.gameObject.name);
                // UI 표시 등 추가 기능을 원하면 여기서 구현
            }
        }

        // Trigger Exit 이벤트: NPC에서 벗어날 때
        void OnTriggerExit(Collider other)
        {
            NPCInteraction npc = other.GetComponent<NPCInteraction>();
            if (npc != null && npc == currentNPC)
            {
                currentNPC = null;
                Debug.Log("NPC 근접 해제: " + npc.gameObject.name);
                // UI 숨기기 등 추가 기능을 원하면 여기서 구현
            }
        }
    }
}
