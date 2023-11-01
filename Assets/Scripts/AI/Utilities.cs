using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;


public static class Utilities
{
    /// <summary>
    /// 用于根据给定的概率来产生结果的函数
    /// </summary>
    /// <param name="possibilities">参与随机的各个概率</param>
    /// <returns>返回的结果序号，从1开始</returns>
    public static int RandomChoose(params float[] possibilities)
    {
        int choose = 0;
        float rand, addup = 0f, tmp = 0f;
        foreach (var pos in possibilities)
            addup += pos;
        rand = UnityEngine.Random.Range(0f, addup);
        foreach (var pos in possibilities)
        {
            choose++;
            if (rand >= tmp && rand < tmp + pos)
            {
                break;
            }

            tmp += pos;
        }

        return choose;
    }
    public static int RandomChoose(List<float> possibilities)
    {
        int choose = 0;
        float rand, addup = 0, tmp = 0;
        foreach (var pos in possibilities)
            addup += pos;
        rand = Random.Range(0, addup);
        foreach (var pos in possibilities)
        {
            choose++;
            if (rand >= tmp && rand < tmp + pos)
            {
                break;
            }

            tmp += pos;
        }

        return choose;
    }
    public static int RandomChoose(List<int> possibilities)
    {
        int choose = 0;
        float rand, addup = 0, tmp = 0;
        foreach (var pos in possibilities)
            addup += pos;
        rand = Random.Range(0, addup);
        foreach (var pos in possibilities)
        {
            choose++;
            if (rand >= tmp && rand < tmp + pos)
            {
                break;
            }

            tmp += pos;
        }

        return choose;
    }

    public static Team Opponent(this Team team)
    {
        if (team == Team.None) return Team.None;
        else return team == Team.A ? Team.B : Team.A;
    }

    public static void Disturb<T>(this List<T> list)
    {
        for (int n = list.Count - 1; n > 0; n--)
        {
            int k = UnityEngine.Random.Range(0, n + 1);
           list.Swap(list[n],list[k]);
        }
    }

    public static void Swap<T>(this List<T> list,T a,  T b)
    {
        int indexA = list.IndexOf(a), indexB = list.IndexOf(b);
        list[indexB] = a;
        list[indexA] = b;
    }

    public static T RandomPick<T>(this List<T> list)
    {
        List<int> tmp = new List<int>();
        for(int i=0;i<list.Count;i++)
            tmp.Add(1);
        int choose = RandomChoose(tmp)-1;
        return list[choose];
    }
}