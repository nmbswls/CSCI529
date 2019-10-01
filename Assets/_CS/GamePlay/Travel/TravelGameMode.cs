using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelGameMode : GameModeBase {



	bool isMovingMap = false;
	bool isContinueMovingMap = false;

	Vector3 toMove = Vector3.zero;


	float[] cameraBound = new float[4];
	float cameraHalfHeight;
	float cameraHalfWidth;


	public GameObject map;

	public Camera mainCamera;
	public GameObject playerSymbol;

    IResLoader mResLoader;


    public void FinishTravel()
    {
        GameMain.GetInstance().GetModule<CoreManager>().ChangeScene("Main");
    }

    public override void OnRelease()
    {
        mResLoader.UnloadAsset("Travel/pawn");
    }

    private void BindGameObject()
    {
        map = GameObject.Find("Map");
        mainCamera = Camera.main;
        mResLoader = GameMain.GetInstance().GetModule<ResLoader>();
        playerSymbol = mResLoader.Instantiate("Travel/pawn");
        Debug.Log("finish init travel");
    }



    public override void Init(){
        BindGameObject();
        initCameraControl();

		Vector3 startPos = new Vector3(0,0,0);
		{
            startPos = ClampPosInBound(startPos);
        }
		SetMap ();
		mainCamera.transform.position = new Vector3(startPos.x, startPos.y, mainCamera.transform.position.z);

		isMovingMap = false;
		isContinueMovingMap = false;
        Initialized = true;

    }

	public override void Tick(float dTime){
        if (!Initialized) return;
		MoveMap ();

        if (Input.GetKeyDown(KeyCode.A))
        {
            FinishTravel();
        }
    }


    public static float MAX_MAP_SPEED = 20f;
    public static float MAX_DIFF = 0.01f;
    public static float MAP_MOVE_RATE = 0.067f;

    void MoveMap()
    {
        if (!isMovingMap && !isContinueMovingMap)
            return;

        toMove = ClampPosInBound(toMove);

        toMove.z = mainCamera.transform.localPosition.z;

        if ((toMove - mainCamera.transform.localPosition).magnitude < MAX_DIFF)
        {
            mainCamera.transform.localPosition = toMove;
        }
        else
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, toMove, 0.5f);
        }
    }

    public void UpdateMoveTarget(Vector3 dragDir){
		Vector3 moveDir = dragDir * MAP_MOVE_RATE; //blend
		moveDir.z = 0;
		toMove = mainCamera.transform.localPosition - moveDir;
	}


	public void SetMap(){
		{
			ClickableEventlistener2D listener = map.AddComponent<ClickableEventlistener2D> ();
			listener.BeginDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				isMovingMap = true;
				isContinueMovingMap = false;
				toMove = mainCamera.transform.localPosition;
			};

			listener.OnDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				UpdateMoveTarget(dragDir);
			};
			listener.EndDragEvent += delegate(GameObject gb,Vector3 dragDir) {
				isContinueMovingMap = true;
				isMovingMap = false;
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


    public void initCameraControl(){
		SpriteRenderer activeArea = map.GetComponent<SpriteRenderer> ();
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
