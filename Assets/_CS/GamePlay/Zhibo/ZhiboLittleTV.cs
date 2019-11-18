using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ZhiboLittleTvView{

    public CanvasGroup rootCG;
    public Image Content;
    public Animator animator;

    public Transform GemsTr;
    public List<Image> GemList = new List<Image>();

    public Transform TokenContainer;

    public Text NowScore;
}



public class ZhiboLittleTV : MonoBehaviour
{
    public static int MaxGem = 16;

    public ZhiboAudienceMgr audienceMgr;
    public bool IsEmpty;
    public RectTransform rt;

    public ZhiboLittleTvView view;

    public ZhiboAudience TargetAudience;

    //public float TimeLeft;

    public IResLoader pResLoader;

    public void Init(ZhiboAudienceMgr audienceMgr)
    {

        this.audienceMgr = audienceMgr;
        this.rt = transform as RectTransform;
        view = new ZhiboLittleTvView();
        BindView();

        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        view.animator = GetComponent<Animator>();
        view.animator.Play("Empty");
        gameObject.SetActive(false);
        view.rootCG.alpha = 1;
        //animator.ResetTrigger("");
    }




    private void BindView()
    {
        view.rootCG = transform.GetComponent<CanvasGroup>();
        view.Content = transform.Find("Bg").GetComponent<Image>();
        view.GemsTr = transform.Find("Bg").Find("Gems");

        view.NowScore = transform.Find("Bg").Find("Score").GetComponent<Text>();
        view.TokenContainer = transform.Find("Bg").Find("Tokens");

        view.GemList.Clear();
        for (int i = 0; i < MaxGem; i++)
        {
            Transform child = view.GemsTr.GetChild(i);
            view.GemList.Add(child.GetComponent<Image>());
        }
    }





    public void Show(ZhiboAudience TargetAudience)
    {
        gameObject.SetActive(true);
        view.animator.SetTrigger("Appear");
        this.TargetAudience = TargetAudience;
        UpdateHp();
    }

    public void UpdateScore()
    {

    }

    public void Affected()
    {
        //TimeLeft += 10f;
        view.animator.SetTrigger("Affected");
    }

    public void Disappear()
    {
        view.animator.SetTrigger("Disappear");
        if(TargetAudience != null)
        {
            //int idx = audienceMgr.gameMode.nowAudiences.IndexOf(TargetAudience);
            //if(idx == -1)
            //{
            //    return;
            //}
            //audienceMgr.gameMode.nowAudiences.Remove(TargetAudience);
        }
    }

    public void Attracted()
    {
        view.rootCG.alpha = 0.4f;
    }

    Tween preTween;

    private void UpdateHp()
    {
        int idx = 0;

        for (int j = 0; j < TargetAudience.BlackHp; j++)
        {
            view.GemList[idx].gameObject.SetActive(true);
            view.GemList[idx].sprite = pResLoader.LoadResource<Sprite>("ZhiboMode2/Gems/6");
            idx++;
        }

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
        if (TargetAudience.isDead())
        {
            audienceMgr.ShowAudienceEffect(TargetAudience);
            Attracted();
        }
    }

    public void HpFadeOut()
    {

        float alpha = 1;
        List<int> toFadeOut = new List<int>();

        int preIdx = 0;
        for(int i = 0; i < 6; i++)
        {
            int num = TargetAudience.preHp[i] - TargetAudience.GemHp[i];
            for(int j= preIdx+ TargetAudience.GemHp[i]; j < num; j++)
            {
                toFadeOut.Add(j);
            }
            preIdx = preIdx + TargetAudience.preHp[i];
        }

        if(preTween != null)
        {
            preTween.Kill();
        }

        preTween = DOTween.To
        (
            () => alpha,
            (x) => { alpha = x; },
            0,
            1f
        ).OnUpdate(delegate { 

            for(int i=0;i< toFadeOut.Count; i++)
            {
                view.GemList[toFadeOut[i]].color = new Color(0,0,0,alpha);
            }

        }).OnComplete(delegate {

            UpdateHp();
            preTween = null;
        }).OnKill(delegate {
            UpdateHp();
        });
    }

    public void Hit()
    {

    }

}
