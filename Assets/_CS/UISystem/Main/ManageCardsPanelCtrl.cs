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
    public Text DetailDesp;
    public Text DetailName;
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

    public void BindView(Transform transform)
    {
        root = transform as RectTransform;
        CardFace = transform.Find("CardFace") as RectTransform;
        CardCG = CardFace.GetComponent<CanvasGroup>();
        Bg = CardFace.GetComponent<Image>();
        Picture = CardFace.Find("Picture").GetComponent<Image>();
        Name = CardFace.Find("Name").GetComponent<Text>();
        Desp = CardFace.Find("Desp").GetComponent<Text>();

        Hint = transform.Find("Hint").GetComponent<Image>();
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


        view.DetailDesp = root.Find("DetailPanel").Find("DetailDesp").GetComponent<Text>();
        view.DetailName = root.Find("DetailPanel").Find("DetailName").GetComponent<Text>();

    }


    public override void PostInit()
    {
        //ShowCards();
    }



    public override void RegisterEvent(){
		view.tabGroup.InitTab (typeof(CardsTabView));
		view.tabGroup.OnValueChangeEvent += SwitchChoose;
		view.tabGroup.switchTab (0);

		view.filter01.onValueChanged.AddListener (delegate(int arg0) {
			Debug.Log(arg0);	
		});

        view.Close.onClick.AddListener(delegate ()
        {
            mUIMgr.CloseCertainPanel(this);
        });


    }

    private void ShowCards()
    {
        List<CardInfo> infos = pCardMgr.GetAllCards();


        foreach(CardOutView vv in view.CardsViewList)
        {
            pResLoader.ReleaseGO("UI/Card",vv.root.gameObject);
        }
        view.CardsViewList.Clear();
        preCardView = null;
        model.NowCardInfos = infos;
        foreach (CardInfo c in infos)
        {
            GameObject go = pResLoader.Instantiate("UI/Card", view.CardsContainer);
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

        }

    }

    public void ShowCardDetail(CardOutView vv)
    {
        Debug.Log(vv);
        int idx = view.CardsViewList.IndexOf(vv);
        if(idx == -1)
        {
            return;
        }

        CardInfo info = model.NowCardInfos[idx];
        CardAsset ca = pCardMgr.GetCardInfo(info.CardId);

        view.DetailDesp.text = ca.CardDesp;
        view.DetailName.text = ca.CardName;

        if(preCardView != null)
        {
            preCardView.Hint.gameObject.SetActive(false);
        }
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
            ShowCards();
        }

    }
}


