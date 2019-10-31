using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZhiboLittleTV : MonoBehaviour
{

    public ZhiboUI zhiboUI;
    public bool IsEmpty;
    public RectTransform root;
    public Animator animator;
    public Image content;

    public float TimeLeft;

    public void Init(Transform root, ZhiboUI zhiboUI)
    {

        this.zhiboUI = zhiboUI;
        this.root = (RectTransform)root;

        animator = GetComponent<Animator>();
        animator.Play("Empty");
        //animator.ResetTrigger("");
        content = transform.GetChild(0).GetComponent<Image>();
    }

    public void TickSec()
    {
        if(TimeLeft > 0)
        {
            TimeLeft -= 1;
            if (TimeLeft <= 0)
            {
                Disappear();
            }
        }

    }

    public void Show()
    {
        TimeLeft = 20f;
        //animator.SetTrigger("start");
        animator.SetTrigger("Appear");
    }

    public void Attract()
    {
        TimeLeft += 10f;
        //animator.SetTrigger("attracted");
    }

    public void Disappear()
    {
        animator.SetTrigger("Disappear");
    }

}
