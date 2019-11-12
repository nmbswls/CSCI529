using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    SKillCtrl pSkillCtrl;

    public string SkillId;
    public GameObject LinePrefab;

    public RectTransform rt;


    GameObject Expands;
    Transform Lines;
    Transform Subskills;

    Image Icon;

    public List<SubskillItem> SubskillList = new List<SubskillItem>();


    public void Init(SKillCtrl pSkillCtrl)
    {
        this.pSkillCtrl = pSkillCtrl;

        BindView();
        CollectAllSubSKills();

        TurnOrigin();
    }

    public void TurnOrigin()
    {
        transform.localScale = Vector3.one;
        Expands.gameObject.SetActive(false);
    }

    public void FocusNow()
    {
        transform.localScale = Vector3.one * 1.3f;
        Expands.gameObject.SetActive(true);
    }

    public void BindView()
    {
        rt = (RectTransform)transform;
        Expands = transform.Find("Expand").gameObject;
        Lines = Expands.transform.Find("Lines");
        Subskills = Expands.transform.Find("Tabs");

        Icon = transform.Find("Icon").GetComponent<Image>();

        ClickEventListerner listener = Icon.gameObject.GetComponent<ClickEventListerner>();
        if (listener == null)
        {
            listener = Icon.gameObject.AddComponent<ClickEventListerner>();
        }
        listener.ClearClickEvent();
        listener.OnClickEvent += delegate
        {
            pSkillCtrl.ChooseBaseSkill(this);
        };
    }


    public void CollectAllSubSKills()
    {
        foreach(Transform child in Subskills)
        {
            SubskillItem subItem = child.GetComponent<SubskillItem>();
            subItem.Init(this);
            SubskillList.Add(subItem);
        }

    }

    [ContextMenu("GenLines")]
    public void GenLines()
    {
        Transform lines = transform.Find("Expand").Find("Lines");
        if(lines == null)
        {
            return;
        }
        Debug.Log(lines.childCount);

        foreach (Transform line in lines)
        {
            GameObject.DestroyImmediate(line.gameObject);
        }

        Transform subskills = transform.Find("Expand").Find("Tabs");

        List<Transform> ll = new List<Transform>();
        foreach (Transform child in subskills)
        {
            ll.Add(child);
        }
        for (int i=0;i< ll.Count; i++)
        {
            GameObject line = Instantiate(LinePrefab, lines);
            float angle = Vector3.SignedAngle(transform.up, (ll[i].position - transform.position),Vector3.forward);
            (line.transform as RectTransform).sizeDelta = new Vector2(5, (ll[i].position - transform.position).magnitude);
            line.transform.position = (ll[i].position + transform.position) / 2;
            line.transform.localEulerAngles = new Vector3(0,0, angle);
        }
    }


    public void Click(string skillId)
    {
        pSkillCtrl.ChooseSubskill(skillId);
    }
}
