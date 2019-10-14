using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using DG.Tweening;


public class DialogView : BaseView{

	public Button skipButton;
	public Transform mask;
	public Text nameText;
	public Text dialogContent;
	public RawImage bg;

	public DialogBranchView branchView;

	public Transform LihuiContainer;
	public List<LihuiView> LihuiList = new List<LihuiView> ();

}

public class DialogBranchView{

	public RectTransform root;
	public Transform BranchContainer;
	public List<DialogBranchSingle> Branches = new List<DialogBranchSingle>();
	public void BindView(Transform root){
		this.root = (RectTransform)root;
		BranchContainer = root.GetChild (0);
		foreach (Transform child in BranchContainer) {
			//Button btn = child.GetComponent<Button> ();
			DialogBranchSingle v = new DialogBranchSingle ();
			v.BindView (child);
			Branches.Add (v);
		}
	}
}

public class DialogBranchSingle{
	public RectTransform root;
	public Button btn;
	public Text Title;
	public CanvasGroup Cg;

	public void BindView(Transform root){
		this.root = (RectTransform)root;
		btn = root.GetChild(0).GetComponent<Button> ();
		Title = root.GetChild (0).GetComponentInChildren<Text> ();
		Cg = root.GetComponent<CanvasGroup> ();
	}
}

public class LihuiView{

	public RectTransform root;
	public Image pic;
	public bool NeedMove = false;
	public Vector2 Target = Vector2.zero;

	public void BindView(Transform root){
		this.root = (RectTransform)root;
		this.pic = root.GetChild (0).GetComponent<Image> ();
	}
}

public class DialogModel : BaseModel{
	public string dialogId = "";
	public List<DialogFrameBase> frames = new List<DialogFrameBase>();
	public List<string> LihuiIds = new List<string>();

	//public Vector2[] LihuiTargetMovingPos = new Vector2[4];
}




public class DialogManager : UIBaseCtrl<DialogModel,DialogView>
{

	public delegate void OnDialogEnd(string[] args);
	public event OnDialogEnd DiglogEndEvent;

	DialogModule dm;

	public static float wordSpeed = 10f;

	//状态
	bool isReading = false;
	bool isMovingLihui = false;
	bool isEffecting = false;
	bool isBranchFlashing = false;
	int numLihuiNeedMove = 0;

	bool locked = false;

	int frameIdx = -1;
	float cursorF = 0.0f;
	int cursorI = 0;

	public override void Init(){
		dm = GameMain.GetInstance ().GetModule<DialogModule> ();
		
	}

    public void StartDialog(string dialogId, OnDialogEnd callback=null)
    {
        DialogBlock block = dm.LoadDialog(dialogId);
        DiglogEndEvent = callback;
        if (block == null)
        {
            Debug.Log("No Dialog Found");
            mUIMgr.CloseCertainPanel(this);
            return;
        }
        model.frames = block.frames;
        model.dialogId = dialogId;
        frameIdx = 0;
    }

    public override void PostInit(){
		isReading = true;
		frameIdx = -1;
		view.dialogContent.text = "";
		//view.bg. = null;
		view.nameText.text = "";
		view.branchView.root.gameObject.SetActive (false);
	}

	public override void BindView ()
	{
		view.bg = root.GetChild(0).GetComponent<RawImage>();
		view.skipButton = root.Find ("Btn_Skip").GetComponent<Button>();

        view.nameText = root.Find("Speaker").GetComponentInChildren<Text> ();
		view.dialogContent = root.Find("DIalog").GetComponentInChildren<Text>();
		view.LihuiContainer = root.Find ("LihuiContainer");
		DialogBranchView branchView = new DialogBranchView ();
		branchView.BindView (root.Find ("BranchesPanel"));
		view.branchView = branchView;

	}


	public Vector3[] targetPos = new Vector3[4];

