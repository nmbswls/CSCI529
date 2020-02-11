using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MailView : BaseView
{
    public ScrollRect ScrollView;
    public Transform Content;

    public Image Back;

    public Transform MailDesView;
    public Transform SimpleView;
    public Transform FunctionView;

    public Text MailContent;

    public Image MailBack;
    public Image MailDelete;
    public Image MailGetReward;
}

public class MailModel : BaseModel
{

}

public class MailUI : UIBaseCtrl<MailModel, MailView>
{
    IUIMgr pUIMgr;
    IResLoader pResLoader;
    IRoleModule pRoleMgr;
    MailModule pMailMgr;

    ICardDeckModule pCardMdl;
    Mail curMail;

    Dictionary<Mail, Transform> mailToTransform = new Dictionary<Mail, Transform>();

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
        pMailMgr = GameMain.GetInstance().GetModule<MailModule>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        //load emails;
        pMailMgr.checkMailListLoaded();
    }

    public override void BindView()
    {
        view.ScrollView = root.Find("ScrollView").GetComponent<ScrollRect>();
        view.Content = view.ScrollView.transform.Find("Viewport").Find("Content");

        view.Back = root.Find("Back").GetComponent<Image>();
        view.MailDesView = root.Find("MailDesView");
        view.SimpleView = view.MailDesView.transform.Find("simpleView");

        view.FunctionView = view.MailDesView.transform.Find("FunctionView");

        view.MailContent = view.MailDesView.Find("Description").GetComponent<Text>();

        
        view.MailBack = view.FunctionView.GetChild(0).GetComponent<Image>();
        view.MailDelete = view.FunctionView.GetChild(1).GetComponent<Image>();
        view.MailGetReward = view.FunctionView.GetChild(2).GetComponent<Image>();

        //bind load email;

        reloadMailView();
        view.FunctionView.gameObject.SetActive(true);
    }

    public void reloadMailView()
    {
        for (int i = pMailMgr.mailList.mailBox.Count-1; i>=0; i--)
            //mail 从上往下更新，最后入列的是最新的mail
        {
            int index = pMailMgr.mailList.mailBox.Count-1 - i;
            Mail tmpMail = pMailMgr.mailList.mailBox[i];
            GameObject go = pResLoader.Instantiate("UI/UIPanels/Mail", view.Content);
            Transform simpleMail = view.Content.GetChild(index).GetChild(0);
            
            if (tmpMail.avatar.Length > 0)
            {
                //find avatar;
            }
            simpleMail.GetChild(1).GetComponent<Text>().text = tmpMail.title;
            simpleMail.GetChild(2).GetComponent<Text>().text = tmpMail.fromPeople;
            if(tmpMail.isRead == true)
            {
                simpleMail.GetChild(3).gameObject.SetActive(true);
            }
            //Debug.Log(simpleMail.GetChild(1).GetComponent<Text>().text);
            //Debug.Log(simpleMail.GetChild(2).GetComponent<Text>().text);

            mailToTransform.Add(tmpMail, simpleMail);
        }
    }

    public void reRegisterMailEvent()
    {
        for (int i = 0; i < pMailMgr.mailList.mailBox.Count; i++)
        {
            
            Transform child = view.Content.GetChild(i);
            ClickEventListerner listener = child.GetChild(0).gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = child.GetChild(0).gameObject.AddComponent<ClickEventListerner>();
                Debug.Log("register event i = " + i);
            }
            Debug.Log(child.GetChild(0).gameObject.name);
            int index = pMailMgr.mailList.mailBox.Count - 1 - i;
            Mail tmpMail = pMailMgr.mailList.mailBox[index];
            listener.OnClickEvent += delegate
            {
                curMail = tmpMail;
                view.SimpleView = child.transform;
                view.MailContent.text = curMail.content;
                if (curMail.numOfBonus > 0)
                {
                    view.MailGetReward.gameObject.SetActive(true);
                }
                else
                {
                    view.MailGetReward.gameObject.SetActive(false);
                }
                view.FunctionView.gameObject.SetActive(true);
                view.MailDesView.gameObject.SetActive(true);
                setMailReaded(curMail);
            };
        }
    }

    public override void RegisterEvent()
    {
        reRegisterMailEvent();
        {
            ClickEventListerner listener = view.Back.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.Back.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                mUIMgr.CloseCertainPanel(this);
            };
        }
        {
            ClickEventListerner listener = view.MailBack.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.MailBack.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if (curMail != null)
                {
                    view.MailDesView.gameObject.SetActive(false);
                    curMail = null;
                }
            };
        }
        {
            ClickEventListerner listener = view.MailDelete.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.MailDelete.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if (curMail != null)
                {
                    DeleteEmail(curMail);
                    Debug.Log("rest mails = " + pMailMgr.mailList.mailBox.Count);
                    view.MailDesView.gameObject.SetActive(false);
                    curMail = null;
                }
            };
        }
        {
            ClickEventListerner listener = view.MailGetReward.gameObject.GetComponent<ClickEventListerner>();
            if (listener == null)
            {
                listener = view.MailGetReward.gameObject.AddComponent<ClickEventListerner>();
            }

            listener.OnClickEvent += delegate
            {
                if (curMail != null)
                {
                    //curMail.isGetReward = true;
                    view.MailGetReward.gameObject.SetActive(false);
                }
            };
        }
    }

    public void DeleteEmail(Mail email)
    {
        pMailMgr.deleteMail(curMail);
        GameObject.Destroy(mailToTransform[email].parent.gameObject);
    }

    public void setMailReaded(Mail email)
    {
        email.isRead = true;
        mailToTransform[email].GetChild(3).gameObject.SetActive(true);
    }

}
