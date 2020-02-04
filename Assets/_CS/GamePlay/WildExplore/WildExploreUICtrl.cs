using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopListView {
    public Button close;
}


public class WildExploreUICtrl : MonoBehaviour
{
    public WildExploreCtrl mainCtrl;
    public GameObject PopupList;
    public GameObject HintPanel;
    public GameObject HintTextPrefab;
    public GameObject Timer;
    public List<GameObject> msgList = new List<GameObject>();
    public PopListView PopListView = new PopListView();

    // Start is called before the first frame update
    void Start()
    {
        //PopupList.SetActive(false);
        PopListView.close = PopupList.GetComponentInChildren<Button>();
        PopListView.close.onClick.AddListener(delegate {
            mainCtrl.HidePop();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateTimer()
    {

        Timer.GetComponent<Text>().text = (int)mainCtrl.gameTime + "";
    }

    public void ShowPopup(Vector3 pos)
    {
        PopupList.SetActive(true);
        Vector3 p = Camera.main.WorldToScreenPoint(pos);
        PopupList.transform.position = p;
    }
    public void HidePopup()
    {
        PopupList.SetActive(false);
    }

    public void ShowHint(string hintMsg)
    {
        GameObject go = null;
        if(msgList.Count > 10)
        {
            go = msgList[0];
            msgList.RemoveAt(0);
            go.transform.SetAsLastSibling();
            msgList.Add(go);
        }
        else
        {
            go = Instantiate<GameObject>(HintTextPrefab, HintPanel.transform);
        }
        go.GetComponent<Text>().text = hintMsg;
        msgList.Add(go);
    }
}
