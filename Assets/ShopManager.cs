using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour,IPointerExitHandler
{
    public GameObject panelObj;
    public float aniTime = 0.2f;
    
    private float height;
    private bool ifShow;
    private bool ifAnim=false;//防止协程多次被开始
    private RectTransform panelRect;

    private int loopTimes;
    private void Awake()
    {
        panelRect = panelObj.GetComponent<RectTransform>();
        height = panelRect.sizeDelta.y;
        ifShow = panelRect.anchoredPosition.y > -100;

        loopTimes = (int)(aniTime / 0.02f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShowShop();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            HideShop();
        }
    }

    public void ShowShop()
    {
        if (ifShow||ifAnim) return;
        ifShow = true;
        ifAnim = true;
        StartCoroutine(IeShowPanel());

    }

    public void HideShop()
    {
        if (!ifShow|| ifAnim) return;
        ifShow = false;
        ifAnim = true;
        StartCoroutine(IeHidePanel());
    }

    IEnumerator IeShowPanel()
    {
        float posx = panelRect.anchoredPosition.x;
        for (int i = 1; i <= loopTimes; i++)
        {
            panelRect.anchoredPosition = new Vector2(posx, -1 * height + i * (height / loopTimes));
            yield return new WaitForFixedUpdate();
        }
        ifAnim = false;
        panelRect.anchoredPosition = new Vector2(posx, 0);
    }

    IEnumerator IeHidePanel()
    {
        float posx = panelRect.anchoredPosition.x;
        for (int i = 1; i <= loopTimes; i++)
        {
            panelRect.anchoredPosition = new Vector2(posx, -1 *  i * (height / loopTimes));
            yield return new WaitForFixedUpdate();
        }

        ifAnim = false;
        panelRect.anchoredPosition = new Vector2(posx, -1 * height);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit");
        HideShop();
    }
}
