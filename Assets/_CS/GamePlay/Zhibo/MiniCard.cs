using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniCardView
{

    public Image Bg;
    public Image Picture;
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


    public bool isBacking = false;
    public bool isDestroying = false;

    public RectTransform rt;
    public Animator anim;

    public float nowValue;

    //上拉多少触发卡片
    float maxValue = 100f;

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

        transform.SetParent(container.transform);
        nowDegree = 20f;
        targetDegree = 20f;

        container.PutToInitPos(this);
        isBacking = false;
    }

    public void Tick(float dTime)
    {
        if(isBacking)
        {
            if (nowValue > 0)
            {
                nowValue -= 300f * dTime;
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
        view.Bg = transform.Find("CardFace").GetComponent<Image>();
        view.Picture = transform.Find("CardFace").GetComponentInChildren<Image>();
        view.Name = transform.Find("CardFace").GetComponentInChildren<Text>();
        view.TimeLeft = transform.Find("CardFace").Find("TimeLeft").GetComponent<Text>();
    }

    private void RegisterEvent()
    {


        DragEventListener listener = view.Picture.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.Picture.gameObject.AddComponent<DragEventListener>();


            listener.OnBeginDragEvent += delegate (PointerEventData eventData) {
                if (isDestroying || isBacking)
                {
                    return;
                }
                preY = eventData.position.y;
            };

            listener.OnDragEvent += delegate (PointerEventData eventData) {
                if (isDestroying || isBacking)
                {
                    return;
                }
                float nowY = eventData.position.y;
                float dy = nowY - preY;

                nowValue += dy*1.5f;

                HandleScale();

                preY = nowY;
            };

            listener.OnEndDragEvent += delegate (PointerEventData eventData) {
                if (isDestroying)
                {
                    return;
                }
                if(nowValue >= maxValue)
                {
                    UseCard();
                }
                else
                {
                    isBacking = true;
                }
                preY = 0;
            };
        }
    }

    public void HandleScale()
    {
        float viewValue = nowValue;
        viewValue = viewValue < 0 ? 0 : viewValue;
        viewValue = viewValue > maxValue ? maxValue : viewValue;
        //nowValue = nowValue < 0 ? 0 : nowValue;
        //nowValue = nowValue > maxValue ? maxValue : nowValue;
        if(nowValue > maxValue)
        {
            SetHighLight();
        }
        else
        {
            CancelHighLight();
        }
        view.Picture.rectTransform.anchoredPosition = new Vector3(0, 0.4f* viewValue, 0);
        float scaleRate = 1f + 0.3f * viewValue / maxValue;
        view.Picture.rectTransform.localScale = new Vector3(scaleRate, scaleRate,1);
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
}
