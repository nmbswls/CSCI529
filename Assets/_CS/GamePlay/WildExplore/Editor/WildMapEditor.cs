using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(WildMap))]
[CanEditMultipleObjects]
public class WildMapEditor : Editor
{
    // Start is called before the first frame update
    void OnSceneGUI()
    {
        WildMap map = (WildMap)target;
        Handles.color = Color.green;

        float h = map.Height / map.meterPerUnit;
        float w = map.Width / map.meterPerUnit;

        Vector2[] co = new Vector2[] { new Vector2(0, 0), new Vector2(w, 0), new Vector2(w, h), new Vector2(0, h) };

        Handles.DrawSolidRectangleWithOutline(new Rect(0,0,w,h),Color.clear,Color.blue);

        for(int i = 0; i < 4; i++)
        {
            //Handles.PositionHandle(map.transform.TransformPoint(co[i]),Quaternion.identity, HandleUtility.GetHandleSize(map.transform.position),EventType.Ignore);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);

        }
        
    }
}
