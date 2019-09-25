using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShootDanmuView
{
    public Transform viewRoot;
    public Transform container;
    public Image hotZhu;
    public Image hotHead;
    public Text hotValue;

    public Image Special;
    public Animator hotAnimator;

    public List<OperatorView> operators;

    public CardContainerLayout cardContainer;

    public Text Xianchan;

}

public class OperatorView
{
    public Image ActiveButton;
    public Image Outline;
}



public enum ENM_HitMode
{
    ZAN=0,
    THANK=1,
    DELETE=2
}
public class ShootDanmuMiniGame : MiniGame
{
    public ShootDanmuView view = new ShootDanmuView();

    public int hot = 0;
    public int maxHot = 100;

    [HideInInspector]
    public int xianchangzhi = 0;


    public ENM_HitMode mode = ENM_HitMode.ZAN;

    public List<Danmu> danmus = new List<Danmu>();

    public GameObject cardPrefab;
    public GameObject danmuPrefab;

    public GameObjectPool pool = new GameObjectPool();

    List<MiniCardInfo> cards = new List<MiniCardInfo>();

    public RectTransform field;
    private int width = 0;
    private int height = 0;
    private int numOfGridVertical = 20;


    private int preDanmuGrid;
    private int[] preDanmuIdx;

    public float lastTick = 0;
    public float nextTick = 0;

    private int bigOneNext = 3;
    private int bigOneCount = 0; 

    public override void Init()
    {
        width = (int)field.rect.width;
        height = (int)field.rect.height;

        numOfGridVertical = 20;
        //preDanmuDis = new int[numOfGridVertical];
        preDanmuGrid = -1;

        InitOperators();

        BindView();
        RegisterEvent();

        hot = 0;
        view.hotZhu.fillAmount = 0;

        cards.Clear();
        mode = ENM_HitMode.ZAN;
        //view.operators[0].Outline.gameObject.SetActive(true);
        xianchangzhi = 0;


    }


    private void AddNewCard()
    {

        view.cardContainer.AddCard();

    }

    private void BindView()
    {
        view.viewRoot = GameObject.Find("MiniGamePanel").transform;
        //view.container = view.viewRoot.transform.Find("OperatorsContainer");
        Transform hotView = view.viewRoot.transform.Find("Score");
        view.hotZhu = hotView.GetChild(0).GetComponent<Image>();
        view.hotValue = hotView.GetChild(2).GetComponent<Text>();
        view.hotHead = hotView.GetChild(1).GetComponent<Image>();

        view.cardContainer = view.viewRoot.transform.Find("OperatorsContainer").GetComponent<CardContainerLayout>();

        view.hotAnimator = hotView.GetComponent<Animator>();
        view.operators = new List<OperatorView>();

        view.Xianchan = view.viewRoot.Find("Xianchang").GetComponent<Text>();
        //foreach(Transform child in view.container)
        //{
        //    OperatorView ov = new OperatorView();
        //    ov.ActiveButton = child.GetChild(1).GetComponent<Image>();
        //    ov.Outline = child.GetChild(0).GetComponent<Image>();
        //    view.operators.Add(ov);
        //}
    }
    private void RegisterEvent()
    {

        foreach(OperatorView ov in view.operators)
        {
            PointEventListener listener = ov.ActiveButton.gameObject.GetComponent<PointEventListener>();
            if (listener == null)
            {
                Debug.Log("cnm");
                listener = ov.ActiveButton.gameObject.AddComponent<PointEventListener>();
                listener.OnClickEvent += delegate (PointerEventData eventData) {
                    //view.
                    Debug.Log("c");
                    switchOperator(view.operators.IndexOf(ov));
                };
            }
        }
        //{
        //    AddClickFunc(view.Special.gameObject, delegate (PointerEventData eventData) {
        //        Debug.Log("use");
        //        view.Special.GetComponent<Animator>().SetTrigger("Disappear");
        //        useSpecial();
        //    }); 
        //}

    }

    private void AddClickFunc(GameObject target, PointEventListener.OnClickDlg func)
    {
        PointEventListener listener = target.GetComponent<PointEventListener>();
        if (listener == null)
        {
            listener = target.AddComponent<PointEventListener>();
            listener.OnClickEvent += func;
        }
    }

