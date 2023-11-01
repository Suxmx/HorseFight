using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using Cores;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public GameObject AIModePanel;
    public GameObject HelpPanel;
    public List<GameObject> objsToHide;
    
    private SceneController sceneController;
    private Animator animator;
    private Animation _animation;
    private PlayModeConfig _modeConfig;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        sceneController = ServiceLocator.Get<SceneController>();
        _modeConfig = ServiceLocator.Get<PlayModeConfig>();
    }

    public void StartButtonOnClick()
    {
        HideObjs();
        _modeConfig.SetPlayModeConfig(false);
        animator.Play("MenuAnim");
    }

    public void OpenAIModePanel()
    {
        AIModePanel.SetActive(!AIModePanel.activeInHierarchy);
    }

    public void OpenHelpPanel()
    {
        HelpPanel.SetActive(!HelpPanel.activeInHierarchy);
    }

    private void ChooseAIModeOnClick(AIMode aiMode)
    {
        HideObjs();
        _modeConfig.SetPlayModeConfig(true,aiMode);
        animator.Play("MenuAnim");
    }

    public void EasyButton()
    {
        ChooseAIModeOnClick(AIMode.Easy);
    }

    public void NormalButton()
    {
        ChooseAIModeOnClick(AIMode.Normal);
    }

    public void HardButton()
    {
        ChooseAIModeOnClick(AIMode.Hard);
    }

    public void ExtremeHardButton()
    {
        ChooseAIModeOnClick(AIMode.ExtremeHard);
    }

    public void RandomButton()
    {
        HideObjs();
        _modeConfig.SetPlayModeConfig(true,AIMode.Easy,true);
        animator.Play("MenuAnimToRandomMode");
    }

    public void LoadPlayerScene()
    {
        sceneController.LoadScene(1);
    }

    public void LoadRandomScene()
    {
        sceneController.LoadScene(2);
    }

    private void HideObjs()
    {
        foreach (var objs in objsToHide)
        {
            objs.SetActive(false);
        }
    }
}