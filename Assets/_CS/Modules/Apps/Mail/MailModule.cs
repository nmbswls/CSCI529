using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum MailCondition
{
    toRoundOnce,
    toRound,
    whenTag
}

public enum MailBonusType
{
    gainCard,
    gainKoucai,
    gainKangya,
    gainWaiguan,
    gainCaiyi,
    gainJishu,
    gainAllStat
}

public class MailBonus
{
    public MailBonusType BonusType;
    public string BonusValue;
}

public class Mail
{
    public int index;
    public string title;
    public string description;
    public string content;
    public string fromPeople;
    public string time;
    public string avatar;
    public string tag;
    public MailCondition condition;
    public string conditionValue1;
    public string conditionValue2;
    public bool withPicture;
    public string pictureLink;
    public bool withBonus;
    public int numOfBonus;
    public List<MailBonus> bonuses;

    //read
    public bool isRead = false;
    public bool isGetReward = false;

}

public class MailList
{
    public List<Mail> mailsSetup = new List<Mail>();
    public List<Mail> mailBox = new List<Mail>();
    public Dictionary<string, List<Mail>> mailToBeSend;
    public HashSet<string> tags;

    public void loadMail()
    {
        mailToBeSend = new Dictionary<string, List<Mail>>();
        tags = new HashSet<string>();
        MailContentList mailContent = GameMain.GetInstance().GetModule<ResLoader>().LoadResource<MailContentList>("MailContent/MailContentList", false);
        foreach (MailAsset m in mailContent.Entities)
        {
            Mail mail = new Mail();
            mail.index = m.Index;
            mail.title = m.Title;
            mail.description = m.Desp;
            mail.content = m.Content;
            mail.fromPeople = m.From;
            mail.time = m.Time;
            mail.avatar = m.Avatar;
            mail.condition = (MailCondition)System.Enum.Parse(typeof(MailCondition), m.Condition);
            mail.conditionValue1 = m.ConditionValue1;
            mail.conditionValue2 = m.ConditionValue2;
            mail.withPicture = m.WithPicture=="Yes";
            mail.pictureLink = m.PictureLink;
            mail.withBonus = m.WithBonus == "Yes";
            mail.numOfBonus = m.NumOfBonus;

            mail.tag = m.Tag;
            
            if(mail.numOfBonus>0)
            {
                string[] bonusType = m.BonusType.Split(',');
                string[] bonusValue = m.BonusValue.Split(',');
                for(int i = 0; i<mail.numOfBonus; i++)
                {
                    MailBonus mailBonus = new MailBonus();
                    mailBonus.BonusType = (MailBonusType)System.Enum.Parse(typeof(MailBonusType), bonusType[i]);
                    mailBonus.BonusValue = bonusValue[i];
                    mail.bonuses.Add(mailBonus);
                }
            }
            mailsSetup.Add(mail);

            //save tag

            if(!tags.Contains(mail.tag))
            {
                tags.Add(mail.tag);
            }

            //match tag
            if(!mailToBeSend.ContainsKey(mail.tag))
            {
                mailToBeSend.Add(mail.tag, new List<Mail>());
            }
            mailToBeSend[mail.tag].Add(mail);
        }
    }

    public void reloadMail()
    {
        
        if (mailsSetup.Count == 0)
        {
            loadMail();
        }
        else
        {
            mailToBeSend = new Dictionary<string, List<Mail>>();
            tags = new HashSet<string>();
            foreach (Mail mail in mailsSetup)
            {
                if (!tags.Contains(mail.tag))
                {
                    tags.Add(mail.tag);
                }
                if (!mailToBeSend.ContainsKey(mail.tag))
                {
                    mailToBeSend.Add(mail.tag, new List<Mail>());
                }
                mailToBeSend[mail.tag].Add(mail);
            }
        }
    }
}

public class MailModule : ModuleBase
{

    IRoleModule pRoleMdl;

    public MailList mailList = new MailList();
    public override void Setup()
    {
        mailList.loadMail();
        pRoleMdl = GameMain.GetInstance().GetModule<RoleModule>();
    }

    public void checkTurnStartToBeSentEmail()
    {
        int round = pRoleMdl.GetCurrentTurn();
        foreach(string t in mailList.tags)
        {
            sendEmail(t, round);
        }
    }

    public void sendEmail(string tag, int value = 0, bool isTurnStart = true)
    {
        List<Mail> toBeSent = new List<Mail>();
        switch (tag)
        {
            case "roundOnce":
                toBeSent = getOnceRoundMail(value);
                break;
            case "round":
                toBeSent = getRoundMail(value);
                break;
            default:
                break;            
        }

        if(toBeSent.Count>0)
        {
            foreach(Mail mail in toBeSent)
            {
                mailList.mailBox.Add(mail);
                Debug.Log("Received!, now emails = " + mailList.mailBox.Count);
            }
        }
    }

    public List<Mail> getOnceRoundMail(int round)
    {
        List<Mail> toBeSent = new List<Mail>();
        if(mailList.mailToBeSend.ContainsKey("roundOnce"))
        {
            foreach(Mail mail in mailList.mailToBeSend["roundOnce"])
            {
                int value;
                if(Int32.TryParse(mail.conditionValue1, out value)){
                    if (value == round)
                    {
                        toBeSent.Add(mail);
                    }
                }
            }
            foreach(Mail mail in toBeSent)
            {
                mailList.mailToBeSend["roundOnce"].Remove(mail);
            }
        }
        return toBeSent;
    }

    public List<Mail> getRoundMail(int round)
    {
        List<Mail> toBeSent = new List<Mail>();
        if (mailList.mailToBeSend.ContainsKey("round"))
        {
            foreach (Mail mail in mailList.mailToBeSend["round"])
            {
                int value;
                if (Int32.TryParse(mail.conditionValue1, out value))
                {
                    if (value == round)
                    {
                        toBeSent.Add(mail);
                    }
                }
            }
        }
        return toBeSent;
    }

    public bool checkMailListLoaded()
    {
        if(mailList.mailsSetup.Count == 0)
        {
            mailList.reloadMail();
        }
        return true;
    }

    public void deleteMail(Mail mail)
    {
        mailList.mailBox.Remove(mail);
    }

}
