using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldMovableCamera : MonoBehaviour
{
    private Camera mCamera;

    

    float[] cameraBound = new float[4];
    float cameraHalfHeight;
    float cameraHalfWidth;

    Vector3 CameraToMove = Vector3.zero;
    bool isMovingCamera = false;
    float OriginCameraZ = -10;

    public void Init(Camera camera, Rect activeArea)
    {
        this.mCamera = camera;
        OriginCameraZ = camera.transform.position.z;
        InitCameraControl(activeArea);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCemera();
    }


    public void MoveTo(Vector3 target)
    {
        isMovingCamera = true;
        CameraToMove = target;
    }

    void MoveCemera()
    {
        if (!isMovingCamera)
            return;

        CameraToMove = ClampPosInBound(CameraToMove);

        CameraToMove.z = OriginCameraZ;

        if ((CameraToMove - mCamera.transform.localPosition).magnitude < 1e-2)
        {
            mCamera.transform.localPosition = CameraToMove;
            isMovingCamera = false;
        }
        else
        {
            mCamera.transform.localPosition = Vector3.Lerp(mCamera.transform.localPosition, CameraToMove, 0.5f);
        }
    }

    private void InitCameraControl(Rect activeArea)
    {
        cameraBound[0] = activeArea.xMin;
        cameraBound[1] = activeArea.yMin;
        cameraBound[2] = activeArea.xMax;
        cameraBound[3] = activeArea.yMax;


        mCamera.transform.position = new Vector3(0, 0, -10);

        Vector2 cameraBoundInWorld = mCamera.ScreenToWorldPoint(new Vector3(mCamera.pixelWidth, mCamera.pixelHeight, 0));

        cameraHalfHeight = cameraBoundInWorld.y;
        cameraHalfWidth = cameraBoundInWorld.x;
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
}
