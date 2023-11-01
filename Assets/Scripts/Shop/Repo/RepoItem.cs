using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Shop.Repo
{
    public class RepoItem : ShopItem
    {
        private RepoManager _repoManager;

        public override void OnPointerClick(PointerEventData eventData)
        {
            _repoManager.GetHorse(this);
        }

        protected override void Start()
        {
            
        }

        public void SetType(EHorse type)
        {
            this.type = type;
            price = horseFactory.GetHorsePrice(type);
            damageText.text = horseFactory.GetHorseDamage(type).ToString();
            speedText.text = horseFactory.GetHorseSpeed(type).ToString();
            string desc = horseFactory.GetHorseDesc(type);
            desc = string.IsNullOrEmpty(desc) ? "无特殊技能" : desc;
            descRoot.gameObject.SetActive(true);
            desctext = descRoot.Find("DescBg").Find("DescText").GetComponent<TextMeshProUGUI>();
            desctext.text = $"<align=center><size=135%><color=orange>{type.ToString()}</color></align>\n" + "<size=30%>\n" +
                            $"<size=100%>{desc}";
            descRoot.gameObject.SetActive(false);
            icon.sprite = Resources.Load<Sprite>(type.ToString());
        }

        public void Init(HorseFactory horseFactory, RepoManager repoManager)
        {
            this.horseFactory = horseFactory;
            this._repoManager = repoManager;
        }
    }
}