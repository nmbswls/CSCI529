using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;


[System.Serializable]
public enum eBuffType
{
    None=0,
    Meili_Add,
    Meili_Add_P,
    Tili_Add,
    Tili_Add_P,
    Fanying_Add,
    Fanying_Add_P,
    Koucai_Add,
    Koucai_Add_P,
    Jiyi_Add,
    Jiyi_Add_P,
    Extra_Score,
    Extra_Score_Rate,
    Score_Per_Sce,
    Score_Per_Turn,
    Extra_Neg_Level,
    No_Neg_Danmu,

    Next_Card_Extra_Score,
    Next_Card_Extra_Status,

    Success_Rate_Multi,
    Success_Rate_Max,
}

public enum eBuffLastType
{
    PERMANENT = 0,
    TURN_BASE = 0x01,
    TIME_BASE = 0x02,
    CARD_BASE = 0x04,
}

[System.Serializable]
public class ZhiboBuffInfo
{
    public eBuffType BuffType;
    public int BuffLevel;

    public bool AffectCard;
    public string filterString;

    public int TurnLast = 0;
    public int CardNum = 0;
    public float SecLast = 0;


}

public class ZhiboBuff : MonoBehaviour
{

    private static float FlashInterval = 0.5f;

    ZhiboGameMode gameMode;

    public ZhiboBuffInfo bInfo;

    public int BuffLastType = 0;

    public float LeftTime;
    public int LeftTurn;
    public int LeftCardNum = 0;
    public CardFilter filter = new CardFilter();

    private static float basicAlpht = 0.8f;
    private Image icon;

    public void Init(ZhiboGameMode gameMode, ZhiboBuffInfo buffInfo)
    {
        this.bInfo = buffInfo;
        this.LeftTurn = buffInfo.TurnLast;
        this.LeftCardNum = buffInfo.CardNum;
        this.gameMode = gameMode;

        if (bInfo.TurnLast > 0)
        {
            BuffLastType |= (int)eBuffLastType.TURN_BASE;
        }
        if (bInfo.SecLast > 0)
        {
            BuffLastType |= (int)eBuffLastType.TIME_BASE;
        }
        if (bInfo.CardNum > 0)
        {
            BuffLastType |= (int)eBuffLastType.CARD_BASE;
        }



        if (buffInfo.filterString != null && buffInfo.filterString!=string.Empty)
        {
            filter = CardFilter.parseFilterFromString(buffInfo.filterString);
        }
        BindView();
        RegisterEvent();
    }

    public bool isBasedOn(eBuffLastType type)
    {
        if((BuffLastType & (int)type) > 0)
        {
            return true;
        }
        return false;
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
        float a = Mathf.Abs(1 - (LeftTime - (int)(LeftTime / FlashInterval) * FlashInterval) / FlashInterval * 2);
        return new Color(1, 1, 1, a* basicAlpht);
    }

    public bool WillAffectCard(CardInZhibo card)
    {
        if (!bInfo.AffectCard)
        {
            return false;
        }
        if (!card.ca.ApplyFilter(filter))
        {
            return false;
        }
        return false;
    }

}
