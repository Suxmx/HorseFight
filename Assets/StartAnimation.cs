using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class StartAnimation : MonoBehaviour
{
   public Button button;
   
   private SceneController sceneController;
   private Animator animator;

   private void Awake()
   {
      animator = GetComponent<Animator>();
   }

   private void Start()
   {
      sceneController = ServiceLocator.Get<SceneController>();
   }

   public void OnClick()
   {
      button.gameObject.SetActive(false);
      animator.Play("MenuAnim");
   }
   public void LoadScene()
   {
      sceneController.LoadNextScene();
   }
}
