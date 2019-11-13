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

    public Transform TVContainer;
    public List<ZhiboLittleTV> LittleTvList = new List<ZhiboLittleTV>();

    public Transform container;
    public Slider Score;

    public Text QifenValue;
    public Transform BuffDetailPanel;
    public Text BuffDetail;

    public Text hotValue;
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

    public float TurnTimeMaxFillAmount = 0.35f;
    public float TurnTimeMinFillAmount = 0.02f;
    public float TiliMaxFillAmount = 0.5f;
    public float TiliMinFillAmount = 0.2f;

    public RectTransform DanmuField;
    public RectTransform DanmuFieldNormal;
    public RectTransform DanmuFieldSuper;
    public RectTransform SuperDanmuPreview;

    public RectTransform SpeField;

}
public class OperatorView
{
    public Image ActiveButton;
    public Image Outline;
}
public class ZhiboModel : BaseModel
{

    public List<int> EmptyTVList = new List<int>();
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

    public SuperDanmu[] SuperDanmuSlots = new SuperDanmu[8];
    public List<int> EmptySuperDanmuSlot = new List<int>();

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
        view.HpBar.fillAmount = gameMode.state.Hp / 100;
        view.HpValue.text = gameMode.state.Hp + "<color=blue>"+"+" + gameMode.state.TmpHp+"</color>";
    }

    public void UpdateScore()
    {
        float nowScore = gameMode.state.Score;
        view.hotValue.text = (int)nowScore + "";
        view.Score.value = nowScore * 1.0f / gameMode.state.MaxScore;
    }

    public void UpdateTili()
    {
        //view.TiliValue.text = (int)gameMode.state.Tili + "";
        int points = (int)gameMode.state.Tili;
        for(int i = 0; i < 5; i++)
        {
            view.TiliPoints[i].color = Color.white;
        }
        Color fullColor = Color.white;
        ColorUtility.TryParseHtmlString("#40AB0D", out fullColor);
        for (int i = 0; i < points / 2; i++)
        {
            view.TiliPoints[i].color = fullColor;
        }
        if (points % 2 == 1)
        {
            view.TiliPoints[points/2].color = Color.green;
        }
        //view.TiliImage.fillAmount = (view.TiliMaxFillAmount - view.TiliMinFillAmount) * nowTili / 100 + view.TiliMinFillAmount;

    }


    public void ShowGengEffect()
    {

    }

    public void UpdateTurnLeft(int turn)
    {
        view.TurnLeft.text = (int)turn + "";
    }

    public void ShowNewAudience()
    {
        if (model.EmptyTVList.Count == 0)
        {
            return;
        }
        int idx = Random.Range(0, model.EmptyTVList.Count);

        idx = model.EmptyTVList[idx];

        model.EmptyTVList.Remove(idx);

        view.LittleTvList[idx].Show();
    }

    public void AttractAudience()
    {
        int idx = Random.Range(0, view.LittleTvList.Count);
        while (true)
        {
            if (view.LittleTvList[idx].IsEmpty == false)
            {
                break;
            }
            idx = (idx + 1) % view.LittleTvList.Count;
        }

        view.LittleTvList[idx].Attract();
    }

    public void UpdateQifen()
    {
        view.QifenValue.text = gameMode.state.Qifen + "";
    }

    public override void BindView()
    {
        view.TurnLeft = root.Find("TurnLeft").GetComponentInChildren<Text>();


        view.TVContainer = root.Find("Audience");
        foreach (Transform tv in view.TVContainer)
        {
            ZhiboLittleTV vv = tv.GetComponent<ZhiboLittleTV>();
            vv.Init(tv, this);
            view.LittleTvList.Add(vv);
        }
        for (int i = 0; i < view.LittleTvList.Count; i++)
        {
            model.EmptyTVList.Add(i);
        }

        view.DanmuField = (RectTransform)(root.Find("DanmuField"));
        view.DanmuFieldNormal = (RectTransform)(view.DanmuField.Find("Normal"));
        view.DanmuFieldSuper = (RectTransform)(view.DanmuField.Find("Super"));

        view.SuperDanmuPreview = (RectTransform)(root.Find("SuperDanmuPreview"));
        //view.container = view.viewRoot.transform.Find("OperatorsContainer");
        Transform hotView = root.transform.Find("Score");
        view.Score = hotView.GetComponent<Slider>();
        view.hotValue = hotView.Find("Value").GetComponent<Text>();

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

        view.hotAnimator = hotView.GetComponent<Animator>();

        view.BuffDetailPanel = root.Find("BuffDetail");
        view.BuffDetail = view.BuffDetailPanel.GetChild(0).GetComponent<Text>();

        view.SpeField = root.Find("SpeField") as RectTransform;
        view.BuffContainer = root.Find("BuffContainer") as RectTransform;

        view.Actions = root.Find("Actions").GetComponent<Image>();
        view.DeckLeft = view.Actions.transform.GetChild(0).GetComponent<Text>();
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

        if (triggerSec)
        {
            for (int i = view.LittleTvList.Count - 1; i >= 0; i--)
            {
                view.LittleTvList[i].TickSec();
                if (view.LittleTvList[i].TimeLeft <= 0)
                {
                    if (!model.EmptyTVList.Contains(i))
                    {
                        model.EmptyTVList.Add(i);
                    }
                }
            }
            //view.TurnTimeValue.text = (int)gameMode.state.TurnTimeLeft+"";
        }
        view.TurnTimeBar.fillAmount = gameMode.state.TurnTimeLeft / 30;
        //view.TurnTimeBar.fillAmount = (view.TurnTimeMaxFillAmount - view.TurnTimeMinFillAmount) * gameMode.state.TurnTimeLeft / 30 + view.TurnTimeMinFillAmount;

        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowNewAudience();
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

    public Danmu GenDanmu(bool isBad)
    {
        List<int> DanmuSlots = new List<int>();


        for(int i=2;i< numOfGridVertical - 2; i++)
        {
            if (Time.time - preDanmuTime[i] > MinDanmuInterval)
            {
                DanmuSlots.Add(i);
            }
        }
        int gridY;

        if (DanmuSlots.Count == 0)
        {
            gridY = Random.Range(2, numOfGridVertical - 2);

        }
        else
        {
            gridY = DanmuSlots[Random.Range(0, DanmuSlots.Count)];
        }



        preDanmuGrid = gridY;

        float posY = gridY * 1.0f / numOfGridVertical * height;
        //posY += Random.Range(-3f, 3f);

        preDanmuTime[gridY] = Time.time;

        GameObject danmuGo = mResLoader.Instantiate("Zhibo/Danmu");

        Danmu danmu = danmuGo.GetComponent<Danmu>();


        danmu.init(gameMode.getRandomDanmu(), isBad,gameMode);
        danmuGo.transform.SetParent(view.DanmuFieldNormal, false);
        danmu.rect.anchoredPosition = new Vector3(width + 30, -posY, 0);


        return danmu;
    }





    public CardContainerLayout GetCardContainer()
    {
        return view.CardContainer;
    }


    public SuperDanmu ShowSuperDanmu()
    {
        if(EmptySuperDanmuSlot.Count == 0)
        {
            return null;
        }
        GameObject danmuGo = mResLoader.Instantiate("Zhibo/SuperDanmu",view.SuperDanmuPreview);
        SuperDanmu danmu = danmuGo.GetComponent<SuperDanmu>();
        //
        int randSlot = EmptySuperDanmuSlot[Random.Range(0, EmptySuperDanmuSlot.Count)];
        EmptySuperDanmuSlot.Remove(randSlot);

        SuperDanmuSlots[randSlot] = danmu;
        danmu.transform.localPosition = new Vector3(0, randSlot*-80f,0);
        danmu.init("maybe sssss asd",eSuperDanmuType.Jianpanxia,gameMode);
        return danmu;
    }

    public void ClearSuperDanmu()
    {
        EmptySuperDanmuSlot.Clear();
        for(int i = 0; i < 8; i++)
        {
            EmptySuperDanmuSlot.Add(i);
        }
        for(int i=0;i< SuperDanmuSlots.Length; i++)
        {
            SuperDanmuSlots[i] = null;
        }
    }


    public void MoveSuperDanmu(SuperDanmu toMove)
    {
        toMove.transform.SetParent(view.DanmuFieldSuper, true);
    }

    public void AdjustSuperDanmuOrder()
    {
        for (int i = 0; i < SuperDanmuSlots.Length; i++)
        {
            if(SuperDanmuSlots[i] != null)
            {
                SuperDanmuSlots[i].transform.SetSiblingIndex(i);
            }
        }
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

    
}
