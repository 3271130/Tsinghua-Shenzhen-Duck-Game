using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quit : MonoBehaviour
{
     // Update is called once per frame  
    void Update()  
    {  
        // 检查玩家是否按下了Q键  
        if (Input.GetKeyDown(KeyCode.Q))  
        {  
            QuitGame();  
        }  
    }  
  
    // 退出游戏的方法  
    void QuitGame()  
    {  
        #if UNITY_EDITOR  
        // 在Unity编辑器中停止播放  
        UnityEditor.EditorApplication.isPlaying = false;  
        #else  
        // 在发布的游戏中退出应用程序  
        Application.Quit();  
        #endif  
    }  
}
