using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

public class ZhiboLittleTvView{

    public CanvasGroup rootCG;
    public Image Content;
    public Image AvaContent;
    public Animator animator;

    public Transform GemsTr;
    public List<ZBAudienceReqView> GemList = new List<ZBAudienceReqView>();

    public Transform TokenContainer;
    public GameObject TokenInfo;
    public GameObject MoreToken;
    public List<Image> TokenList=new List<Image>();

    public Text NowScore;
    public Text tvName;

    public List<Image> TimeLeftBlocks = new List<Image>();
}



public class ZBAudienceReqView
{
    public GameObject root;
    public Image bg;
    public Image icon;
}

public class ZhiboLittleTV : MonoBehaviour
{
    public static int MaxGem = 16;

    public ZhiboAudienceMgr audienceMgr;
    public bool IsEmpty;
    public RectTransform rt;

    public ZhiboLittleTvView view;

    public ZhiboAudience TargetAudience;

    public static string[] BlockColorArray = new string[] { "#8C2626", "#BFB706", "#778F15", "#77851F", "#4B8510"};

    public IResLoader pResLoader;

    public bool isAttracted = false;

    public List<Sprite> audienceImage;

    public void Init(ZhiboAudienceMgr audienceMgr)
    {

        this.audienceMgr = audienceMgr;
        this.rt = transform as RectTransform;
        view = new ZhiboLittleTvView();
        BindView();
        RegisterEvents();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        audienceImage.Add(pResLoader.LoadResource<Sprite>("AudienceImage/" + "Normal"));
        audienceImage.Add(pResLoader.LoadResource<Sprite>("AudienceImage/" + "Heizi"));
        view.animator = GetComponent<Animator>();
        view.animator.Play("Empty");
        view.Content.sprite = pResLoader.LoadResource<Sprite>("Zhibo/AudienceBG/normal1_bg"); ;
        gameObject.SetActive(false);
        view.rootCG.alpha = 1;
        isAttracted = false;

        for(int i = 0; i < 5; i++)
        {
            view.TokenList[i].enabled = false;
        }
        //animator.ResetTrigger(""); 
    }

    public void UpdateTimeLeft()
    {

        int count = view.TimeLeftBlocks.Count;

        if(TargetAudience.OriginTimeLast < 0)
        {
            for (int i = 0; i < count; i++)
            {
                view.TimeLeftBlocks[i].fillAmount = 1;
                view.TimeLeftBlocks[i].color = Color.black;
            }
            return;
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                view.TimeLeftBlocks[i].fillAmount = 1;
                Color nowColor = Color.white;
                ColorUtility.TryParseHtmlString(BlockColorArray[i], out nowColor);
                view.TimeLeftBlocks[i].color = nowColor;
            }
        }


