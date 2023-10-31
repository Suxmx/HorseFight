using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpButton : MonoBehaviour
{
    public Sprite Sprite;
    public Image texturePanel;

    public void SetTexture()
    {
        texturePanel.sprite = Sprite;
    }
}