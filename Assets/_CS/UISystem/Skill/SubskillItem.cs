using UnityEngine;
using System.Collections;

public class SubskillItem : MonoBehaviour
{

    public string SkillId;

    public GameObject ReachedLine;
    public GameObject PreNode;

    SkillItem baseItem;
    // Use this for initialization
    public void Init(SkillItem baseItem)
    {
        this.baseItem = baseItem;

        ClickEventListerner listener = GetComponent<ClickEventListerner>();
        if(listener == null)
        {
            listener = gameObject.AddComponent<ClickEventListerner>();
        }
        listener.ClearClickEvent();
        listener.OnClickEvent += delegate
        {
            baseItem.Click(SkillId);
        };
    }
}
