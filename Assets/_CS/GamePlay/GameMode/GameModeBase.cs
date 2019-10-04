using UnityEngine;
using System.Collections;

public class GameModeBase
{
    public delegate void OnGameFinishedDlg();
    public delegate void OnInitDlg();
    public OnGameFinishedDlg GameFinishedCallback;
    public bool Initialized = false;

	public virtual void Tick(float dTime){
		return;
	}
	public virtual void Init(){
		return;
	}

    public virtual void OnRelease()
    {

    }
}

