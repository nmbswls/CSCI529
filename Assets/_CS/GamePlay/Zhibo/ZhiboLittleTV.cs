using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ZhiboLittleTvView{
    public Image Content;
    public Animator animator;

    public Transform GemsTr;
    public List<Image> GemList = new List<Image>();
}



public class ZhiboLittleTV : MonoBehaviour
{
    public static int MaxGem = 16;

    public ZhiboUI zhiboUI;
    public bool IsEmpty;
    public RectTransform rt;

    public ZhiboLittleTvView view;

    public ZhiboAudience TargetAudience;

    public float TimeLeft;

    public IResLoader pResLoader;

    public void Init(ZhiboUI zhiboUI)
    {

        this.zhiboUI = zhiboUI;
        this.rt = transform as RectTransform;
        view = new ZhiboLittleTvView();
        BindView();

        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        view.animator = GetComponent<Animator>();
        view.animator.Play("Empty");
        //animator.ResetTrigger("");
    }




    private void BindView()
    {
        view.Content = transform.Find("Bg").GetComponent<Image>();
        view.GemsTr = transform.Find("Bg").Find("Gems");
        view.GemList.Clear();
        for (int i = 0; i < MaxGem; i++)
        {
            Transform child = view.GemsTr.GetChild(i);
            view.GemList.Add(child.GetComponent<Image>());
        }
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
        view.animator.SetTrigger("Appear");
    }

    public void Show(ZhiboAudience TargetAudience)
    {
        TimeLeft = 20f;
        //animator.SetTrigger("start");
        view.animator.SetTrigger("Appear");
        this.TargetAudience = TargetAudience;

        UpdateHp();

        //view.Content.sprite =  
    }



    public void Attract()
    {
        //TimeLeft += 10f;
        //animator.SetTrigger("attracted");
    }

    public void Disappear()
    {
        view.animator.SetTrigger("Disappear");
        if(TargetAudience != null)
        {
            int idx = zhiboUI.gameMode.nowAudiences.IndexOf(TargetAudience);
            if(idx == -1)
            {
                return;
            }
            zhiboUI.gameMode.nowAudiences.Remove(TargetAudience);
        }
    }


    public void UpdateHp()
    {
        int idx = 0;
        for (int i = 0; i < TargetAudience.GemHp.Length; i++)
        {
            for (int j = 0; j < TargetAudience.GemHp[i]; j++)
            {
                view.GemList[idx].gameObject.SetActive(true);
                view.GemList[idx].sprite = pResLoader.LoadResource<Sprite>("ZhiboMode2/Gems/" + i);
                idx++;
            }
        }
        for (int i = idx; i < MaxGem; i++)
        {
            view.GemList[i].gameObject.SetActive(false);
        }
    }

    public void Hit()
    {

    }

}
