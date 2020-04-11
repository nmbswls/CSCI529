using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class MiniCardView
{

    public RectTransform root;

    public RectTransform CardRoot;
    public RectTransform RotateArea;
    public RectTransform CardFace;
    public RectTransform CardBack;

    public CanvasGroup CardCG;
    public Image Bg;

    public Image Background;
    public Image BackBackground;

    public Image Picture;
    public Image BackPicture;
    public Image PictureCover;
    public Image BackPictureCover;

    public Text Desp;
    public Text BackDesp;
    public Text Name;
    public Text BackName;
    public Image NamePicture;
    public Image BackNamePicture;

    public Image TypePicture;
    public Image BackTypePicture;

    public Transform TimeLeftComp;
    public Text TimeLeft;
    public Animator ClockAnimator;
    public Text Cost;

    public Transform GemsIncludeContainer;
    public List<CardGemSingleIconView> CardGemSingleIconList = new List<CardGemSingleIconView>();

    public Transform GemContainer;
    public List<CardGemView> CardGemList = new List<CardGemView>();

    public Transform GemBackContainer;
    public List<CardGemBackView> CardGemBackList = new List<CardGemBackView>();
    public Text BackCost;
}

public class CardGemBackView
{
    public Text Num;
    public Image Icon;
    public Transform Content;

    public void BindView(Transform root)
    {
        Content = root.Find("Content");
        Icon = Content.Find("Icon").GetComponent<Image>();
        Num = Content.Find("Text").GetComponent<Text>();
    }
}
public class CardGemView
{
    public Text Num;
    public Image Icon;

    public void BindView(Transform root)
    {
        Icon = root.Find("Icon").GetComponent<Image>();
        Num = root.Find("Text").GetComponent<Text>();
    }
}
public class CardGemSingleIconView
{
    public Image Icon;

    public void BindView(Transform root)
    {
        Debug.Log(root.name);
        Icon = root.Find("Icon").GetComponent<Image>();
    }
}

public class MiniCard : MonoBehaviour
{

    MiniCardView view = new MiniCardView();
    CardContainerLayout container;

    public static string[] CostColor = { "#eaff2d", "#21acc5", "#2ddfff" };

    public float nowDegree;
    public float targetDegree;

    public bool isTmp;
    public Vector2 TargetPos;
    public Vector2 NowPos;

    private float moveSpeed = 1500;
    private float returnSpeed = 1800;


    public bool PosDirty = false;
    public bool isBacking = false;
    public bool isDestroying = false;
    public bool isHighlight = false;
    public bool isDragging = false;


    public bool isInChain = false;



    public RectTransform rt;
    public Animator anim;

    public float nowValue;

    private static Vector3 MinimizeScale = new Vector3(0.7f, 0.7f, 0.7f);
    private static Vector3 NormalScale = new Vector3(1f, 1f, 1f);

    private static float NormalYOffset = 200f;
    private static float DragScaleRate = 0.3f;

    private static float NowValueDecRate = 500f;
    //上拉多少触发卡片
    private static float MaxValue = 120f;
    private static float TriggerValue = 100f;

    float preY = 0;

    public bool isFaceUp = true;

    private bool IsFanmian = false;

    private Vector2 CardRootPosition = Vector2.zero;

