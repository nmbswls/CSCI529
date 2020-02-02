using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPlayer : MonoBehaviour
{
    WildMap wildMap;

    public Vector2 PosXY;
    private List<Vector2> targets;
    private bool followPath;
    // Start is called before the first frame update
    void Start()
    {
        wildMap = GameObject.Find("Map").GetComponent<WildMap>();
        Init();
    }
    void Init()
    {
        followPath = false;
        ClickableManager2D.BindClickEvent(gameObject, delegate (GameObject go, Vector3 pos) {
            Debug.Log("Click player");
            wildMap.mainCtrl.ShowPop(transform.position);
        });
    }
    //全局倍速
    // Update is called once per frame
    void Update()
    {
        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            dir += Vector3.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += Vector3.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += Vector3.right;
        }
        dir = SquareProjectToCircle(dir);
        if (dir.magnitude > 0.1f)
        {
            if (followPath)
            {
                FinishFollowPath();
            }
            transform.position += Time.deltaTime * dir * 2f;
        }
        else
        {
            if (followPath)
            {
                while (targets.Count > 0)
                {
                    float diff = (PosXY - targets[0]).magnitude;
                    if (diff < 1e-2)
                    {
                        targets.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
                if(targets.Count > 0)
                {
                    Vector3 followDir = (targets[0] - PosXY).normalized;
                    transform.position += Time.deltaTime * followDir * 2f;
                }
                else
                {
                    FinishFollowPath();
                }
            }
            
            
        }
        PosXY = new Vector2(transform.position.x, transform.position.y);
        

    }

    private static Vector2 SquareProjectToCircle(Vector2 input)
    {
        input.x = Mathf.Clamp(input.x, -1, 1);
        input.y = Mathf.Clamp(input.y,-1, 1);
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1-(input.y * input.y)/2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);
        return output;
    }


    public void FinishFollowPath()
    {
        followPath = false;
        this.targets = null;
        //Debug.Log("寻路结束");
    }
    public void FollowPath(List<Vector2> targets)
    {
        followPath = true;
        this.targets = targets;
    }

    private void FixedUpdate()
    {
        
    }

    public void TriggerSomething(string otherObj)
    {

    }


}
