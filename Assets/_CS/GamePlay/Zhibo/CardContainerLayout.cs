using UnityEngine;
using System.Collections.Generic;

public class CardContainerLayout : MonoBehaviour
{
    public List<MiniCard> cards = new List<MiniCard>();
    public List<MiniCard> TmpCards = new List<MiniCard>();

    public Transform FixedContainer;
    //public GameObject cardPrefab;


    [HideInInspector]
    public int CardNow;


    RectTransform rt;


    float Width = 1000f;

    float R = 0;
    float MaxDegree = 10;
    float CardMoveSpd = 5f;
    float DefaultIntervalDegree = 2f;

    public ZhiboGameMode gameMode;
    IResLoader mResLoader;


    public int DraggingIdx = -1;

    public void Init(ZhiboGameMode gameMode)
    {
        rt = (RectTransform)transform;
        Width = rt.rect.width;
        this.gameMode = gameMode;
        R = Width * 0.5f / Mathf.Sin(MaxDegree * 0.5f * Mathf.Deg2Rad);
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();

        FixedContainer = transform.Find("FixedArea");
    }

    public void PutToInitPos(MiniCard card)
    {
        card.rt.anchoredPosition = new Vector3(Mathf.Sin(MaxDegree*0.5f*Mathf.Deg2Rad) * R, Mathf.Cos(MaxDegree * 0.5f * Mathf.Deg2Rad) * R);
        card.rt.localEulerAngles = new Vector3(0, 0, -MaxDegree);
    }

    public bool AddCard(CardInZhibo cardInfo)
    {
        IResLoader loader = GameMain.GetInstance().GetModule<ResLoader>();

        GameObject cardGo = loader.Instantiate("Zhibo/Card");
        if(cardGo == null)
        {
            return false;
        }
        MiniCard card = cardGo.GetComponent<MiniCard>();
        card.Init(cardInfo, this);
        card.transform.SetParent(transform, false);
        cards.Add(card);
        Adjust();
        return true;
    }

    public bool AddTmpCard(CardInZhibo cardInfo)
    {
        IResLoader loader = GameMain.GetInstance().GetModule<ResLoader>();

        GameObject cardGo = loader.Instantiate("Zhibo/Card", FixedContainer);
        if (cardGo == null)
        {
            return false;
        }
        MiniCard card = cardGo.GetComponent<MiniCard>();
        card.Init(cardInfo, this);
        card.rt.anchoredPosition = Vector3.zero;
        card.rt.localEulerAngles = Vector3.zero;
        TmpCards.Add(card);
        AdjustTmp();
        return true;
    }

    public void RecycleCard(GameObject go)
    {
        mResLoader.ReleaseGO("Zhibo/Card",go);
    }

    private void Adjust()
    {
        float interval = DefaultIntervalDegree;
        if (cards.Count > 6)
        {
            interval = MaxDegree / cards.Count;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            float angleDegree = i * interval - 10*0.5f;
            cards[i].transform.SetSiblingIndex(i);
            cards[i].targetDegree = angleDegree;
            cards[i].PosDirty = true;
            //Vector2 posInWorld = transform.localToWorldMatrix * new Vector4(i * interval, 0, 0, 1);
            //cards[i].setTargetPosition(posInWorld);
        }
    }

    private void AdjustTmp()
    {


        for (int i = 0; i < TmpCards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
            cards[i].TargetPos = new Vector3(30*i,30*i,0);
            cards[i].PosDirty = true;
        }
    }

    public void Tick(float dTime)
    {
        for(int i = 0; i < cards.Count; i++)
        {
            MiniCard card = cards[i];
            card.Tick(dTime);
            if (gameMode.state.Cards[i].TimeLeft < 3f)
            {
                //card.SetFlashingColor(gameMode.state.Cards[i].TimeLeft);
            }
            if (!card.PosDirty || Mathf.Abs(card.targetDegree - card.nowDegree) <= 1e-6)
            {
                card.PosDirty = false;
                card.CheckIsHighlight();
                continue;
            }
            card.nowDegree += (card.targetDegree - card.nowDegree) * dTime * CardMoveSpd;
            card.rt.anchoredPosition = new Vector3(Mathf.Sin(card.nowDegree * Mathf.Deg2Rad) * R, Mathf.Cos(card.nowDegree * Mathf.Deg2Rad) * R - R);
            card.rt.localEulerAngles = new Vector3(0, 0, -card.nowDegree);
        }

        for(int i = 0; i < TmpCards.Count; i++)
        {
            MiniCard card = TmpCards[i];
            if (!card.PosDirty || Mathf.Abs(card.targetDegree - card.nowDegree) <= 1e-6)
            {
                card.PosDirty = false;
                card.CheckIsHighlight();
                continue;
            }
            card.rt.anchoredPosition = (card.TargetPos - card.rt.anchoredPosition) * dTime * 10f;
        }
    }




    public bool UseCard(MiniCard toUse)
    {
        int cardIdx = cards.IndexOf(toUse);
        if (gameMode.TryUseCard(cardIdx))
        {
            //removeCard(toUse);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void removeCard(MiniCard toRemove)
    {
        if(DraggingIdx == cards.IndexOf(toRemove)){
            DraggingIdx = -1;
        }
        cards.Remove(toRemove);
        Adjust();
    }

    public void RemoveCard(int CardIdx)
    {
        if(CardIdx<0 || CardIdx >= cards.Count)
        {
            return;
        }
        cards[CardIdx].Disappaer();
        cards.RemoveAt(CardIdx);
        Adjust();
    }

    public Vector3 GetCardPosition(CardInZhibo card)
    {
        Vector3 ret = Vector3.zero;
        if (card.isTmp)
        {
            ret = TmpCards[gameMode.state.TmpCards.IndexOf(card)].transform.position;
        }
        else
        {
            ret = cards[gameMode.state.Cards.IndexOf(card)].transform.position;
        }
        return ret;
    }
    public void RemoveTmpCard(int CardIdx)
    {
        if (CardIdx < 0 || CardIdx >= TmpCards.Count)
        {
            return;
        }
        TmpCards[CardIdx].Disappaer();
        TmpCards.RemoveAt(CardIdx);
        AdjustTmp();
    }

    public void UpdateCard(int CardIdx, CardInZhibo cinfo)
    {
        if (CardIdx < 0 || CardIdx >= cards.Count)
        {
            return;
        }
        cards[CardIdx].UpdateView(cinfo);
    }


}
