using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface ICardDeckModule : IModule
{
    CardInfo GainNewCard(string cid);

    List<CardInfo> GetAllCards();

    CardAsset GetCardInfo(string cardId);

    void CheckOverdue();

    void AddSkillCards(string skillId, List<string> cid);


    void AddCards(List<string> cards);

    void RemoveSkillCards(string skillId);

    void CheckTurnBonux();
}
