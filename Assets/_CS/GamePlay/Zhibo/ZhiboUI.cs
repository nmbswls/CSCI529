using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ZhiboView : BaseView
{

    public Text TurnLeft;
    public Button NextTurnBtn;

    public Button RoleSkill;

    public Transform TVContainer;
    //public List<ZhiboLittleTV> LittleTvList = new List<ZhiboLittleTV>();

    public Transform container;
    public Slider Score;

    public Text QifenValue;
    public Transform BuffDetailPanel;
    public Text BuffDetail;

    public Text ScoreValue;
    public Text TiliValue;

    public Transform TiliBar;
    public List<Image> TiliPoints = new List<Image>();

    public Image Actions;
    public Text DeckLeft;
    public Animator hotAnimator;


    public Transform BuffContainer;

    public CardContainerLayout CardContainer;

    public Text HpValue;
    public Image HpBar;

    public Image TurnTimeBar;

    public float LeftBarMaxFillAmount = 0.35f;
    public float LeftBarMinFillAmount = 0.02f;
    public float TiliMaxFillAmount = 0.5f;
    public float TiliMinFillAmount = 0.2f;

    public RectTransform DanmuField;
    public ScrollRect DanmuFieldSR;
    public RectTransform DanmuFieldNormal;
    public RectTransform DanmuFieldSuper;
    public RectTransform SuperDanmuPreview;

    public RectTransform SpeField;

    public GameObject TokenDetailPanel;

    public Text targetValue;

}
//public class OperatorView
//{
//    public Image ActiveButton;
//    public Image Outline;
//}
public class ZhiboModel : BaseModel
{

    //public List<int> EmptyTVList = new List<int>();
    public int nowScoreText = 0;
}



public class ZhiboUI : UIBaseCtrl<ZhiboModel, ZhiboView>
{

    private int width = 0;
    private int height = 0;
    private int numOfGridVertical;
    private static float MinDanmuInterval = 0.5f;

    private float TimerPerSec = 0;

    private int preDanmuGrid;
    private float[] preDanmuTime;

    private string DanmuFengxiang;


    IResLoader mResLoader;
    public ZhiboGameMode gameMode;

    bool lockNextTurn;
    float lockNextTurnTime = 5;

    public void LockNextTurnBtn()
    {
        lockNextTurn = true;
        lockNextTurnTime = 3;
    }

