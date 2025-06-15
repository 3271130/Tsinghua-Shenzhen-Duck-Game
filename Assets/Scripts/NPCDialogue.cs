using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string targetSceneName;
    public string[] dialogues;  // NPC 대화 내용 배열
    public int currentDialogueIndex = 0;  // private에서 public으로 변경
    public bool hasCompletedMiniGame = false;  // 미니게임 완료 여부 추가

    // 현재 대화 인덱스를 반환하는 public 프로퍼티 추가

    public int GetCurrentDialogueIndex()
    {
        return currentDialogueIndex;
    }
    public int CurrentDialogueIndex
    {
        get { return currentDialogueIndex; }
    }

    public string GetCurrentDialogue()
    {
        if (dialogues != null && dialogues.Length > currentDialogueIndex)
        {
            return dialogues[currentDialogueIndex];
        }
        return string.Empty;
    }
    public void AdvanceDialogue()
    {
        currentDialogueIndex++;
    }

    public bool IsLastDialogue()
    {
        return currentDialogueIndex >= dialogues.Length - 1;
    }

    public void SetCompletedMiniGame()
    {
        hasCompletedMiniGame = true;
    }

    public void ResetDialogue()
    {
        currentDialogueIndex = 1;  // Element1부터 시작하도록 설정
    }

    public void SetDialogueIndex(int index)
    {
        currentDialogueIndex = index;
        Debug.Log($"Dialogue index set to: {index}");
    }

}