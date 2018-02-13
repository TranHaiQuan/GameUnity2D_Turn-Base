using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public int damageMin, damageMax, hp, totalEnemy, totalQuestion, timer, currentTime;
	public int levelChoosed;
	public GameObject[] posEnemyArr;
	public GameObject[] enemyArr;
	public bool isAnswer;
	public GameObject currentEnemy;
	public int indexCurrentEnemy;
	public bool isChoosed;
	public bool isFinishWave;
	public List<GameObject> enemyList;
	Vector3 dirPlayerEnemy;
	public float angleKunai;
	public Text questionTxt;
	public Text[] answerOptions;
	public List<int> questionIndexList;
	JsonData qaData, enemyData, bossData;
	public string answerExactly;
	Player player;
	public bool isDead;
	public GameObject gameoverDialog;
	public GameObject victoryDialog;
	int _damage;
	int randomQues;
	int questionAnwsered;
	public int indexLevel;
	public GameObject boss;
	public GameObject posBossStart;
	string difficulty;
	public bool isBoss;
	bool isFirstBoss;
	public Text bossWarningTxt;
	public Text levelFinishTxt;
	public Text gameOverLevelTxt;
	public Text levelGameTxt;
	public Text timerTxt;
//	string linkApi = "https://opentdb.com/api.php?amount=50&difficulty=medium";
	void Awake (){
		difficulty = PlayerPrefs.GetString ("difficulty");
		if (PlayerPrefs.HasKey("currentLevel")) {
			indexLevel = PlayerPrefs.GetInt ("currentLevel" + difficulty);
		}
		levelChoosed = PlayerPrefs.GetInt ("levelChoosed");
		player = GameObject.FindGameObjectWithTag ("Hero").GetComponent<Player> ();
		isFinishWave = true;
		LoadParameterEnemy (levelChoosed);
		SetPositionForEnemy (totalEnemy);
		LoadDataForBoss ("");
		levelGameTxt.text = "Level: " + levelChoosed;
	}
	void Start () {
		
//		StartCoroutine (Request ());
		currentTime = timer;
		timerTxt.text = timer.ToString() + "s";
	}

	public void ChangeCurrentEnemy(){
		if (isChoosed && isFinishWave) {
			if (!isBoss) {
				SetCurrentEnemy (enemyList [indexCurrentEnemy]);
			} else {
				SetCurrentEnemy (boss);
			}
		}
	}


	public void MyAnswer(GameObject thisAnswer){
		if (isFinishWave) {
			StopAllCoroutines ();
			Debug.Log ("<color=yellow>RandomQuws: " + randomQues);
			questionIndexList.RemoveAt (randomQues);
			CheckAnswer (thisAnswer.transform.GetChild (0).gameObject.GetComponent<Text> ().text);
			isChoosed = true;
			isFinishWave = false;
			questionAnwsered++;
			if (!isBoss) {
				indexCurrentEnemy++;

				if (indexCurrentEnemy >= enemyList.Count) {
					indexCurrentEnemy = 0;
				}
				currentEnemy.GetComponent<Enemy> ().StartAction ();
				if (questionAnwsered >= totalQuestion && !isDead) {
					isBoss = true;
					StartCoroutine (BlinkBossWarning ());
				}
			} else {
				currentEnemy.GetComponent<Boss> ().StartAction ();
			}
		}
	}

	public void CheckLimitQuestion(){
		if (questionAnwsered >= totalQuestion && !isDead && !isFirstBoss) {
			boss.SetActive (true);
			questionAnwsered = 0;
			LoadParameterBoss (levelChoosed);
			boss.transform.position = posBossStart.transform.position;
			isFirstBoss = true;
			StartCoroutine (BlinkSprite ());
		}
	}
	IEnumerator BlinkBossWarning(){
		yield return new WaitForSeconds (1f);
		for (int i = 0; i < 5; i++) {
			bossWarningTxt.enabled = true;
			yield return new WaitForSeconds (0.2f);
			bossWarningTxt.enabled = false;
			yield return new WaitForSeconds (0.2f);
		}
	}
	IEnumerator BlinkSprite(){
		yield return new WaitForSeconds (1f);
		for (int i = 0; i < enemyList.Count; i++) {
			enemyList [i].GetComponent<SpriteRenderer>().enabled = false;
		}
		yield return new WaitForSeconds (0.2f);
		for (int i = 0; i < enemyList.Count; i++) {
			enemyList [i].GetComponent<SpriteRenderer>().enabled = true;
		}
		yield return new WaitForSeconds (0.2f);
		for (int i = 0; i < enemyList.Count; i++) {
			enemyList [i].SetActive(false);
		}
	}

	public void CheckVictory(){
		if (questionAnwsered >= totalQuestion && isBoss) {
			Debug.Log ("<color=green>QuestionAnswer = " + questionAnwsered + " //// total = " + totalQuestion + "</color>");
			Victory ();
		}
	}
	public void GameOver(){
		isDead = true;
		gameoverDialog.SetActive (true);
		gameOverLevelTxt.text = "Level: " + levelChoosed;
		StopAllCoroutines ();
	}

	void Victory() {
		levelFinishTxt.text = "Level: " + levelChoosed;
		victoryDialog.SetActive (true);
		if (levelChoosed >= indexLevel) {
			indexLevel++;
			Debug.Log ("]]]]]]]]]]]]]]]]]]]]]]]>");
			PlayerPrefs.SetInt ("currentLevel" + difficulty, indexLevel);
		}
		StopAllCoroutines ();
	}

	public void NextLevel(){
		levelChoosed++;
		PlayerPrefs.SetInt ("levelChoosed", levelChoosed);
		if (victoryDialog.activeSelf) {
			victoryDialog.SetActive (false);
		}
		if (gameoverDialog.activeSelf) {
			gameoverDialog.SetActive (false);
		}
		isBoss = false;
		questionAnwsered = 0;
		isFinishWave = true;
		LoadParameterEnemy (levelChoosed);
		SetPositionForEnemy (totalEnemy);
		LoadDataForBoss ("");
		boss.transform.position = posBossStart.transform.position;
		boss.SetActive (false);
		player.ResetGame ();
		isDead = false;
		isFirstBoss = false;
		levelGameTxt.text = "Level: " + levelChoosed;
	}

	public void PlayAgain(){
		if (victoryDialog.activeSelf) {
			victoryDialog.SetActive (false);
		}
		if (gameoverDialog.activeSelf) {
			gameoverDialog.SetActive (false);
		}
		isBoss = false;
		questionAnwsered = 0;
		isFinishWave = true;
		LoadParameterEnemy (levelChoosed);
		SetPositionForEnemy (totalEnemy);
		LoadDataForBoss ("");
		boss.transform.position = posBossStart.transform.position;
		boss.SetActive (false);
		player.ResetGame ();
		isDead = false;
		isFirstBoss = false;
		levelGameTxt.text = "Level: " + levelChoosed;
	}

	void SetPositionForEnemy(int _totalEnemy){
		for (int i = 0; i < _totalEnemy; i++) {
			enemyArr [i].SetActive (true);
			enemyArr [i].transform.position = posEnemyArr [i].transform.position;
			enemyList.Add (enemyArr [i]);
		}
		SetCurrentEnemy (enemyList [indexCurrentEnemy]);
	}

	void SetCurrentEnemy(GameObject _currentEnemy){
		currentEnemy = _currentEnemy;
		dirPlayerEnemy = currentEnemy.transform.GetChild(1).gameObject.transform.position - player.gameObject.transform.position;
		angleKunai = Mathf.Rad2Deg * Mathf.Atan2 (dirPlayerEnemy.y, dirPlayerEnemy.x);
		Debug.Log ("Rad" +  Mathf.Atan2 (dirPlayerEnemy.y, dirPlayerEnemy.x) + "//Angle: " + angleKunai);
		Debug.Log ("<color=black> Set current Boss: " + currentEnemy + "</color>");
	}
	void LoadParameterEnemy(int level){ // load chi so cua tung nac level
    string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/Enemy.json");
    enemyData = JsonMapper.ToObject(jsonStr);
		damageMax = int.Parse(enemyData["level" + level]["damageMax"].ToString());
		damageMin = int.Parse(enemyData["level" + level]["damageMin"].ToString());
		hp = int.Parse(enemyData["level" + level]["hp"].ToString());
		totalEnemy = int.Parse(enemyData["level" + level]["enemies"].ToString());
		totalQuestion = int.Parse(enemyData["level" + level]["questions"].ToString());
		timer = int.Parse(enemyData["level" + level]["time"].ToString());
	}

	void LoadParameterBoss(int level){ // load chi so cua tung nac level
		string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/Boss.json");
		bossData = JsonMapper.ToObject(jsonStr);
		damageMax = int.Parse(bossData["level" + level]["damageMax"].ToString());
		damageMin = int.Parse(bossData["level" + level]["damageMin"].ToString());
		hp = int.Parse(bossData["level" + level]["hp"].ToString());
		totalQuestion = int.Parse(bossData["level" + level]["questions"].ToString());
		timer = int.Parse(bossData["level" + level]["time"].ToString());
	}

	void LoadJson(string difficulty){
		string jsonStr = File.ReadAllText (Application.dataPath + "/Resources/Quiz" + difficulty + ".json");
		qaData = JsonMapper.ToObject (jsonStr);
	}
	JsonData GetData(string level, string questionNumber, string type){
		return qaData[level][questionNumber][type];
	}

	void AddAllQuestionIndex(int level){
		questionIndexList.Clear ();
		for (int i = 1; i <= qaData ["level" + level].Count; i++) {
			questionIndexList.Add (i);
		}
	}

	public void ShowQuestion(){
		if (questionAnwsered < totalQuestion) {
			randomQues = Random.Range (0, questionIndexList.Count);
			Debug.Log ("<color=red>|||||||||||||| random question: </color>" + randomQues);
			LoadAnswerAndQuestion (levelChoosed + 1, questionIndexList [randomQues]);
			currentTime = timer;
			timerTxt.text = timer.ToString () + "s";
			currentTime = timer;
			StopCoroutine (WaitForCountTime ());
			StartCoroutine (WaitForCountTime ());
		}
	}

	public void LoadAnswerAndQuestion(int level, int quesIndex){
		questionTxt.text = "Quiz " + (questionAnwsered + 1) + ": " + GetData ("level" + level , "quiz" + quesIndex, "question").ToString();

		for (int i = 0; i < 4; i++) {
			answerOptions [i].text = GetData ("level" + level, "quiz" + quesIndex, "options") [i].ToString ();
		}
		answerExactly = GetData("level" + level, "quiz" + quesIndex, "answer").ToString ();
	}

	public void LoadDataForBoss(string bossStr){
		LoadJson (bossStr + difficulty);
		AddAllQuestionIndex (levelChoosed + 1);
		ShowQuestion ();

	}

	IEnumerator WaitForCountTime(){
		while (currentTime <= timer) {
			yield return new WaitForSeconds (1);
			Debug.Log (">>>>>>>>>>>>>>>>>>>>>>");
			if (currentTime > 0) {
				currentTime--;
				timerTxt.text = currentTime.ToString () + "s";
			} else {
				break;
			}
		}
		if (isFinishWave) {
			Debug.Log ("<color=yellow>RandomQuws: " + randomQues);
			questionIndexList.RemoveAt (randomQues);
			isAnswer = false;
			isChoosed = true;
			isFinishWave = false;
			questionAnwsered++;
			if (!isBoss) {
				indexCurrentEnemy++;
				if (indexCurrentEnemy >= enemyList.Count) {
					indexCurrentEnemy = 0;
				}
				currentEnemy.GetComponent<Enemy> ().StartAction ();
				if (questionAnwsered >= totalQuestion && !isDead) {
					isBoss = true;
					StartCoroutine (BlinkBossWarning ());
				}
			} else {
				currentEnemy.GetComponent<Boss> ().StartAction ();
			}
		}
	}
	void CheckAnswer(string answer){
		if (answer == answerExactly) {
			isAnswer = true;
			Debug.Log ("<color=green> You are true answer</color>");
		} else {
			isAnswer = false;
			Debug.Log ("<color=red> You are False answer</color>");
		}
	}
	public void BackHome(){
		SceneManager.LoadScene ("Start");
	}

	public void RandomDamage(){
		if (isAnswer) {
			_damage = Random.Range (0, damageMin / 2);
			Debug.Log ("<color=red>True damge = </color>" + _damage);
		} else {
			_damage = Random.Range (damageMin, damageMax);
			Debug.Log ("<color=red>False damge = </color>" + _damage);
		}
		player.DecreaseHp (_damage);
	}

//	IEnumerator Request()
//	{
//		Debug.Log ("Requesting values");
//		WWW www = new WWW (linkApi);
//		yield return www;
//		if (www.error != null) {
//			print ("There was an error getting the data: " + www.error);
//			yield break;
//		}
//		string json = www.text;
//		Debug.Log (json);
//		JsonData jsonData = JsonMapper.ToObject (json);
//		Debug.Log (jsonData ["results"] [1] ["question"]);
//	}
}

