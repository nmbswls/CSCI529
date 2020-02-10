using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new Mail", menuName = "Ctm/mail")]
public class MailAsset
{
	public int Index;
	public string Title;
	public string Desp;
	public string Content;
    public string From;
    public string Avatar;
	public string Time;
	public string Condition;
    public string ConditionValue1;
    public string ConditionValue2;
    public string WithPicture;
    public string PictureLink;
    public string WithBonus;
    public int NumOfBonus;
    public string BonusValue;
    public string BonusType;
    public string Tag;
}