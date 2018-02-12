using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : MonoBehaviour {

	public float speed;
	Player player;
	public bool isRayToPlayer;
	Vector3 dir;
	GameManager gameManager;
	float angle;
	void Awake () {
		gameManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<GameManager> ();
		player = GameObject.FindGameObjectWithTag ("Hero").GetComponent<Player> ();
		Debug.Log ("Start");
	}

	void OnEnable(){
		Debug.Log ("OnEnable");
		angle = gameManager.angleKunai;
		Debug.Log ("Angle of Kunai = " + angle);
		gameObject.transform.localEulerAngles = new Vector3 (0, 0, angle);
		StartCoroutine (CollisionWithPlayer ());
	}

	void Movement(){
		transform.Translate (Vector3.left * speed * Time.deltaTime);
	}

	IEnumerator CollisionWithPlayer(){
		Debug.Log ("<color=blue>Vo roisssss</color>");
		while (!isRayToPlayer) {
			RaycastHit2D _ray = Physics2D.Linecast (gameObject.transform.position, gameObject.transform.GetChild (0).gameObject.transform.position);
			if (_ray.collider != null){
				if (_ray.collider.gameObject.tag == "Hero") {
					isRayToPlayer = true;
					player.GetComponent<Animator> ().SetBool ("isHit", true);
					Debug.Log ("<color=red> KUNAI PLAYER</color>");
					gameManager.RandomDamage ();
				}
			}
			Movement ();
			yield return new WaitForFixedUpdate ();
		}
		this.gameObject.SetActive (false);
	}

	void OnDisable(){
		isRayToPlayer = false;
		transform.position = new Vector3 ();
	}
}
