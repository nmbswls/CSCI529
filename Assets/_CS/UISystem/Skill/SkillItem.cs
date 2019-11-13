using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class SkillItem : MonoBehaviour
{
    SKillCtrl pSkillCtrl;

    public string SkillId;
    public GameObject LinePrefab;

    public RectTransform rt;


    GameObject Expands;
    CanvasGroup ExpandGroup;
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
        ExpandGroup.alpha = 1;
        Expands.gameObject.SetActive(false);
        transform.localScale = Vector3.one*0.7f;
    }

    public void FocusNow()
    {
        Tween t = DOTween.To
            (
                () => transform.localScale,
                (x) => transform.localScale = x,
                Vector3.one * 1f,
                0.3f
            ).OnKill(delegate {
                transform.localScale = Vector3.one * 1f;
            });
        DOTween.To
            (
                () => ExpandGroup.alpha,
                (x) => ExpandGroup.alpha = x,
                1f,
                0.3f
            );

        ExpandGroup.alpha = 0f;
        //transform.localScale = Vector3.one * 1f;
        Expands.gameObject.SetActive(true);
    }

    public void BindView()
    {
        rt = (RectTransform)transform;
        Expands = transform.Find("Expand").gameObject;
        ExpandGroup = Expands.GetComponent<CanvasGroup>();
        Lines = Expands.transform.Find("Lines");
        Subskills = Expands.transform.Find("Tabs");

        Icon = transform.Find("BaseIcon").GetComponent<Image>();

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

        Transform subskills = transform.Find("Expand").Find("Tabs");

        List<Transform> ll = new List<Transform>();
        foreach (Transform child in subskills)
        {

            SubskillItem sub = child.GetComponent<SubskillItem>();
            if(sub == null || sub.PreNode==null || sub.ReachedLine == null)
            {
                continue;
            }
            float angle = Vector3.SignedAngle(transform.up, (sub.transform.position - sub.PreNode.transform.position), Vector3.forward);
            (sub.ReachedLine.transform as RectTransform).sizeDelta = new Vector2(5, (sub.transform.position - sub.PreNode.transform.position).magnitude);
            sub.ReachedLine.transform.position = (sub.transform.position + sub.PreNode.transform.position) / 2;
            sub.ReachedLine.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }


    public void Click(string skillId)
    {
        pSkillCtrl.ChooseSubskill(skillId);
    }
}
