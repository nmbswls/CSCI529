using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class WeiboGameMode : GameModeBase
{
    ICardDeckModule pCardMdl;
    
    public override void Init()
    {
        pCardMdl = GameMain.GetInstance().GetModule<CardDeckModule>();
        if(pCardMdl == null)
        {
            Debug.Log("getCardFailed!");
        }
    }
    
}