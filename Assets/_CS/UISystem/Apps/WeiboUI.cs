using System;
using System.Collections;
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
    IResLoader pResLoader;
    IRoleModule pRoleMgr;
    WeiboModule pWeiboMgr;

    ICardDeckModule pCardMdl;

    const string prefix = "card";

    float originalY;
    float diffY;

    bool isGengGet = false;
    bool isValidDrag = false;

    string cardName;

    public override void Init()
    {
        pCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pUIMgr = GameMain.GetInstance().GetModule<IUIMgr>();
        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        pWeiboMgr = GameMain.GetInstance().GetModule<WeiboModule>();

    }
    public override void PostInit()
    {
        randomWeibo();
    }

    public void getRandomCard()
    {
        int randInt = UnityEngine.Random.Range(1,5);
        //string cardName = prefix + randInt.ToString().PadLeft(4,'0');
        cardName = "card800" + randInt.ToString();
        Debug.Log(cardName);
    }

    public void insertCard(string cardName)
    {
        if(cardName != null)
        {
            List<string> st = new List<string>();
            st.Add(cardName);
            pCardMdl.AddCards(st);
            mUIMgr.ShowHint("获得卡牌" + cardName);
        }
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
                if(scrVec.y - diffY <= originalY && originalY + diffY - scrVec.y <= 2)
                {
                    vec.y = scrVec.y - diffY;
                    view.GetGeng.transform.position = vec;
                    if(originalY + diffY - scrVec.y >= 1.3)
                    {
                        isValidDrag = true;
                    }   
                }
            };
            listener.OnEndDragEvent += delegate
            {
                Vector3 vec = new Vector3();
                vec = view.GetGeng.transform.position;
                Tween tween = DOTween.To(
                                () => view.GetGeng.rectTransform.anchoredPosition,
                                (x) => view.GetGeng.rectTransform.anchoredPosition = x,
                                new Vector2(0, originalY - vec.y),
                                0.3f
                            );
                Debug.Log(isValidDrag);
                if (isValidDrag)
                {
                    if (pWeiboMgr.IsShuable)
                    {
                        randomWeibo();
                        getRandomCard();
                        isGengGet = false;
                    }
                    else
                    {
                        mUIMgr.ShowHint("啊，没什么瓜可以吃的，之后再来吧");
                        pWeiboMgr.disableRealRandom();
                    }
                }
                isValidDrag = false;
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
                    pWeiboMgr.ReduceShuaTime();
                    insertCard(cardName);
                    
                }
            };
        }
    }

    public void randomWeibo()
    {
        view.Time.text = randomTime();
        view.Name.text = randomName();
        view.Description.text = randomDescription();
    }

    public string randomTime()
    {
        return pWeiboMgr.randomTime();
    }

    public string randomName()
    {
        return pWeiboMgr.randomName();
    }

    public string randomDescription()
    {
        return pWeiboMgr.randomDescription();
    }

}
