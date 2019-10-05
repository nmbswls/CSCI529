using UnityEngine;
using System.Collections.Generic;

public class CardContainerLayout : MonoBehaviour
{
    public List<MiniCard> cards = new List<MiniCard>();


    //public GameObject cardPrefab;


    [HideInInspector]
    public int CardNow;

    [HideInInspector]
    public float Width;

    RectTransform rt;

    [HideInInspector]
    public float R = 1500f;

    float MaxDegree = 20;

    public ZhiboGameMode gameMode;

    public void Init(ZhiboGameMode gameMode)
    {
        rt = (RectTransform)transform;
        Width = rt.rect.width;
        this.gameMode = gameMode;
    }

    public bool AddCard(string cardId)
    {


        IResLoader loader = GameMain.GetInstance().GetModule<ResLoader>();

        GameObject cardGo = loader.Instantiate("Zhibo/Card");
        if(cardGo == null)
        {
            return false;
        }
        MiniCard card = cardGo.GetComponent<MiniCard>();
        card.Init(cardId,this);
        cards.Add(card);
        Adjust();
        return true;
    }

    private void Adjust()
    {
        float intervalDegree = 3f;
        if (cards.Count > 6)
        {
            intervalDegree = MaxDegree / cards.Count;
        }

        for (int i = 0; i < cards.Count; i++)
        {
            float angleDegree = i * intervalDegree - 10;
            cards[i].transform.SetSiblingIndex(i);
            cards[i].targetDegree = angleDegree;
            //Vector2 posInWorld = transform.localToWorldMatrix * new Vector4(i * interval, 0, 0, 1);
            //cards[i].setTargetPosition(posInWorld);
        }
    }

    public void Tick(float dTime)
    {
        foreach(MiniCard card in cards)
        {
            card.Tick(dTime);
            if (Mathf.Abs(card.targetDegree - card.nowDegree) <= 1e-6)
            {
                continue;
            }
            card.nowDegree += (card.targetDegree - card.nowDegree) * dTime * 5f;
            card.rt.anchoredPosition = new Vector3(Mathf.Sin(card.nowDegree * Mathf.Deg2Rad) * R, Mathf.Cos(card.nowDegree * Mathf.Deg2Rad) * R - R);
            card.rt.localEulerAngles = new Vector3(0, 0, -card.nowDegree);


        }
    }


    public bool UseCard(MiniCard toUse)
    {
        int cardIdx = cards.IndexOf(toUse);
        if (gameMode.TryUseCard(cardIdx))
        {
            removeCard(toUse);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void removeCard(MiniCard toRemove)
    {
        cards.Remove(toRemove);
        Adjust();
    }




}
