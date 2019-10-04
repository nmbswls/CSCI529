using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICardDeckModule : IModule
{
	void GainNewCard(string cid);

    List<CardInfo> GetAllCards();

    CardAsset GetCardInfo(string cardId);

    void CheckOverdue();
}
