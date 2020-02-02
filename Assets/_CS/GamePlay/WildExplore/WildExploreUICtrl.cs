using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopListView {
    public Button close;
}


public class WildExploreUICtrl : MonoBehaviour
{

    public GameObject PopupList;
    public PopListView PopListView = new PopListView();

    // Start is called before the first frame update
    void Start()
    {
        //PopupList.SetActive(false);
        PopListView.close = PopupList.GetComponentInChildren<Button>();
        PopListView.close.onClick.AddListener(delegate {

            HidePopup();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
