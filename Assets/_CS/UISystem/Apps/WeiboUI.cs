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

    public Image ForwardButton;
    public Image ReviewButton;
    public Image Reviews;
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
    Weibo curWeibo;

    int curWeiboIdx = -1;
    int lastWeiboIdx = -1;

    ICardDeckModule pCardMdl;

    const string prefix = "card";

    float originalY;
    float diffY;
    
    bool isCardGot = false;
    bool isBranchSelected = false;
    bool isValidDrag = false;

    public override void Init()
    {
        pCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        pUIMgr = GameMain.GetInstance().GetModule<UIMgr>();
        pRoleMgr = GameMain.GetInstance().GetModule<RoleModule>();
        pWeiboMgr = GameMain.GetInstance().GetModule<WeiboModule>();
    }
    public override void PostInit()
    {
        randomWeibo();
    }

    //public void getRandomCard()
    //{
    //    int randInt = UnityEngine.Random.Range(1,5);
    //    cardName = "card800" + randInt.ToString();
    //}

    public void insertCard(string cardName)
    {
        if(cardName != null)
        {
            List<string> st = new List<string>();
            st.Add(cardName);
            pCardMdl.AddCards(st);
            CardAsset ca = pCardMdl.Load(cardName);
            if(ca != null)
            {
                mUIMgr.ShowHint("获得卡牌:" + ca.CardName);
            }
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
        view.ForwardButton = view.GetGeng.transform.Find("Forward").GetComponent<Image>();
        view.ReviewButton = view.GetGeng.transform.Find("Review").GetComponent<Image>();

        view.Reviews = view.GetGeng.transform.Find("Reviews").GetComponent<Image>();

        BindButtons();

        
    }

    public void BindButtons()
    {
        if (isNotNullWeibo())
        {
            if (curWeibo.reviewable)
            {
                view.ReviewButton.gameObject.SetActive(true);
            }
            else
            {
                view.ReviewButton.gameObject.SetActive(false);
            }
            if (curWeibo.forwardable)
            {
                view.ForwardButton.gameObject.SetActive(true);
            }
            else
            {
                view.ForwardButton.gameObject.SetActive(false);
            }
        }
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
                        isCardGot = false;
                    }
                    else
                    {
                        mUIMgr.ShowHint("啊，没什么瓜可以吃的，之后再来吧");
                        //TODO random切换
                        //pWeiboMgr.disableRealRandom();
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
        registerBranchEvent();
    }

    public void registerBranchEvent()
    {
        {
            ClickEventListerner listener = view.ForwardButton.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.ForwardButton.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if (!isCardGot)
                {
                    Debug.Log("Press the forward");
                    isCardGot = true;
                    mUIMgr.ShowHint("转发完毕");
                    pWeiboMgr.ReduceShuaTime();
                    string cardName = curWeibo.gainCardId;
                    if(cardName.Length>0)insertCard(cardName);
                }
            };
        }
        {
            ClickEventListerner listener = view.ReviewButton.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.ReviewButton.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if (!isCardGot)
                {
                    Debug.Log("Press the review");
                    view.Reviews.gameObject.SetActive(true);
                    int validReviews = curWeibo.reviews.Count; //2
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < validReviews)
                        {
                            view.Reviews.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = curWeibo.reviews[i].content;
                            view.Reviews.transform.GetChild(i).gameObject.SetActive(true);
                            ClickEventListerner Optionlistener = view.Reviews.transform.GetChild(i).GetComponent<ClickEventListerner>();
                            if (Optionlistener == null)
                            {
                                Optionlistener = view.Reviews.transform.GetChild(i).gameObject.AddComponent<ClickEventListerner>();
                            }

                            int tmp = i;//防止异步更新i值
                            Optionlistener.OnClickEvent += delegate
                            {
                                if (!isCardGot)
                                {
                                    Debug.Log("idx = " + tmp);
                                    handleReviewEffect(tmp);
                                    isCardGot = true;
                                    pWeiboMgr.ReduceShuaTime();
                                }
                            };
                        }
                        else
                        {
                            view.Reviews.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }
            };
        }
    }

    public void randomWeibo()
    {
        //view.Time.text = randomTime();
        //view.Name.text = randomName();
        //view.Description.text = randomDescription();
        int count = pWeiboMgr.weiboList.weibos.Count;
        
        while(curWeiboIdx == lastWeiboIdx)
        {
            curWeiboIdx = UnityEngine.Random.Range(0, count);
        }
        lastWeiboIdx = curWeiboIdx;
        
        curWeibo = pWeiboMgr.weiboList.weibos[curWeiboIdx];
        loadWeibo();
        BindButtons();
        registerBranchEvent();
    }

    public void loadWeibo()
    {
        view.Time.text = randomTime();
        view.Name.text = curWeibo.name;
        view.Description.text = curWeibo.content;
    }

    public string randomTime()
    {
        return pWeiboMgr.randomTime();
    }

    public void handleReviewEffect(int idx)
    {
        
        int value = curWeibo.reviews[idx].value;
        switch(curWeibo.reviews[idx].effect)
        {
            case WeiboReviewEffect.AddCaiyi:
                pRoleMgr.AddFanying(value);
                mUIMgr.ShowHint("才艺 + " + value);
                break;
            case WeiboReviewEffect.AddJishu:
                pRoleMgr.AddJiyi(value);
                mUIMgr.ShowHint("技术 + " + value);
                break;
            case WeiboReviewEffect.AddKangya:
                pRoleMgr.AddTili(value);
                mUIMgr.ShowHint("抗压 + " + value);
                break;
            case WeiboReviewEffect.AddWaiguan:
                pRoleMgr.AddMeili(value);
                mUIMgr.ShowHint("外观 + " + value);
                break;
            case WeiboReviewEffect.AddKoucai:
                pRoleMgr.AddKoucai(value);
                mUIMgr.ShowHint("口才 + " + value);
                Debug.Log("ADDKOUCAI");
                break;
            case WeiboReviewEffect.AddAllState:
                pRoleMgr.AddKoucai(value);
                pRoleMgr.AddMeili(value);
                pRoleMgr.AddTili(value);
                pRoleMgr.AddJiyi(value);
                pRoleMgr.AddFanying(value);
                mUIMgr.ShowHint("所有属性 + " + value);
                break;
            case WeiboReviewEffect.AddFensi:
                //TODO Addfensi
                //pRoleMgr.AddFensi(value);
                break;
            case WeiboReviewEffect.none:
                break;
        }
        UIMainCtrl mainui = (UIMainCtrl)pUIMgr.GetCtrl("UIMain") as UIMainCtrl;
        mainui.UpdateWords();
    }

    //public string randomName()
    //{
    //    return pWeiboMgr.randomName();
    //}

    //public string randomDescription()
    //{
    //    return pWeiboMgr.randomDescription();
    //}

    public bool isNotNullWeibo()
    {
        return curWeibo != null;
    }

    
}
