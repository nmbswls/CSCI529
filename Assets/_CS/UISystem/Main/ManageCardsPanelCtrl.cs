using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManageCardsModel : BaseModel
{
    public List<CardInfo> NowCardInfos = new List<CardInfo>();
    public int nowTab = -1;
}

public class ManageCardsView : BaseView
{
	public TabGroup tabGroup;
	public Dropdown filter01;

    public ScrollRect CardsSR;
	public Transform CardsContainer;

    public List<CardOutView> CardsViewList = new List<CardOutView>();

    public Button Close;

    public Transform DetailPanel;
    public Text DetailDesp;
    public Text DetailName;
    public Text DetailEffectDesp;

    public Toggle DisableBtn;
    public Text DisableHint;

}

public class CardOutView
{

    public RectTransform root;
    public RectTransform CardFace;
    public CanvasGroup CardCG;
    public Image Bg;
    public Image Picture;
    public Text Desp;
    public Text Name;
    public Text TurnLeft;

    public Image Hint;
    public GameObject DaGou;

    public void BindView(Transform transform)
    {
        root = transform as RectTransform;
        CardFace = transform.Find("CardFace") as RectTransform;
        CardCG = CardFace.GetComponent<CanvasGroup>();
        Bg = CardFace.GetComponent<Image>();
        Picture = CardFace.Find("Picture").GetComponent<Image>();
        Name = CardFace.Find("Name").GetComponent<Text>();
        Desp = CardFace.Find("Desp").GetComponent<Text>();

        DaGou = root.Find("DaGou").gameObject;


        Hint = transform.Find("Hint").GetComponent<Image>();
    }

    public void ChangeEnable(bool enabled)
    {
        if (enabled)
        {
            DaGou.SetActive(true);
        }
        else
        {
            DaGou.SetActive(false);
        }
    }

}
public class CardsTabView : TabGroupChildView{

	public Text Title;
	public Image BG;
	public override void BindView ()
	{
		base.BindView ();
		Title = root.GetChild (0).GetComponent<Text>();
		BG = root.GetComponent<Image> ();
	}
}

public class ManageCardsPanelCtrl : UIBaseCtrl<ManageCardsModel, ManageCardsView>
{

    ICardDeckModule pCardMgr;
    IResLoader pResLoader;

    CardOutView preCardView;


    public override void Init(){
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
    }

	public override void BindView(){
		if (root == null) {
			Debug.Log ("bind fail no root found");
		}

		view.tabGroup = root.Find("LeftTag").GetComponent<TabGroup>();
        view.CardsSR = root.Find("CardScroallView").GetComponent<ScrollRect>();
        view.CardsContainer = view.CardsSR.content.transform;
		view.filter01 = root.Find("FilterBar").GetChild (0).GetComponent<Dropdown> ();
        view.Close = root.Find("Close").GetComponent<Button>();

        view.DetailPanel = root.Find("DetailPanel");

        view.DetailDesp = view.DetailPanel.Find("DetailDesp").GetComponent<Text>();
        view.DetailName = view.DetailPanel.Find("DetailName").GetComponent<Text>();
        view.DetailEffectDesp = view.DetailPanel.Find("DetailEffectDesp").GetComponent<Text>();
        view.DisableBtn = view.DetailPanel.Find("DisableBtn").GetComponent<Toggle>();
        view.DisableHint = view.DetailPanel.Find("DisableHint").GetComponent<Text>();
    }


    public override void PostInit()
    {
        //ShowCards();
        SwitchChoose(0);
    }