	public override void RegisterEvent(){

		ClickEventListerner listener = view.bg.gameObject.AddComponent<ClickEventListerner> ();
		listener.OnClickEvent += delegate(PointerEventData eventData) {
			ClickScreen();
		};

		foreach (DialogBranchSingle vv in view.branchView.Branches) {
			int idx = view.branchView.Branches.IndexOf (vv);
			vv.btn.onClick.AddListener(delegate() {
				ChooseBranch(idx);
			});
		}
        view.skipButton.onClick.RemoveAllListeners();
        view.skipButton.onClick.AddListener(delegate
        {

            finishDialog();

        });

    }

	public void ChooseBranch(int idx){
		if (frameIdx < 0 || frameIdx >= model.frames.Count) {
			return;
		}
		if (model.frames [frameIdx].DialogType != eDialogFrameType.SHOW_BRANCH) {
			return;
		}
		DialogFrameBranch realFrame = model.frames [frameIdx] as DialogFrameBranch;
		if (idx < 0 || idx > realFrame.Choices.Count) {
			return;
		}
		ChooseBranchEffect (idx);

	}

	public void ChooseBranchEffect(int idx){
		//
		for (int i = 0; i < view.branchView.Branches.Count; i++) {
			DialogBranchSingle brch = view.branchView.Branches [i];
			if (idx == i) {
				brch.root.DOShakeRotation (2f).OnComplete (delegate() {
					view.branchView.root.gameObject.SetActive (false);
					nextAction ();
				});
				
			}else if (brch.root.gameObject.activeSelf) {
				
				Tween tween = DOTween.To
					(
						()  => brch.Cg.alpha, 
						(x) => brch.Cg.alpha = x, 
						0, 
						0.5f
					);
			}
		}
	}

	public override void Tick(float dTime){
		
		if (frameIdx < 0 || frameIdx >= model.frames.Count) {
			return;
		}

		if (model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_TEXT) {
			if (!isReading)
				return;

			DialogFrameText realFrame = model.frames [frameIdx] as DialogFrameText;

			if (cursorI >= realFrame.TextLines.Count) {
				isReading = false;
				return;
			}

			cursorF += Time.deltaTime * wordSpeed;

			int newCursorIdx = (int)cursorF;
			if (newCursorIdx == cursorI) {
				return;
			}

			if (newCursorIdx > realFrame.TextLines.Count) {
				newCursorIdx = realFrame.TextLines.Count;
			}

			for (int i = cursorI; i < newCursorIdx; i++) {
				textAppend (realFrame.TextLines[i]);
			}
			cursorI = newCursorIdx;

		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_LIHUI){
			if (!isMovingLihui) {
				
				DialogFrameLihui realFrame = model.frames [frameIdx] as DialogFrameLihui;

				for (int i = 0; i < view.LihuiList.Count; i++) {
					view.LihuiList [i].NeedMove = true;
				}
				numLihuiNeedMove = 0;
				for (int i = 0; i < realFrame.Opts.Count; i++) {
					if (realFrame.Opts [i] == "Enter") {
						if (realFrame.SlotIdxs [i] > model.LihuiIds.Count) {
							realFrame.SlotIdxs [i] = model.LihuiIds.Count;
						}
						model.LihuiIds.Insert (realFrame.SlotIdxs [i],realFrame.Lids [i]);
						GameObject l = GameMain.GetInstance ().GetModule<ResLoader> ().Instantiate ("Dialog/LihuiView",view.LihuiContainer);
						if (l == null) {
							Debug.Log ("error");
							continue;
						}
						LihuiView vv = new LihuiView ();
						vv.BindView (l.transform);
						vv.NeedMove = false;
						vv.Target = Vector2.zero;

						view.LihuiList.Insert (realFrame.SlotIdxs [i],vv);
					}else if(realFrame.Opts [i] == "Leave"){
						model.LihuiIds.RemoveAt (realFrame.SlotIdxs [i]);
						LihuiView vv = view.LihuiList [realFrame.SlotIdxs [i]];
						view.LihuiList.RemoveAt (realFrame.SlotIdxs [i]);
						Vector2 target = vv.root.anchoredPosition + Vector2.up * 1000f;
						Tween tween = DOTween.To
							(
								()  => vv.root.anchoredPosition, 
								(x) => vv.root.anchoredPosition = x, 
								target, 
								0.5f
							).OnComplete(delegate() {
								GameObject.Destroy(vv.root.gameObject);
							});
					}
				}

				for (int i=0;i<view.LihuiList.Count;i++) {
					Vector2 target = GetPosition (i);
                    view.LihuiList[i].root.SetSiblingIndex(i);

                    if (!view.LihuiList [i].NeedMove) {
						view.LihuiList [i].root.anchoredPosition = target;
					} else {
						LihuiView vv = view.LihuiList [i];
						if ((vv.root.anchoredPosition - target).magnitude < 1f) {
							continue;
						}
						numLihuiNeedMove += 1;
						Tween tween = DOTween.To
							(
								()  => vv.root.anchoredPosition, 
								(x) => vv.root.anchoredPosition = x, 
								target, 
								0.5f
							).OnComplete(delegate() {
								numLihuiNeedMove -= 1;
							});
					}
				}
				isMovingLihui = true;
			} else {
				if (numLihuiNeedMove <= 0) {
					isMovingLihui = false;
					nextAction ();
				}
			}
		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_BG){
			nextAction ();
		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.EFFECT){
			if (!isEffecting) {
				isEffecting = true;
			}
			if (isEffecting) {
				view.bg.color = new Color (view.bg.color.r,view.bg.color.g,view.bg.color.b,view.bg.color.a - 1f*dTime);
				if (view.bg.color.a <= 0) {
					view.bg.color = new Color (view.bg.color.r,view.bg.color.g,view.bg.color.b,1);
					nextAction ();
					isEffecting = false;
				}
			}

		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.END){
			Debug.Log ("has finished");

            finishDialog ();
			
		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.SHOW_BRANCH){
			if (isBranchFlashing) {


			}
		}


	}

