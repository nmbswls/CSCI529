using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniCardView
{

    public Image picture;
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

    public void init(CardContainerLayout container)
    {
        rt = (RectTransform)transform;
        anim = GetComponent<Animator>();

        BindView();
        RegisterEvent();
        this.container = container;
        transform.SetParent(container.transform);
        nowDegree = 20f;
        targetDegree = 20f;

        rt.anchoredPosition = new Vector3(0.34f * container.R, -0.07f*container.R);
        rt.localEulerAngles = new Vector3(0, 0, -20f);
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
                }
            }
            else
            {
                isBacking = false;
            }
        }

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
        view.picture = transform.GetChild(1).GetComponent<Image>();
    }

    private void RegisterEvent()
    {


        DragEventListener listener = view.picture.gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = view.picture.gameObject.AddComponent<DragEventListener>();


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

        nowValue = nowValue < 0 ? 0 : nowValue;
        nowValue = nowValue > maxValue ? maxValue : nowValue;

        view.picture.rectTransform.anchoredPosition = new Vector3(0, 0.4f*nowValue,0);
        float scaleRate = 1f + 0.3f * nowValue / maxValue;
        view.picture.rectTransform.localScale = new Vector3(scaleRate, scaleRate,1);
    }

    private void UseCard()
    {
        anim.SetTrigger("Disappear");
        Debug.Log("destroy");
        isDestroying = true;
        container.removeCard(this);
        GetComponent<CanvasGroup>().blocksRaycasts=false;
        GameObject.DestroyObject(this,0.5f);
    }
}
