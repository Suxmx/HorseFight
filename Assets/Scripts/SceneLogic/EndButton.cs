using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using EventSystem = Services.EventSystem;

public class EndButton : MonoBehaviour
{
    private SceneController sceneController;
    private GameCore core;
    private EventSystem _eventSystem;

    private void Start()
    {
        sceneController = ServiceLocator.Get<SceneController>();
        core = ServiceLocator.Get<GameCore>();
        _eventSystem = ServiceLocator.Get<EventSystem>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider is not null&&hit.collider.gameObject == gameObject)
            {
                ReturnMenu();
            }
        }
    }

    public void ReturnMenu()
    {
        sceneController.LoadScene(0);
        sceneController.DestroySelf();
        _eventSystem.DestroySelf();
        
    }
    
}
