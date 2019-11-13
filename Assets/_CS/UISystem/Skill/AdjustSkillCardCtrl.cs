using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AdjustSKillCardView : BaseView {
    public Transform CardsContainer;
    public Button CloseBtn;
    public Text CardNum;
    public List<CardOutView> AllCards = new List<CardOutView>();

    public Text SkillName;
    public Toggle TotalOn;
}

public class AdjustSKillCardModel : BaseModel
{
    public SkillInfo skillInfo;
    public List<CardInfo> infos;
}

public class AdjustSkillCardCtrl : UIBaseCtrl<AdjustSKillCardModel, AdjustSKillCardView>
{
    ICardDeckModule pCardMgr;
    ISkillTreeMgr pSKillMgr;
    IResLoader pResLoader;

    public override void Init()
    {
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        pSKillMgr = GameMain.GetInstance().GetModule<SkillTreeMgr>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
    }

    public override void BindView()
    {
        if (root == null)
        {
            Debug.Log("bind fail no root found");
        }

        view.CardsContainer = root.Find("CardScroallView").GetChild(0).GetChild(0);
        view.CardNum = root.Find("MinNum").Find("Text").GetComponent<Text>();
        view.CloseBtn = root.Find("Close").GetComponent<Button>();

        view.SkillName = root.Find("SkillName").GetComponent<Text>();

        view.TotalOn = root.Find("TotalOn").GetComponent<Toggle>();

        //view.DetailPanel = root.Find("DetailPanel");

        //view.DetailDesp = view.DetailPanel.Find("DetailDesp").GetComponent<Text>();
        //view.DetailName = view.DetailPanel.Find("DetailName").GetComponent<Text>();
        //view.DetailEffectDesp = view.DetailPanel.Find("DetailEffectDesp").GetComponent<Text>();
        //view.DisableBtn = view.DetailPanel.Find("DisableBtn").GetComponent<Toggle>();
        //view.DisableHint = view.DetailPanel.Find("DisableHint").GetComponent<Text>();
    }

    public override void RegisterEvent()
    {
        view.CloseBtn.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });

        view.TotalOn.onValueChanged.AddListener(delegate(bool v) {
            model.skillInfo.isOn = v;
            Debug.Log(model.skillInfo.isOn);
        });
    }

    public override void PostInit()
    {
        //ShowCards();
    }
    public void SetContent(string skillId)
    {
        if(skillId == null || skillId == string.Empty)
        {
            return;
        }
        SkillInfo info = pSKillMgr.GetOwnedSkill(skillId);
        if (info == null)
        {
            return;
        }
        model.skillInfo = info;
        BaseSkillAsset bsa = info.sa as BaseSkillAsset;
        if(bsa == null)
        {
            return;
        }
        List<CardInfo> gooo = pCardMgr.GetSkillCards(skillId);
        model.infos = gooo;

        for(int i=0;i< gooo.Count; i++)
        {
            GameObject go = pResLoader.Instantiate("UI/CardOut", view.CardsContainer);
            CardOutView cardOutView = new CardOutView();
            cardOutView.BindView(go.transform);
            view.AllCards.Add(cardOutView);
            cardOutView.Hint.gameObject.SetActive(false);

            CardInfo cardInfo = gooo[i];

            {
                DragEventListener listener = cardOutView.CardFace.gameObject.GetComponent<DragEventListener>();
                if (listener == null)
                {
                    listener = cardOutView.CardFace.gameObject.AddComponent<DragEventListener>();
                }

                listener.ClearClickEvent();
                listener.ClearPointerEvent();
                listener.OnClickEvent += delegate {
                    ChangeEnable(cardOutView);
                };
                listener.PointerEnterEvent += delegate {
                    ShowPopupInfo(cardInfo);
                    //idx
                };
                listener.PointerExitEvent += delegate {
                    HidePopupInfo();
                };
            }

            CardAsset ca = cardInfo.ca;
            cardOutView.Name.text = ca.CardName;
            cardOutView.Desp.text = ca.CardEffectDesp;
            cardOutView.DaGou.SetActive(!cardInfo.isDisabled);
            cardOutView.Picture.sprite = ca.Picture;
        }

        UpdateUsedNumver();
        view.SkillName.text = model.skillInfo.sa.SkillName;
        view.TotalOn.isOn = model.skillInfo.isOn;
    }

    public void UpdateEnable(CardOutView vv, bool isOn)
    {
        vv.DaGou.SetActive(isOn);


        UpdateUsedNumver();


    }

    public void UpdateUsedNumver()
    {

        List<CardInfo> infoList = pCardMgr.GetSkillCards(model.skillInfo.SkillId);
        int nowEnabled = 0;
        for (int i = 0; i < infoList.Count; i++)
        {
            if (!infoList[i].isDisabled)
            {
                nowEnabled++;
            }
        }
        int minNum = (model.skillInfo.sa as BaseSkillAsset).BaseCardList.Count - 1;
        view.CardNum.text = nowEnabled + "/" + minNum;
    }

    public void ChangeEnable(CardOutView vv)
    {
        int idx = view.AllCards.IndexOf(vv);
        if(idx == -1)
        {
            return;
        }
        //model.infos[idx];
        bool ret = pCardMgr.ChangeEnable(model.infos[idx].InstId, model.infos[idx].isDisabled);
        if (ret)
        {
            UpdateEnable(vv, !model.infos[idx].isDisabled);
        }
    }

    public void ShowPopupInfo(CardInfo info)
    {

    }

    public void HidePopupInfo()
    {

    }
}
