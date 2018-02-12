using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public int damageMin, damageMax, hp, totalEnemy, totalQuestion;
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
	JsonData qaData, enemyData;
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
//	public string difficulty;
//	string linkApi = "https://opentdb.com/api.php?amount=50&difficulty=medium";
	void Awake (){
		if (PlayerPrefs.HasKey("currentLevel")) {
			indexLevel = PlayerPrefs.GetInt ("currentLevel");
		}
		levelChoosed = PlayerPrefs.GetInt ("levelChoosed");
		player = GameObject.FindGameObjectWithTag ("Hero").GetComponent<Player> ();
		isFinishWave = true;
		LoadJson ();
		AddAllQuestionIndex (levelChoosed + 1);
		ShowQuestion ();
	}
	void Start () {
		levelChoosed = 1;
		LoadParameterEnemy (1);
		SetPositionForEnemy (totalEnemy);
//		StartCoroutine (Request ());
	}
	
	void Update () {
		
	}

	public void ChangeCurrentEnemy(){
		if (isChoosed && isFinishWave) {
			SetCurrentEnemy ();

		}
	}

	public void MyAnswer(GameObject thisAnswer){
		if (isFinishWave) {
			Debug.Log ("<color=yellow>RandomQuws: " + randomQues);
			questionIndexList.RemoveAt (randomQues);
			CheckAnswer (thisAnswer.transform.GetChild (0).gameObject.GetComponent<Text> ().text);
			isChoosed = true;
			isFinishWave = false;
			indexCurrentEnemy++;
			if (indexCurrentEnemy >= enemyList.Count) {
				indexCurrentEnemy = 0;
			}
			currentEnemy.GetComponent<Enemy> ().StartAction ();
			questionAnwsered++;
		}
	}

	public void CheckLimitQuestion(){
		if (questionAnwsered >= totalQuestion && !isDead) {
			boss.SetActive (true);
			boss.transform.position = posBossStart.transform.position;
			StartCoroutine (BlinkSprite ());
		}
	}

	IEnumerator BlinkSprite(){
		yield return new WaitForSeconds (2);
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
		
	public void GameOver(){
		isDead = true;
		gameoverDialog.SetActive (true);
	}

	void Victory() {
		if (indexLevel == levelChoosed) {
			indexLevel++;
			PlayerPrefs.SetInt ("currentLevel", indexLevel);
		} else {
			levelChoosed++;
			PlayerPrefs.SetInt ("levelChoosed", levelChoosed);
		}
		victoryDialog.SetActive (true);
	}

	void SetPositionForEnemy(int _totalEnemy){
		for (int i = 0; i < _totalEnemy; i++) {
			enemyArr [i].SetActive (true);
			enemyArr [i].transform.position = posEnemyArr [i].transform.position;
			enemyList.Add (enemyArr [i]);
		}
		SetCurrentEnemy ();
	}
	void SetCurrentEnemy(){
		currentEnemy = enemyList [indexCurrentEnemy];
		dirPlayerEnemy = currentEnemy.transform.GetChild(1).gameObject.transform.position - player.gameObject.transform.position;
		angleKunai = Mathf.Rad2Deg * Mathf.Atan2 (dirPlayerEnemy.y, dirPlayerEnemy.x);
		Debug.Log ("Rad" +  Mathf.Atan2 (dirPlayerEnemy.y, dirPlayerEnemy.x) + "//Angle: " + angleKunai);
	}
	void LoadParameterEnemy(int level){ // load chi so cua tung nac level
    string jsonStr = File.ReadAllText(Application.dataPath + "/Resources/Enemy.json");
    enemyData = JsonMapper.ToObject(jsonStr);
		damageMax = int.Parse(enemyData["level" + level]["damageMax"].ToString());
		damageMin = int.Parse(enemyData["level" + level]["damageMin"].ToString());
		hp = int.Parse(enemyData["level" + level]["hp"].ToString());
		totalEnemy = int.Parse(enemyData["level" + level]["enemies"].ToString());
		totalQuestion = int.Parse(enemyData["level" + level]["questions"].ToString());
	}
	void LoadJson(){
		string jsonStr = File.ReadAllText (Application.dataPath + "/Resources/Quiz.json");
		qaData = JsonMapper.ToObject (jsonStr);
	}
	JsonData GetData(string level, string questionNumber, string type){
		return qaData[level][questionNumber][type];
	}

	void AddAllQuestionIndex(int level){
		for (int i = 1; i <= qaData ["level" + level].Count; i++) {
			questionIndexList.Add (i);
		}
	}

	public void ShowQuestion(){
		randomQues = Random.Range (0, questionIndexList.Count);
		Debug.Log ("<color=red>|||||||||||||| random question: </color>" + randomQues);
		LoadAnswerAndQuestion (levelChoosed + 1, questionIndexList [randomQues]);
	}

	public void LoadAnswerAndQuestion(int level, int quesIndex){
		questionTxt.text = "Quiz " + (questionAnwsered + 1) + ": " + GetData ("level" + level , "quiz" + quesIndex, "question").ToString();

		for (int i = 0; i < 4; i++) {
			answerOptions [i].text = GetData ("level" + level, "quiz" + quesIndex, "options") [i].ToString ();
		}
		answerExactly = GetData("level" + level, "quiz" + quesIndex, "answer").ToString ();
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

	public void PlayAgain(){
		Debug.Log ("Play Again");
	}

	public void NextLevel(){
		Debug.Log ("Next Level");
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