    public void Init(CardInZhibo cardInfo, CardContainerLayout container)
    {
        rt = (RectTransform)transform;
        anim = GetComponent<Animator>();
        this.container = container;

        CardAsset ca = cardInfo.ca;
        this.isTmp = cardInfo.isTmp;

        BindView();
        RegisterEvent();
        view.TimeLeftComp.gameObject.SetActive(false);
        if(!cardInfo.ca.IsConsume)
        {
            view.TimeLeftComp.gameObject.SetActive(false);
        }
        else
        {
            view.TimeLeftComp.gameObject.SetActive(true);
            view.TimeLeft.text = (int)cardInfo.UseLeft + "";
            view.TimeLeft.color = Color.black;
        }

        anim.ResetTrigger("Disappear");
        anim.Play("Normal");

        //初始化卡面
        view.Name.text = ca.CardName;
        view.BackName.text = view.Name.text;

        view.NamePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardName/" + ca.CatdImageName);
        view.BackNamePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardName/" + ca.CatdImageName);

        view.Desp.text = ca.CardEffectDesp;
        view.BackDesp.text = ca.CardBackDesp;
        if (ca.cost == -1)
        {
            view.Cost.text = "X";
            view.BackCost.text = "1";
        }
        else
        {
            view.Cost.text = ca.cost + "";
            view.BackCost.text = ca.cost + "";
        }

        if (ca.CatdImageName == null || ca.CatdImageName == string.Empty)
        {
            view.Picture.sprite = ca.Picture;
            view.BackPicture.sprite = ca.Picture;
        }
        else
        {
            view.Picture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardImage/" + ca.CatdImageName);
            view.BackPicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardImage/" + ca.CatdImageName);
        }

        //调整card外壳
        if(ca.CardType == eCardType.GENG)
        {
            view.PictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Geng");
            view.BackPictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Geng");
            view.Background.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Geng");
            view.BackBackground.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Geng");
            view.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Geng");
            view.BackTypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Geng");
            Color nowColor = Color.white;
            ColorUtility.TryParseHtmlString(CostColor[2], out nowColor);  //color follow the type
            view.Cost.color = nowColor;
            view.BackCost.color = nowColor;
        }
        else if (ca.CardType == eCardType.ABILITY)
        {
            view.PictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Ability");
            view.BackPictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Ability");
            view.Background.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Ability");
            view.BackBackground.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Ability");
            view.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Ability");
            view.BackTypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Ability");
            Color nowColor = Color.white;
            ColorUtility.TryParseHtmlString(CostColor[1], out nowColor);  //color follow the type
            view.Cost.color = nowColor;
            view.BackCost.color = nowColor;
        }
        else if (ca.CardType == eCardType.ITEM)
        {
            view.PictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Item");
            view.BackPictureCover.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardCover/Item");
            view.Background.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Item");
            view.BackBackground.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardBackground/Item");
            view.TypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Item");
            view.BackTypePicture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardType/Item");
            Color nowColor = Color.white;
            ColorUtility.TryParseHtmlString(CostColor[0], out nowColor);  //color follow the type
            view.Cost.color = nowColor;
            view.BackCost.color = nowColor;
        }


            foreach (Transform child in view.GemContainer)
        {
            container.mResLoader.ReleaseGO("Zhibo/CardGem",child.gameObject);
        }

        view.CardGemList.Clear();

        for (int i = 0; i < cardInfo.OverrideGems.Length; i++)
        {
            if (cardInfo.OverrideGems[i] > 0)
            {
                GameObject go = container.mResLoader.Instantiate("Zhibo/CardGem",view.GemContainer);
                CardGemView vv = new CardGemView();
                vv.BindView(go.transform);
                view.CardGemList.Add(vv);
                //vv.Icon.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("Zhibo/Gems/" + i);
                vv.Icon.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("Zhibo/Gems_icon/" + i);    //icon skin
                vv.Num.text = cardInfo.OverrideGems[i] + "";
            }
        }


        foreach (Transform child in view.GemBackContainer)
        {
            container.mResLoader.ReleaseGO("Zhibo/CardGemBack", child.gameObject);
        }

        view.CardGemBackList.Clear();
        int types = 0;

        for (int i = 0; i < cardInfo.OverrideGems.Length; i++)
        {
            if (cardInfo.OverrideGems[i] > 0)
            {

                GameObject go = container.mResLoader.Instantiate("Zhibo/CardGemBack", view.GemBackContainer);
                CardGemBackView vv = new CardGemBackView();
                vv.BindView(go.transform);
                view.CardGemBackList.Add(vv);
                //vv.Icon.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("Zhibo/Gems/" + i);
                vv.Icon.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("Zhibo/Gems_icon/" + i);    //icon skin
                vv.Num.text = cardInfo.OverrideGems[i] + "";
                types++;

            }
        }
        int offset = 0;
        if (types > 1)
        {
            offset = 100 / (types - 1);
        }
        for (int i=0;i< view.CardGemBackList.Count; i++)
        {
            //view.CardGemBackList[i].Content.localPosition = new Vector3(i * offset, 0, 0);
        }

        foreach (Transform child in view.GemsIncludeContainer)
        {
            container.mResLoader.ReleaseGO("Zhibo/CardGemSingleIcon", child.gameObject);
        }

        view.CardGemSingleIconList.Clear();
        for (int i = 0; i < cardInfo.OverrideGems.Length; i++)
        {
            if (cardInfo.OverrideGems[i] > 0)
            {

                GameObject go = container.mResLoader.Instantiate("Zhibo/CardGemSingleIcon", view.GemsIncludeContainer);
                CardGemSingleIconView vv = new CardGemSingleIconView();
                vv.BindView(go.transform);
                view.CardGemSingleIconList.Add(vv);
                vv.Icon.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("Zhibo/Gems_icon/" + i);    //icon skin
                types++;

            }
        }

        nowDegree = 20f;
        targetDegree = 20f;


        container.PutToInitPos(this);
        isBacking = false;
        isDragging = false;
        isDestroying = false;
        isHighlight = false;
        PosDirty = false;
        isInChain = false;

        view.CardCG.alpha = 1f;

        nowValue = 0;
        view.CardRoot.anchoredPosition = Vector2.zero;
        view.CardRoot.localScale = MinimizeScale;

        view.CardCG.blocksRaycasts = true;
        CancelHighLight();

        transform.localEulerAngles = Vector3.zero;
        isFaceUp = false;
        IsFanmian = false;
        //TurnToFace();
        Reset();
    }

