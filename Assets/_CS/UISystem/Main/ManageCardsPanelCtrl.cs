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
    public Image NamePicture;

    public Text TurnLeft;

    public Image Cover;
    public Image TypePicture;

    public Text Cost;

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

        NamePicture = Name.transform.GetChild(0).GetComponent<Image>();
        Cover = CardFace.Find("Cover").GetComponent<Image>();
        TypePicture = CardFace.Find("CardType").GetComponent<Image>();

        Cost = CardFace.Find("Cost").GetComponent<Text>();

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
    public Image TitlePic;
	public override void BindView ()
	{
		base.BindView ();
		Title = root.GetChild (0).GetComponent<Text>();
        TitlePic = root.GetChild(1).GetComponent<Image>();
        BG = root.GetComponent<Image> ();
	}
}

public class ManageCardsPanelCtrl : UIBaseCtrl<ManageCardsModel, ManageCardsView>
{

    ICardDeckModule pCardMgr;
    IResLoader pResLoader;

    CardOutView preCardView;

    List<Sprite> titlePicSprite = new List<Sprite>();

    public static string[] CostColor = { "#eaff2d", "#21acc5", "#2ddfff" };

    public override void Init(){
        pCardMgr = GameMain.GetInstance().GetModule<CardDeckModule>();
        pResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        loadSprite();
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
            cardOutView.Cost.text = ca.cost + "";

            cardOutView.NamePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardName/" + ca.CatdImageName);

            switch (ca.CardType)
            {

                case eCardType.GENG:
                    cardOutView.Cover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Geng");
                    cardOutView.Bg.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Geng");
                    cardOutView.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Geng");
                    Color nowColor1 = Color.white;
                    ColorUtility.TryParseHtmlString(CostColor[2], out nowColor1);  //color follow the type
                    cardOutView.Cost.color = nowColor1;
                    break;
                case eCardType.ABILITY:
                    cardOutView.Cover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Ability");
                    cardOutView.Bg.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Ability");
                    cardOutView.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Ability");
                    Color nowColor2 = Color.white;
                    ColorUtility.TryParseHtmlString(CostColor[1], out nowColor2);  //color follow the type
                    cardOutView.Cost.color = nowColor2;
                    break;
                case eCardType.ITEM:
                    cardOutView.Cover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Item");
                    cardOutView.Bg.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Item");
                    cardOutView.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Item");
                    Color nowColor3 = Color.white;
                    ColorUtility.TryParseHtmlString(CostColor[0], out nowColor3);  //color follow the type
                    cardOutView.Cost.color = nowColor3;
                    break;
            }


            cardOutView.DaGou.SetActive(!c.isDisabled);
            if (ca.CatdImageName == null || ca.CatdImageName == string.Empty)
            {
                cardOutView.Picture.sprite = ca.Picture;
            }
            else
            {
                cardOutView.Picture.sprite = pResLoader.LoadResource<Sprite>("CardImage/" + ca.CatdImageName);
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

        //TODO: 修改tag text显示颜色
        for (int i = 0; i < view.tabGroup.tabs.Count; i++) {
			CardsTabView childView = view.tabGroup.tabs[i] as CardsTabView;
            //childView.BG.color = Color.white;
            childView.BG.transform.localScale = Vector3.one;
            childView.TitlePic.sprite = titlePicSprite[i * 2];
		}
		{
			CardsTabView childView = view.tabGroup.tabs [newTab] as CardsTabView;
            childView.BG.transform.localScale = Vector3.Scale(childView.BG.transform.localScale, new Vector3(1.1f, 1.1f, 1.1f));
            childView.TitlePic.sprite = titlePicSprite[newTab * 2 + 1];
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

    public void loadSprite()
    {
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "a_1"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "a_2"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "s_1"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "s_2"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "i_1"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "i_2"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "a_3"));
        titlePicSprite.Add(pResLoader.LoadResource<Sprite>("CardDeck/CardDeckTagBg/" + "a_4"));
    }
}


