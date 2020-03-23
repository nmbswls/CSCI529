using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "new TVProfix", menuName = "Ctm/TVProfix")]
public class TVProfixLoader
{
    public string Name;
    public string Description;
    public string Effect1;
    public string Effect2;
    public string Effect3;
    public int Value1;
    public int Value2;
    public int Value3;
    public int Probability;
    public string IsBoss;
}

public enum TVProfixEffect
{
    none,
    addRequest,
    extraRequest,
    addWaitTime
}

public class TVProfix
{
    public List<TVProfixEffect> effects = new List<TVProfixEffect>();
    public List<int> values = new List<int>();
    public string name;
    public string description;
    public int probability;
    public bool isBoss;
}

public class TVProfixList
{
    public List<TVProfix> profixs = new List<TVProfix>();

    public void loadProfix()
    {
        ProfixContentList ProfixContent = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<ProfixContentList>("AudianceProfixs/ProfixContentList", false);
        foreach (TVProfixLoader c in ProfixContent.Entities)
        {
            TVProfix tvsf = new TVProfix();
            tvsf.name = c.Name;
            tvsf.description = c.Description;
            tvsf.probability = c.Probability;
            tvsf.isBoss = c.IsBoss == "TRUE";
            if (c.Effect1 != "none" && c.Effect1.Length != 0)
            {
                TVProfixEffect sufEffect = (TVProfixEffect)System.Enum.Parse(typeof(TVProfixEffect), c.Effect1);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value1);
            }
            if (c.Effect2 != "none" && c.Effect2.Length != 0)
            {
                TVProfixEffect sufEffect = (TVProfixEffect)System.Enum.Parse(typeof(TVProfixEffect), c.Effect2);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value2);
            }
            if (c.Effect3 != "none" && c.Effect3.Length != 0)
            {
                TVProfixEffect sufEffect = (TVProfixEffect)System.Enum.Parse(typeof(TVProfixEffect), c.Effect3);
                tvsf.effects.Add(sufEffect);
                tvsf.values.Add(c.Value3);
            }
            profixs.Add(tvsf);
        }
        
    }
}