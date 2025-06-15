using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooseNPCGameManager : MonoBehaviour
{
    public static GooseNPCGameManager Instance;

    // Transform 정보
    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;
    private Vector3 savedCameraPosition;
    private Quaternion savedCameraRotation;

    private bool hasPositionSaved = false;
    public bool hasCompletedGame = false; 
    public int savedDialogueIndex = 0;    

    private bool isInDialogue;
    private int dialogueIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGameState(Transform player)
    {
        savedPlayerPosition = player.position;
        savedPlayerRotation = player.rotation;
        hasPositionSaved = true;
        Debug.Log($"save position: {savedPlayerPosition}");
    }

    public void RestoreGameState(Transform player)
    {
        if (!hasPositionSaved) return;

        player.position = savedPlayerPosition;
        player.rotation = savedPlayerRotation;
        Debug.Log($"restore position: {player.position}");
    }

    private IEnumerator RestoreCameraWithDelay(Transform camera)
    {
        camera.position = savedCameraPosition;
        camera.rotation = savedCameraRotation;

        yield return new WaitForEndOfFrame();

        camera.position = savedCameraPosition;
        camera.rotation = savedCameraRotation;
    }
    public bool ShouldShowNextDialogue()
    {
        return hasCompletedGame && dialogueIndex == 1;
    }

    public void SetGameCompleted(bool completed)
    {
        hasCompletedGame = completed;
        savedDialogueIndex = completed ? 1 : 0;
        Debug.Log($"Game completed set to: {completed}");
    }
}