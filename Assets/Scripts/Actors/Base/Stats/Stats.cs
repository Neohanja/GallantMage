using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public List<IndividualStat> stats;

    public Stats()
    {
        stats = new List<IndividualStat>();
    }

    public Stats CopyList()
    {
        Stats copyList = new Stats();

        foreach(IndividualStat stat in stats)
        {
            IndividualStat addStat = new IndividualStat(stat);
            copyList.stats.Add(addStat);
        }

        return copyList;
    }

    public void ModStats(Stats other)
    {
        if (other == null || other.stats.Count < 1) return;

        foreach(IndividualStat stat in other.stats)
        {
            if(stats.Contains(stat))
            {
                int sIndex = stats.IndexOf(stat);
                stats[sIndex].AddMod(stat.modValue);
                if (stats[sIndex].baseValue < stat.baseValue) stats[sIndex].SetBase(stat.baseValue);
            }
            else
            {
                stats.Add(new IndividualStat(stat));
            }
        }
    }
}

[System.Serializable]
public class IndividualStat : System.IEquatable<IndividualStat>
{
    public string statName;
    public string shortName;
    public int baseValue;
    public int modValue;
    public StatCalc statType;

    public IndividualStat()
    {
        statName = "Default";
        shortName = "Def";
        baseValue = 1;
        modValue = 1;
    }

    public IndividualStat(IndividualStat copyStat)
    {
        statName = copyStat.statName;
        shortName = copyStat.shortName;
        baseValue = copyStat.baseValue;
        modValue = copyStat.modValue;
        statType = copyStat.statType;
    }

    public void AddMod(int val)
    {
        switch(statType)
        {
            case StatCalc.Total:
                modValue += val;
                break;
            case StatCalc.OutOf:
                modValue += val;
                baseValue += val;
                break;
        }
    }

    public void SetBase(int val)
    {
        switch (statType)
        {
            case StatCalc.Total:
                baseValue = val;
                break;
            case StatCalc.OutOf:
                modValue = val;
                baseValue = val;
                break;
        }
    }

    public void RaiseBase(int val)
    {
        switch (statType)
        {
            case StatCalc.Total:
                baseValue += val;
                break;
            case StatCalc.OutOf:
                modValue += val;
                baseValue += val;
                break;
        }
    }

    public int Total { get { return baseValue + modValue; } }

    public bool Equals(IndividualStat other)
    {
        return statName == other.statName || shortName == other.shortName;
    }

    public enum StatCalc
    {
        Total,
        OutOf
    }
}