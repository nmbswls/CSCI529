using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ZhiboView : BaseView
{

    public Transform container;
    public Image hotZhu;
    public Image hotHead;
    public Text hotValue;

    public Image Special;
    public Animator hotAnimator;

    public Transform SpeObjContainer;

    public Transform BuffContainer;

    public CardContainerLayout CardContainer;

    public Text Xianchan;

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
    private int numOfGridVertical = 20;


    private int preDanmuGrid;
    private int[] preDanmuIdx;



    IResLoader mResLoader;

    public ZhiboGameMode gameMode;

    public override void Init()
    {
        view = new ZhiboView();
        model = new ZhiboModel();

        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        mUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        gameMode = GameMain.GetInstance().GetModule<CoreManager>().GetGameMode() as ZhiboGameMode;
    }

    public override void PostInit()
    {
        width = (int)view.field.rect.width;
        height = (int)view.field.rect.height;

        numOfGridVertical = 20;
        preDanmuGrid = -1;

        view.hotZhu.fillAmount = 0;

        gameMode.state.Cards.Clear();
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

    public void UpdateScore(int nowScore)
    {
        view.hotValue.text = nowScore + "";
    }


    public void ShowGengEffect()
    {

    }


    public override void BindView()
    {
        view.field = (RectTransform)(root.Find("DanmuField"));
        //view.container = view.viewRoot.transform.Find("OperatorsContainer");
        Transform hotView = root.transform.Find("Score");
        view.hotZhu = hotView.GetChild(0).GetComponent<Image>();
        view.hotValue = hotView.GetChild(2).GetComponent<Text>();
        view.hotHead = hotView.GetChild(1).GetComponent<Image>();

        view.CardContainer = root.Find("CardsContainer").GetComponent<CardContainerLayout>();
        view.CardContainer.Init(gameMode);
        view.SpeObjContainer = root.Find("SpecialObjContainer");

        view.hotAnimator = hotView.GetComponent<Animator>();

        view.Xianchan = root.Find("Xianchang").GetComponent<Text>();

        view.SpeField = root.Find("SpeField") as RectTransform;
        view.BuffContainer = root.Find("BuffContainer") as RectTransform;
    }


    public  override void RegisterEvent()
    {

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
        ret.Init("b",1,10f, gameMode);
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


    public void ChangeXianChange(string txt)
    {
        view.Xianchan.text = txt;
    }

    public Danmu GenDanmu()
    {
        int gridY = Random.Range(2, numOfGridVertical - 2);
        while (gridY == preDanmuGrid)
        {
            gridY = Random.Range(2, numOfGridVertical - 2);
        }
        preDanmuGrid = gridY;
        float posY = gridY * 1.0f / numOfGridVertical * height;
        posY += Random.Range(-3f, 3f);

        GameObject danmuGo = mResLoader.Instantiate("Zhibo/Danmu");

        Danmu danmu = danmuGo.GetComponent<Danmu>();


        danmu.init(gameMode.getRandomDanmu(),gameMode);
        danmuGo.transform.SetParent(view.field);
        danmu.rect.anchoredPosition = new Vector3(width + 30, -posY, 0);
        danmu.view.textField.fontSize = Random.Range(22, 28);

        return danmu;
    }



    public void HitDanmu(Danmu danmu)
    {
        danmu.left -= 1;
        if (danmu.left <= 0)
        {
            danmu.OnDestroy();
            gameMode.state.Danmus.Remove(danmu);
            gameMode.GainHot(10);
        }
    }


}
