using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorsePutter : MonoBehaviour
{
    public Horse cacheHorse;
    public void Update()
    {
        if (!cacheHorse) return;
        
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
        cacheHorse.transform.position = mouseWorldPos;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null)
            {
                if ((cacheHorse.horseTeam == Team.A && hit.transform.name[0] == 'R') ||
                    (cacheHorse.horseTeam == Team.B && hit.transform.name[0] == 'L')) return;
                if (!hit.transform.parent.GetComponent<Road>().SetHorse(cacheHorse)) return;
                cacheHorse = null;
                
            }
        }
    }

    public void SetHorse(Horse horse)
    {
        cacheHorse = horse;
    }

    public void RevertHorse()
    {
        
    }
}