    public override void Init()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode;
    }

    public override void PostInit()
    {
        width = (int)view.DanmuField.rect.width;
        height = (int)view.DanmuField.rect.height;

        numOfGridVertical = 40;
        preDanmuGrid = -1;
        preDanmuTime = new float[numOfGridVertical];

        UpdateScore();
        //view.TiliValue.text = "10";
        //view.TiliImage.fillAmount = view.TiliMaxFillAmount;

        UpdateQifen();

        HideBuffDetail();
    }


    public void GainHotEffect(int num)
    {
        if (num > 5)
        {
            view.hotAnimator.ResetTrigger("Activate");
            view.hotAnimator.SetTrigger("Activate");
        }
    }

    public void UpdateHp()
    {
        view.HpBar.fillAmount = view.LeftBarMinFillAmount + (gameMode.state.Hp * 1.0f/ gameMode.state.MaxHp) * (view.LeftBarMaxFillAmount - view.LeftBarMinFillAmount);
        //if(gameMode.state.Hp > 20)view.HpBar.color = new Color(175,245,108,255);  //green
        //else if(gameMode.state.Hp > 10) view.HpBar.color = new Color(245, 222, 108, 255);  //yellow
        //else view.HpBar.color = new Color(245, 125, 108, 255);  //red
        if (gameMode.state.Hp > 20) view.HpBar.color = Color.green;  //green
        else if (gameMode.state.Hp > 10) view.HpBar.color = Color.yellow;  //yellow
        else view.HpBar.color = Color.red;  //red
        view.HpValue.text = gameMode.state.Hp + "<color=blue>"+"+" + gameMode.state.TmpHp+"</color>";
    }

    public void UpdateScore()
    {
        float nowScore = gameMode.state.Score;
        if(nowScore>=1000)
        {
            int tenthValue = (int)nowScore / 100 - (int)(nowScore / 1000) * 10;
            view.ScoreValue.text = (int)(nowScore/1000) +"."+ tenthValue + "K";
        }
        else
        {
            view.ScoreValue.text = (int)nowScore + "";
        }
        view.Score.value = nowScore * 1.0f / gameMode.state.MaxScore;
        if(nowScore>gameMode.state.Target)
        {
            UpdateTarget();
        }
    }

    public void UpdateTili()
    {
        //view.TiliValue.text = (int)gameMode.state.Tili + "";
        int points = (int)gameMode.state.Tili;
        for(int i = 0; i < view.TiliPoints.Count; i++)
        {
            view.TiliPoints[i].color = Color.white;
        }
        Color fullColor = Color.white;

        ColorUtility.TryParseHtmlString("#40AB0D", out fullColor);
        for (int i = 0; i < points; i++)
        {
            view.TiliPoints[i].color = fullColor;
        }
        //for (int i = 0; i < points / 2; i++)
        //{
        //    view.TiliPoints[i].color = fullColor;
        //}
        //if (points % 2 == 1)
        //{
        //    view.TiliPoints[points/2].color = Color.green;
        //}
        //view.TiliImage.fillAmount = (view.TiliMaxFillAmount - view.TiliMinFillAmount) * nowTili / 100 + view.TiliMinFillAmount;

    }


    public void ShowGengEffect()
    {

    }

    public void UpdateTurnLeft(int turn)
    {
        view.TurnLeft.text = (int)turn + "";
    }



    public void ShowDamageAmountEffect(Vector3 pos, int value)
    {
        GameObject go = mResLoader.Instantiate("ZhiboMode2/DamageNumber", root);
        Text t = go.GetComponentInChildren<Text>();
        if (value >= 0)
        {
            t.text = "+" + value;
            t.color = Color.green;
        }
        else
        {
            t.text = value + "";
            t.color = Color.red;
        }

        go.transform.position = pos;
        float time = 1.5f;
        Vector3 TargetPos = pos + Vector3.up * time * 1f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                TargetPos,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("ZhiboMode2/DamageNumber", go);
            });

    }
    public void ShowAudienceHitEffect(Vector3 pos)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/AudienceEffect", root);
        Text t = go.GetComponentInChildren<Text>();
        go.transform.position = pos;
        //Vector3 TargetPos = pos + Vector3.up * time * 1f;
        DOTween.To
            (
                () => go.transform.localScale,
                (x) => go.transform.localScale = x,
                Vector3.one,
                1.5f
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/AudienceEffect", go);
            });

    }



    public void UpdateQifen()
    {
        view.QifenValue.text = gameMode.state.Qifen + "";
    }

    public override void BindView()
    {


        view.TVContainer = root.Find("Audience");
        view.TokenDetailPanel = root.Find("TokenDetail").gameObject;

        view.DanmuFieldSR = root.Find("DanmuField").GetComponent<ScrollRect>();
        view.DanmuField = (RectTransform)(view.DanmuFieldSR.content);
        //view.DanmuFieldNormal = (RectTransform)(view.DanmuField.Find("Normal"));
        //view.DanmuFieldSuper = (RectTransform)(view.DanmuField.Find("Super"));

        //view.SuperDanmuPreview = (RectTransform)(root.Find("SuperDanmuPreview"));
        //view.container = view.viewRoot.transform.Find("OperatorsContainer");

        Transform hotView = root.transform.Find("Score");

        view.Score = hotView.GetComponent<Slider>();

        Transform topField = root.Find("TopField");

        view.targetValue = topField.Find("Goal").Find("Goal_value").GetComponent<Text>();

        view.ScoreValue = topField.Find("NowScore").Find("NowScore_value").GetComponent<Text>();
        view.TurnLeft = topField.Find("TurnLeft").Find("LeftTurn_value").GetComponentInChildren<Text>();

        view.RoleSkill = root.Find("RoleSkill").GetComponent<Button>();

        Transform lbArea = root.Find("LeftBottom");

        view.QifenValue = lbArea.Find("QifenValue").GetComponent<Text>();
        view.NextTurnBtn = lbArea.Find("NextTurn").GetComponent<Button>();
        //view.TiliValue = lbArea.Find("Tili").GetComponent<Text>();

        view.HpValue = lbArea.Find("HpValue").GetComponent<Text>();

        view.TurnTimeBar = lbArea.Find("Timer").GetChild(0).GetComponent<Image>();

        view.TiliBar = lbArea.Find("TiliBar");
        foreach(Transform child in view.TiliBar)
        {
            view.TiliPoints.Add(child.GetComponent<Image>());
        }

        view.HpBar = lbArea.Find("EnegyBar").Find("Content").GetComponent<Image>();


        view.CardContainer = root.Find("CardsContainer").GetComponent<CardContainerLayout>();
        view.CardContainer.Init(gameMode);

        //view.hotAnimator = hotView.GetComponent<Animator>();

        view.BuffDetailPanel = root.Find("BuffDetail");
        view.BuffDetail = view.BuffDetailPanel.GetChild(0).GetComponent<Text>();

        view.SpeField = root.Find("SpeField") as RectTransform;
        view.BuffContainer = root.Find("BuffContainer") as RectTransform;

        view.Actions = root.Find("Actions").GetComponent<Image>();
        view.DeckLeft = view.Actions.transform.Find("Left").GetComponent<Text>();
    }

    public void UpdateDeckLeft()
    {
        view.DeckLeft.text = gameMode.state.CardDeck.Count + "";
    }

    //public void UpdateShieldView()
    //{
    //    if(gameMode.state.ScoreArmor <= 0)
    //    {

    //    }
    //    else
    //    {

    //    }
    //}


    public void ShowNewReqEffect(Vector3 from, Vector3 to)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Effect", root);
        go.transform.position = from;
        float time = 0.3f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                to,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/Effect", go);
            });
    }


    public void ShowDanmuEffect(Vector3 pos)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Effect",root);
        go.transform.position = pos;
        float time = (view.Score.transform.position - go.transform.position).magnitude / 10f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                view.Score.transform.position,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/Effect", go);
            });

    }

    public void ShowDamageEffect(Vector3 pos)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/DamageEffect", root);
        go.transform.position = pos;
        float time = (view.HpBar.transform.position - go.transform.position).magnitude / 10f;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                view.HpBar.transform.position,
                time
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/DamageEffect", go);
            });

    }


    public void ShowBuffDetail(ZhiboBuff buff)
    {

        view.BuffDetailPanel.transform.position = buff.gameObject.transform.position;
        view.BuffDetailPanel.gameObject.SetActive(true);
        string format = gameMode.GetBuffDesp(buff.bInfo.BuffType);
        view.BuffDetail.text = string.Format(format,10);
    }

    public void HideBuffDetail()
    {
        view.BuffDetailPanel.gameObject.SetActive(false);
    }

    public  override void RegisterEvent()
    {
        {
            ClickEventListerner listener = view.Actions.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.Actions.gameObject.AddComponent<ClickEventListerner>();
            }
            listener.ClearClickEvent();
            listener.OnClickEvent += delegate {
                Debug.Log(gameMode.state.CardDeck);
            };
        }

        view.NextTurnBtn.onClick.AddListener(delegate {

            NextTurn();
        });


        view.RoleSkill.onClick.AddListener(delegate
        {
            gameMode.UseRoleSkill();
        });

    }
    public void SKillBtnEnable(bool enable)
    {
        view.RoleSkill.interactable = enable;
    }

    private void NextTurn()
    {
        if (lockNextTurn)
        {
            return;
        }
        gameMode.NextTurnCaller();
    }

    public ZhiboSpecial GenSpecial(string sType)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Special",view.SpeField);
        if (go == null)
        {
            Debug.LogError("fail");
        }
        ZhiboSpecial ret = go.GetComponent<ZhiboSpecial>();
        ret.Init(sType,gameMode);
        ret.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-200,200), Random.Range(-200, 200));
        return ret;
    }



    private void AddClickFunc(GameObject target, DragEventListener.OnClickDlg func)
    {
        DragEventListener listener = target.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = target.AddComponent<DragEventListener>();
            listener.OnClickEvent += func;
        }
    }





    public override void Tick(float dTime)
    {
        dTime = dTime * gameMode.spdRate;

        if (view.CardContainer != null)
        {
            view.CardContainer.Tick(dTime);
        }
        if (lockNextTurn)
        {
            lockNextTurnTime -= dTime;
            if(lockNextTurnTime <= 0)
            {
                lockNextTurn = false;
            }
        }

        TimerPerSec += dTime;
        bool triggerSec = false;
        if(TimerPerSec > 1f)
        {
            TimerPerSec -= 1f;
            triggerSec = true;
        }


        //view.TurnTimeBar.fillAmount = gameMode.state.TurnTimeLeft / 30;
        //view.TurnTimeBar.fillAmount = (view.TurnTimeMaxFillAmount - view.TurnTimeMinFillAmount) * gameMode.state.TurnTimeLeft / 30 + view.TurnTimeMinFillAmount;

        if (Input.GetKeyDown(KeyCode.T))
        {
            //ShowNewAudience();
        }

    }

    public bool AddNewCard(CardInZhibo cardInfo)
    {
        return view.CardContainer.AddCard(cardInfo);
    }

    public bool AddNewTmpCard(CardInZhibo cardInfo)
    {
        return view.CardContainer.AddTmpCard(cardInfo);
    }

    public void ChangeTurnTime(float value)
    {
        //view.TurnTimeValue.text = (int)value+"";
        view.TurnTimeBar.fillAmount = value / 30;
    }







    public CardContainerLayout GetCardContainer()
    {
        return view.CardContainer;
    }




    public void ShowHitEffect(Vector2 posIn2D)
    {

    }

    public ZhiboBuff GenBuff()
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Buff", view.BuffContainer);
        if (go == null)
        {
            Debug.LogError("fail");
        }
        ZhiboBuff ret = go.GetComponent<ZhiboBuff>();
        return ret;
    }

    public Transform GetAudienceRoot()
    {
        return view.TVContainer;
    }
    public ScrollRect GetDanmuRoot()
    {
        return view.DanmuFieldSR;
    }

    public GameObject GetTokenDetailPanel()
    {
        return view.TokenDetailPanel;
    }

    public void UpdateTarget()
    {
        gameMode.updateTarget();
        float nowTarget = gameMode.state.Target;
        if(nowTarget>=1000)
        {
            int tenthValue = (int)nowTarget / 100 - (int)(nowTarget / 1000) * 10;
            view.targetValue.text = (int)(nowTarget / 1000) +"."+ tenthValue + "K";
        }
        else
        {
            view.targetValue.text = (int)(nowTarget) + "";
        }
        
    }

}
