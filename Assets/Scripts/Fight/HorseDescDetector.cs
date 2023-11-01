using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class HorseDescDetector : Service
{
    private GameCore core;
    private GameState state => core.currentState;
    private HorseDesc cacheDesc;

    protected override void Start()
    {
        base.Start();
        core = ServiceLocator.Get<GameCore>();
    }

    private void Update()
    {
        // if (cacheDesc is not null && cacheDesc.gameObject.activeInHierarchy)
            cacheDesc?.DisableDesc();
        if (state == GameState.Fighting) return;
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        transform.position = mouseWorldPos;
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
        if (!hit.collider) return;
        Team team = hit.transform.name[0] == 'L' ? Team.A : Team.B;
        Road road = hit.transform.parent.GetComponent<Road>();
        Horse horse = road.GetHorse(team);
        if (!horse) return;
        cacheDesc = horse.GetComponent<HorseDesc>();
        cacheDesc.EnableDesc();
    }
}