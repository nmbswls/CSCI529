using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TravelPotInfo
{
    public string Name;
    public string Desp;
    public List<string> Opts = new List<string>();

    public TravelPotInfo(string Name)
    {
        this.Name = Name;
    }
}

public class TravelPot : MonoBehaviour
{


    public TravelPotInfo potInfo;

    SpriteRenderer image;

    bool selected = false;

    TravelGameMode gm;
    public void Init(TravelPotInfo info, TravelGameMode gm)
    {
        this.gm = gm;
        this.potInfo = info;
        image = GetComponent<SpriteRenderer>();
        RegisterEvent();

    }


    public void RegisterEvent()
    {
        {
            ClickableEventlistener2D listener = gameObject.GetComponent<ClickableEventlistener2D>();
            if(listener == null)
            {
                listener = gameObject.AddComponent<ClickableEventlistener2D>();
                listener.ClickEvent += delegate {
                    gm.ChoosePot(this);
                };
                Debug.Log(listener.hasClickEvent());
            }

        }
    }

    public void Selected()
    {
        image.color = Color.red;
    }

    public void CancelSelect()
    {
        image.color = Color.white;
    }
}
