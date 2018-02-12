using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {

	
	AudioSource audioGame;
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

}
