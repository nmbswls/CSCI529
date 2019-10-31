using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;




public class TravelMapInfo
{
    public string MapName;
    public List<TravelPotInfo> pots = new List<TravelPotInfo>();
    public List<int[]> edges = new List<int[]>();
}

public class TravelGameState
{
    public List<TravelPot> Pots = new List<TravelPot>();

    public List<LineRenderer> lines = new List<LineRenderer>();

    public int PlayerPotIdx = 0;

    public Dictionary<int, List<int>> graph = new Dictionary<int, List<int>>();
}


public class TravelGameMode : GameModeBase {

    public TravelGameState state = new TravelGameState();



    bool isMovingCamera = false;
	bool isContinueMovingCamera = false;

    bool isMovingPlayer = false;
    int highlightPotIdx = -1;

	Vector3 CameraToMove = Vector3.zero;


	float[] cameraBound = new float[4];
	float cameraHalfHeight;
	float cameraHalfWidth;


	public GameObject Map;
    Transform PotLyaer;
    GameObject Pots;
    Transform LineLayer;
    

	public Camera mainCamera;
	GameObject PlayerPawn;


    ClickableManager2D clickableManager;
    IResLoader mResLoader;
    IUIMgr pUIMgr;

    TravelUI travelUI;

    private static int CameraZ = -10;
    private static int PawnZ = 10;
    private static int LineZ = 95;

    private void GeneratePots(TravelMapInfo info)
    {
        int idx = 0;
        foreach(Transform child in Pots.transform)
        {
            GameObject potGo = mResLoader.Instantiate("Travel/Pot",child);
            TravelPot pot = potGo.GetComponent<TravelPot > ();
            pot.Init(info.pots[idx],this);
            state.Pots.Add(pot);
            idx += 1;
        }




        foreach (int[] edge in info.edges)
        {
            GameObject lineGo = mResLoader.Instantiate("Travel/Line", LineLayer);
            LineRenderer line = lineGo.GetComponent<LineRenderer>();
            Vector3 e0 = state.Pots[edge[0]].transform.position;
            Vector3 e1 = state.Pots[edge[1]].transform.position;
            
            Vector3[] positions = { e0 + Vector3.forward, e1 + Vector3.forward };
            line.SetPositions(positions);
            state.lines.Add(line);
        }

        PlayerPawn = mResLoader.Instantiate("Travel/pawn");
        int startIdx = 0;
        Vector2 startPos = state.Pots[startIdx].transform.position;
        PlayerPawn.transform.position = new Vector3(startPos.x, startPos.y, PawnZ);
        Vector3 cameraPos = ClampPosInBound(startPos);
        cameraPos.z = mainCamera.transform.position.z;
        mainCamera.transform.position = cameraPos;
    }

    public override void Init()
    {
        BindGameObject();
        InitCameraControl();

        travelUI = pUIMgr.ShowPanel("TravelPanel",false) as TravelUI;

        SetMap();

        LoadMap(FakeMapInfo());

        isMovingCamera = false;
        isContinueMovingCamera = false;
        Initialized = true;
    }


    private TravelMapInfo FakeMapInfo()
    {
        TravelMapInfo ret = new TravelMapInfo();

        for(int i = 0; i < 7; i++)
        {
            TravelPotInfo pot = new TravelPotInfo("地点" + i);
            pot.Opts = new List<string>(new string[] {"a","b","c"});
            ret.pots.Add(pot);
        }

        List<int[]> edges = new List<int[]>();
        edges.Add(new int[] { 0, 1});
        edges.Add(new int[] { 1, 2 });
        edges.Add(new int[] { 1, 3 });
        edges.Add(new int[] { 2, 4 });
        edges.Add(new int[] { 2, 5 });
        edges.Add(new int[] { 4, 6 });
        edges.Add(new int[] { 5, 6 });

        ret.MapName = "Map00";
        ret.edges = edges;
        return ret;
    }


    public void FinishTravel()
    {
        GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main");
        mainCamera.transform.position = new Vector3(0,0, CameraZ);
    }

    public override void OnRelease()
    {
        mResLoader.UnloadAsset("Travel/pawn");
    }

    private void BindGameObject()
    {
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        pUIMgr = GameMain.GetInstance().GetModule<UIMgr>();

        clickableManager = GameObject.Find("ClickManager").GetComponent<ClickableManager2D>() ;
        PotLyaer = GameObject.Find("Pots").transform;
        LineLayer = GameObject.Find("Lines").transform;

        Map = GameObject.Find("Map");
        mainCamera = pUIMgr.GetCamera();

        clickableManager.m_camera = mainCamera;

    }

    public void LoadMap(TravelMapInfo info)
    {
        int numPot = info.pots.Count;
        Pots = mResLoader.Instantiate("Travel/Maps/"+ info.MapName, PotLyaer);
        GeneratePots(info);

        for(int i = 0; i < info.edges.Count; i++)
        {
            int[] edge =info.edges[i];
            if (!state.graph.ContainsKey(edge[0]))
            {
                state.graph.Add(edge[0], new List<int>());
            }
            if (!state.graph.ContainsKey(edge[1]))
            {
                state.graph.Add(edge[1], new List<int>());
            }
            state.graph[edge[0]].Add(edge[1]);
            state.graph[edge[1]].Add(edge[0]);
        }
    }