    public void switchOperator(int newMode)
    {
        if ((int)mode == newMode)
        {
            return;
        }
        mode = (ENM_HitMode)newMode;
        foreach (OperatorView ov in view.operators)
        {
            ov.Outline.gameObject.SetActive(false);
        }
        view.operators[newMode].Outline.gameObject.SetActive(true);
    }

    private void AutoDisappear(Danmu danmu)
    {
        recycleDanmu(danmu);
        danmus.Remove(danmu);

        xianchangzhi += 3;
        view.Xianchan.text = xianchangzhi+"";

    }

    public override void SomeTick(float dTime)
    {
        for(int i = danmus.Count - 1; i >= 0; i--)
        {
            danmus[i].Tick(dTime);
            if (danmus[i].NeedDestroy)
            {
                AutoDisappear(danmus[i]);

            }
        }

        lastTick += dTime;
        if (lastTick > nextTick)
        {
            genDanmu();
            lastTick = 0;
            nextTick = Random.Range(0.1f,0.3f);
            //Debug.Log("gen");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddNewCard();
        }

        if (xianchangzhi > 100)
        {

            xianchangzhi = 0;
            view.Xianchan.text = xianchangzhi + "";
            AddNewCard();
        }
    }

    public void genDanmu()
    {



        bool success = false;
        int gridY = Random.Range(2, numOfGridVertical - 2);
        while(gridY == preDanmuGrid)
        {
            gridY = Random.Range(2, numOfGridVertical - 2);
        }
        preDanmuGrid = gridY;
        float posY = gridY * 1.0f / numOfGridVertical * height;
        posY += Random.Range(-3f, 3f);

        GameObject danmuGo = pool.Spawn(danmuPrefab,Vector3.zero,Quaternion.identity,out success);
        Danmu danmu = danmuGo.GetComponent<Danmu>();


        danmu.init(getRandomDanmu());
        danmuGo.transform.SetParent(field);
        danmu.rect.anchoredPosition = new Vector3(width + 30, -posY, 0);
        danmu.view.textField.fontSize = Random.Range(22,28);

        bigOneCount++;
        if (bigOneCount > bigOneNext)
        {
            danmu.view.textField.fontSize = 38;
            bigOneCount = 0;
            bigOneNext = Random.Range(4,8);
        }

        danmus.Add(danmu);
    }

    private string getRandomDanmu()
    {
        return "你麻痹死了";
    }

    public void HitDanmu(Danmu danmu)
    {
        danmu.left -= 1;
        if (danmu.left <= 0)
        {
            //GameObject.Destroy(gameObject);
            danmu.OnDestroy();
            //StartCoroutine();
            //recycleDanmu(danmu);
            danmus.Remove(danmu);
            gainHot(10);
        }

    }

    public void gainHot(int v)
    {
        hot += v;
        view.hotZhu.fillAmount += v * 1.0f / maxHot;
        if (v > 0)
        {
            view.hotAnimator.SetTrigger("Activate");
            view.hotValue.text = hot+"";
            view.hotHead.rectTransform.anchoredPosition = new Vector2(0,4+Mathf.Min(hot,maxHot));
        }
    }

    public void recycleDanmu(Danmu danmu)
    {
        pool.Release(danmuPrefab, danmu.gameObject);
    }

    public void InitOperators()
    {

    }

    public void useSpecial()
    {
        List<Danmu> toClean = randomPick(10);
        foreach(Danmu danmu in toClean)
        {
            danmu.OnDestroy();
            danmus.Remove(danmu);
            gainHot(10);
        }
    }

    private List<Danmu> randomPick(int n)
    {
        if (danmus.Count <= n)
        {
            return new List<Danmu>(danmus);
        }
        List<Danmu> ret = new List<Danmu>();
        List<int> choosed = new List<int>();
        int nowC = 0;
        while (nowC < n)
        {
            int randIdx = Random.Range(0, danmus.Count);
            if (!choosed.Contains(randIdx))
            {
                choosed.Add(randIdx);
                nowC++;
            }
        }
        foreach(int idx in choosed)
        {
            ret.Add(danmus[idx]);
        }
        return ret;
    }

}
