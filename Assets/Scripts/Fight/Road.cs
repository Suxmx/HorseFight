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
    private float spriteSize;

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
        core.RegisterRoad(this);
    }

    /// <summary>
    /// �����ƶ��������
    /// </summary>
    /// <param name="dir">1������ߵ���,2�����ұ�</param>
    /// <param name="t">��ǰ��Ϸʱ��</param>
    /// <param name="speed">��ƥ�ٶ�</param>
    private Vector2 CalcPos(Team team, float t,int speed)
    {
        Vector2 dirVec = team == Team.A ? Vector2.right : Vector2.left;
        Vector2 start = team == Team.A ? leftPos : rightPos;
        return start + t * dirVec * unitLength*speed;
    }

    public void OnStart()
    {
        // leftHorse.SetDir(Team.A);
        // rightHorse.SetDir(Team.B);
    }

    public void LogicUpdate(float t)
    {
        if (finish) return;
        //TODO: �����ƥ״̬���
        if(!leftHorse.HasStatus(EStatus.Die))
        {
            leftHorse.transform.position = CalcPos(Team.A, t, leftHorse.speed);
        }
        if(!rightHorse.HasStatus(EStatus.Die))
        {
            rightHorse.transform.position = CalcPos(Team.B, t, rightHorse.speed);
        }
        if (CheckHit())
        {
            HorseFight();
            Debug.Log($"��ײʱ��{t}");
        }
        
        if(leftHorse.transform.position.x>rightPos.x||rightHorse.transform.position.x<leftPos.x )
        {
            finish = true;
            Debug.Log($"����ʱ��{t}");
        }
    }

    private bool CheckHit()
    {
        return leftHorse.transform.position.x >= rightHorse.transform.position.x;
    }

    private Team HorseFight()
    {
        if (leftHorse.damage > rightHorse.damage)
        {
            rightHorse.LoseCG();
            return Team.A;
        }
        else if (leftHorse.damage < rightHorse.damage)
        {
            leftHorse.LoseCG();
            return Team.B;
        }
        else
        {
            return Team.None;
        }
    }

    public void SetHorse(Horse horse)
    {
        if (horse.horseTeam == Team.A)
        {
            if(leftHorse!=null)return;
            leftHorse = horse;
            horse.transform.position = leftPos;
            horse.SetPutMode(horse.horseTeam,false);
            horse.SetDir(horse.horseTeam);
            spriteSize = leftHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        else if (horse.horseTeam == Team.B)
        {
            if(rightHorse!=null)return;
            rightHorse = horse;
            horse.transform.position = rightPos;
            horse.SetPutMode(horse.horseTeam,false);
            horse.SetDir(horse.horseTeam);
            spriteSize = leftHorse.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        
    }
}