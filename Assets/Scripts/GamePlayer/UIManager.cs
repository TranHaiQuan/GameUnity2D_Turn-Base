using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

	
	AudioSource audioGame;
	public GameObject[] choosedDiff;

	void Awake(){
		if (PlayerPrefs.HasKey ("difficulty")) {
			SetActiveChoose ();
		} else {
			PlayerPrefs.SetString ("difficulty", "Easy");
			choosedDiff [0].SetActive (true);
		}
	}

	void Start(){
		audioGame = GetComponent<AudioSource> ();
	}
	void Update () {
		
	}

	public void PlayGame(){
		SceneManager.LoadScene ("Play");
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
	}
}
