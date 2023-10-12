using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public enum EHorse //因为用英文还要另外弄一堆中文名，干脆用中文枚举一劳永逸了
{
    None,
    慢马,
    小马,
    风暴,
    爆弹魔,
    沉默者,
    成长,
    肥马,
    教官,
    救星,
    狂马,
    幽灵,
    装甲马,
    冠军,
    后勤,
    大法师,
    冲锋,
    无面,
    超级兵,
    飞驰,
    烧马,
    双面,
    黑暗双面,
    巨人
}

public enum Team
{
    None,
    A,
    B
}

public class Horse : MonoBehaviour
{
    public string horseName => type.ToString();

    [Header("属性"), OnValueChanged(nameof(ResetText))]
    public EHorse type;

    [OnValueChanged(nameof(ResetText))] public int speed;
    [OnValueChanged(nameof(ResetText))] public int damage;
    [OnValueChanged(nameof(ResetText))] public int price;
    [Header("面板")]
    public Team horseTeam=Team.None;
    [NonSerialized] public Skill skill;


    private Transform attributeTransform;
    private TextMeshPro damageText;
    private TextMeshPro speedText;
    private TextMeshPro nameText;
    private Sprite backgroundL, backgroundR,backgroundM;
    private List<Status> statuses;
    private StatusFactory statusFactory;

    private void Awake()
    {
        ResetText();
        skill = transform.Find("Skill").GetComponent<Skill>();
        backgroundL = Resources.Load<Sprite>("HorseBackgroundL");
        backgroundR = Resources.Load<Sprite>("HorseBackgroundR");
        backgroundM = Resources.Load<Sprite>("HorseBackgroundM");
        statuses = new List<Status>();
    }

    private void Start()
    {
        statusFactory = ServiceLocator.Get<StatusFactory>();
    }

    private void ResetText()
    {
        attributeTransform = transform.Find("Texts");
        damageText = attributeTransform.Find("DamageText").GetComponent<TextMeshPro>();
        speedText = attributeTransform.Find("SpeedText").GetComponent<TextMeshPro>();
        nameText = attributeTransform.Find("Name").GetComponent<TextMeshPro>();

        damageText.text = damage.ToString();
        speedText.text = speed.ToString();
        nameText.text = horseName;
    }

    public void SetDir(Team team)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float backSize = sr.sprite.bounds.size.x;
        horseTeam = team;
        if (team == Team.A)
        {
            sr.sprite = backgroundL;
            attributeTransform.localPosition = new Vector3(-backSize / 2f, 0, 1);
        }
        else if (team == Team.B)
        {
            sr.sprite = backgroundR;
            attributeTransform.localPosition = new Vector3(backSize / 2f, 0, 1);
        }
        else
        {
            Debug.LogWarning("设置方向错误");
        }
    }

    public void LoseCG()
    {
        if (HasStatus(EStatus.Die)) return;
        AddStatus(EStatus.Die);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = backgroundM;
        attributeTransform.localPosition = new Vector3(0, 0, 1);
        StartCoroutine(IeLose());
    }

    private IEnumerator IeLose()
    {
        Transform tmpParent = new GameObject("TempParent").transform;
        transform.SetParent(tmpParent);
        float rotate = horseTeam == Team.A ? 50 : -50;
        Vector2 flyVec = horseTeam == Team.A ? new Vector2(-1, 1) : new Vector2(1, 1);
        for (int i = 1; i <= 30; i++)
        {
            transform.Rotate(0,0,rotate);
            tmpParent.Translate(flyVec*0.3f);
            yield return new WaitForFixedUpdate();
        }
    }

    public bool HasStatus(EStatus tag)
    {
        foreach (var status in statuses)
        {
            if (status.StatusTag == tag)
                return true;
        }

        return false;
    }

    public bool AddStatus(EStatus status)
    {
        if (HasStatus(status)) return false;
        statuses.Add(statusFactory.GetStatus(status));
        return true;
    }

}