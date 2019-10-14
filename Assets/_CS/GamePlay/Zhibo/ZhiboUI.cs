using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ZhiboView : BaseView
{

    public Text TimeLeft;

    public Transform container;
    public Image hotZhu;
    public Image hotHead;

    public Transform BuffDetailPanel;
    public Text BuffDetail;

    public Text hotValue;
    public Text TiliValue;
    public Image TiliImage;

    public Image Actions;
    public Animator hotAnimator;


    public Transform BuffContainer;

    public CardContainerLayout CardContainer;

    public Text ChoukaValue;
    public Image ChoukaImage;

    public float ChoukaMaxFillAmount = 0.35f;
    public float ChoukaMinFillAmount = 0.02f;
    public float TiliMaxFillAmount = 0.5f;
    public float TiliMinFillAmount = 0.2f;

    public RectTransform field;

    public RectTransform SpeField;
}
public class OperatorView
{
    public Image ActiveButton;
    public Image Outline;
}
public class ZhiboModel : BaseModel
{


}

public class ZhiboUI : UIBaseCtrl<ZhiboModel, ZhiboView>
{

    private int width = 0;
    private int height = 0;
    private int numOfGridVertical;
    private static float MinDanmuInterval = 0.5f;


    private int preDanmuGrid;
    private float[] preDanmuTime;

    private string DanmuFengxiang;


    IResLoader mResLoader;
    public ZhiboGameMode gameMode;

    public override void Init()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode;
    }

    public override void PostInit()
    {
        width = (int)view.field.rect.width;
        height = (int)view.field.rect.height;

        numOfGridVertical = 40;
        preDanmuGrid = -1;
        preDanmuTime = new float[numOfGridVertical];

        view.hotZhu.fillAmount = 0;
        view.TiliValue.text = "10";
        view.TiliImage.fillAmount = view.TiliMaxFillAmount;

        view.ChoukaValue.text = "0";
        view.ChoukaImage.fillAmount = view.ChoukaMinFillAmount;


        HideBuffDetail();
    }


    public void GainHotEffect(int num)
    {
        view.hotZhu.fillAmount += num * 1.0f / gameMode.state.MaxHot;
        if (num > 0)
        {
            view.hotAnimator.SetTrigger("Activate");
            view.hotValue.text = gameMode.state.Score + "";
            view.hotHead.rectTransform.anchoredPosition = new Vector2(0, 4 + Mathf.Min(gameMode.state.Score, gameMode.state.MaxHot));
        }
    }

    public void UpdateScore(float nowScore)
    {
        view.hotValue.text = (int)nowScore + "";
    }
    public void UpdateTili(float nowTili)
    {
        view.TiliValue.text = (int)nowTili + "";
        view.TiliImage.fillAmount = (view.TiliMaxFillAmount - view.TiliMinFillAmount) * nowTili / 100 + view.TiliMinFillAmount;

    }


    public void ShowGengEffect()
    {

    }

    public void UpdateTimeLeft(float time)
    {
        view.TimeLeft.text = (int)time + "";
    }

    public override void BindView()
    {
        view.TimeLeft = root.Find("TimeLeft").GetComponentInChildren<Text>();

        view.field = (RectTransform)(root.Find("DanmuField"));
        //view.container = view.viewRoot.transform.Find("OperatorsContainer");
        Transform hotView = root.transform.Find("Score");
        view.hotZhu = hotView.GetChild(0).GetComponent<Image>();
        view.hotValue = hotView.GetChild(2).GetComponent<Text>();
        view.hotHead = hotView.GetChild(1).GetComponent<Image>();

        Transform lbArea = root.Find("LeftBottom");


        view.TiliValue = lbArea.Find("Tili").GetComponent<Text>();
        view.ChoukaValue = lbArea.Find("Chouka").GetComponent<Text>();

        view.TiliImage = lbArea.Find("TiliBar").Find("Content").GetComponent<Image>();
        view.ChoukaImage = lbArea.Find("EnegyBar").Find("Content").GetComponent<Image>();


        view.CardContainer = root.Find("CardsContainer").GetComponent<CardContainerLayout>();
        view.CardContainer.Init(gameMode);

        view.hotAnimator = hotView.GetComponent<Animator>();

        view.BuffDetailPanel = root.Find("BuffDetail");
        view.BuffDetail = view.BuffDetailPanel.GetChild(0).GetComponent<Text>();

        view.SpeField = root.Find("SpeField") as RectTransform;
        view.BuffContainer = root.Find("BuffContainer") as RectTransform;

        view.Actions = root.Find("Actions").GetComponent<Image>();
    }

    public void ShowDanmuEffect(Vector3 pos)
    {
        GameObject go = mResLoader.Instantiate("Zhibo/Effect",root);
        go.transform.position = pos;
        DOTween.To
            (
                () => go.transform.position,
                (x) => go.transform.position = x,
                view.hotZhu.transform.position,
                1.5f
            ).OnComplete(delegate ()
            {
                mResLoader.ReleaseGO("Zhibo/Effect", go);
            });

    }


    public void ShowBuffDetail(ZhiboBuff buff)
    {

        view.BuffDetailPanel.transform.position = buff.gameObject.transform.position;
        view.BuffDetailPanel.gameObject.SetActive(true);
        string format = gameMode.GetBuffDesp(buff.buffId);
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
        if (view.CardContainer != null)
        {
            view.CardContainer.Tick(dTime);
        }
    }

    public bool AddNewCard(string cardId)
    {
        return view.CardContainer.AddCard(cardId);
    }


    public void ChangeChouka(float value)
    {
        view.ChoukaValue.text = (int)value+"";
        view.ChoukaImage.fillAmount = (view.ChoukaMaxFillAmount - view.ChoukaMinFillAmount) * value / 100 + view.ChoukaMinFillAmount;

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
        danmuGo.transform.SetParent(view.field,false);
        danmu.rect.anchoredPosition = new Vector3(width + 30, -posY, 0);
        danmu.view.textField.fontSize += Random.Range(0,6);

        return danmu;
    }



    public void HitDanmu(Danmu danmu)
    {
        if (danmu.left > 0)
        {
            danmu.left -= 1;
            if (!danmu.isBad)
            {
                gameMode.GainScore(danmu.isBig ? 3:1);
                danmu.view.textField.color = Color.gray;
                danmu.view.textField.raycastTarget = false;
                ShowDanmuEffect(danmu.transform.position);
            }
            else
            {


                if (danmu.left <= 0)
                {
                    danmu.OnDestroy();
                    gameMode.state.Danmus.Remove(danmu);
                    //gameMode.GainScore(10);
                }
            }

        }
    }

    public CardContainerLayout GetCardContainer()
    {
        return view.CardContainer;
    }


}
