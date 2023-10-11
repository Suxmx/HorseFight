using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    [Header("属性")] 
    public string horseName;
    public int speed;
    public int damage;
    public int price;

    private Transform attributeTransform;
    private TextMesh damageText;
    private TextMesh speedText;
    private TextMesh nameText;

    private void Awake()
    {
        attributeTransform = transform.Find("Attributes");
        damageText = attributeTransform.Find("DamageText").GetComponent<TextMesh>();
        speedText = attributeTransform.Find("SpeedText").GetComponent<TextMesh>();
        nameText = attributeTransform.Find("Name").GetComponent<TextMesh>();

        damageText.text = damage.ToString();
        speedText.text = damage.ToString();
        nameText.text = horseName;
    }

    private void Start()
    {
        
    }
}
