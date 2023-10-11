using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public Vector2 leftPos => leftTrans.position;
    public Vector2 rightPos => rightTrans.position;
    public float roadLength => rightPos.x - leftPos.x;
    public int num=>Convert.ToInt32((name.Split("Road ")[1]));

    private Transform leftTrans, rightTrans;
    private EHorse leftHorse, rightHorse;
    private void Awake()
    {
        leftTrans = transform.Find("LeftStart");
        rightTrans = transform.Find("RightStart");
        leftHorse = EHorse.None;
        rightHorse = EHorse.None;
    }
}
