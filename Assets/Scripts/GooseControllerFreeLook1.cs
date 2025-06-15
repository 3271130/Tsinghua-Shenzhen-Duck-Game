using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GooseControllerFreeLook : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public bool isDead = false;

    [Header("UI Elements")]
    public GameObject pressQPrompt;

    [Header("Cameras")]
    public CinemachineFreeLook normalCamera;
    public Camera npcCamera;

    private Animator m_animator;
    private Rigidbody rb;
    private Vector3 movement;
    private NPCDialogue currentNPC;
    private Camera mainCamera;

    private UIManager uiManager;
    private bool isInDialogue = false;

    public string subtitle;

    void Start()
    {
        // Animator 컴포넌트 체크
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.LogError("Animator component is missing!");
        }

        // check Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

        // set Rigidbody
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // find main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found in the scene!");
        }

        // check ui prompt
        if (pressQPrompt != null)
        {
            pressQPrompt.SetActive(false);
        }

        uiManager = FindObjectOfType<UIManager>();

        if (GooseNPCGameManager.Instance != null)
        {
            StartCoroutine(RestoreStateWithDelay());
        }
    }

    private IEnumerator RestoreStateWithDelay()
    {
        // 씬이 완전히 로드될 때까지 기다림
        yield return new WaitForSeconds(0.1f);

        GooseNPCGameManager.Instance.RestoreGameState(transform);

        // 게임 완료 후 돌아왔다면 Element1 대화 표시
        if (GooseNPCGameManager.Instance.hasCompletedGame && currentNPC != null)
        {
            currentNPC.currentDialogueIndex = GooseNPCGameManager.Instance.savedDialogueIndex;
            isInDialogue = true;
            uiManager.ShowDialogue(currentNPC.GetCurrentDialogue());
        }

        //if (mainCamera != null)
        //{
        //    mainCamera.enabled = false;
        //    yield return new WaitForEndOfFrame();
        //    mainCamera.enabled = true;
        //}
    }

    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        movement = (right * horizontal + forward * vertical);

        if (movement.magnitude > 0.1f)
        {
            movement.Normalize();
            if (m_animator != null)
            {
                m_animator.SetBool("IsRunning", true);
            }

            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                720f * Time.deltaTime
            );
        }
        else
        {
            movement = Vector3.zero;
            if (m_animator != null)
            {
                m_animator.SetBool("IsRunning", false);
            }
        }

        if (pressQPrompt != null)
        {
            pressQPrompt.SetActive(currentNPC != null);
        }
        if (currentNPC != null && uiManager != null)
        {
            if (GooseNPCGameManager.Instance != null && GooseNPCGameManager.Instance.hasCompletedGame)
            {
                if (!isInDialogue)
                {
                    StartCoroutine(SwitchToNPCCamera());
                    isInDialogue = true;
                    uiManager.ShowDialogue(currentNPC.GetCurrentDialogue());
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    StartCoroutine(SwitchToNormalCamera());
                    isInDialogue = false;
                    uiManager.HideDialogue();
                    GooseNPCGameManager.Instance.SetGameCompleted(false);
                    currentNPC.SetDialogueIndex(0);
                }
            }
            else
            {
                if (!isInDialogue)
                {
                    uiManager.ShowSubtitle(subtitle);
                }

                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    if (!isInDialogue)
                    {
                        StartCoroutine(SwitchToNPCCamera());
                        isInDialogue = true;
                        uiManager.HideSubtitle();
                        uiManager.ShowDialogue(currentNPC.GetCurrentDialogue());
                    }
                    else
                    {
                        string sceneToLoad = currentNPC.targetSceneName;
                        if (!string.IsNullOrEmpty(sceneToLoad))
                        {
                            GooseNPCGameManager.Instance.SaveGameState(transform);
                            SceneManager.LoadScene(sceneToLoad);
                        }
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDead && movement != Vector3.zero && rb != null)
        {
            Vector3 movePosition = rb.position + movement * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(movePosition);
        }
    }

    void Die()
    {
        isDead = true;
        if (m_animator != null)
        {
            m_animator.SetInteger("AnimIndex", 2);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        NPCDialogue npc = other.GetComponent<NPCDialogue>();
        if (npc != null)
        {
            currentNPC = npc;
            isInDialogue = false;

            if (GooseNPCGameManager.Instance != null && GooseNPCGameManager.Instance.hasCompletedGame)
            {
                currentNPC.SetDialogueIndex(1);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        NPCDialogue npc = other.GetComponent<NPCDialogue>();
        if (npc != null && npc == currentNPC)
        {
            StartCoroutine(SwitchToNormalCamera());
            currentNPC = null;
            isInDialogue = false;
            uiManager.HideDialogue();
            uiManager.HideSubtitle();
        }
    }

    private IEnumerator SwitchToNPCCamera()
    {
        if (npcCamera != null)
        {
            //yield return new WaitForEndOfFrame();
            yield return null;
            mainCamera.enabled = false; 
            npcCamera.enabled = false;
            npcCamera.enabled = true;
        }
    }

    private IEnumerator SwitchToNormalCamera()
    {
        if (npcCamera != null)
        {
            npcCamera.enabled = false;
            mainCamera.enabled = false;
            mainCamera.enabled = true;
        }
        yield return null;
    }
}