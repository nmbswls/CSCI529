using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WeiboView : BaseView
{
    public Image GetGeng;
    public ScrollRect ScrollView;
    public Transform Content;

    public Image Back;

    public Image TouXiang;
    public Image WeiboImage;
    public Text Name;
    public Text Time;
    public Text Description;

    public Image Post;
}

public class WeiboModel : BaseModel
{
    
}

public class WeiboUI : UIBaseCtrl<WeiboModel, WeiboView>
{
    IUIMgr pUIMgr;
    IResLoader mResLoader;

    ICardDeckModule pCardMdl;
    const string prefix = "card";
    const int shuaTimeLimit = 3;     //每回合只有3次刷到牌的机会
    int curShuaTime = 0;

    float originalY;
    float diffY;

    bool isGengGet = false;

    string cardName;

    public List<string> nameArr = new List<string>
    {
        {"老王" },
        {"老刘" },
        {"老铁" },
        {"老张" }
    };

    public List<string> DescriArr = new List<string>
    {
        {"来今儿个给大家搞个二斤地瓜烧" },
        {"当朝大学士，总共有五位，朕不得不罢免三位" },
        {"转发这条锦鲤，也没什么卵用" },
        {"卧槽" }
    };

    public override void Init()
    {
        pCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pUIMgr = GameMain.GetInstance().GetModule<IUIMgr>();

        if (pCardMdl == null)
        {
            Debug.Log("getCardFailed!");
        }
    }

    public void getRandomCard()
    {
        int randInt = UnityEngine.Random.Range(1,3);
        //string cardName = prefix + randInt.ToString().PadLeft(4,'0');
        cardName = "card800" + randInt.ToString();
        Debug.Log(cardName);
    }

    public void insertCard(string cardName)
    {
        List<string> st = new List<string>();
        st.Add(cardName);
        pCardMdl.AddCards(st);
    }

    public override void BindView()
    {
        view.ScrollView = root.Find("ScrollView").GetComponent<ScrollRect>();
        view.Content = view.ScrollView.transform.Find("Viewport").Find("Content");
        view.GetGeng = view.Content.Find("GetGengPanel").GetComponent<Image>();
        Debug.Log(view.GetGeng.transform.position);

        view.Back = root.Find("Back").GetComponent<Image>();

        view.Name = view.GetGeng.transform.Find("Name").GetComponent<Text>();
        view.Time = view.GetGeng.transform.Find("Time").GetComponent<Text>();
        view.Description = view.GetGeng.transform.Find("Description").GetComponent<Text>();

        view.TouXiang = view.GetGeng.transform.Find("TouXiang").GetComponent<Image>();
        view.WeiboImage = view.GetGeng.transform.Find("WeiboImage").GetComponent<Image>();
        view.Post = view.GetGeng.transform.Find("Post").GetComponent<Image>();
    }

    public override void RegisterEvent()
    {
        {
            DragEventListener listener = view.GetGeng.gameObject.GetComponent<DragEventListener>();
            if (listener == null)
            {
                listener = view.GetGeng.gameObject.AddComponent<DragEventListener>();
            }
            listener.ClearClickEvent();
            listener.ClearDragEvent();

            listener.OnBeginDragEvent += delegate
            {
                originalY = view.GetGeng.transform.position.y;
                diffY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - originalY;
            };
            listener.OnDragEvent += delegate
            {
                Vector3 vec = new Vector3();
                Vector3 scrVec = new Vector3();
                scrVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                vec = view.GetGeng.transform.position;
                vec.y = scrVec.y - diffY;
                view.GetGeng.transform.position = vec;
            };
            listener.OnEndDragEvent += delegate
            {
                
                Vector3 vec = new Vector3();
                vec = view.GetGeng.transform.position;
                //vec.y = originalY;
                //view.GetGeng.transform.position = vec;
                Tween tween = DOTween.To(
                        () => view.GetGeng.rectTransform.anchoredPosition,
                        (x) => view.GetGeng.rectTransform.anchoredPosition = x,
                        new Vector2(0, originalY - vec.y),
                        0.3f
                    );
                if(curShuaTime<shuaTimeLimit)
                {
                    randomWeibo();
                    getRandomCard();
                    isGengGet = false;
                } else
                {
                    Debug.Log("啊，没什么瓜可以吃的，之后再来吧");
                }
            };
        }
        {
            ClickEventListerner listener = view.Back.gameObject.GetComponent<ClickEventListerner>();
            if(listener == null)
            {
                listener = view.Back.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                mUIMgr.CloseCertainPanel(this);
            };
        }
        {
            ClickEventListerner listener = view.Post.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.Post.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if(!isGengGet)
                {
                    isGengGet = true;
                    curShuaTime++;
                    insertCard(cardName);
                }
            };
        }
    }

    public void randomWeibo()
    {
        randomTime();
        randomName();
        randomDescription();
    }

    public void randomTime()
    {
        int rn = UnityEngine.Random.Range(0,80);
        string timeMessage = rn + " 分钟前";
        view.Time.text = timeMessage;
    }

    public void randomName()
    {
        int rn = UnityEngine.Random.Range(0, 3);
        string nameMessage = nameArr[rn];
        view.Name.text = nameMessage;
    }

    public void randomDescription()
    {
        int rn = UnityEngine.Random.Range(0, 3);
        string descriptionMessage = DescriArr[rn];
        view.Description.text = descriptionMessage;
    }

    public void resetShua()
    {
        curShuaTime = 0;
    }


}
