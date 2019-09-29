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

 

    public override void OnRelease()
    {

    }

    private void BindGameObject()
    {
        map = GameObject.Find("Map");
        mainCamera = Camera.main;
        IResLoader loader = GameMain.GetInstance().GetModule<ResLoader>();
        playerSymbol = loader.Instantiate("Travel/pawn");
    }



    public override void Init(){
		initCameraControl ();
        BindGameObject();
		//playerSymbol = GameMain.GetInstance ().GetModule<ResLoader> ().Instantiate ("travel/pawn");

		Vector2 startPos = new Vector2(0,0);
		{
			if (startPos.y < (cameraBound [1] + cameraHalfHeight)) {
				startPos.y = (cameraBound [1] + cameraHalfHeight);
			}
			if (startPos.y > (cameraBound [3] - cameraHalfHeight)) {
				startPos.y = (cameraBound [3] - cameraHalfHeight);
			}
			if (startPos.x < (cameraBound [0] + cameraHalfWidth)) {
				startPos.x = (cameraBound [0] + cameraHalfWidth);
			}
			if (startPos.x > (cameraBound [2] - cameraHalfWidth)) {
				startPos.x = (cameraBound [2] - cameraHalfWidth);
			}
		}
		SetMap ();
		mainCamera.transform.position = startPos;
		isMovingMap = false;
		isContinueMovingMap = false;
        Initialized = true;

    }

	public override void Tick(float dTime){
        if (!Initialized) return;
		MoveMap ();
	}




	public void UpdateMoveTarget(Vector3 dragDir){

		Vector3 moveDir = dragDir/15f; //blend
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







	public static float MAX_MAP_SPEED = 20f;
	void MoveMap ()
	{
		if(!isMovingMap && !isContinueMovingMap)
			return;

		if (toMove.y < (cameraBound [1] + cameraHalfHeight)) {
			toMove.y = (cameraBound [1] + cameraHalfHeight);
		}
		if (toMove.y > (cameraBound [3] - cameraHalfHeight)) {
			toMove.y = (cameraBound [3] - cameraHalfHeight);
		}
		if (toMove.x < (cameraBound [0] + cameraHalfWidth)) {
			toMove.x = (cameraBound [0] + cameraHalfWidth);
		}
		if (toMove.x > (cameraBound [2] - cameraHalfWidth)) {
			toMove.x = (cameraBound [2] - cameraHalfWidth);
		}
		toMove.z = mainCamera.transform.localPosition.z;
		if ((toMove - mainCamera.transform.localPosition).magnitude < 0.01f) {
			mainCamera.transform.localPosition = toMove;
		} else {
			mainCamera.transform.localPosition = Vector3.Lerp (mainCamera.transform.localPosition,toMove,0.5f);
		}
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
