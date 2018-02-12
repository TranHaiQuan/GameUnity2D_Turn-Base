using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using LitJson;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	Animator anim;
	GameManager gameManager;
	public int damageMax, damageMin, hp;
	JsonData playerData;
	public Image hpImg;
	public int currentHp;
	bool _isWaitFinish;
	public Text notice;
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		anim = GetComponent<Animator> ();
		LoadParameterEnemy ();
		currentHp = hp; 
	}

	void SetAnimHit(bool isHit){
		anim.SetBool ("isHit", isHit);
	}

	public void Hiited(){
		SetAnimHit (false);
	}
	void LoadParameterEnemy(){ // load chi so cua tung nac level
		string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/Player.json");
		playerData = JsonMapper.ToObject(jsonStr);
		damageMax = int.Parse(playerData["Player"]["damageMax"].ToString());
		damageMin = int.Parse(playerData["Player"]["damageMin"].ToString());
		hp = int.Parse(playerData["Player"]["hp"].ToString());
	}

	public void DecreaseHp(int damageEnemy){
		if (damageEnemy == 0) {
			notice.text = "Miss";
		} else {
			notice.text = "-" + damageEnemy;
		}
		currentHp -= damageEnemy;
		float _temp = (float)currentHp / hp;
		Debug.Log ("<color=green>temp = " + _temp + "</color>");
		hpImg.fillAmount = _temp;
		if (currentHp <= 0) {
			anim.SetBool ("isDead", true);
			gameManager.GameOver ();
			hp = 0;
		} else {
			SetAnimHit (true);
			StartCoroutine (WaitForFinishWave ());
		}
	}

	IEnumerator WaitForFinishWave(){
		
		while (!_isWaitFinish) {
			if(gameManager.isFinishWave){
				gameManager.CheckLimitQuestion (); // show Boss
				Debug.Log("<color=blue>Chay vao Finish Wave</color>");
				_isWaitFinish = true;
			}
			yield return new WaitForFixedUpdate();
		}
		_isWaitFinish = false;
	}
}
