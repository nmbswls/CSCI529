using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="role",menuName="Ctm/role")]
[System.Serializable]
public class RoleStoryAsset : ScriptableObject {

	public string Name = "未命名";

	[TextArea(3,10)]
	public string Desp;

	public int[] initProperties = new int[5];

	public int initMoney;

	public int initFreePoint;

	public int initSkillPoint;

	public List<string> initOwning = new List<string>();

    public List<string> initCards = new List<string>();

    public List<string> initSkills = new List<string>();

	[TextArea(3,10)]
	public List<string> specialList = new List<string> ();

	public string imageStr;
}
