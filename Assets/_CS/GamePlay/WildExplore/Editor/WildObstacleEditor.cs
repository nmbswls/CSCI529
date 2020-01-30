using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WildObstacle))]
[CanEditMultipleObjects]
public class WildObstacleEditor : Editor
{
    // Start is called before the first frame update
    void OnSceneGUI()
    {
        WildObstacle obstacle = (WildObstacle)target;
        Handles.color = Color.green;
        

        //if (GUILayout.Button("Rest Area"))
        //{
        //    arraw.shieldAre += 1.0f;  //改变数值
        //}
        for(int i=0;i< obstacle.PointsList.Count; i++)
        {
            _DoBodyFreeMoveHandle(obstacle.PointsList[i], i);
        }
       
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);

        }

        //画出弧线
        //Handles.DrawWireArc(arraw.transform.position, arraw.transform.up, -arraw.transform.right, 180, arraw.shieldAre);
        ////计算值
        //arraw.shieldAre = Handles.ScaleValueHandle(arraw.shieldAre,
        //arraw.transform.position + arraw.transform.forward * arraw.shieldAre,
        //arraw.transform.rotation, 1, Handles.ConeCap, 1);
    }

    private void _DoBodyFreeMoveHandle(Vector3 vPos, int idx)
    {
        WildObstacle obstacle = (WildObstacle)target;
        Vector3 vTransform = obstacle.transform.TransformPoint(vPos);


        EditorGUI.BeginChangeCheck();
        Handles.color = Color.green;
        Handles.Label(vTransform + Vector3.up * 0.2f, "P"+idx);

        
        //NOTE: vBodyHandler will be the body size change difference
        Vector3 vBodyHandler = Handles.FreeMoveHandle(vTransform, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(obstacle.transform.position), Vector3.zero, SphereCap) - vTransform;
        //Vector3 vBodyHandler = Handles.FreeMoveHandle(vTransform, Quaternion.identity, 0.15f * HandleUtility.GetHandleSize(footHold.transform.position), Vector3.zero, EditorCompatibilityUtils.SphereCap) - vTransform;
        vBodyHandler = obstacle.transform.InverseTransformVector(vBodyHandler);

        if (EditorGUI.EndChangeCheck())
        {
            obstacle.PointsList[idx] += new Vector2(vBodyHandler.x, vBodyHandler.y);
            EditorUtility.SetDirty(target);
        }
        
    }

    

    public static void SphereCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType = EventType.Ignore)
    {
        Handles.SphereHandleCap(controlID, position, rotation, size, Event.current.type);
    }
}
