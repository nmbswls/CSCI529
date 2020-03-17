using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "new TVSuffix", menuName = "Ctm/TVSuffix")]
public class TVSuffixLoader
{
    public string Name;
    public string Description;
    public string Effect1;
    public string Effect2;
    public string Effect3;
    public string Effect4;
    public int Value1;
    public int Value2;
    public int Value3;
    public int Value4;
    public int Probability;
    public string IsBoss;
}

public enum TVSuffixEffect
{
    none,
    requestKoucai,
    requestCaiyi,
    requestJishu,
    requestKangya,
    requestWaiguan,
    requestAll
}

public class TVSuffix
{
    public List<TVSuffixEffect> effects = new List<TVSuffixEffect>();
    public List<int> values = new List<int>();
    public string name;
    public string description;
    public int probability;
    public bool isBoss;
}

public class TVSuffixList
{
    public List<TVSuffix> suffixs = new List<TVSuffix>();

    public void loadSuffix()
    {
        SuffixContentList suffixContent = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<SuffixContentList>("AudianceSuffixs/SuffixContentList", false);
        foreach (TVSuffixLoader c in suffixContent.Entities)
        {
            TVSuffix tvsf = new TVSuffix();
            tvsf.name = c.Name;
            tvsf.description = c.Description;
            tvsf.probability = c.Probability;
            tvsf.isBoss = c.IsBoss == "TRUE";
            if (c.Effect1 != "none" && c.Effect1.Length != 0)
            {
                TVSuffixEffect sufEffect = (TVSuffixEffect)System.Enum.Parse(typeof(TVSuffixEffect), c.Effect1);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value1);
            }
            if (c.Effect2 != "none" && c.Effect2.Length != 0)
            {
                TVSuffixEffect sufEffect = (TVSuffixEffect)System.Enum.Parse(typeof(TVSuffixEffect), c.Effect2);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value2);
            }
            if (c.Effect3 != "none" && c.Effect3.Length != 0)
            {
                TVSuffixEffect sufEffect = (TVSuffixEffect)System.Enum.Parse(typeof(TVSuffixEffect), c.Effect3);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value3);
            }
            if (c.Effect4 != "none" && c.Effect4.Length != 0)
            {
                TVSuffixEffect sufEffect = (TVSuffixEffect)System.Enum.Parse(typeof(TVSuffixEffect), c.Effect4);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value4);
            }
            suffixs.Add(tvsf);
        }
    }
}