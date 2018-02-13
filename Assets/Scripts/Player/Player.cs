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
	public Text hpTxt;
	void Start () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		anim = GetComponent<Animator> ();
		LoadParameterEnemy ();
		currentHp = hp;
		hpTxt.text = currentHp.ToString ();
	}
	public void ResetGame(){
		currentHp = hp;
		float _temp = (float)currentHp / hp;
		hpImg.fillAmount = _temp;
		hpTxt.text = currentHp.ToString ();
		anim.SetBool ("isDead", false);
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
		StartCoroutine (WaitForBlur ());
		currentHp -= damageEnemy;

		float _temp = (float)currentHp / hp;
		Debug.Log ("<color=green>temp = " + _temp + "</color>");
		hpImg.fillAmount = _temp;
		if (currentHp <= 0) {
			anim.SetBool ("isDead", true);
			gameManager.GameOver ();
			currentHp = 0;
		} else {
			SetAnimHit (true);
		
			StartCoroutine (WaitForFinishWave ());
		}
		hpTxt.text = currentHp.ToString ();
	}
	IEnumerator WaitForBlur(){
		while (notice.color.a < 1) {
			notice.color = new Color(notice.color.r, notice.color.g, notice.color.b, notice.color.a + 0.1f);
			yield return new WaitForFixedUpdate ();
		}
		yield return new WaitForSeconds (0.5f);
		while (notice.color.a > 0) {
			notice.color = new Color(notice.color.r, notice.color.g, notice.color.b, notice.color.a - 0.1f);
			yield return new WaitForSeconds (0.01f);
			yield return new WaitForFixedUpdate ();
		}

	}
	IEnumerator WaitForFinishWave(){
		while (!_isWaitFinish) {
			if(gameManager.isFinishWave){
				gameManager.CheckLimitQuestion (); // show Boss
				Debug.Log("<color=blue>Chay vao Finish Wave</color>");
				if (gameManager.isBoss) {
					gameManager.CheckVictory ();
				}
				_isWaitFinish = true;
			}
			yield return new WaitForFixedUpdate();
		}
		_isWaitFinish = false;
	}
}
