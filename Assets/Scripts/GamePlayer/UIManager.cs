using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

	
	AudioSource audioGame;
	public GameObject[] choosedDiff;
	int currentLevel;
	public GameObject[] level;
	string difficulty;
	void Start(){
//		PlayerPrefs.DeleteAll ();
		if (PlayerPrefs.HasKey ("difficulty")) {
			SetActiveChoose ();
		} else {
			PlayerPrefs.SetString ("difficulty", "Easy");
			choosedDiff [0].SetActive (true);
			difficulty = PlayerPrefs.GetString ("difficulty");
			Debug.Log ("$$$$$$$$$$$$$$Key = " + difficulty);
		}
		if (PlayerPrefs.HasKey("currentLevel" + difficulty)) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel" + difficulty);
		}
		ActiveLevel (currentLevel);
		audioGame = GetComponent<AudioSource> ();
	}
		
	public void ActiveDialog(GameObject dialog){
		dialog.GetComponent<Animator> ().SetBool ("isActive", true);
	}

	public void NonActiveDialog(GameObject dialog){
		dialog.GetComponent<Animator> ().SetBool ("isActive", false);
	}

	public void Sound(Text name){
		if (audioGame.enabled) {
			name.text = "Sound: Off";
			audioGame.enabled = false;
		} else {
			name.text = "Sound: On";
			audioGame.enabled = true;
		}
	}

	public void ChooseDifficulty(GameObject thisObject){
		if (PlayerPrefs.GetString ("difficulty") != thisObject.name) {
			PlayerPrefs.SetString ("difficulty", thisObject.name);
			SetActiveChoose ();
		}
	}

	void SetActiveChoose(){
		for (int i = 0; i < choosedDiff.Length; i++) {
			if (choosedDiff [i].transform.parent.name == PlayerPrefs.GetString ("difficulty")) {
				choosedDiff [i].SetActive (true);
			} else {
				choosedDiff [i].SetActive (false);
			}
		}
		difficulty = PlayerPrefs.GetString ("difficulty");
		if (PlayerPrefs.HasKey("currentLevel" + difficulty)) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel" + difficulty);
		}
		Debug.Log ("currnenenenenenen = " + currentLevel);
		ActiveLevel (currentLevel);
	}

	void ActiveLevel(int _currentLevel){
		
		for (int j = 0; j < level.Length; j++) {
			level [j].GetComponent<Button> ().enabled = false;
			level [j].transform.GetChild (0).gameObject.SetActive (false);
			level [j].transform.GetChild (2).gameObject.SetActive (true);
		}
		for (int i = 0; i <= _currentLevel; i++){
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
