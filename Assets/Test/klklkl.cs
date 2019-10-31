using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class klklkl : MonoBehaviour,IPointerClickHandler
{
    //public TextMeshProUGUI m_TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (m_TextMeshPro)
        //{
        //    //NOTE 如果UGUI没用Camera渲染，TMPText不传入Camera
        //    int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, null);
        //    if (linkIndex != -1)
        //    {
        //        TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
        //        Debug.Log(linkInfo.textComponent.text);
        //    }
        //}
    }
}
