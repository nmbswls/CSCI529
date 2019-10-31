using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum eDialogFrameType{
	CHANGE_TEXT,
	CHANGE_LIHUI,
	CHANGE_BG,
	EFFECT,
	SHOW_BRANCH,
	CHECK_COND,
	END
}

public class DialogFrameBase{
	public int Index;
	public eDialogFrameType DialogType;
}

public class DialogFrameText : DialogFrameBase{

	public string Name;
	public int LihuiIdx;
	public List<DialogRichWord> TextLines = new List<DialogRichWord>();
}

public class DialogFrameLihui : DialogFrameBase{
	public List<string> Opts = new List<string>();
	public List<string> Lids = new List<string>();
	public List<int> SlotIdxs = new List<int>();
}

public class DialogFrameBG : DialogFrameBase{
	public string BG;
	public string BGM;
}

public class DialogFrameEffect : DialogFrameBase{
	public string EffectId;
}

public class DialogFrameEnd : DialogFrameBase{
	public string NextDialogBlock;
}

public class DialogFrameBranch : DialogFrameBase{
	public List<string> Choices = new List<string>();
}

public class DialogRichWord{
	public string content;
	public float size;
	public string color;
	public string font;
}

public class DialogBlock{
    public string DialogId;
	public List<DialogFrameBase> frames = new List<DialogFrameBase>();
	public string NextBlock;
}



public class DialogModule : ModuleBase, IDialogModule
{

	private readonly Dictionary<string, DialogBlock> DialogDict = new Dictionary<string, DialogBlock>();

	public override void Setup(){
		
	}

	public DialogBlock LoadDialog(string DialogBlockId){
		if (DialogDict.ContainsKey (DialogBlockId)) {
			return DialogDict [DialogBlockId];
		}
		TextAsset txt = GameMain.GetInstance ().GetModule<ResLoader> ().LoadResource<TextAsset>("Dialog/"+DialogBlockId,false);
		List<string> lines = new List<string>(txt.text.Split('\n'));
		DialogBlock block = null;
		try{
			block = ReadFromTxt (lines);
		}catch(Exception e){
			Debug.Log (e.StackTrace);
		}
		return block;
	}

	public DialogBlock LoadDialog(){
		return LoadDialog ("d0");
	}

	public static float NORMAL_SIZE = 30;
	public static string NORMAL_COLOR = "";
	public static string NOTMAL_FONT = "";

	private bool isDigitOrLetter(char c){
		if (c <= '9' && c >= '0' || c >= 'a' && c <= 'z' || c>= 'A' && c<='Z' || c == '=') {
			return true;
		}
		return false;
	}

	private List<DialogRichWord> preHandleWords(string input){

		int idx = 0;
		List<DialogRichWord> sout = new List<DialogRichWord> ();
		float size = NORMAL_SIZE;
		string color = NORMAL_COLOR;

		while (idx<input.Length) {
			if (input [idx] == '#') {
				int j = idx + 1;
				while (j < input.Length && isDigitOrLetter (input [j])) {
					j++;
				}
				string opt = input.Substring (idx, j - idx);
				if (opt.StartsWith ("#c=")) {
					color = opt.Substring (3);
				} else if (opt.Equals ("#c")) {
					color = NORMAL_COLOR;
				}else if(opt.Equals ("#size=big")){
					size = 60;
				}else if(opt.Equals ("#size")){
					size = NORMAL_SIZE;
				}
				idx = j;
			} else {
				DialogRichWord w = new DialogRichWord ();
				w.color = color;
				w.size = size;
				w.content = input [idx]+"";
				sout.Add (w);
				idx++;
			}
		}
		return sout;

	}


	public DialogBlock ReadFromTxt(List<string> input){

		List<DialogFrameBase> dialogs = new List<DialogFrameBase> ();
		int InstId = 0;
		foreach (string line in input) {
			if (string.IsNullOrEmpty (line)) {
				continue;
			}
			DialogFrameBase dialogFrame;

			if (line.StartsWith ("[")) {
				dialogFrame = new DialogFrameText ();
				dialogFrame.DialogType = eDialogFrameType.CHANGE_TEXT;
				int i = 1;
				int j = i;
				while (j < line.Length && line [j] != ']') {
					j++;
				}
				string speaker = line.Substring (i, j - i);

				if (speaker.Contains (",")) {
					((DialogFrameText)dialogFrame).Name = speaker.Substring (0, line.IndexOf (","));
					((DialogFrameText)dialogFrame).LihuiIdx = int.Parse (speaker.Substring (line.IndexOf (",") + 1));
				} else {
					((DialogFrameText)dialogFrame).Name = speaker;
				}

				i = line.IndexOf (":") + 1;
				string words = line.Substring (i);
				((DialogFrameText)dialogFrame).TextLines = preHandleWords (words);

			} else if (line.StartsWith ("Lihui")) {
				dialogFrame = new DialogFrameLihui ();
				dialogFrame.DialogType = eDialogFrameType.CHANGE_LIHUI;
				string str = line.Substring (6);
				string[] cmds = str.Split (',');
				foreach (string cmd in cmds) {
					string[] args = cmd.Trim().Split (' ');
					string Opt = args [0];
					string Lid = args [1];
					int SlotIdx = int.Parse (args [2]);
					((DialogFrameLihui)dialogFrame).Opts.Add (Opt);
					((DialogFrameLihui)dialogFrame).Lids.Add (Lid);
					((DialogFrameLihui)dialogFrame).SlotIdxs.Add (SlotIdx);
				}

			} else if (line.StartsWith ("BG")) {
				dialogFrame = new DialogFrameBG ();
				dialogFrame.DialogType = eDialogFrameType.CHANGE_BG;
				string str = line.Substring (3);
				string[] args = str.Trim().Split (' ');
				if (args.Length > 1) {
					((DialogFrameBG)dialogFrame).BGM = args [1];
				}
				((DialogFrameBG)dialogFrame).BG = args [0];
			} else if(line.StartsWith("Effect")){
				dialogFrame = new DialogFrameEffect ();
				dialogFrame.DialogType = eDialogFrameType.EFFECT;
				string str = line.Substring (7);

				((DialogFrameEffect)dialogFrame).EffectId = str;

			} else if(line.StartsWith("End")){
				dialogFrame = new DialogFrameEnd ();
				dialogFrame.DialogType = eDialogFrameType.END;

			} else if(line.StartsWith("Branch")){
				dialogFrame = new DialogFrameBranch ();
				dialogFrame.DialogType = eDialogFrameType.SHOW_BRANCH;

				string str = line.Substring (7);
				string[] branches = str.Split (',');
				foreach (string branch in branches) {
					string[] ops = branch.Split (' ');
					((DialogFrameBranch)dialogFrame).Choices.Add(ops[0]);
				}


			} else{
				throw new UnityException ("not defined dialog type");
			}
			dialogFrame.Index = InstId++;
			dialogs.Add (dialogFrame);
		}
		DialogBlock ret = new DialogBlock ();
		ret.frames = dialogs;
		return ret;
	}
}