    private void ClearInfo()
    {
        model.LihuiIds.Clear();
        foreach(LihuiView vv in view.LihuiList)
        {
            GameObject.Destroy(vv.root.gameObject);
        }
        view.LihuiList.Clear();
    }


    int[] intervalMap = new int[]{0,0,600,500,400,300};

	public Vector2 GetPosition(int LihuiIdx){
		int c = model.LihuiIds.Count;
		if (c == 1) {
			return Vector2.zero;
		}
		int interval = intervalMap[c];

		return new Vector2(-(c-1) * 0.5f * interval + LihuiIdx * interval,0);

	}

	public void ClickScreen(){
		if(locked)
			return;
		if (frameIdx < 0 || frameIdx >= model.frames.Count) {
			return;
		}
		if (model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_TEXT) {
			if (isReading) {
				showAllWords ();
			} else {
				nextAction ();
			}
		} else {
		}
	}

	public void nextAction(){
		frameIdx += 1;

		if (model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_TEXT) {
			DialogFrameText realFrame = model.frames [frameIdx] as DialogFrameText;
			cursorF = 0.0f;
			cursorI = 0;
			view.dialogContent.text = "";
			isReading = true;
			view.nameText.text = realFrame.Name;

		} else if(model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_LIHUI){
			
			isMovingLihui = false;

		}else if(model.frames [frameIdx].DialogType == eDialogFrameType.CHANGE_BG){
			DialogFrameBG realFrame = model.frames [frameIdx] as DialogFrameBG;
			Debug.Log ("change");
			//view.bg.sprite = null;
		} else if(model.frames [frameIdx].DialogType == eDialogFrameType.END){
			
		} else if(model.frames [frameIdx].DialogType == eDialogFrameType.SHOW_BRANCH){
			DialogFrameBranch realFrame = model.frames [frameIdx] as DialogFrameBranch;
			ShowBranch (realFrame.Choices);

		} else if(model.frames [frameIdx].DialogType == eDialogFrameType.EFFECT){
			isEffecting = false;
		}
	}

