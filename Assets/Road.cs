using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class Road : MonoBehaviour
{
    public float roadLength => rightPos.x - leftPos.x;
    public int num => Convert.ToInt32((name.Split("Road ")[1]));
    public bool finish = false;

    private Vector2 leftPos => leftTrans.position;
    private Vector2 rightPos => rightTrans.position;
    private Transform leftTrans, rightTrans;
    [SerializeField]private Horse leftHorse, rightHorse;
    private GameCore core;

    private float unitLength => roadLength / 20f;
    // private float curTime => core.curTime;

    private void Awake()
    {
        leftTrans = transform.Find("LeftStart");
        rightTrans = transform.Find("RightStart");
    }

    private void Start()
    {
        core = ServiceLocator.Get<GameCore>();
    }

    /// <summary>
    /// 计算移动后的向量
    /// </summary>
    /// <param name="dir">1代表左边的马,2代表右边</param>
    /// <param name="t">当前游戏时间</param>
    /// <param name="speed">马匹速度</param>
    private Vector2 CalcPos(int dir, float t,int speed)
    {
        Vector2 dirVec = dir == 1 ? Vector2.right : Vector2.left;
        Vector2 start = dir == 1 ? leftPos : rightPos;
        return start + t * dirVec * unitLength*speed;
    }

    public void OnStart()
    {
    }

    public void LogicUpdate(float t)
    {
        if (finish) return;
        //TODO: 添加马匹状态检测
        leftHorse.transform.position = CalcPos(1, t,leftHorse.speed);
        rightHorse.transform.position = CalcPos(2, t,rightHorse.speed);
        if(leftHorse.transform.position.x>rightPos.x||rightHorse.transform.position.x<leftPos.x )
        {
            finish = true;
            Debug.Log($"到达时间{t}");
        }
    }
}