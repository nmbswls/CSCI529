﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TurnMsg
{
    public string content;

    public TurnMsg(string content)
    {
        this.content = content;
    }
}

public class MainGMInitData : GameModeInitData
{
    public bool isNextTurn;
}
public class MainGameMode : GameModeBase
{

	IUIMgr pUIMgr;
    IRoleModule rm;
    ISpeEventMgr pEventMgr;

    UIMainCtrl mainUI;

    Queue<SpecialEvent> UnHandledEvent = new Queue<SpecialEvent>();

    List<string> TurnInfos = new List<string>();

    public int TurnPracticeNum = 0;

    

    public int GetPracticeCost()
    {
        return TurnPracticeNum * 5;
    }

    public override void Tick(float dTime){


    }

	public override void Init(GameModeInitData initData)
    {
		pUIMgr = GameMain.GetInstance ().GetModule<UIMgr> ();
        rm = GameMain.GetInstance().GetModule<RoleModule>();
        pEventMgr = GameMain.GetInstance().GetModule<SpeEventMgr>();
        
        mainUI = pUIMgr.ShowPanel("UIMain") as UIMainCtrl;

        MainGMInitData realData = initData as MainGMInitData;

        if(realData != null)
        {
            if (realData.isNextTurn)
            {
                NextTurn();
            }
        }
    }

    public void NextTurn()
    {
        Debug.Log("next turn");
        TurnInfos.Clear();
        rm.NextTurn();
        HandleEvents(pEventMgr.CheckEvent());
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckOverdue();
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckTurnBonux();
        GameMain.GetInstance().GetModule<WeiboModule>().resetShua();
        GameMain.GetInstance().GetModule<WeiboModule>().enableRealRandom();
        GameMain.GetInstance().GetModule<MailModule>().checkTurnStartToBeSentEmail();
        GameMain.GetInstance().GetModule<TaobaoModule>().LoadProductInDifferentTurn();
        mainUI.ShowMsg(new List<TurnMsg>());
        TurnPracticeNum = 0;
    }

    public void HandleEvents(List<SpecialEvent> list)
    {
        UnHandledEvent.Clear();
        foreach (SpecialEvent e in list)
        {
            UnHandledEvent.Enqueue(e);

        }
        cachedEvents.Clear();
        HandleNextEvent();
    }



    private List<string> cachedEvents = new List<string>();


    public void HandleNextEvent()
    {
        if (UnHandledEvent.Count > 0)
        {

            SpecialEvent head = UnHandledEvent.Dequeue();
            pEventMgr.RemoveListener(head.EventId);
            foreach (string action in head.actions)
            {

                string[] cmd = action.Split(',');
                if (cmd[0] == "dialog")
                {
                    //DialogManager dm = pUIMgr.ShowPanel("DialogManager") as DialogManager;
                    //if (dm == null)
                    //{
                    //    Debug.LogError("dialog mgr load fail");
                    //}
                    //dm.StartDialog(cmd[1], delegate (string[] args)
                    //{
                    //    HandleNextEvent();
                    //});
                }
                else if (cmd[0] == "event")
                {
                    cachedEvents.Add(cmd[1]);
                    //pEventMgr.AddListener(cmd[1]);
                }

            }

        }
        else
        {
            AddCachedEvents();
        }

    }

    public void AddCachedEvents()
    {
        for(int i = 0; i < cachedEvents.Count; i++)
        {
            pEventMgr.AddListener(cachedEvents[i]);
        }
    }


}

