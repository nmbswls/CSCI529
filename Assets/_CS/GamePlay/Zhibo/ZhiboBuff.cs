using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ZhiboBuff : MonoBehaviour
{

    private static float FlashInterval = 0.5f;

    ZhiboGameMode gameMode;
    public string buffId;
    public int buffLevel;
    public float totalLastTime;
    public float leftTime;
    public int leftTurn;

    private static float basicAlpht = 0.8f;
    private Image icon;

    public void Init(string buffId, int buffLevel, int turnLeft, ZhiboGameMode gameMode)
    {
        this.buffId = buffId;
        this.buffLevel = buffLevel;
        this.leftTurn = turnLeft;
        this.gameMode = gameMode;

        BindView();
        RegisterEvent();
    }


    public void RegisterEvent()
    {
        //鼠标悬浮显示buff
        DragEventListener listener = gameObject.GetComponent<DragEventListener>();
        if (listener == null)
        {
            listener = gameObject.AddComponent<DragEventListener>();
            listener.PointerEnterEvent += delegate (PointerEventData eventData) {
                gameMode.mUICtrl.ShowBuffDetail(this);
            };

            listener.PointerExitEvent += delegate (PointerEventData eventData) {
                gameMode.mUICtrl.HideBuffDetail();
            };
        }
    }
    public void BindView()
    {
        icon = GetComponentInChildren<Image>();
    }

    public void Tick(float dTime)
    {
        //leftTime -= dTime;
        //if(leftTime < 3f)
        //{
        //    if (leftTime > 0)
        //    {
        //        icon.color = GetFlashingColor();
        //    }
        //}
    }

    public Color GetFlashingColor()
    {
        float a = Mathf.Abs(1 - (leftTime - (int)(leftTime / FlashInterval) * FlashInterval) / FlashInterval * 2);
        return new Color(1, 1, 1, a* basicAlpht);
    }

}
