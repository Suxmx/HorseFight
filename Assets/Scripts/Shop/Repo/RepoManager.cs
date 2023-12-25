using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventSystem = Services.EventSystem;

namespace Shop.Repo
{
    [SuppressMessage("ReSharper", "Unity.RedundantSerializeFieldAttribute")]
    public class RepoManager : Service, IShop
    {
        public TextMeshProUGUI aiCoinText;
        public TextMeshProUGUI playerCoinText;
        public List<Button> hideShopButtons;

        [SerializeField] private RectTransform panelRect;
        [SerializeField] private TextMeshProUGUI roundText;
        [SerializeField] private Button openButton;
        [SerializeField] private AudioSource buy;
        [SerializeField] private AudioSource enter;
        private float height;
        private bool ifShow = false;
        private bool ifAnim = false;
        private PlayerInfo _aiInfo;
        private PlayerInfo _playerInfo;
        [Other] private HorseFactory _horseFactory;
        [Other] private RoadManager _roadManager;
        [Other] private StatusFactory _statusFactory;
        [Other] private GameCore _core;
        [Other] private EventSystem _eventSystem;
        private int playerPuts; //玩家已放置的马的数量
        private Team _curTeam = Team.A;
        private int _curRound = 1;
        [SerializeField] private HorsePutter _horsePutter;
        [SerializeField] private Dictionary<int, List<RepoItem>> items;
        
        private Sprite shopButtonTex;
        private Sprite cancelButtonTex;

        private int CurRound
        {
            get => _curRound;
            set
            {
                _curRound = value;
                roundText.text = $"{value}/5";
            }
        }

        protected override void Awake()
        {
            base.Awake();
            height = panelRect.sizeDelta.y;
            shopButtonTex = Resources.Load<Sprite>("deck");
            cancelButtonTex = Resources.Load<Sprite>("cancel");
        }

        protected override void Start()
        {
            base.Start();
            RandomInitRepo();
        }

        # region 仓库动画

        public void ShowShop()
        {
            if (ifShow || ifAnim) return;
            ifShow = true;
            ifAnim = true;
            foreach (var btn in hideShopButtons)
            {
                btn.gameObject.SetActive(true);
            }
            StartCoroutine(IeShowPanel());
        }

        public void HideShop()
        {
            if (!ifShow || ifAnim) return;
            ifShow = false;
            ifAnim = true;
            foreach (var btn in hideShopButtons)
            {
                btn.gameObject.SetActive(false);
            }
            StartCoroutine(IeHidePanel());
        }

        IEnumerator IeShowPanel()
        {
            enter.Play();
            enter.time = 0.2f;
            float posx = panelRect.anchoredPosition.x;
            for (int i = 1; i <= 10; i++)
            {
                panelRect.anchoredPosition = new Vector2(posx, -1 * height + i * (height / 10));
                yield return new WaitForFixedUpdate();
            }

            ifAnim = false;
            panelRect.anchoredPosition = new Vector2(posx, 0);
        }

        IEnumerator IeHidePanel()
        {
            float posx = panelRect.anchoredPosition.x;
            for (int i = 1; i <= 10; i++)
            {
                panelRect.anchoredPosition = new Vector2(posx, -1 * i * (height / 10));
                yield return new WaitForFixedUpdate();
            }

            ifAnim = false;
            panelRect.anchoredPosition = new Vector2(posx, -1 * height);
        }
        
        #endregion

        public bool AIShopRequest(EHorse type, int num)
        {
            int price = _horseFactory.GetHorsePrice(type);
            if (_aiInfo.Coins < price)
            {
                Debug.LogWarning($"AI购买{type}失败,AI所持有金币:{_aiInfo.Coins},目标价格:{price}");
                return false;
            }

            //购买

            _aiInfo.Coins -= price;
            _aiInfo.ownHorses.Add(type);
            Debug.Log($"AI购买{type}成功,剩余金币{_aiInfo.Coins}");
            //初始化马
            Transform horseTrans = Instantiate(_horseFactory.GetHorseObj(type), _aiInfo.trans).transform;
            Horse horse = horseTrans.GetComponent<Horse>();
            horse.SetTeam(Team.B);
            horse.Init(_statusFactory, _roadManager);
            return _roadManager.GetRoad(num).SetHorse(horse);
        }

        public void NextRound()
        {
            ResetButton();
            _roadManager.HideAllHalo(Team.A);
            _roadManager.ShowAllHorses();
            if ((playerPuts == 5 || _playerInfo.Coins == 0) && (_aiInfo.ownHorses.Count == 5 || _aiInfo.Coins == 0))
            {
                roundText.transform.parent.gameObject.SetActive(false);
                _core.FightReady();
                return;
            }

            Team nextTeam = _curTeam.Opponent();
            if (nextTeam == Team.A)
            {
                CurRound++;
            }

            if (nextTeam == Team.A && (playerPuts != 5 && _playerInfo.Coins != 0))
                _curTeam = nextTeam;
            else if (nextTeam == Team.B && (_aiInfo.ownHorses.Count != 5 && _aiInfo.Coins != 0))
                _curTeam = nextTeam;
            else if (nextTeam == Team.B)
                CurRound++;
            _eventSystem.Invoke(EEvent.OnNextRound, _curTeam, CurRound);
        }

        private void RandomInitRepo()
        {
            for (int i = 1; i <= 4; i++)
            {
                var list = _horseFactory.GetHorsesByPrice(i);
                for (int j = 0; j < items[i].Count; j++)
                {
                    var horse = list.RandomPick();
                    items[i][j].Init(_horseFactory, this);
                    items[i][j].SetType(horse);
                    list.Remove(horse);
                }
            }
        }

        private void ResetButton()
        {
            openButton.GetComponent<Image>().sprite = shopButtonTex;
            openButton.onClick.RemoveAllListeners();
            openButton.onClick.AddListener(ShowShop);
        }

        public void GetHorse(RepoItem item)
        {
            if (!ifShow || _horsePutter.cacheHorse) return;
            if (_playerInfo.Coins < item.price)
            {
                Debug.LogWarning($"购买{item.type}失败,Player所持有金币:{_playerInfo.Coins},目标价格:{item.price}");
                return;
            }

            _playerInfo.Coins -= item.price;
            buy.Play();
            buy.time = 0.2f;
            Transform gainItem = Instantiate(_horseFactory.GetHorseObj(item.type), _playerInfo.trans).transform;
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10f));
            gainItem.position = worldPosition;
            gainItem.GetComponent<Horse>().SetTeam(Team.A);
            gainItem.GetComponent<Horse>().Init(_statusFactory, _roadManager);
            _horsePutter.SetHorse(gainItem.GetComponent<Horse>());
            // item.gameObject.SetActive(false);
            playerPuts++;
            _roadManager.ShowAllHalo(Team.A);
            //撤回
            openButton.GetComponent<Image>().sprite = cancelButtonTex;
            openButton.onClick.AddListener(() =>
            {
                _roadManager.HideAllHalo(Team.A);
                _playerInfo.Coins += item.price;
                playerPuts--;
                Destroy(gainItem.gameObject);
                // item.gameObject.SetActive(true);
                ResetButton();
            });
            HideShop();
        }

        public void SetInfos(PlayerInfo AI, PlayerInfo player)
        {
            _aiInfo = AI;
            _playerInfo = player;
        }
    }
}