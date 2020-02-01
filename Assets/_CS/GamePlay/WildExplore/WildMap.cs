using CosTomUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildMap : MonoBehaviour
{


    public float meterPerUnit = 3; 
    public float Width = 100;
    public float Height = 100;


    public GameObject startGo;
    public GameObject obstacles;
    public GameObject endGo;

    public WildPlayer player;

    public List<WildObstacle> Obstacles = new List<WildObstacle>();
    public float ColDis = 0.3f;
    public ClickableManager2D clickableManager;
    private List<Polygon> polygons;
    public GameObject Mark;
    // Start is called before the first frame update
    void Start()
    {
        clickableManager = GameObject.Find("ClickableManager").GetComponent<ClickableManager2D>();
        Init();
        InitMap();
    }

    void Init()
    {
        ClickableManager2D.BindClickEvent(gameObject, delegate (GameObject go, Vector3 pos) {
            Debug.Log("Clicka");
            Vector3 posInWorld = clickableManager.m_camera.ScreenToWorldPoint(pos);
            Vector3 posLocal = transform.InverseTransformPoint(posInWorld);
            posLocal.z = 0;
            if (Mark != null)
            {
                Mark.transform.localPosition = posLocal;
            }
            player.FinishFollowPath();

            List<Vector2> path = StartSearch(player.transform.localPosition, posLocal);
            if(path.Count > 0)
            {
                player.FollowPath(path);
            }
            
        });
        {
            ClickableEventlistener2D listener = gameObject.AddComponent<ClickableEventlistener2D>();
            listener.BeginDragEvent += delegate (GameObject gb, Vector3 dragDir) {
                
            };

            listener.OnDragEvent += delegate (GameObject gb, Vector3 dragDir) {
                
            };
            listener.EndDragEvent += delegate (GameObject gb, Vector3 dragDir) {
                
            };
        }
    }

    public static void TransformPositionToGamePosition()
    {

    }
    public static void GamePositionToTransformPosition()
    {

    }

    private List<Vector2> ShowPath(AstarSearchNode root)
    {
        List<Vector2> list = new List<Vector2>();
        AstarSearchNode p = root;
        while (p != null)
        {
            list.Add(p.pnt.Pos);
            p = p.pre;
        }
        list.Reverse();
        return list;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector2 startPos = startGo.transform.localPosition;
            Vector2 endPos = endGo.transform.localPosition;
            StartSearch(startPos, endPos);
        }

        if(pathToShow != null)
        {
            for(int i = 0; i < pathToShow.Count-1; i++)
            {
                Debug.DrawLine(pathToShow[i],pathToShow[i+1],Color.red);
            }
        }

        if (dis != null)
        {
            for (int i = 0; i < dis.GetLength(0); i++)
            {
                for (int j = i + 1; j < dis.GetLength(1); j++)
                {
                    if (dis[i, j] < float.MaxValue)
                    {
                        Debug.DrawLine(PolyPoints[i].Pos, PolyPoints[j].Pos);
                    }
                }
            }
        }
    }
    class Polygon
    {
        public int Num;
        public PolygonPoint[] Points;
    }

    class AstarSearchNode
    {
        public PolygonPoint pnt;
        public float f;
        public float h;
        public float g;
        public AstarSearchNode pre;

        public AstarSearchNode(PolygonPoint pnt)
        {
            this.pnt = pnt;
        }

        public AstarSearchNode(PolygonPoint pnt, float g, float h, float f)
        {
            this.pnt = pnt;
            this.f = f;
            this.g = g;
            this.h = h;
        }
    }

    class NodeComparater : IComparer<AstarSearchNode>
    {
        public int Compare(AstarSearchNode n1, AstarSearchNode n2)
        {
            if(n1.f > n2.f)
            {
                return -1;
            }else if(n1.f < n2.f)
            {
                return 1;
            }
            return 0;
        }
    }

    private List<Vector2> pathToShow;
    public List<Vector2> StartSearch(Vector2 startPos, Vector2 endPos)
    {
        pathToShow = null;

        Dictionary<PolygonPoint, List<PolygonPoint>> routeMap = new Dictionary<PolygonPoint, List<PolygonPoint>>();


        if(polygons == null)
        {
            return new List<Vector2>();
        }


        for(int i = 0; i < dis.GetLength(0); i++)
        {
            for(int j = 0; j < dis.GetLength(1); j++)
            {
                if (dis[i,j] < float.MaxValue)
                {
                    if (!routeMap.ContainsKey(PolyPoints[i]))
                    {
                        routeMap.Add(PolyPoints[i], new List<PolygonPoint>());
                    }
                    
                    routeMap[PolyPoints[i]].Add(PolyPoints[j]);
                }
                
            }
        }


       

        PolygonPoint startPoint = new PolygonPoint(startPos);
        PolygonPoint endPoint = new PolygonPoint(null,PolyPoints.Count, endPos);


        if(canPass(startPos, endPos))
        {
            Debug.Log("直线可达");
            return new List<Vector2>() {startPos,endPos};
        }

        for (int i = 0; i < polygons.Count; i++)
        {
            //计算新加入两个点以后的扩展图
            if(isPointOutPoly(startPos, polygons[i]) == -1)
            {
                Debug.Log("初始卡住啦");
                return new List<Vector2>();
            }
            for(int j = 0; j < polygons[i].Num; j++)
            {
                if (canPass(startPos, polygons[i].Points[j].Pos))
                {
                    if (!routeMap.ContainsKey(startPoint))
                    {
                        routeMap.Add(startPoint, new List<PolygonPoint>());
                    }
                    routeMap[startPoint].Add(polygons[i].Points[j]);
                    //add routes
                }
                if(canPass(endPos, polygons[i].Points[j].Pos))
                {
                    if (!routeMap.ContainsKey(polygons[i].Points[j]))
                    {
                        routeMap.Add(polygons[i].Points[j], new List<PolygonPoint>());
                    }
                    routeMap[polygons[i].Points[j]].Add(endPoint);
                    //add routes
                }
            }
        }

        float[] disss = new float[PolyPoints.Count+1];
        for(int i = 0; i < disss.Length; i++)
        {
            disss[i] = float.MaxValue;
        }
        float originH = (endPoint.Pos - startPoint.Pos).magnitude;
        AstarSearchNode firstNode = new AstarSearchNode(startPoint,0, originH, originH);
        AstarSearchNode minHNode = firstNode;
        PriorityQueue<AstarSearchNode> pq = new PriorityQueue<AstarSearchNode>(new NodeComparater());
        pq.Push(firstNode);
        while (pq.Count > 0)
        {
            AstarSearchNode pHead = pq.Pop();

            if(pHead.pnt == endPoint)
            {
                Debug.Log("you way");
                pathToShow = ShowPath(pHead);
                //found
                return new List<Vector2>(pathToShow);
            }
            if (disss[pHead.pnt.PntIdx] < float.MaxValue)
            {
                //表示来过
                continue;
            }

            if (routeMap.ContainsKey(pHead.pnt))
            {
                for(int i = 0; i < routeMap[pHead.pnt].Count; i++)
                {
                    PolygonPoint next = routeMap[pHead.pnt][i];
                    
                    float newG = (next.Pos - pHead.pnt.Pos).magnitude + pHead.g;
                    float newH = (next.Pos - endPoint.Pos).magnitude;
                    float newF = newG + newH;
                    AstarSearchNode newNode = new AstarSearchNode(next,newG,newH,newF);
                    newNode.pre = pHead;
                    if (newF < disss[next.PntIdx])
                    {
                        pq.Push(newNode);
                    }

                    if(newH < minHNode.h)
                    {
                        minHNode = newNode;
                    }
                }
            }
        }
        Debug.Log("no way");
        //如果没找到
        //返回minHNode

        pathToShow = ShowPath(minHNode);
        return new List<Vector2>(pathToShow);
    }


    

    class PolygonPoint
    {
        public Polygon polygon;
        public int PntIdx;
        public Vector2 Pos;

        public PolygonPoint(Polygon parent, int pntIdx, Vector2 Pos)
        {
            this.polygon = parent;
            this.PntIdx = pntIdx;
            this.Pos = Pos;
        }
        public PolygonPoint(Vector2 Pos)
        {
            this.Pos = Pos;
        }
    }
    List<PolygonPoint> PolyPoints = new List<PolygonPoint>();
    float[,] dis;
    private void InitMap()
    {
        foreach(Transform child in obstacles.transform)
        {
            if(child.gameObject.GetComponent<WildObstacle>() != null)
            {
                Obstacles.Add(child.gameObject.GetComponent<WildObstacle>());
            }
        }

        polygons = new List<Polygon>();
        PolyPoints.Clear();
        int pIdx = 0;
        for(int i=0;i< Obstacles.Count; i++)
        {
            List<Vector2> expandedPoints = Obstacles[i].GetExpandedOuterInWorld(ColDis);
            Polygon polygon = new Polygon();
            polygon.Num = expandedPoints.Count;
            polygon.Points = new PolygonPoint[polygon.Num];

            for (int j=0;j< expandedPoints.Count; j++)
            {
                Vector2 p = transform.InverseTransformPoint(expandedPoints[j]);
                PolygonPoint pp = new PolygonPoint(polygon, pIdx++, p);
                polygon.Points[j] = pp;
                PolyPoints.Add(pp);
            }
            polygons.Add(polygon);
        }
        dis = new float[pIdx,pIdx];
        for(int i = 0; i < pIdx; i++)
        {
            for(int j=0;j< pIdx; j++)
            {
                dis[i, j] = float.MaxValue;
            }
        }
        //Dictionary<PolygonPoint, PolygonPoint> map = new Dictionary<PolygonPoint, PolygonPoint>();
        for (int i = 0; i < polygons.Count; i++)
        {
            for(int j = i; j < polygons.Count; j++)
            {
                calculateRoutes(polygons[i], polygons[j],i==j);
            }
        }


    }

    

    private void calculateRoutes(Polygon from, Polygon to, bool isSelf)
    {
        //仅针对凸多边形
        if (isSelf)
        {
            for (int i = 0; i < from.Num; i++)
            {
                if (canPass(from.Points[i].Pos, from.Points[(i+from.Num-1)%from.Num].Pos))
                {
                    dis[from.Points[i].PntIdx, from.Points[(i + from.Num - 1) % from.Num].PntIdx] = (from.Points[i].Pos - from.Points[(i + from.Num - 1) % from.Num].Pos).magnitude;
                    dis[from.Points[(i + from.Num - 1) % from.Num].PntIdx,from.Points[i].PntIdx] = (from.Points[i].Pos - from.Points[(i + from.Num - 1) % from.Num].Pos).magnitude;
                }
                if (canPass(from.Points[i].Pos, from.Points[(i + 1) % from.Num].Pos))
                {
                    dis[from.Points[i].PntIdx, from.Points[(i + 1) % from.Num].PntIdx] = (from.Points[i].Pos - from.Points[(i + 1) % from.Num].Pos).magnitude;
                    dis[from.Points[(i + 1) % from.Num].PntIdx, from.Points[i].PntIdx] = (from.Points[i].Pos - from.Points[(i + 1) % from.Num].Pos).magnitude;
                }
            }
            return;
        }
        for (int i = 0; i < from.Num; i++)
        {
            for(int j = 0; j < to.Num; j++)
            {
                
                if(canPass(from.Points[i].Pos, to.Points[j].Pos))
                {
                    dis[from.Points[i].PntIdx,to.Points[j].PntIdx] = (from.Points[i].Pos - to.Points[j].Pos).magnitude;
                    dis[to.Points[j].PntIdx,from.Points[i].PntIdx] = (from.Points[i].Pos - to.Points[j].Pos).magnitude;
                }
            }
        }
    }

    private bool canPass(Vector2 from, Vector2 to)
    {
        for (int i = 0; i < polygons.Count; i++)
        {
            Polygon polygon = polygons[i];
            for(int j = 0; j < polygon.Num; j++)
            {
                Vector2 edgeV1 = polygon.Points[(j + 1) % polygon.Num].Pos;
                Vector2 edgeV2 = polygon.Points[j].Pos;

                int line1v1ToPoly2 = isPointOutPoly(from, polygon);
                int line1v2ToPoly2 = isPointOutPoly(to, polygon);
                if (line1v1ToPoly2 < 0 || line1v2ToPoly2 < 0)
                {
                    return false;
                }

                if (areLinesCross(from, to, edgeV1, edgeV2, polygon))
                {
                    return false;
                }

            }
        }
        return true;
    }

    private bool areLinesCross(Vector2 line1v1, Vector2 line1v2, Vector2 line2v1, Vector2 line2v2,Polygon line2Polygon)
    {
        if(Vector3.Cross(line1v2 - line1v1, line2v2 - line2v1).z == 0)
        {
            //平行或重合 不碰撞
            return false;
        }

        

        {
            float a1 = Vector3.Cross(line1v1 - line2v1, line2v2 - line2v1).z;
            float a2 = Vector3.Cross(line1v2 - line2v1, line2v2 - line2v1).z;
            if (a1 * a2 >= 0)
            {
                return false;
            }
            //if (a1 * a2 == 0)
            //{
            //    if (isPointOutPoly(line1v1, line2Polygon) || isPointOutPoly(line1v2, line2Polygon))
            //    {
            //        return false;
            //    }

            //}
        }
        {
            float a1 = Vector3.Cross(line2v1 - line1v1, line1v2 - line1v1).z;
            float a2 = Vector3.Cross(line2v2 - line1v1, line1v2 - line1v1).z;
            if (a1 * a2 >= 0)
            {
                return false;
            }

        }

        return true;
    }

    private int isPointOutPoly(Vector2 point, Polygon polygon)
    {
        float sign;
        {
            Vector2 p0 = polygon.Points[0].Pos;
            Vector2 p1 = polygon.Points[1].Pos;
            sign = Vector3.Cross(point - p0, p1 - p0).z;
        }
        if (sign == 0) return 0;
        for(int i = 1; i < polygon.Num; i++)
        {
            Vector2 p0 = polygon.Points[i].Pos;
            Vector2 p1 = polygon.Points[(i+1)%polygon.Num].Pos;
            float a = Vector3.Cross(point - p0, p1 - p0).z;
            if(a == 0)
            {
                return 0;
            }
            if(a * sign < 0)
            {
                return 1;
            }
        }
        return -1;
    }
}
