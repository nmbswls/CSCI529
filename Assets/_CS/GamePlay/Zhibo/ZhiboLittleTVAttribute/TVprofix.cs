using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "new TVPrefix", menuName = "Ctm/TVPrefix")]
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
    public string Like;
    public string DisLike;
    public string WaitTime;
}

public enum TVProfixEffect
{
    none,
    addRequest,
    extraRequest,
    addWaitTime
}

public enum TVProfixLike
{
    none,
    koucai,
    caiyi,
    jishu,
    kangya,
    waiguan
}

public class TVPrefix
{
    public List<TVProfixEffect> effects = new List<TVProfixEffect>();
    public List<int> values = new List<int>();
    public string name;
    public string description;
    public int probability;
    public bool isBoss;
    public TVProfixLike like;
    public TVProfixLike dislike;
    public string waitTime;
}

public class TVProfixList
{
    public List<TVPrefix> profixs = new List<TVPrefix>();

    public void loadProfix()
    {
        ProfixContentList ProfixContent = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<ProfixContentList>("AudianceProfixs/ProfixContentList", false);
        foreach (TVProfixLoader c in ProfixContent.Entities)
        {
            TVPrefix tvsf = new TVPrefix();
            tvsf.name = c.Name;
            tvsf.description = c.Description;
            tvsf.probability = c.Probability;
            tvsf.isBoss = c.IsBoss == "YES";
            tvsf.like = (TVProfixLike)System.Enum.Parse(typeof(TVProfixLike), c.Like);
            tvsf.dislike = (TVProfixLike)System.Enum.Parse(typeof(TVProfixLike), c.DisLike);
            tvsf.waitTime = c.WaitTime;
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