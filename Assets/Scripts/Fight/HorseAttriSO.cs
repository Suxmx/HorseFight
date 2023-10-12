using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct horseUnit
{
    [LabelText("种类")]public EHorse type;
    [LabelText("攻击")]public int damage;
    [LabelText("速度")]public int speed;
    [LabelText("价格")]public int price;
    [LabelText("描述")]public string description;
}

[CreateAssetMenu(fileName = "骑士属性",menuName = "SO/HorseAttribute")]
public class HorseAttriSO: ScriptableObject
{
    public GameObject baseHorse;
    public List<horseUnit> horses;

    [Button("一键生成预制体")]
    public void GeneratePrefabs()
    {
        string folderPath = "Assets/Prefabs/Horses/";
        GameObject tmp = new GameObject("tmp");
        foreach (var unit in horses)
        {
            string path = folderPath + unit.type+".prefab";
            GameObject newHorseObj = Instantiate(baseHorse, tmp.transform, true);
            newHorseObj.name = unit.type.ToString();
            Horse newHorse = newHorseObj.GetComponent<Horse>();
            newHorse.damage = unit.damage;
            newHorse.type = unit.type;
            newHorse.speed = unit.speed;
            newHorse.price = unit.price;
            newHorse.ResetText();
            PrefabUtility.SaveAsPrefabAsset(newHorseObj, path);
            AssetDatabase.Refresh(); 
        }
    }
}