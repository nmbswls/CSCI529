using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainGameMode : GameModeBase
{

	IUIMgr pUIMgr;
    IRoleModule rm;
    ISpeEventMgr pEventMgr;


    Queue<SpecialEvent> UnHandledEvent = new Queue<SpecialEvent>();
    public override void Tick(float dTime){

        if (Input.GetKeyDown(KeyCode.A))
        {
            pUIMgr.ShowMsgBox();
        }
    }

	public override void Init(){
		pUIMgr = GameMain.GetInstance ().GetModule<UIMgr> ();
        rm = GameMain.GetInstance().GetModule<RoleModule>();
        pEventMgr = GameMain.GetInstance().GetModule<SpeEventMgr>();


        pUIMgr.ShowPanel("UIMain");
    }

    public void NextTurn()
    {
        HandleEvents(pEventMgr.CheckEvent());
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckOverdue();
        GameMain.GetInstance().GetModule<CardDeckModule>().CheckTurnBonux();
    }

    public void HandleEvents(List<SpecialEvent> list)
    {
        UnHandledEvent.Clear();
        foreach (SpecialEvent e in list)
        {
            UnHandledEvent.Enqueue(e);

        }
        HandleNextEvent();
    }





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
                    DialogManager dm = pUIMgr.ShowPanel("DialogManager") as DialogManager;
                    if (dm == null)
                    {
                        Debug.LogError("dialog mgr load fail");
                    }
                    dm.StartDialog(cmd[1], delegate (string[] args)
                    {
                        HandleNextEvent();
                    });
                }else if (cmd[0] == "event")
                {
                    pEventMgr.AddListener(cmd[1]);
                }

            }

        }

    }


}

