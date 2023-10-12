using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
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
        if (!ifPause) return;
        OnResume?.Invoke();
        ifPause = false;
        Time.timeScale = timeScale;
    }

    [Button("改变时间流逝速度")]
    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        if (Time.timeScale > 0.05f)
            Time.timeScale = timeScale;
    }
}