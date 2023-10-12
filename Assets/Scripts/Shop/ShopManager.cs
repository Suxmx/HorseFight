using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : Service,IPointerExitHandler
{
    public GameObject panelObj;
    public float aniTime = 0.2f;
    
    private float height;
    private bool ifShow;
    private bool ifAnim=false;//防止协程多次被开始
    private RectTransform panelRect;
    private List<ShopItem> shopItems;
    private Dictionary<Team, PlayerInfo> playerDic;
    private PlayerInfo playerA, playerB;
    private Team curTeam=Team.A;//当前正在购买的对象
    private HorseFactory horseFactory;

    private int loopTimes;
    protected override void Awake()
    {
        base.Awake();
        panelRect = panelObj.GetComponent<RectTransform>();
        height = panelRect.sizeDelta.y;
        ifShow = panelRect.anchoredPosition.y > -100;

        loopTimes = (int)(aniTime / 0.02f);
        shopItems = new List<ShopItem>();
    }

    protected override void Start()
    {
        base.Start();
        horseFactory = ServiceLocator.Get<HorseFactory>();
    }

    # region 商店动画
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
        HideShop();
    }
    #endregion

    public void RegisterItem(ShopItem item)
    {
        shopItems.Add(item);
    }

    public void SetPlayerInfo(Dictionary<Team, PlayerInfo> dic)
    {
        playerDic = dic;
    }

    public void ShopRequest(ShopItem item)
    {
        if (playerDic[curTeam].coins < item.price)
        {
            Debug.LogWarning($"购买{item.type}失败,{curTeam}所持有金币:{playerDic[curTeam].coins},目标价格:{item.price}");
            return;
        }

        playerDic[curTeam].coins -= item.price;
        playerDic[curTeam].ownHorses.Add(item.type);
        Debug.Log($"{curTeam}购买{item.type}成功,剩余金币{playerDic[curTeam].coins}");
        Transform gainItem= Instantiate(horseFactory.GetHorseObj(item.type), playerDic[curTeam].trans).transform;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
        gainItem.position = worldPosition;
        gainItem.GetComponent<Horse>().SetPutMode(curTeam,true);
        HideShop();
    }
}
