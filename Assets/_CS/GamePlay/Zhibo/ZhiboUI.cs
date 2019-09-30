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

    public CardContainerLayout cardContainer;

    public Text Xianchan;

    public RectTransform field;
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

        gameMode.Cards.Clear();
    }


    public void GainHotEffect(int num)
    {
        view.hotZhu.fillAmount += num * 1.0f / gameMode.maxHot;
        if (num > 0)
        {
            view.hotAnimator.SetTrigger("Activate");
            view.hotValue.text = gameMode.hot + "";
            view.hotHead.rectTransform.anchoredPosition = new Vector2(0, 4 + Mathf.Min(gameMode.hot, gameMode.maxHot));
        }
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

        view.cardContainer = root.Find("CardsContainer").GetComponent<CardContainerLayout>();
        view.SpeObjContainer = root.Find("SpecialObjContainer");

        view.hotAnimator = hotView.GetComponent<Animator>();

        view.Xianchan = root.Find("Xianchang").GetComponent<Text>();
    }
    public  override void RegisterEvent()
    {


       

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






       
    }

    public void AddNewCard()
    {
        view.cardContainer.AddCard();
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
            gameMode.danmus.Remove(danmu);
            gameMode.gainHot(10);
        }
    }



   










}
