using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildObstacle : MonoBehaviour
{
    // 世界
    public List<Vector2> PointsList = new List<Vector2>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Ajust")]
    void Adjust()
    {
        for(int i = 0; i < PointsList.Count; i++)
        {
            PointsList[i] = AdjustPos(PointsList[i],1);
        }
        transform.localPosition = AdjustPos(transform.localPosition, 1);
        InitCollider();
    }

    public static Vector2 AdjustPos(Vector2 pos, int digit)
    {
        float rate = 1;
        for (int i = 0; i < digit; i++)
        {
            rate *= 10;
        }
        float x = (int)Mathf.Round(pos.x * rate) / rate;
        float y = (int)Mathf.Round(pos.y * rate) / rate;

        return new Vector2(x, y);
    }

    public List<Vector2> GetExpandedOuterInWorld(float colDis)
    {
        if (PointsList.Count < 3)
        {
            return new List<Vector2>(PointsList);
        }
        List<Vector2> ret = new List<Vector2>();
        float L = colDis;
        int numPoints = PointsList.Count;
        for (int i = 0; i < PointsList.Count; i++)
        {
            Vector2 v1 = PointsList[(i + numPoints - 1) % numPoints] - PointsList[i];
            Vector2 v2 = PointsList[(i + 1) % numPoints] - PointsList[i];
            float sin = Vector3.Cross(v1.normalized, v2.normalized).magnitude;
            float mo = L / sin;
            Vector2 dir = -(v1.normalized + v2.normalized);
            Vector2 diff = dir * mo;
            Vector2 pInWorld = transform.TransformPoint(PointsList[i] + diff);
            ret.Add(pInWorld);
        }
        return ret;
    }


    private void InitCollider()
    {


        if(PointsList.Count < 2)
        {
            Debug.Log("点太少了");
            return;
        }

        //List<Vector2> ret = GetOuter();
        List<Vector2> edgePoints = PointsList;

        DestroyImmediate(gameObject.GetComponent<Collider2D>());
        
        EdgeCollider2D collider = gameObject.AddComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[edgePoints.Count + 1];
        for(int i=0;i< edgePoints.Count; i++)
        {
            points[i] = edgePoints[i];
        }
        points[edgePoints.Count] = edgePoints[0];

        collider.points = points;
        collider.usedByEffector = true;
    }
}
