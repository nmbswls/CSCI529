using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class AudienceReqContentList : ScriptableObject
{
	public List<AudienceReqLoader> Requests; // Replace 'EntityType' to an actual type that is serializable.
    public List<AudienceHpTemplateLoader> HpTemplates; // Replace 'EntityType' to an actual type that is serializable.

}

[System.Serializable]
public class AudienceReqLoader
{
    public int Index;
    public string AudienceName;
    public string Gem;
    public int Level;
    public int LastTurn;
    public int OriginTimeLast;
    public int ProbabilityOfPrefix;
    public int ProbabilityOfSuffix;

    public string ApplyRandom;
    public int OuterTurnStart;
    public int OuterTurnEnd;

    public int ProbabilityOfBonus;

    public string ApplyRandomBonus;
    public int BonusEffectId;
    public int BonusValue;
    
}

[System.Serializable]
public class AudienceHpTemplateLoader
{
    public int Index;
    public int Gem1;
    public int Gem2;
    public int Gem3;
    public int Gem4;
    public int Gem5;
    public int Gem6;

    public int EffectOuterTurnlUnlock = 0;
    public int EffectOuterTurnlRemove = 30;

    public int EffectInnerTurnlUnlock = 0;
    public int EffectInnerTurnlRemove = 12;
}