        float timePerBlock = TargetAudience.OriginTimeLast / count;
        int completeBlockCount = (int)(TargetAudience.TimeLeft / timePerBlock);
        float extraRate = (TargetAudience.TimeLeft - completeBlockCount * timePerBlock) / timePerBlock;
        //for(int i = 0; i < completeBlockCount; i++)
        //{
        //    view.TimeLeftBlocks[i].fillAmount = 1;
        //}
        for(int i= completeBlockCount; i< count; i++)
        {
            view.TimeLeftBlocks[i].fillAmount = 0;
        }
        if (completeBlockCount < count && completeBlockCount >=0)
        {
            view.TimeLeftBlocks[completeBlockCount].fillAmount = extraRate;
        }
        

    }
    public Vector3 GetPivotPos()
    {
        return transform.position;
    }

    private void BindView()
    {
        view.rootCG = transform.GetComponent<CanvasGroup>();
        view.Content = transform.Find("Bg").GetComponent<Image>();
        view.AvaContent = transform.Find("Bg").Find("Content").GetComponent<Image>();     
        view.GemsTr = transform.Find("Bg").Find("Gems");

        view.NowScore = transform.Find("Bg").Find("Score").Find("Text").GetComponent<Text>();
        view.TokenContainer = transform.Find("Bg").Find("Tokens");
        view.MoreToken = transform.Find("Bg").Find("MoreToken").gameObject;
        view.TokenInfo = transform.Find("Bg").Find("TokenInfo").gameObject;

        view.tvName = transform.Find("Bg").Find("Text").GetComponent<Text>();

        view.MoreToken.SetActive(false);

        //view.TurnLeft = transform.Find("Bg").Find("TurnLeft").Find("Text").GetComponent<Text>();

        Transform timeLeftBLocks = transform.Find("Bg").Find("TimeLeft");
        foreach(Transform child in timeLeftBLocks)
        {
            view.TimeLeftBlocks.Add(child.GetComponent<Image>());
        }

        view.TokenList.Clear();
        foreach (Transform tr in view.TokenContainer)
        {
            view.TokenList.Add(tr.GetComponent<Image>());
        }

        view.GemList.Clear();
        for (int i = 0; i < MaxGem; i++)
        {
            Transform child = view.GemsTr.GetChild(i);
            ZBAudienceReqView reqView = new ZBAudienceReqView();
            reqView.root = child.gameObject;
            reqView.bg = child.Find("BG").GetComponent<Image>();
            reqView.icon = child.Find("Front").GetComponent<Image>();
            view.GemList.Add(reqView);  
        }
    }

    private void RegisterEvents() {

        {
            DragEventListener listener = view.TokenInfo.GetComponent<DragEventListener>();
            if(listener == null)
            {
                listener = view.TokenInfo.AddComponent<DragEventListener>();
                listener.PointerEnterEvent += delegate {
                    audienceMgr.ShowTokenDetail(this);
                };
                listener.OnClickEvent += delegate {
                    Debug.Log("cnm");
                };
                listener.PointerExitEvent += delegate {
                    audienceMgr.HideTokenDetail();
                };
            }
        }
        {
            DragEventListener listener = view.Content.GetComponent<DragEventListener>();
            if (listener == null)
            {
                listener = view.Content.gameObject.AddComponent<DragEventListener>();
                listener.PointerEnterEvent += delegate {
                    if (TargetAudience == null) return;
                    audienceMgr.gameMode.mUICtrl.MouseInputLittleTV(this);
                };
                listener.PointerExitEvent += delegate {
                    if (TargetAudience == null) return;
                    audienceMgr.gameMode.mUICtrl.MouseOutLittleTV(this);
                };
            }

        }
    }


    public void ConvertToHeizi()
    {
        DOTween.To
        (
            () => view.Content.transform.localEulerAngles,
            (x) => { view.Content.transform.localEulerAngles = x; },
            new Vector3(0, 90, 0f),
            0.075f
        ).OnComplete(delegate {
            
            view.Content.sprite = pResLoader.LoadResource<Sprite>("Zhibo/AudienceBG/heizi_bg");
            UpdateHp();

            DOTween.To
            (
                () => view.Content.transform.localEulerAngles,
                (x) => { view.Content.transform.localEulerAngles = x; },
                new Vector3(0, 0, 0f),
                0.075f
            ).OnComplete(delegate {

            });
        });
    }

    




    public void InitLittleTvView(ZhiboAudience TargetAudience)
    {

        //gameObject.SetActive(true);
        view.animator.Play("Empty");
        view.animator.ResetTrigger("Disappear");
        view.animator.SetTrigger("Appear");
        this.TargetAudience = TargetAudience;

        if (TargetAudience.Type == eAudienceType.Heizi)
        {
            view.tvName.text = "KeyBoard Man";
            //view.AvaContent.sprite = audienceImage[1];
        }
        else
        {
            if(TargetAudience.showSuffixName().Length>0)
            {
                view.tvName.text = TargetAudience.showProfixName() + TargetAudience.showSuffixName();
            } else
            {
                view.tvName.text = "New Audience";
            }
            
            //view.AvaContent.sprite = audienceImage[0];
        }
        UpdateHp();
        UpdateBuffs();
        UpdateTurnLeft();
        UpdateTimeLeft();
    }

    public void Show(float delay)
    {
        int a = 0;
        DOTween.To
        (
            () => a,
            (x) => { a = x; },
            0,
            delay
        ).OnComplete(delegate {
            gameObject.SetActive(true);
            view.animator.SetTrigger("Appear");
        });

    }


    Tween preTextTween;
    int fromScore;
    int refScore;
    public void UpdateScore()
    {
        if(preTextTween != null)
        {
            preTextTween.Kill();
            fromScore = (int)TargetAudience.preScore;
            refScore = fromScore;
        }
        else
        {
            fromScore = refScore;
        }
        preTextTween = DOTween.To
        (
            () => refScore,
            (x) => { refScore = x; },
            (int)TargetAudience.NowScore,
            0.3f
        ).OnUpdate(delegate {
            view.NowScore.text = refScore + "";
        });

    }

    public void UpdateTurnLeft()
    {
        if(TargetAudience == null)
        {
            return;
        }
        //view.TurnLeft.text = TargetAudience.LastTurn + "";
    }

    public void UpdateBuffs()
    {

        if(TargetAudience.Skills.Count > 5)
        {
            //默认aura不会超过2个

            for (int i = 0; i < 5; i++)
            {
                view.TokenList[i].enabled = true;
            }

            view.MoreToken.SetActive(true);
        }
        else
        {
            int idx = 0;
            for (int i = 0; i < TargetAudience.Skills.Count; i++)
            {
                view.TokenList[idx].enabled = true;
                idx++;
            }
            //for (int i = 0; i < TargetAudience.Bonus.Count; i++)
            //{
            //    view.TokenList[idx].enabled = true;
            //    idx++;
            //}
            for (int i = idx; i < 5; i++)
            {
                view.TokenList[i].enabled = false;
            }
            view.MoreToken.SetActive(false);
        }

    }

    public void Affected()
    {
        //TimeLeft += 10f;
        view.animator.SetTrigger("Affected");
    }

    public void Disappear()
    {
        view.animator.SetTrigger("Disappear");
        audienceMgr.EmptimizeLittleTV(this);
        if(TargetAudience != null)
        {
            TargetAudience = null;
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
        if (isAttracted) return;
        isAttracted = true;
        view.rootCG.alpha = 0.4f;
        audienceMgr.ShowAudienceKilledEffect(TargetAudience);
    }

    Tween preTween;

    private void ChangeHpGem()
    {
        int idx = 0;

        for (int j = 0; j < TargetAudience.BlackHp; j++)
        {

            view.GemList[idx].root.SetActive(true);
            view.GemList[idx].bg.color = Color.white;
            view.GemList[idx].bg.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems/6");

            view.GemList[idx].icon.enabled = true;
            view.GemList[idx].icon.color = Color.white;
            view.GemList[idx].icon.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems/6");

            idx++;
        }

        for (int i = 0; i < TargetAudience.MaxReq.Length; i++)
        {
            for (int j = 0; j < TargetAudience.MaxReq[i]; j++)
            {
                view.GemList[idx].root.SetActive(true);
                view.GemList[idx].bg.color = new Color(1,1,1,0.3f);
                view.GemList[idx].bg.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems/" + i + "_bg");
                if(j < 6) view.GemList[idx].bg.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems_icon/" + i + "_bg");
                if (j < TargetAudience.NowReq[i])
                {
                    //有血量的部分
                    view.GemList[idx].icon.enabled = true;
                    view.GemList[idx].icon.color = Color.white;
                    view.GemList[idx].icon.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems/" + i);
                }
                else
                {
                    view.GemList[idx].icon.enabled = false;
                    view.GemList[idx].icon.color = Color.white;
                    view.GemList[idx].icon.sprite = pResLoader.LoadResource<Sprite>("Zhibo/Gems/" + i);
                }
                idx++;
            }
        }
        for (int i = idx; i < MaxGem; i++)
        {
            view.GemList[i].root.SetActive(false);
        }

        //路径不对 该由逻辑层控制 视图变化！
        //if (TargetAudience.isSatisfied())
        //{
        //    //Affected();
        //    Attracted();
        //}
    }

    public void UpdateHp()
    {
        int changes = TargetAudience.ReqChangeNum();
        ChangeHpGem();
    }

    //public void HpFadeOut()
    //{

    //    float alpha = 1;
    //    List<int> toFadeOut = new List<int>();

    //    int preIdx = 0;
    //    for(int i = 0; i < 6; i++)
    //    {
    //        //int num = TargetAudience.preHp[i] - TargetAudience.GemHp[i];
    //        for(int j= preIdx+ TargetAudience.GemHp[i]; j < preIdx+TargetAudience.preHp[i]; j++)
    //        {
    //            toFadeOut.Add(j);
    //        }
    //        preIdx = preIdx + TargetAudience.preHp[i];
    //    }

    //    if(preTween != null)
    //    {
    //        preTween.Kill();
    //    }

    //    preTween = DOTween.To
    //    (
    //        () => alpha,
    //        (x) => { alpha = x; },
    //        0,
    //        1f
    //    ).OnUpdate(delegate { 

    //        for(int i=0;i< toFadeOut.Count; i++)
    //        {
    //            view.GemList[toFadeOut[i]].color = new Color(0,0,0,alpha);
    //        }

    //    }).OnComplete(delegate {

    //        ChangeHpGem();
    //        preTween = null;
    //    }).OnKill(delegate {
    //        ChangeHpGem();
    //    });
    //}

    public void Hit()
    {

    }

    private int originSiblingIdx;
    public void HighLight()
    {
        originSiblingIdx = transform.GetSiblingIndex();
        view.Content.transform.localScale = Vector3.one * 1.3f;
        transform.SetAsLastSibling();
    }

    public void CancelHightLight()
    {
        view.Content.transform.localScale = Vector3.one;
        transform.SetSiblingIndex(originSiblingIdx);
        originSiblingIdx = -1;
    }

}