    public void Tick(float dTime)
    {
        if (isTmp)
        {
            return;
        }

        if (isBacking)
        {
            if (nowValue > 0)
            {
                nowValue -= NowValueDecRate * dTime;
                HandleScale();
                if (nowValue <= 0)
                {
                    isBacking = false;
                    nowValue = 0;
                }
            }
            else
            {
                isBacking = false;
            }
        }

        view.CardRoot.anchoredPosition = CardRootPosition + shakingVector;
    }

    public void UpdateView(CardInZhibo info)
    {
        //if (info.ca.WillOverdue)
        //{
        //    view.TimeLeft.text = ((int)(info.TimeLeft) + 1) + "";
        //    if (info.TimeLeft < 3)
        //    {
        //        view.TimeLeft.color = Color.red;
        //    }
        //}
    }

    public void CheckIsHighlight()
    {
        bool isUI = RectTransformUtility.RectangleContainsScreenPoint(view.CardRoot, Input.mousePosition);
        if (!isUI)
        {
            return;
        }
        if (isDestroying)
        {
            return;
        }
        isHighlight = true;
        if (container.DraggingIdx != -1)
        {
            return;
        }
        if (isBacking)
        {
            isBacking = false;
            nowValue = 0;
        }


        view.CardRoot.localScale = NormalScale;
        CardRootPosition = new Vector3(0, 0 + NormalYOffset, 0);

    }
    public void setTargetPosition(Vector2 position)
    {
        //if (state == 0 || state == 2)
        //{
        //    moveTarget = position;
        //    moving = true;
        //}
    }

    private void BindView()
    {
        view.root = transform as RectTransform;
        view.CardRoot = transform.Find("CardRoot") as RectTransform;
        view.RotateArea = view.CardRoot.Find("RotateArea") as RectTransform;

        view.CardFace = view.RotateArea.Find("CardFace") as RectTransform;
        view.CardBack = view.RotateArea.Find("CardBack") as RectTransform;
        view.CardCG = view.CardRoot.GetComponent<CanvasGroup>();

        view.Bg = view.CardFace.Find("Outline").GetComponent<Image>();
        view.Picture = view.CardFace.Find("Picture").GetComponent<Image>();
        view.BackPicture = view.CardBack.Find("Picture").GetComponent<Image>();

        view.PictureCover = view.CardFace.Find("PictureCover").GetComponent<Image>();
        view.BackPictureCover = view.CardBack.Find("PictureCover").GetComponent<Image>();

        

        view.Name = view.CardFace.Find("Name").GetComponent<Text>();
        view.NamePicture = view.Name.transform.GetChild(0).GetComponent<Image>();
        view.BackName = view.CardBack.Find("Name").GetComponent<Text>();
        view.BackNamePicture = view.BackName.transform.GetChild(0).GetComponent<Image>();

        view.TypePicture = view.CardFace.Find("CardType").GetComponent<Image>();
        view.BackTypePicture = view.CardBack.Find("CardType").GetComponent<Image>();

        view.Background = view.CardFace.Find("Background").GetComponent<Image>();
        view.BackBackground = view.CardBack.Find("Background").GetComponent<Image>();

        view.Desp = view.CardFace.Find("Desp").GetComponent<Text>();
        view.Cost = view.CardFace.Find("Cost").GetComponent<Text>();

        view.GemsIncludeContainer = view.CardFace.Find("GemsInclude");

        view.GemContainer = view.CardFace.Find("Gems");
        view.GemBackContainer = view.CardBack.Find("Gems");

        view.BackCost = view.CardBack.Find("Cost").GetComponent<Text>();
        view.BackDesp = view.CardBack.Find("Desp").GetComponent<Text>();

        

        view.TimeLeftComp = view.CardFace.Find("TimeLeft");
        view.TimeLeft = view.TimeLeftComp.Find("Text").GetComponent<Text>();

        view.ClockAnimator = view.TimeLeftComp.Find("Clock").GetComponent<Animator>();
    }

