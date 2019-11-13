using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MiniCardView
{

    public RectTransform root;
    public RectTransform CardFace;
    public CanvasGroup CardCG;
    public Image Bg;
    public Image Picture;
    public Text Desp;
    public Text Name;
    public Transform TimeLeftComp;
    public Text TimeLeft;
    public Animator ClockAnimator;
    public Text Cost;
}
public class MiniCard : MonoBehaviour
{

    MiniCardView view = new MiniCardView();
    CardContainerLayout container;

    public float nowDegree;
    public float targetDegree;

    public bool isTmp;
    public Vector2 TargetPos;

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

    private static float NormalYOffset = 150f;
    private static float DragScaleRate = 0.3f;

    private static float NowValueDecRate = 500f;
    //上拉多少触发卡片
    private static float MaxValue = 120f;
    private static float TriggerValue = 100f;

    float preY = 0;

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
        view.Desp.text = ca.CardEffectDesp;
        if(ca.cost == -1)
        {
            view.Cost.text = "X";
        }
        else
        {
            view.Cost.text = ca.cost + "";
        }

        if (ca.CatdImageName == null || ca.CatdImageName == string.Empty)
        {
            view.Picture.sprite = ca.Picture;
        }
        else
        {
            view.Picture.sprite = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<Sprite>("CardImage/" + ca.CatdImageName);

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
        view.CardFace.anchoredPosition = Vector2.zero;
        view.CardFace.localScale = MinimizeScale;

        view.CardCG.blocksRaycasts = true;
        CancelHighLight();

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
        bool isUI = RectTransformUtility.RectangleContainsScreenPoint(view.CardFace, Input.mousePosition);
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


        view.CardFace.localScale = NormalScale;
        view.CardFace.anchoredPosition = new Vector3(0, 0 + NormalYOffset, 0);

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
        view.CardFace = transform.Find("CardFace") as RectTransform;
        view.CardCG = view.CardFace.GetComponent<CanvasGroup>();
        view.Bg = view.CardFace.GetComponent<Image>();
        view.Picture = view.CardFace.Find("Picture").GetComponent<Image>();
        view.Name = view.CardFace.Find("Name").GetComponent<Text>();
        view.Desp = view.CardFace.Find("Desp").GetComponent<Text>();
        view.Cost = view.CardFace.Find("Cost").GetComponent<Text>();

        view.TimeLeftComp = view.CardFace.Find("TimeLeft");
        view.TimeLeft = view.TimeLeftComp.Find("Text").GetComponent<Text>();

        view.ClockAnimator = view.TimeLeftComp.Find("Clock").GetComponent<Animator>();
    }

    private void RegisterEvent()
    {


        DragEventListener listener = view.CardFace.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.CardFace.gameObject.AddComponent<DragEventListener>();

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
                UseCard();
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


            view.CardFace.localScale = NormalScale;
            view.CardFace.anchoredPosition = new Vector3(0,0+ NormalYOffset, 0);
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


            MinimizeIgnoreBacking();

        };

    }

    public void MinimizeIgnoreBacking()
    {
        nowValue = 0;
        DOTween.To
                    (
                        () => view.CardFace.localScale,
                        (x) => { view.CardFace.localScale = x; },
                        MinimizeScale,
                        0.1f
                    );
        DOTween.To
            (
                () => view.CardFace.anchoredPosition,
                (x) => { view.CardFace.anchoredPosition = x; },
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
        view.CardFace.anchoredPosition = new Vector3(0, NormalYOffset + 0.4f* nowValue, 0);
        float scaleRate = 1f + DragScaleRate * nowValue / TriggerValue;
        view.CardFace.localScale = new Vector3(scaleRate, scaleRate,1);
    }

    public void SetHighLight()
    {
        view.Bg.color = Color.red;
    }

    public void CancelHighLight()
    {
        view.Bg.color = Color.white;
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

    public void Disappaer()
    {
        anim.SetTrigger("Disappear");
        isDestroying = true;
        view.CardCG.blocksRaycasts = false;
        Invoke("Recycle", 0.5f);
    }

    public void Recycle()
    {
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
}