	private void finishDialog(){

        isReading = true;
        frameIdx = -1;
        view.dialogContent.text = "";
        view.nameText.text = "";
        view.branchView.root.gameObject.SetActive(false);

        if (model.dialogId == "d0")
        {

            ClearInfo();
            StartDialog("d1");
        }
        else
        {
            mUIMgr.CloseCertainPanel(this);
            if (DiglogEndEvent != null)
            {
                DiglogEndEvent(null);
            }
        }

	}

	private void ShowBranch(List<string> choices){
		view.branchView.root.gameObject.SetActive (true);
		for (int i = 0; i < view.branchView.Branches.Count; i++) {
			if (i < choices.Count) {
				view.branchView.Branches [i].Title.text = choices [i];
				view.branchView.Branches [i].root.gameObject.SetActive (true);
			} else {
				view.branchView.Branches [i].root.gameObject.SetActive (false);
			}
		}
	}

//	public void chooseBranch(int choice){
//
//		if (choice < 0 || choice >= converters.Count) {
//			return;
//		}
//		stageIndex = converters [choice].nextStageIdx;
//		changeView (stageIndex);
//	}
//
//	void changeView(int stageIndex){
//		EncounterStage stage = null;
//		converters.Clear ();
//		if (!encounter.stages.TryGetValue (stageIndex, out stage)) {
//			return;
//		}
//		if (stage.extra == "monster") {
//			EnemyCombo ec = GameStaticData.getInstance ().getEnemyWithValue (5);
//			GameManager.getInstance ().chaseByEnemy (ec);
//		} else if (stage.extra == "toturial_monster") {
//			EnemyCombo ec = GameStaticData.getInstance ().getSpecifiedEnemy ("toturial");
//			GameManager.getInstance ().chaseByEnemy (ec);
//		}
//
//		if (stage.stageType == eStageType.FINISH) {
//			getRes (stage.res);
//			return;
//		} else if (stage.stageType == eStageType.BATTLE) {
//			EncounterBattleInfo battleInfo = stage.battleInfo;
//
//			PlayerData.getInstance ().initBattle (encounter.eId, stageIndex,battleInfo);
//			GameManager.getInstance ().enterBattle ();
//			this.Hide ();
//			return;
//		} else if (stage.stageType == eStageType.CHECK) {
//			Debug.Log ("check");
//
//			int nextStage = -1;
//			for (int i=0;i<stage.converts.Count-1;i++) {
//				EncounterConvert convert = stage.converts [i];
//				bool canConvert = checkConditions (convert.checks);
//				if (canConvert) {
//					nextStage = convert.nextStageIdx;
//					break;
//				}
//			}
//			if (nextStage == -1) {
//				nextStage = stage.converts [stage.converts.Count - 1].nextStageIdx;
//			}
//			if (nextStage != -1) {
//				stageIndex = nextStage;
//				changeView (stageIndex);
//			} else {
//				Debug.Log ("error");
//				panelHide ();
//			}
//		} else if(stage.stageType == eStageType.RANDOM){
//
//			int randomInt = Random.Range (0, 99);
//			int nextStage = -1;
//			for (int i=0;i<stage.converts.Count-1;i++) {
//				EncounterConvert convert = stage.converts [i];
//				bool canConvert = checkConditions (convert.checks,randomInt);
//				if (canConvert) {
//					nextStage = convert.nextStageIdx;
//					break;
//				}
//			}
//			if (nextStage == -1) {
//				nextStage = stage.converts [stage.converts.Count - 1].nextStageIdx;
//			}
//			if (nextStage != -1) {
//				stageIndex = nextStage;
//				changeView (stageIndex);
//			} else {
//				Debug.Log ("error");
//				panelHide ();
//			}
//
//		}else {
//			_text.text = stage.desp;
//			int idx = 0;
//
//
//			_branches.ClearSelection();
//			_branches.RemoveChildrenToPool();
//
//
//			//			for(int i=0;i<items.Count;i++)
//			//			{
//			//				NewItem item = (NewItem)_new_item_list.AddItemFromPool();
//			//				item.init (items[i]);
//			//				item.onClick.Add (delegate() {
//			//					if(_new_item_list.selectedIndex!=-1){
//			//						_confirm.visible = true;
//			//					}
//			//					if (PlayerData.getInstance ().guideStage == 10) {
//			//						GuideManager.getInstance ().showGuideConfirmChooseItem ();
//			//						PlayerData.getInstance ().guideStage = 11;
//			//					}
//			//				});
//			//				item.GetChild ("detail").onTouchBegin.Add (delegate() {
//			//					//Debug.Log("Show Detail");
//			//				});
//			//			}
//
//			int numOfBranch = 0;
//
//
//			for (int i = 0; i < stage.converts.Count; i++) {
//				EncounterConvert convert = stage.converts [i];
//				bool canShow = checkConditions (convert.checks);
//				//EncounterConvert convert = stage.converts [i];
//				if (canShow) {
//
//					SelectionBranch item = (SelectionBranch)_branches.AddItemFromPool ().asButton;
//					item.init (numOfBranch ++,convert.choiceDesp);
//					item.visible = true;
//					converters.Add (convert);
//				}
//			}
//			_branches.ResizeToFit (numOfBranch);
//
//			//			for (int i = 0; i < stage.converts.Count; i++) {
//			//				EncounterConvert convert = stage.converts [i];
//			//				bool canShow = checkConditions (convert.checks);
//			//				//EncounterConvert convert = stage.converts [i];
//			//				if (canShow) {
//			//
//			//					SelectionBranch item = (SelectionBranch)_branches.GetChildAt (i).asButton;
//			//					item.idx = numOfBranch ++ ;
//			//
//			//					_branches.GetChildAt(idx).asButton.title = convert.choiceDesp;
//			//					_branches.GetChildAt(idx).visible = true;
//			//					converters.Add (convert);
//			//					idx++;
//			//
//			//
//			//				}
//			//			}
//			//			for (int i = idx; i < MAX_BRANCH_NUM; i++) {
//			//				_branches.GetChildAt(i).asButton.title ="";
//			//				_branches.GetChildAt(i).visible = false;
//			//			}
//
//			_loader.url = "Explore/default_bg";
//		}
//
//		if (PlayerData.getInstance ().guideStage == 2) {
//			PlayerData.getInstance ().guideStage = 3;
//			GuideManager.getInstance ().showGuideChooseBranch ();
//		}else if (PlayerData.getInstance ().guideStage == 3) {
//			PlayerData.getInstance ().guideStage = 4;
//			GuideManager.getInstance ().showGuideChooseBranch ();
//		}else if (PlayerData.getInstance ().guideStage == 4) {
//			GuideManager.getInstance ().hideGuide ();
//		}
//	}


	void showAllWords(){
		cursorF += 100f;
	}


	void textAppend(DialogRichWord word){
		if (word.color != DialogModule.NORMAL_COLOR) {
			view.dialogContent.text += "<color="+ word.color +">";
		}
		if (word.size != DialogModule.NORMAL_SIZE) {
			view.dialogContent.text += "<size="+ word.size +">";
		}
		view.dialogContent.text += word.content +"";
		if (word.size != DialogModule.NORMAL_SIZE) {
			view.dialogContent.text += "</size>";
		}
		if (word.color != DialogModule.NORMAL_COLOR) {
			view.dialogContent.text += "</color>";
		}
	}

}