    private void RegisterEvent()
    {


        DragEventListener listener = view.CardRoot.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.CardRoot.gameObject.AddComponent<DragEventListener>();

        }
        listener.ClearDragEvent();
        listener.ClearClickEvent();
        if (isTmp)
        {
            return;
        }
        listener.OnBeginDragEvent += delegate (PointerEventData eventData) {
            if (isInChain || isDestroying || isBacking || isDragging)
            {
                return;
            }
            preY = eventData.position.y;
            isDragging = true;
            container.DraggingIdx = container.cards.IndexOf(this);
        };

        listener.OnDragEvent += delegate (PointerEventData eventData) {
            if (isInChain || isDestroying || isBacking)
            {
                return;
            }
            float nowY = eventData.position.y;
            float dy = nowY - preY;

            nowValue += dy*1.4f;

            HandleScale();

            preY = nowY;
        };

        listener.OnEndDragEvent += delegate (PointerEventData eventData) {
            if (isInChain || isDestroying)
            {
                return;
            }
            if(nowValue >= TriggerValue)
            {
                //if (isFaceUp)
                //{
                //    UseCard();
                //}
                //else
                //{
                //    UseCardGem();
                //}
                if (isFaceUp)
                {
                    UseCardGem();
                }
                else
                {
                    UseCard();
                }
            }
            else
            {
                isBacking = true;
            }

            if (!isHighlight)
            {
                MinimizeIgnoreBacking();
            }
            preY = 0;
            isDragging = false;
            container.DraggingIdx = -1;
        };

        listener.PointerEnterEvent += delegate (PointerEventData eventData) {
            if (isDestroying)
            {
                return;
            }

            if (container.DraggingIdx != -1)
            {
                return;
            }
            isHighlight = true;
            if (isBacking)
            {
                isBacking = false;
                nowValue = 0;
            }

            SetSibling();
            view.CardRoot.localScale = NormalScale;
            CardRootPosition = new Vector3(0,0+ NormalYOffset, 0);
        };

        listener.PointerExitEvent += delegate (PointerEventData eventData) {
            if (isDestroying)
            {
                return;
            }
            isHighlight = false;
            if (container.DraggingIdx != -1)
            {
                return;
            }

            CancelSibling();
            MinimizeIgnoreBacking();

        };

