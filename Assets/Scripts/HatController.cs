using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatController : MonoBehaviour
{
    private Vector3 rawPosition;
    private Vector3 hatPosition;
    private float maxWidth;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 screenPos= new Vector3(Screen.width, 0,0);
        Vector3 moveWidth=Camera.main.ScreenToWorldPoint(screenPos);
        //计算帽子的宽度
        float hatWidth = GetComponent<Renderer>().bounds.extents.x;
        //获得帽子的初始位置
        hatPosition =transform.position;
        //计算帽子的移动宽度
        maxWidth = moveWidth.x- hatWidth;
    }

    // Update is called once per frame
    void FixedUpdate()  
{  
    // 获取当前帽子的位置  
    Vector3 currentHatPosition = GetComponent<Rigidbody2D>().position;  
  
    // 定义移动速度和最大范围  
    float moveSpeed = 50.0f; // 这个值可以根据你的需求调整  
    float moveAmount = 0.0f;  
  
    // 检查键盘输入  
    if (Input.GetKey(KeyCode.LeftArrow))  
    {  
        // 向左移动  
        moveAmount = -moveSpeed;  
    }  
    else if (Input.GetKey(KeyCode.RightArrow))  
    {  
        // 向右移动  
        moveAmount = moveSpeed;  
    }  
  
    // 计算新的X位置，并限制在最大范围内  
    float newXPosition = currentHatPosition.x + moveAmount * Time.fixedDeltaTime;  
    newXPosition = Mathf.Clamp(newXPosition, -maxWidth - 10, maxWidth + 10);  
  
    // 设置新的位置  
    Vector3 newHatPosition = new Vector3(newXPosition, currentHatPosition.y, currentHatPosition.z);  
    GetComponent<Rigidbody2D>().MovePosition(newHatPosition);  
}
}
