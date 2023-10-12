using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopManager : Service, IPointerExitHandler
{
    public float aniTime = 0.2f;

    private float height;
    private bool ifShow;

    private bool ifAnim = false; //防止协程多次被开始

    //子物体
    private RectTransform panelRect;
    private GameObject panelObj;
    private Button openButton;
    private TextMeshProUGUI openButtonText;
    [NonSerialized] public TextMeshProUGUI coinTextA, coinTextB;

    private List<ShopItem> shopItems;
    private Dictionary<Team, PlayerInfo> playerDic;
    private Team curTeam = Team.A; //当前正在购买的对象
    private HorseFactory horseFactory;
    private GameCore core;

    private int loopTimes;

    protected override void Awake()
    {
        base.Awake();
        panelObj = transform.Find("ShopPanel").gameObject;
        openButton = transform.Find("ShopButton").GetComponent<Button>();
        openButtonText = openButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        coinTextA = openButton.transform.Find("PlayerACoins/Text").GetComponent<TextMeshProUGUI>();
        coinTextB = openButton.transform.Find("PlayerBCoins/Text").GetComponent<TextMeshProUGUI>();
        panelRect = panelObj.GetComponent<RectTransform>();
        height = panelRect.sizeDelta.y;
        ifShow = false;

        loopTimes = (int)(aniTime / 0.02f);
        shopItems = new List<ShopItem>();
    }

    protected override void Start()
    {
        base.Start();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        core = ServiceLocator.Get<GameCore>();
    }

    # region 商店动画

    public void ShowShop()
    {
        if (ifShow || ifAnim) return;
        ifShow = true;
        ifAnim = true;
        StartCoroutine(IeShowPanel());
    }

    public void HideShop()
    {
        if (!ifShow || ifAnim) return;
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
            panelRect.anchoredPosition = new Vector2(posx, -1 * i * (height / loopTimes));
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
        if (!ifShow) return;
        if (playerDic[curTeam].Coins < item.price)
        {
            Debug.LogWarning($"购买{item.type}失败,{curTeam}所持有金币:{playerDic[curTeam].Coins},目标价格:{item.price}");
            return;
        }

        //购买
        playerDic[curTeam].Coins -= item.price;
        playerDic[curTeam].ownHorses.Add(item.type);
        Debug.Log($"{curTeam}购买{item.type}成功,剩余金币{playerDic[curTeam].Coins}");
        Transform gainItem = Instantiate(horseFactory.GetHorseObj(item.type), playerDic[curTeam].trans).transform;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
        gainItem.position = worldPosition;
        gainItem.GetComponent<Horse>().SetPutMode(curTeam, true);
        //撤回
        openButtonText.text = "撤回并打开商店";
        openButton.onClick.AddListener(() =>
        {
            Destroy(gainItem.gameObject);
            playerDic[curTeam].Coins += item.price;
            playerDic[curTeam].ownHorses.Remove(item.type);
            ResetButton();
            Debug.Log($"{curTeam}撤回{item.type}成功,剩余金币{playerDic[curTeam].Coins}");
        });
        HideShop();
    }

    /// <summary>
    /// 因为简单粗暴的撤回方式 所以要重新设置一下打开商店的按钮
    /// </summary>
    private void ResetButton()
    {
        openButton.onClick.RemoveAllListeners();
        openButton.onClick.AddListener(ShowShop);
        openButtonText.text = "商店";
    }

    public void NextRound()
    {
        ResetButton();
        Team nextTeam = curTeam == Team.A ? Team.B : Team.A;
        if (curTeam == Team.B) core.ShowAllHorses();
        if ((playerDic[Team.A].ownHorses.Count == 5 || playerDic[Team.A].Coins == 0) &&
            (playerDic[Team.B].ownHorses.Count == 5 || playerDic[Team.B].Coins == 0))
        {
            //TODO:进入对战阶段
            Debug.LogWarning("进入对战阶段");
            core.StartFight();
            return;
        }

        if (!(playerDic[nextTeam].ownHorses.Count == 5 || playerDic[nextTeam].Coins == 0))
            curTeam = nextTeam;
        else
        {
            core.ShowAllHorses();
        }
    }
}