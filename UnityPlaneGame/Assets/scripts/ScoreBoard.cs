using UnityEngine;
using System.Collections;

public class ScoreBoard : MonoBehaviour {

	public string ScoreText = "Score: 0";
	
	
	void OnGUI() {
		ScoreText = GUILayout.TextField(ScoreText, 25);
	}
}
