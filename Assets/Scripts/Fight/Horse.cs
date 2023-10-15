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
    public int price;
    public int score = 1;
    [Header("面板")] public Team horseTeam = Team.None;
    [NonSerialized] public Skill skill;
    public Road locateRoad;
    public bool ifHiding = false;

    private Transform attributeTransform;
    private Transform spriteTrans;
    private TextMeshPro damageText;
    private TextMeshPro speedText;
    private TextMeshPro nameText;
    private SpriteRenderer iconSr;
    private Sprite backgroundL, backgroundR, backgroundM;
    [LabelText("词条"), SerializeField] private List<Status> statuses;
    private StatusFactory statusFactory;
    private bool beingPut = false;

    [NonSerialized] public int oriDamage; //初始攻击力
    [NonSerialized] public int oriSpeed;

    private void Awake()
    {
        ResetText();
        skill = transform.Find("Skill").GetComponent<Skill>();
        iconSr = GetComponent<SpriteRenderer>();
        backgroundL = Resources.Load<Sprite>("HorseBackgroundL");
        backgroundR = Resources.Load<Sprite>("HorseBackgroundR");
        backgroundM = Resources.Load<Sprite>("HorseBackgroundM");
        statuses = new List<Status>();
        oriDamage = damage;
        oriSpeed = speed;
    }

    private void Start()
    {
        statusFactory = ServiceLocator.Get<StatusFactory>();
    }

    private void Update()
    {
        if (beingPut)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));
            transform.position = mouseWorldPos;
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
                if (hit.collider != null)
                {
                    if ((horseTeam == Team.A && hit.transform.name[0] == 'R') ||
                        (horseTeam == Team.B && hit.transform.name[0] == 'L')) return;
                    hit.transform.parent.GetComponent<Road>().SetHorse(this);
                }
            }
        }
    }

    public void ResetText()
    {
        if (!attributeTransform)
            attributeTransform = transform.Find("Texts");
        if (!damageText)
            damageText = attributeTransform.Find("DamageText").GetComponent<TextMeshPro>();
        if (!speedText)
            speedText = attributeTransform.Find("SpeedText").GetComponent<TextMeshPro>();
        // if (!nameText)
        //     nameText = attributeTransform.Find("Name").GetComponent<TextMeshPro>();
        if (!iconSr)
            iconSr = GetComponent<SpriteRenderer>();

        damageText.text = damage.ToString();
        speedText.text = speed.ToString();
        // nameText.text = horseName;
    }

    public void SetDir(Team team)
    {
        spriteTrans = transform.Find("Sprite");
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Texture2D test = Resources.Load<Texture2D>("女孩");
        Rect rect = new Rect(0, 0, test.width, test.height); //获得图片的长、宽
        horseTeam = team;
        if (team == Team.A)
        {
            Sprite tsp = Sprite.Create(test, rect, new Vector2(1, 0.5f), sr.sprite.pixelsPerUnit);
            sr.sprite = tsp;
            float backSize = sr.sprite.bounds.size.x;
            attributeTransform.localPosition = new Vector3(-backSize / 2f, 0, 1);
            Vector2 tmp = spriteTrans.localPosition;
            spriteTrans.localPosition = new Vector2(tmp.x - backSize / 2f, tmp.y);
        }
        else if (team == Team.B)
        {
            Sprite tsp = Sprite.Create(test, rect, new Vector2(1, 0.5f), sr.sprite.pixelsPerUnit);
            sr.sprite = tsp;
            float backSize = sr.sprite.bounds.size.x;
            attributeTransform.localPosition = new Vector3(backSize / 2f, 0, 1);
            Vector2 tmp = spriteTrans.localPosition;
            spriteTrans.localPosition = new Vector2(tmp.x + backSize / 2f, tmp.y);
        }
        else
        {
            Debug.LogWarning("设置方向错误");
        }

        iconSr.flipX = horseTeam != Team.A;
    }

    public void LoseCG()
    {
        if (HasStatus(EStatus.Die)) return;
        AddStatus(EStatus.Die);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // sr.sprite = backgroundM;
        attributeTransform.localPosition = new Vector3(0, 0, 1);
        StartCoroutine(IeLose());
    }

    private IEnumerator IeLose()
    {
        Transform oriParent = transform.parent;
        Transform tmpParent = new GameObject("TempParent").transform;
        transform.SetParent(tmpParent);
        float rotate = horseTeam == Team.A ? 50 : -50;
        Vector2 flyVec = horseTeam == Team.A ? new Vector2(-1, 1) : new Vector2(1, 1);
        for (int i = 1; i <= 40; i++)
        {
            transform.Rotate(0, 0, rotate);
            tmpParent.Translate(flyVec * 0.3f);
            yield return new WaitForFixedUpdate();
        }

        transform.parent = oriParent;
        Destroy(tmpParent.gameObject);
    }

    public bool HasStatus(EStatus tag)
    {
        foreach (var status in statuses)
        {
            if (status.statusTag == tag)
                return true;
        }

        return false;
    }

    public bool AddStatus(EStatus status)
    {
        if (HasStatus(status) && !statusFactory.GetTemplateStatus(status).repeatable) return false;
        statuses.Add(statusFactory.GetStatus(status));
        return true;
    }

    public bool AddStatus(EStatus status, int d, int s)
    {
        if (HasStatus(status) && !statusFactory.GetTemplateStatus(status).repeatable) return false;
        Status tmp = statusFactory.GetStatus(status);
        tmp.damageBuffer = d;
        tmp.speedBuffer = s;
        statuses.Add(tmp);
        return true;
    }

    public void ClearStatus()
    {
        statuses.RemoveAll(status => status.ifTmp);
    }

    public void RemoveStatus(EStatus type)
    {
        statuses.RemoveAll(status => status.statusTag==type);
    }

    public void CalcDamageAndSpeed()
    {
        int tmpD = oriDamage, tmpS = oriSpeed;
        foreach (var status in statuses)
        {
            tmpD += status.damageBuffer;
            tmpS += status.speedBuffer;
        }

        damage = tmpD;
        speed = tmpS;
        ResetText();
    }

    public void SetPutMode(Team team, bool mode)
    {
        if (mode == beingPut) return;
        horseTeam = team;
        beingPut = mode;
    }

    #region 隐藏动画

    public void HideSelf()
    {
        if (ifHiding) return;
        ifHiding = true;
        StartCoroutine(IeChangeAlpha(false));
    }

    public void ShowSelf()
    {
        if (!ifHiding) return;
        ifHiding = false;
        StartCoroutine(IeChangeAlpha(true));
    }

    IEnumerator IeChangeAlpha(bool show)
    {
        SpriteRenderer dbg = transform.Find("Sprite/Damage").GetComponent<SpriteRenderer>();
        SpriteRenderer sbg = transform.Find("Sprite/Speed").GetComponent<SpriteRenderer>();
        for (int i = 1; i <= 15; i++)
        {
            //名字与图标
            var ncolor = iconSr.color;
            ncolor = new Color(ncolor.r, ncolor.g, ncolor.b, show ? 1f / 15f * i : 1 - 1f / 15f * i);
            // nameText.color = ncolor;
            iconSr.color = ncolor;
            dbg.color = ncolor;
            sbg.color = ncolor;
            //攻击力文字
            var dcolor = damageText.color;
            dcolor = new Color(dcolor.r, dcolor.g, dcolor.b, show ? 1f / 15f * i : 1 - 1f / 15f * i);
            damageText.color = dcolor;
            //速度文字
            var scolor = speedText.color;
            scolor = new Color(scolor.r, scolor.g, scolor.b, show ? 1f / 15f * i : 1 - 1f / 15f * i);
            speedText.color = scolor;
            yield return new WaitForFixedUpdate();
        }

        //名字与图标
        var c = iconSr.color;
        c = new Color(c.r, c.g, c.b, show ? 1 : 0);
        iconSr.color = c;
        // nameText.color = c;
        dbg.color = c;
        sbg.color = c;
        //攻击力文字
        var dc = damageText.color;
        dc = new Color(dc.r, dc.g, dc.b, show ? 1 : 0);
        damageText.color = dc;
        //速度文字
        var sc = speedText.color;
        sc = new Color(sc.r, sc.g, sc.b, show ? 1 : 0);
        speedText.color = sc;
    }

    #endregion
}