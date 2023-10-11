using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Service
{
    public UnityEvent OnPause;
    public UnityEvent OnResume;

    public float timeScale = 1;
    
    private bool ifPause = false;
    public void GamePause()
    {
        if (ifPause) return;
        OnPause?.Invoke();
        ifPause = true;
        Time.timeScale = 0f;
    }

    public void GameResume()
    {
        if(!ifPause)return;
        OnResume?.Invoke();
        ifPause = false;
        Time.timeScale = timeScale;
    }

    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
    }
}
