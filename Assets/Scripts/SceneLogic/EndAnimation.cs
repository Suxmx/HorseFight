using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class EndAnimation : MonoBehaviour
{
    public TextMeshPro teamAScore, teamBScore;
    private EventSystem eventSystem;
    private GameCore core;
    private Animator animator;

    private void Awake()
    {
        animator=GetComponent<Animator>();
    }

    private void Start()
    {
        eventSystem = ServiceLocator.Get<EventSystem>();
        core = ServiceLocator.Get<GameCore>();
        eventSystem.AddListener<int>(EEvent.AfterLoadScene,StartAnim);

        teamAScore.text = core.GetScore(Team.A).ToString();
        teamBScore.text = core.GetScore(Team.B).ToString();
        core.DestroySelf();
    }

    private void StartAnim(int i)
    {
        animator.Play("EndAnim");
        eventSystem.RemoveListener<int>(EEvent.AfterLoadScene,StartAnim);
    }
}
