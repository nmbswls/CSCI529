using UnityEngine;
using System.Collections.Generic;

public class CardContainerLayout : MonoBehaviour
{
    public List<MiniCard> cards = new List<MiniCard>();


    public GameObject cardPrefab;
    [HideInInspector]
    public int CardMax = 10;

    [HideInInspector]
    public int CardNow;

    [HideInInspector]
    public float Width;

    RectTransform rt;

    [HideInInspector]
    public float R = 1500f;

    float MaxDegree = 20;

    private void Start()
    {
        rt = (RectTransform)transform;
        Width = rt.rect.width;
    }

    public void AddCard()
    {
        if(cards.Count>= CardMax)
        {
            return;
        }
        bool success = false;

        GameObject cardGo = ((ShootDanmuMiniGame)MiniGame.GetInstance()).pool.Spawn(cardPrefab, Vector3.zero, Quaternion.identity, out success);
        MiniCard card = cardGo.GetComponent<MiniCard>();
        card.init(this);

        cards.Add(card);
        Adjust();

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

    private void Update()
    {
        foreach(MiniCard card in cards)
        {
            card.Tick(Time.deltaTime);
            if (Mathf.Abs(card.targetDegree - card.nowDegree) <= 1e-6)
            {
                continue;
            }
            card.nowDegree += (card.targetDegree - card.nowDegree) * Time.deltaTime * 5f;
            card.rt.anchoredPosition = new Vector3(Mathf.Sin(card.nowDegree * Mathf.Deg2Rad) * R, Mathf.Cos(card.nowDegree * Mathf.Deg2Rad) * R - R);
            card.rt.localEulerAngles = new Vector3(0, 0, -card.nowDegree);


        }
    }


    public void removeCard(MiniCard toRemove)
    {
        cards.Remove(toRemove);
        Adjust();
    }



}