    public override void RegisterEvent(){
		view.tabGroup.InitTab (typeof(CardsTabView));
		view.tabGroup.OnValueChangeEvent += SwitchChoose;

		view.filter01.onValueChanged.AddListener (delegate(int arg0) {
			Debug.Log(arg0);	
		});

        view.Close.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });



        view.DisableBtn.onValueChanged.AddListener(delegate(bool v)
        {

            int idx = view.CardsViewList.IndexOf(preCardView);
            CardInfo cinfo = model.NowCardInfos[idx];
            bool ret = pCardMgr.ChangeEnable(cinfo.InstId,v);
            if (ret)
            {
                preCardView.ChangeEnable(!cinfo.isDisabled);
            }
            view.DisableBtn.isOn = !cinfo.isDisabled;
        });

    }


    private void ShowCards(eCardType type)
    {
        List<CardInfo> infos = pCardMgr.GetTypeCards(type);
        UpdateCards(infos);
    }

    private void ShowCardsAll()
    {
        List<CardInfo> infos = pCardMgr.GetAllCards();
        UpdateCards(infos);
    }


    private void UpdateCards(List<CardInfo> infos)
    {
        foreach (CardOutView vv in view.CardsViewList)
        {
            pResLoader.ReleaseGO("UI/CardOut", vv.root.gameObject);
        }
        view.CardsViewList.Clear();
        preCardView = null;
        model.NowCardInfos = infos;
        foreach (CardInfo c in infos)
        {
            GameObject go = pResLoader.Instantiate("UI/CardOut", view.CardsContainer);
            CardOutView cardOutView = new CardOutView();
            cardOutView.BindView(go.transform);
            view.CardsViewList.Add(cardOutView);
            cardOutView.Hint.gameObject.SetActive(false);

            {
                ClickEventListerner listener = cardOutView.CardFace.gameObject.GetComponent<ClickEventListerner>();
                if (listener == null)
                {
                    listener = cardOutView.CardFace.gameObject.AddComponent<ClickEventListerner>();
                }

                listener.ClearClickEvent();
                listener.OnClickEvent += delegate {
                    ShowCardDetail(cardOutView);
                };
            }

            CardAsset ca = pCardMgr.GetCardInfo(c.CardId);
            cardOutView.Name.text = ca.CardName;
            cardOutView.Desp.text = ca.CardEffectDesp;
            cardOutView.DaGou.SetActive(!c.isDisabled);
            if (ca.CatdImageName == null || ca.CatdImageName == string.Empty)
            {
                cardOutView.Picture.sprite = ca.Picture;
            }
            else
            {
                cardOutView.Picture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardImage/" + ca.CatdImageName);
            }
            //Debug.Log(ca.Picture.name);
        }
    }

    public void HideCardDetail()
    {
        view.DetailPanel.gameObject.SetActive(false);
    }

    public void ShowCardDetail(CardOutView vv)
    {
        int idx = view.CardsViewList.IndexOf(vv);
        if(idx == -1)
        {
            return;
        }

        CardInfo info = model.NowCardInfos[idx];
        CardAsset ca = pCardMgr.GetCardInfo(info.CardId);

        view.DetailDesp.text = ca.CardDesp;
        view.DetailName.text = ca.CardName;
        view.DetailEffectDesp.text = ca.CardEffectDesp;

        if(ca.CardType == eCardType.ITEM)
        {
            view.DisableHint.gameObject.SetActive(false);
            view.DisableBtn.interactable = true;
        }
        else
        {
            view.DisableHint.gameObject.SetActive(true);
            view.DisableBtn.interactable = false;
        }
        view.DisableBtn.isOn = !info.isDisabled;

        if (preCardView != null)
        {
            preCardView.Hint.gameObject.SetActive(false);
        }
        view.DetailPanel.gameObject.SetActive(true);
        vv.Hint.gameObject.SetActive(true);
        preCardView = vv;
    }

    public void SwitchChoose(int newTab){
		
		if(newTab == -1 || model.nowTab == newTab)
        {
            return;
        }


        model.nowTab = newTab;


        for (int i = 0; i < view.tabGroup.tabs.Count; i++) {
			CardsTabView childView = view.tabGroup.tabs[i] as CardsTabView;
			childView.BG.color = Color.white;
		}
		{
			CardsTabView childView = view.tabGroup.tabs [newTab] as CardsTabView;
			childView.BG.color = Color.red;
		}

        if (newTab == 0)
        {
            ShowCardsAll();
        }else if (newTab == 1)
        {
            ShowCards(eCardType.ABILITY);
        }
        else if (newTab == 2)
        {
            ShowCards(eCardType.ITEM);
        }
        else if (newTab == 3)
        {
            ShowCards(eCardType.ITEM);
        }
        HideCardDetail();

    }
}


