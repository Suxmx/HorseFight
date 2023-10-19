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
    public AudioSource buy, enter;
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
    private RoadManager roadManager;
    private HorsePutter horsePutter;

    private int curRound=1;
    private int CurRound
    {
        get => curRound;
        set
        {
            curRound = value;
            shopRoundText.text = $"{value}/5";
        }
    }
    private TextMeshProUGUI shopRoundText;

    private int loopTimes;

    protected override void Awake()
    {
        base.Awake();
        //寻找与设置子类UI
        panelObj = transform.Find("ShopPanel").gameObject;
        openButton = transform.Find("ShopButton").GetComponent<Button>();
        openButtonText = openButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        coinTextA = openButton.transform.Find("PlayerACoins/Text").GetComponent<TextMeshProUGUI>();
        coinTextB = openButton.transform.Find("PlayerBCoins/Text").GetComponent<TextMeshProUGUI>();
        shopRoundText = transform.parent.Find("ShopRound/Text").GetComponent<TextMeshProUGUI>();
        panelRect = panelObj.GetComponent<RectTransform>();
        height = panelRect.sizeDelta.y;
        ifShow = false;
        //设置隐藏动画
        loopTimes = (int)(aniTime / 0.02f);
        shopItems = new List<ShopItem>();
        //获取HorsePutter
        horsePutter = transform.Find("HorsePutter").GetComponent<HorsePutter>();
    }

    protected override void Start()
    {
        base.Start();
        horseFactory = ServiceLocator.Get<HorseFactory>();
        core = ServiceLocator.Get<GameCore>();
        roadManager = ServiceLocator.Get<RoadManager>();
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
        enter.Play();
        enter.time = 0.2f;
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
        if (!ifShow|| horsePutter.cacheHorse) return;
        if (playerDic[curTeam].Coins < item.price)
        {
            Debug.LogWarning($"购买{item.type}失败,{curTeam}所持有金币:{playerDic[curTeam].Coins},目标价格:{item.price}");
            return;
        }

        //购买
        buy.Play();
        buy.time = 0.2f;
        playerDic[curTeam].Coins -= item.price;
        playerDic[curTeam].ownHorses.Add(item.type);
        Debug.Log($"{curTeam}购买{item.type}成功,剩余金币{playerDic[curTeam].Coins}");
        Transform gainItem = Instantiate(horseFactory.GetHorseObj(item.type), playerDic[curTeam].trans).transform;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
        gainItem.position = worldPosition;
        gainItem.GetComponent<Horse>().SetTeam(curTeam);
        horsePutter.SetHorse(gainItem.GetComponent<Horse>());
        //撤回
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
        
    }

    public void NextRound()
    {
        ResetButton();


        if ((playerDic[Team.A].ownHorses.Count == 5 || playerDic[Team.A].Coins == 0) &&
            (playerDic[Team.B].ownHorses.Count == 5 || playerDic[Team.B].Coins == 0))
        {
            shopRoundText.transform.parent.gameObject.SetActive(false);
            RecoveryCoinText();
            roadManager.ShowAllHorses();
            horsePutter.gameObject.SetActive(false);
            core.FightReady();
            return;
        }

        Team nextTeam = curTeam == Team.A ? Team.B : Team.A;
        if (curTeam == Team.B)
        {
            roadManager.ShowAllHorses();
            RecoveryCoinText();
            CurRound++;
        }

        //若有一方先花完钱
        if (!(playerDic[nextTeam].ownHorses.Count == 5 || playerDic[nextTeam].Coins == 0))
            curTeam = nextTeam;
        else
        {
            roadManager.ShowAllHorses();
            RecoveryCoinText();
            if(nextTeam==Team.B)
                CurRound++;
        }
    }

    /// <summary>
    /// 将文字显示成xxx:n-?的形式
    /// </summary>
    /// <param name="itemPrice"></param>
    public void SetCoinTextUnknown(int itemPrice)
    {
        coinTextA.text =
            (Convert.ToInt32(coinTextA.text) + itemPrice) + "- ?";
    }

    public void RecoveryCoinText()
    {
        playerDic[Team.A].Coins = playerDic[Team.A].Coins;
        playerDic[Team.B].Coins = playerDic[Team.B].Coins;
    }
}