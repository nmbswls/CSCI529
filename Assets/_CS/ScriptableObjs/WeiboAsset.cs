using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new Weibo", menuName = "Ctm/weibo")]
public class WeiboAsset
{
	public int Index;
	public string Name;
	public string Desp;
	public string Content;
	public string Time;
	public string Avatar;
	public string Forwardable;
	public string GainCard;
	public string Reviewable;
	public int NumOfReview;
	public string Review1;
	public string Bonus1;
	public int Value1;
	public string Review2;
	public string Bonus2;
	public int Value2;
	public string Review3;
	public string Bonus3;
	public int Value3;
}