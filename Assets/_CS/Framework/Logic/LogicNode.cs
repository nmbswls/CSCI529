using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum eLogicType{
	SINGLE,
	AND,
	OR
}
public delegate bool CheckFunc(string[] arg0);

public class LogicNode
{
	public CheckFunc func;
	public string[] arg0;
	public eLogicType type = eLogicType.SINGLE;
	public bool anti = false;

	public List<LogicNode> ChildNodes = new List<LogicNode>();
	public bool Check(){
        bool ret = false;
		if (type == eLogicType.SINGLE) {
			if (func == null)
            {
                ret = false;
            }
            else
            {
                ret = func(arg0);
            }
		} else if (type == eLogicType.AND) {
			ret = true;
			foreach (LogicNode child in ChildNodes) {
				ret &= child.Check ();
				if (!ret)
					break;
			}
		} else {
			ret = false;
			foreach (LogicNode child in ChildNodes) {
				ret |= child.Check ();
				if (ret)
					break;
			}
		}
        if (anti)
        {
            return !ret;
        }
        else
        {
            return ret;
        }
    }

	public LogicNode(CheckFunc func, string[] arg0){
		this.func = func;
		this.arg0 = arg0;
	}
	public LogicNode(eLogicType type){
		this.type = type;
	}
	public void ApeendChild(LogicNode child){
		ChildNodes.Add (child);
	}


	private static LogicNode MergeNode(LogicNode n1, LogicNode n2, char opt){
		LogicNode ret = null;
		if (opt == '&') {
			ret = new LogicNode (eLogicType.AND);
		} else if (opt == '|') {
			ret = new LogicNode (eLogicType.OR);
		} else {
			Debug.Log ("Wrong Opt");
			return null;
		}
		ret.ApeendChild (n1);
		ret.ApeendChild (n2);
		return ret;
	}
	//

	public static LogicNode ConstructFromString(string input, Dictionary<string, CheckFunWrap> funcDict){
		string str = input.Trim ();
		Stack<LogicNode> StackNode = new Stack<LogicNode> ();
		Stack<char> StackOpt = new Stack<char> ();

		Dictionary<char, int> optPriority = new Dictionary<char, int> ();
		optPriority ['('] = 0;
		optPriority ['|'] = 1;
		optPriority ['&'] = 2;
		optPriority [')'] = 3;

		for (int i = 0; i < str.Length; i++) {
			if (char.IsWhiteSpace (input [i])) {
				continue;
			}
			if (input[i]=='!' || char.IsLetterOrDigit (input [i])) {
				bool anti = false;
				if (input [i] == '!') {
					i+=1;
					anti = true;
				}

				int j = i;
				while (j < str.Length && char.IsLetterOrDigit (str [j])) {
					j++;
				}
				if (j >= str.Length || str [j] != '(') {
					Debug.Log ("Error Invalid");
					throw new UnityException("error invalid");
				}
				string funcName = str.Substring (i, j - i);
				i = j + 1;
				j = i;
				while (j < str.Length && str[j]!=')') {
					j++;
				}
				if (j >= str.Length || str [j] != ')') {
					Debug.Log ("Error Invalid");
					throw new UnityException("error invalid");
				}
				string args = str.Substring (i, j - i);
                string[] argc = null;
                if (args == "")
                {
                    argc = new string[0];
                }
                else
                {
                    argc = args.Split(',');
                }
				i = j;
				LogicNode node = null;
				if (funcDict.ContainsKey (funcName)) {
					CheckFunWrap wrap = funcDict [funcName];
					if (wrap.argNeed != argc.Length) {
						throw new UnityException ("func arg error");
					}
					node = new LogicNode (wrap.func,argc);
				} else {
					throw new UnityException ("no func found");
				}
				node.anti = anti;
				StackNode.Push (node);


			} else {
				if (StackOpt.Count==0) {
					StackOpt.Push(str[i]);
					continue;
				}
				if (str[i] == ')') {
					while (StackOpt.Count > 0 && StackOpt.Peek() != '(') {
						LogicNode n2 = StackNode.Pop();
						LogicNode n1 = StackNode.Pop();
						char op = StackOpt.Pop();
						LogicNode newNode = MergeNode(n1,n2,op);
						StackNode.Push(newNode);
					}

					if (StackOpt.Peek() == '(') StackOpt.Pop();
				} else {

					if (str[i] != '(' && optPriority[str[i]] > optPriority[StackOpt.Peek()]) {
						StackOpt.Push(str[i]);
					} else {
						
						while (StackOpt.Count>0 && str[i] != '(' && optPriority[str[i]] <= optPriority[StackOpt.Peek()]) {
							LogicNode n2 = StackNode.Pop();
							LogicNode n1 = StackNode.Pop();
							char op = StackOpt.Pop();
							LogicNode newNode = MergeNode(n1, n2, op);
							StackNode.Push(newNode);
						}

						StackOpt.Push(str[i]);
					}
				}
			}
		}

		while (StackOpt.Count>0) {
			LogicNode n2 = StackNode.Pop();
			LogicNode n1 = StackNode.Pop();
			char op = StackOpt.Pop();
			LogicNode newNode = MergeNode(n1, n2, op);
			StackNode.Push(newNode);
		}

		return StackNode.Pop();
	}





}






//		while(idx<input.Length){
//			if (input [idx] == '(') {
//				node = new LogicNode (eLogicType.AND);
//				idx++;
//				LogicNode ret = ConstructFromString (input);
//				node.ApeendChild (ret);
//			} else if (input [idx] == ')') {
//				idx++;
//				return node;
//			} else if (input [idx] == '&') {
//				if (idx == input.Length || input [idx + 1] != '&') {
//					Debug.Log ("invalid input");
//					return null;
//				}
//				idx += 2;
//				mode = 0;
//
//			} else if (input [idx] == '|') {
//				if (idx == input.Length || input [idx + 1] != '|') {
//					Debug.Log ("invalid input");
//					return null;
//				}
//				idx += 2;
//			} else {
//				int j = idx;
//				while (input [j] != '(') {
//					j++;
//				}
//				string funcName = input.Substring (idx, j - idx);
//				while (input [j] != ')') {
//					j++;
//				}
//				string args = input.Substring (idx, j - idx);
//			}
//		}