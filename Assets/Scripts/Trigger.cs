using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trigger : MonoBehaviour
{
    public Text scoreText; // 公开一个Text类型的变量用于引用UI中的Text组件  
    private int score = 0; // 私有变量用于存储分数  
    // Unity生命周期方法，用于初始化  
    void Start()  
    {  
        // 如果scoreText还没有被赋值（比如通过Inspector面板），则可以在这里添加一些错误处理或默认值  
        if (scoreText == null)  
        {  
            Debug.LogError("scoreText 没有被赋值！请在Inspector面板中设置。");  
            // 可以选择设置一个默认值，比如创建一个新的Text组件并隐藏它  
            // 但这通常不是最佳实践，因为它可能会导致混淆和不必要的资源消耗  
        }  
    }

    // 当触发器被其他物体进入时调用  
    void OnTriggerEnter2D(Collider2D other)
    {
        score++;

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("scoreText가 비어있어 점수를 업데이트할 수 없습니다!");
        }

        // GameController의 CollectCloth 메서드 호출
        GameController.instance.CollectCloth();

        // 충돌한 옷 오브젝트 제거
        Destroy(other.gameObject);
    }

}
