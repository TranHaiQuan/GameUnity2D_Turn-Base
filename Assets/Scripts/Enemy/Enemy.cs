using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Enemy : MonoBehaviour {

	Player player;
	Animator anim;
	GameManager gameManager;
	bool isFinishAttack;
	Vector3 originPos;
	bool isAttacked;

	int AnimRadomed; //{Shoot(Throw), Attack, JumpSlide}
	public GameObject kunai;
	void Start () {
		player = GameObject.FindGameObjectWithTag("Hero").GetComponent<Player>();
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		anim = GetComponent<Animator> ();
		originPos = this.gameObject.transform.position;
	}

	public void StartAction(){
		Debug.Log (isCollisionWithPlayer());
		if (gameManager.isChoosed) {
			RandomAnim ();
			if (AnimRadomed == 0) {
				Shoot ();
			} else {
				StartCoroutine (WaitForAttack ());
			}
			Debug.Log ("<color=red>Out While</color>");
		} else {
			Debug.Log("Not Choose");
		}
	}

	void RandomAnim(){
		AnimRadomed = Random.Range (0, 3);
		Debug.Log ("<color=green>Animation Random</color> = " + AnimRadomed);
	}

	void SetAnimation(string state, bool isState){
		anim.SetBool (state, isState);
	}

	public bool isCollisionWithPlayer(){
		Debug.DrawLine (this.transform.position, this.gameObject.transform.GetChild (1).gameObject.transform.position);
		RaycastHit2D _rayToPlayer = Physics2D.Linecast (this.transform.position, this.gameObject.transform.GetChild (1).gameObject.transform.position);
		Debug.Log (_rayToPlayer.collider);
		if (_rayToPlayer.collider != null) {
			if (_rayToPlayer.collider.gameObject.tag == "Hero") {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	IEnumerator WaitForAttack(){
		while (!isCollisionWithPlayer ()) {
			transform.position = Vector3.MoveTowards (gameObject.transform.position, player.gameObject.transform.position, 5 * Time.deltaTime);
			SetAnimation ("isRun", true);
			yield return new WaitForFixedUpdate ();
		}
		while (!isFinishAttack) {
			AttackOrSlide (true);
			yield return new WaitForFixedUpdate ();
		}
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		while (!gameManager.isFinishWave) {
			transform.position = Vector3.MoveTowards (gameObject.transform.position, originPos, 5 * Time.deltaTime);
			AttackOrSlide (false);
			if (gameObject.transform.position.x == originPos.x) {
				gameManager.isFinishWave = true;
				gameManager.ShowQuestion ();
			}
			yield return new WaitForFixedUpdate ();
		}
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		SetAnimation ("isRun", false);
		isFinishAttack = false;
		FinishWave ();
	}

	void AttackOrSlide(bool isState){
		if (AnimRadomed == 1) {
			SetAnimation ("isAttack", isState);
		} else {
			SetAnimation ("isJumpSlide", isState);		
		}
	}

	public void CheckFinishAttack(){
		isFinishAttack = true;
	}

	public void CheckHitted(){
		isAttacked = true;
		if (isAttacked && isCollisionWithPlayer ()) {
			gameManager.RandomDamage ();
		}
	}



	void FinishWave(){
		isAttacked = false;
		gameManager.ChangeCurrentEnemy ();
	}

	void Shoot(){
		SetAnimation ("isThrow", true);
	}

	public void InstantiateKunai(){
		kunai.SetActive (true);
		kunai.transform.position = this.gameObject.transform.GetChild (1).gameObject.transform.position;
	}

	public IEnumerator FinishShoot(){
		SetAnimation ("isThrow", false);
		while (!kunai.GetComponent<Kunai>().isRayToPlayer) {
			isFinishAttack = true;
			gameManager.isFinishWave = true;
			gameManager.ShowQuestion ();
			FinishWave ();
			yield return new WaitForFixedUpdate ();
		}
	}
}