    public void ChoosePot(TravelPot pot)
    {
        if (state.Pots.IndexOf(pot)==-1)
        {
            return;
        }

        int index = state.Pots.IndexOf(pot);

        if (isMovingPlayer)
            return;

        if (highlightPotIdx != index)
        {
            if(highlightPotIdx != -1)
            {
                state.Pots[highlightPotIdx].CancelSelect();
            }
            state.Pots[index].Selected();
            highlightPotIdx = index;

            ChangeDetail(pot);

        }
        else
        {
            if (state.PlayerPotIdx == index)
            {
                return;
            }
            if (!state.graph.ContainsKey(state.PlayerPotIdx))
            {
                return;
            }
            if (state.graph[state.PlayerPotIdx].IndexOf(index) == -1)
            {
                return;
            }
            isMovingPlayer = true;
            Vector3 target = state.Pots[index].transform.position;
            target.z = PawnZ;
            Tween tween = DOTween.To
                (
                    () =>  PlayerPawn.transform.position,
                    (x) => {  
                            PlayerPawn.transform.position = x;
                            //Vector3 cameraPos = PlayerPawn.transform.position;
                            //cameraPos.z = CameraZ;
                            //isMovingMap = true;
                            //CameraToMove = cameraPos;
                         },
                    target,
                    0.6f
                ).OnComplete(delegate {
                    isMovingPlayer = false;
                    state.PlayerPotIdx = index;
                });
        }
    }





    public override void Tick(float dTime){
        if (!Initialized) return;
		MoveCemera ();

        if (Input.GetKeyDown(KeyCode.A))
        {
            FinishTravel();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach(TravelPot pot in state.Pots)
            {
                Debug.Log(pot.GetComponent<ClickableEventlistener2D>().hasClickEvent());

            }
        }
    }

    public void ChangeDetail(TravelPot pot)
    {

        travelUI.ChangeDetail(pot);
    }

    public static float MAX_MAP_SPEED = 20f;
    public static float MAX_DIFF = 0.01f;
    public static float MAP_MOVE_RATE = 0.067f;

    void MoveCemera()
    {
        if (!isMovingCamera && !isContinueMovingCamera)
            return;

        CameraToMove = ClampPosInBound(CameraToMove);

        CameraToMove.z = CameraZ;

        if ((CameraToMove - mainCamera.transform.localPosition).magnitude < MAX_DIFF)
        {
            mainCamera.transform.localPosition = CameraToMove;
        }
        else
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, CameraToMove, 0.5f);
        }
    }


    public void UpdateMoveTarget(Vector3 dragDir){
		Vector3 moveDir = dragDir * MAP_MOVE_RATE; //blend
		moveDir.z = 0;
		CameraToMove = mainCamera.transform.localPosition - moveDir;
	}




	public void SetMap(){
		{
			ClickableEventlistener2D listener = Map.AddComponent<ClickableEventlistener2D> ();
			listener.BeginDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				isMovingCamera = true;
				isContinueMovingCamera = false;
				CameraToMove = mainCamera.transform.localPosition;
			};

			listener.OnDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				UpdateMoveTarget(dragDir);
			};
			listener.EndDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				isContinueMovingCamera = true;
				isMovingCamera = false;
			};
		}

	}







	

    private Vector3 ClampPosInBound(Vector3 toClamp)
    {
        if (toClamp.y < (cameraBound[1] + cameraHalfHeight))
        {
            toClamp.y = (cameraBound[1] + cameraHalfHeight);
        }
        if (toClamp.y > (cameraBound[3] - cameraHalfHeight))
        {
            toClamp.y = (cameraBound[3] - cameraHalfHeight);
        }
        if (toClamp.x < (cameraBound[0] + cameraHalfWidth))
        {
            toClamp.x = (cameraBound[0] + cameraHalfWidth);
        }
        if (toClamp.x > (cameraBound[2] - cameraHalfWidth))
        {
            toClamp.x = (cameraBound[2] - cameraHalfWidth);
        }
        return toClamp;

    }


    public void InitCameraControl(){

		SpriteRenderer activeArea = Map.GetComponent<SpriteRenderer> ();
		cameraBound[0] = -activeArea.bounds.size.x/2;
		cameraBound[1] = -activeArea.bounds.size.y/2;
		cameraBound[2] = activeArea.bounds.size.x/2;
		cameraBound[3] = activeArea.bounds.size.y/2;

		mainCamera.transform.position = new Vector3 (0,0,-10);

		Vector2 cameraBoundInWorld = mainCamera.ScreenToWorldPoint (new Vector3 (mainCamera.pixelWidth, mainCamera.pixelHeight, 0));

		cameraHalfHeight = cameraBoundInWorld.y;
		cameraHalfWidth = cameraBoundInWorld.x;
	}


}
