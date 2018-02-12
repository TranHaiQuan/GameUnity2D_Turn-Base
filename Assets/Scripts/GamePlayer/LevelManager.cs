using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelManager : MonoBehaviour {
	int currentLevel;
	public GameObject[] level;

	void Awake(){
		if (PlayerPrefs.HasKey("currentLevel")) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel");
		}
		ActiveLevel (currentLevel);
	}

	void ActiveLevel(int _currentLevel){
		for (int i = 0; i <= currentLevel; i++){
			level [i].GetComponent<Button> ().enabled = true;
			level [i].transform.GetChild (0).gameObject.SetActive (true);
			level [i].transform.GetChild (2).gameObject.SetActive (false);
		}
	}

	public void LoadGame(GameObject thisButton){
		int _levelChoosed = int.Parse(thisButton.transform.GetChild (0).GetComponent<Text> ().text);
		PlayerPrefs.SetInt ("levelChoosed", _levelChoosed);
		SceneManager.LoadScene ("Play");
	}
}