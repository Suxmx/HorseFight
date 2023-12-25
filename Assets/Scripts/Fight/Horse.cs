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
    堂吉诃德,
    初级骑士,
    荒原豚,
    飓风,
    刽子手,
    小孩,
    中级骑士,
    教官,
    救星,
    狂豚,
    淑女,
    高级骑士,
    冠军,
    执旗手,
    暴风雨,
    灯笼圆盾,
    无面,
    超级骑士,
    影子凤凰,
    蛇牙,
    魔山
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

    [LabelText("词条"), SerializeField] private List<Status> statuses;
    private StatusFactory statusFactory;

    [NonSerialized] public int oriDamage; //初始攻击力
    [NonSerialized] public int oriSpeed;

    private void Awake()
    {
        ResetText();
        skill = transform.Find("Skill").GetComponent<Skill>();
        iconSr = GetComponent<SpriteRenderer>();

        statuses = new List<Status>();
        oriDamage = damage;
        oriSpeed = speed;
    }

    private void Start()
    {
        // statusFactory = ServiceLocator.Get<StatusFactory>();
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

    public void Init(StatusFactory statusFactory, RoadManager roadManager)
    {
        this.statusFactory = statusFactory;
        skill.Init(roadManager);
    }
    public void SetDir(Team team)
    {
        spriteTrans = transform.Find("Sprite");
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Texture2D texture = Resources.Load<Texture2D>(type.ToString());
        Rect rect = new Rect(0, 0, texture.width, texture.height); //获得图片的长、宽
        horseTeam = team;
        if (team == Team.A)
        {
            Sprite tsp = Sprite.Create(texture, rect, new Vector2(1, 0.5f), sr.sprite.pixelsPerUnit);
            sr.sprite = tsp;
            float backSize = sr.sprite.bounds.size.x;
            attributeTransform.localPosition = new Vector3(-backSize / 2f, 0, 1);
            Vector2 tmp = spriteTrans.localPosition;
            spriteTrans.localPosition = new Vector2(tmp.x - backSize / 2f, tmp.y);
        }
        else if (team == Team.B)
        {
            Sprite tsp = Sprite.Create(texture, rect, new Vector2(1, 0.5f), sr.sprite.pixelsPerUnit);
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

    #region 死亡动画

    public void LoseCG()
    {
        if (HasStatus(EStatus.Die)) return;
        AddStatus(EStatus.Die);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        GetComponent<AudioSource>().Play();
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

    #endregion

    #region 词条相关

    /// <summary>
    /// 查看是否有某词条
    /// </summary>
    /// <param name="tag">词条枚举</param>
    /// <returns></returns>
    public bool HasStatus(EStatus tag)
    {
        foreach (var status in statuses)
        {
            if (status.statusTag == tag)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 添加词条
    /// </summary>
    /// <param name="status">词条枚举</param>
    /// <returns></returns>
    public bool AddStatus(EStatus status)
    {
        if (HasStatus(status) && !statusFactory.GetTemplateStatus(status).repeatable) return false;
        statuses.Add(statusFactory.GetStatus(status));
        return true;
    }

    /// <summary>
    /// 添加词条的重载,给增益不确定的词条使用
    /// </summary>
    /// <param name="status">词条枚举</param>
    /// <param name="d">词条攻击增益</param>
    /// <param name="s">词条速度增益</param>
    /// <returns></returns>
    public bool AddStatus(EStatus status, int d, int s)
    {
        if (HasStatus(status) && !statusFactory.GetTemplateStatus(status).repeatable) return false;
        Status tmp = statusFactory.GetStatus(status);
        tmp.damageBuffer = d;
        tmp.speedBuffer = s;
        statuses.Add(tmp);
        return true;
    }

    /// <summary>
    /// 清除临时(ifTmp==True)的词条
    /// </summary>
    public void ClearStatus()
    {
        statuses.RemoveAll(status => status.ifTmp);
    }

    public void RemoveStatus(EStatus type)
    {
        statuses.RemoveAll(status => status.statusTag == type);
    }

    /// <summary>
    /// 计算所有词条的增益
    /// </summary>
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

    #endregion


    public void SetTeam(Team team)
    {
        horseTeam = team;
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
        yield return new WaitForSecondsRealtime(0.5f);
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