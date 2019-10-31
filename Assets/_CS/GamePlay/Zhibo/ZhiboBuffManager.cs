using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZhiboBuffManager
{
    public List<ZhiboBuff> ZhiboBuffs;


    public int[] BuffAddValue = new int[5];
    public int[] BuffAddPercent = new int[5];

    public float GenScoreRate = 1;
    public float DanmuExtraReward = 0;
    public float GainScorePerSec = 0;
    public float ExtraSuccessPossibility = 0;
    public float BadRateDiff = 0;
    public bool AvoidBadDanmu = false;

    

    private Dictionary<string, string> BuffDesp = new Dictionary<string, string>();


    public string GetBuffDesp(string buffId)
    {
        if (BuffDesp.ContainsKey(buffId))
        {
            return BuffDesp[buffId];
        }
        return "丢失";
    }
    public ZhiboBuffManager(List<ZhiboBuff> ZhiboBuffs)
    {
        this.ZhiboBuffs = ZhiboBuffs;
        LoadBuff();
    }

    private void LoadBuff()
    {
        BuffDesp.Add("m+", "增加{0}点魅力");
        BuffDesp.Add("t+", "增加{0}点体力");
        BuffDesp.Add("k+", "增加{0}点口才");
        BuffDesp.Add("j+", "增加{0}点技艺");
        BuffDesp.Add("f+", "增加{0}点反应");

        BuffDesp.Add("m+%", "增加百分比{0}的魅力");
        BuffDesp.Add("t+%", "增加百分比{0}的体力");
        BuffDesp.Add("k+%", "增加百分比{0}的口才");
        BuffDesp.Add("j+%", "增加百分比{0}的技艺");
        BuffDesp.Add("f+%", "增加百分比{0}的反应");
    }
    public void CalculateBuffExtras()
    {
        for (int i = 0; i < 5; i++)
        {
            BuffAddValue[i] = 0;
            BuffAddPercent[i] = 0;
        }
        GenScoreRate = 0;
        DanmuExtraReward = 0;
        GainScorePerSec = 0;
        ExtraSuccessPossibility = 0;
        BadRateDiff = 0;

        AvoidBadDanmu = false;

        for (int i=0;i< ZhiboBuffs.Count; i++)
        {
            ZhiboBuff buff = ZhiboBuffs[i];
            switch (buff.buffId)
            {
                case "m+":
                    BuffAddValue[0] += buff.buffLevel;
                    break;
                case "m+%":
                    BuffAddPercent[0] += buff.buffLevel;
                    break;
                case "t+":
                    BuffAddValue[1] += buff.buffLevel;
                    break;
                case "t+%":
                    BuffAddPercent[1] += buff.buffLevel;
                    break;
                case "k+":
                    BuffAddValue[2] += buff.buffLevel;
                    break;
                case "k+%":
                    BuffAddPercent[2] += buff.buffLevel;
                    break;
                case "f+":
                    BuffAddValue[3] += buff.buffLevel;
                    break;
                case "f+%":
                    BuffAddPercent[3] += buff.buffLevel;
                    break;
                case "j+":
                    BuffAddValue[4] += buff.buffLevel;
                    break;
                case "j+%":
                    BuffAddPercent[4] += buff.buffLevel;
                    break;
                case "score+":
                    GenScoreRate += buff.buffLevel;
                    break;
                case "sps":
                    GainScorePerSec += buff.buffLevel;
                    break;
                default:
                    break;
            }
        }
    }
}
