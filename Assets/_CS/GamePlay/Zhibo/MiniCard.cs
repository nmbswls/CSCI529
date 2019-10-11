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
    public Text TimeLeft;
}
public class MiniCard : MonoBehaviour
{

    MiniCardView view = new MiniCardView();
    CardContainerLayout container;

    public float nowDegree;
    public float targetDegree;

    private float moveSpeed = 1500;
    private float returnSpeed = 1800;


    public bool PosDirty = false;
    public bool isBacking = false;
    public bool isDestroying = false;
    public bool isHighlight = false;
    public bool isDragging = false;



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

    public void Init(string cardId, CardContainerLayout container)
    {
        rt = (RectTransform)transform;
        anim = GetComponent<Animator>();
        this.container = container;

        CardAsset ca = GameMain.GetInstance().GetModule<CardDeckModule>().GetCardInfo(cardId);


        BindView();
        RegisterEvent();

        view.Name.text = ca.CardName;

        transform.SetParent(container.transform,false);
        nowDegree = 20f;
        targetDegree = 20f;

        container.PutToInitPos(this);
        isBacking = false;
        isDragging = false;
        isDestroying = false;
        isHighlight = false;
        PosDirty = false;

        view.CardCG.alpha = 1f;
    }

    public void Tick(float dTime)
    {
        if(isBacking)
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
        view.TimeLeft.text = (int)(info.TimeLeft) + "";
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
        view.TimeLeft = view.CardFace.Find("TimeLeft").GetComponent<Text>();
    }

    private void RegisterEvent()
    {


        DragEventListener listener = view.CardFace.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.CardFace.gameObject.AddComponent<DragEventListener>();


            listener.OnBeginDragEvent += delegate (PointerEventData eventData) {
                if (isDestroying || isBacking || isDragging)
                {
                    return;
                }
                preY = eventData.position.y;
                isDragging = true;
                container.DraggingIdx = container.cards.IndexOf(this);
            };

            listener.OnDragEvent += delegate (PointerEventData eventData) {
                if (isDestroying || isBacking)
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
                if (isDestroying)
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
        view.Bg.color = Color.black;
    }

    private void UseCard()
    {
        if (container.UseCard(this))
        {
            anim.SetTrigger("Disappear");
            isDestroying = true;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GameObject.DestroyObject(this, 0.5f);
        }
    }

    public void Disappaer()
    {
        anim.SetTrigger("Disappear");
        isDestroying = true;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GameObject.DestroyObject(this, 0.5f);
    }

    private static float BasicAlpha = 1f;
    private static float FlashInterval = 0.5f;

    private static float MinAlpha = 0.3f;

    public void SetFlashingColor(float leftTime)
    {
        if (isDestroying)
        {
            return;
        }
        if (isDragging|| isHighlight)
        {
            view.CardCG.alpha = BasicAlpha;
            return;
        }
        float a = Mathf.Abs(1 - (leftTime - (int)(leftTime / FlashInterval) * FlashInterval) / FlashInterval * 2);
        view.CardCG.alpha = MinAlpha + a * (BasicAlpha - MinAlpha);
    }
}
