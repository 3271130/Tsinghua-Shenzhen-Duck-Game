using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //public GameObject pressQPrompt;
    public GameObject dialoguePanel;
    public TMPro.TextMeshProUGUI dialogueText;
    public GameObject subtitlePanel;
    public TMPro.TextMeshProUGUI subtitleText;

    public void ShowDialogue(string text)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = text;
        }
    }

    public void ShowSubtitle(string text)
    {
        if (subtitlePanel != null)
        {
            subtitlePanel.SetActive(true);
            subtitleText.text = text;
        }
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    public void HideSubtitle()
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
    }
}