        listener.OnClickEvent += delegate(PointerEventData ed) {

            if(ed.button == PointerEventData.InputButton.Right)
            {
                Fanmian();
                //UseCardGem();
            }
        };

    }

    public void MinimizeIgnoreBacking()
    {
        nowValue = 0;
        DOTween.To
                    (
                        () => view.CardRoot.localScale,
                        (x) => { view.CardRoot.localScale = x; },
                        MinimizeScale,
                        0.1f
                    );
        DOTween.To
            (
                () => CardRootPosition,
                (x) => { CardRootPosition = x; },
                Vector2.zero,
                0.1f
            );
    }

    public void HandleScale()
    {
        //float viewValue = nowValue;
        nowValue = nowValue < 0 ? 0 : nowValue;
        nowValue = nowValue > MaxValue ? MaxValue : nowValue;
        //nowValue = nowValue < 0 ? 0 : nowValue;
        //nowValue = nowValue > maxValue ? maxValue : nowValue;
        if(nowValue > TriggerValue)
        {
            SetHighLight();
        }
        else
        {
            CancelHighLight();
        }
        CardRootPosition = new Vector3(0, NormalYOffset + 0.4f* nowValue, 0);
        float scaleRate = 1f + DragScaleRate * nowValue / TriggerValue;
        view.CardRoot.localScale = new Vector3(scaleRate, scaleRate,1);
    }

    private int originSiblingIdx;

    public void SetHighLight()
    {
        //view.Bg.color = Color.red;
        view.Bg.enabled = true;
        SetSibling();
    }

    public void CancelHighLight()
    {
        //view.Bg.color = Color.white;
        view.Bg.enabled = false;
        CancelSibling();
    }

    public void SetSibling()
    {
        originSiblingIdx = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
    }

    public void CancelSibling()
    {
        if(originSiblingIdx>=0)
        {
            transform.SetSiblingIndex(originSiblingIdx);
        }
        originSiblingIdx = -1;
    }

    private void UseCard()
    {
        if (container.UseCard(this))
        {
            //Disappaer();
        }
        else
        {
            //晃动
            nowValue = 0;
            CancelHighLight();
            MinimizeIgnoreBacking();
        }
    }

    private void Reset()
    {
        TurnToFace();
        CardRootPosition = Vector2.zero;
        IsFanmian = false;
    }

    private void UseCardGem()
    {
        if (container.UseCardGem(this))
        {
            //Disappaer();
            //Fanmian();
            //Reset();
        }
        else
        {
            //晃动
            Shake();
            nowValue = 0;
            CancelHighLight();
            MinimizeIgnoreBacking();
        }
    }

    private Vector2 shakingVector = Vector2.zero;
    private Tween shakeTween = null;
    private void Shake()
    {
        if(shakeTween != null)
        {
            shakeTween.Kill();
        }

        shakeTween = DOTween.Shake(() => shakingVector,
                       (x) => { shakingVector = x; },
                        0.5f,
                       new Vector3(30, 0, 0))
        .OnComplete(delegate {
           shakingVector = Vector2.zero;

        })
        .OnKill(delegate {
             shakingVector = Vector2.zero;

         });

    }
    public void Disappaer()
    {
        anim.SetTrigger("Disappear");
        isDestroying = true;
        view.CardCG.blocksRaycasts = false;
        Invoke("Recycle", 0.5f);
    }

    public void Recycle()
    {
        Reset();
        container.RecycleCard(gameObject);
    }


    private static float BasicAlpha = 1f;
    private static float FlashInterval = 0.5f;

    private static float MinAlpha = 0.3f;

    //public void SetFlashingColor(float leftTime)
    //{
    //    if (isDestroying)
    //    {
    //        return;
    //    }
    //    if (isDragging|| isHighlight)
    //    {
    //        view.CardCG.alpha = BasicAlpha;
    //        return;
    //    }
    //    float a = Mathf.Abs(1 - (leftTime - (int)(leftTime / FlashInterval) * FlashInterval) / FlashInterval * 2);
    //    view.CardCG.alpha = MinAlpha + a * (BasicAlpha - MinAlpha);
    //}


    public bool Fanmian()
    {
        if (IsFanmian)
        {
            return false;
        }
        if (!container.Fanmian(this))
        {
            Shake();
            return false;
        }
        IsFanmian = true;
        DOTween.To
        (
            () => view.RotateArea.localEulerAngles,
            (x) => { view.RotateArea.localEulerAngles = x; },
            new Vector3(0,90,0f),
            0.075f
        ).OnComplete(delegate {

            if (isFaceUp)
            {
                TurnToBack();
            }
            else
            {
                TurnToFace();
            }

            DOTween.To
            (
                () => view.RotateArea.localEulerAngles,
                (x) => { view.RotateArea.localEulerAngles = x; },
                new Vector3(0, 0, 0f),
                0.075f
            ).OnComplete(delegate {

                IsFanmian = false;
            });
        });
        return true;
    }

    public void TurnToFace()
    {
        if (isFaceUp)
        {
            return;
        }
        view.CardFace.gameObject.SetActive(false);
        view.CardBack.gameObject.SetActive(true);
        isFaceUp = !isFaceUp;
    }

    public void TurnToBack()
    {
        if (!isFaceUp)
        {
            return;
        }
        view.CardFace.gameObject.SetActive(true);
        view.CardBack.gameObject.SetActive(false);
        isFaceUp = !isFaceUp;
    }

    public void ShowGemIncluded()
    {

    }
}
