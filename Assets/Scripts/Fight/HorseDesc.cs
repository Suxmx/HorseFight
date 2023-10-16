using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HorseDesc : MonoBehaviour
{
    public GameObject descCanvas;

    private RectTransform root;
    private TextMeshProUGUI text;
    private HorseFactory factory;
    private Horse horse;
    private EHorse type;
    private Team team => horse.horseTeam;

    private void Awake()
    {
        Debug.Log(gameObject.name);
        Canvas canvas;
        canvas = Instantiate(descCanvas, transform).GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        root = canvas.transform.Find("Root").GetComponent<RectTransform>();
        text = root.Find("Bg/Text").GetComponent<TextMeshProUGUI>();
        horse = GetComponent<Horse>();
        type = horse.type;
    }

    private void Start()
    {
        factory = ServiceLocator.Get<HorseFactory>();
        Debug.Log($"{factory.GetHorseDesc(type)} {type}");
        string desc = factory.GetHorseDesc(type);
        if (string.IsNullOrEmpty(desc)) desc = "无特殊技能";
        text.text = $"<align=center><size=25><color=orange>{type}</color></size></align><size=30%>\n<size=100%>" + desc;
        root.gameObject.SetActive(false);
    }

    public void EnableDesc()
    {
        if (horse.ifHiding) return;
        if (team == Team.B)
            root.pivot = new Vector2(1, 0.5f);
        root.position = transform.position;
        root.gameObject.SetActive(true);
    }

    public void DisableDesc()
    {
        root.gameObject.SetActive(false);
    }
}