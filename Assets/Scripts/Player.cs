using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood;
	public int pointsPerSoda;
	public float restartLevelDelay = 1f;

	private Animator animator;
	private int food;

	// Use this for initialization
	protected override void Start () {
		
		animator = GetComponent<Animator> ();
		Debug.Log ("food value is: " + food);
		if (GameManager.instance != null) {
			Debug.Log ("food value is: " + food);
			if (GameManager.instance.playerFoodPoints != 0)
				food = GameManager.instance.playerFoodPoints;
			else {
				food = 100;
				Debug.Log ("Food Value manually assigned");
			}
		}
		// for now, the code above is going cra, cuz GameManager.instance is created after Player, so we dont assign food value
		// thats why we do it here
		food = 100;
		base.Start ();
	}

	private void OnDisable(){
		if(GameManager.instance != null)
			GameManager.instance.playerFoodPoints = food;
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance != null)
			if (!GameManager.instance.playersTurn)
				return;
		int horizontal = 0;
		int vertical = 0;
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		if (horizontal != 0) {
			vertical = 0;
		}
		if (horizontal != 0 || vertical != 0) {
			AttemptMove <Wall> (horizontal, vertical);
		}
	}
	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		food--;

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move (xDir, yDir, out hit)) {
			
		}


		CheckIfGameOver ();

		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Exit"){
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if(other.tag == "Food"){
			food += pointsPerFood;
			other.gameObject.SetActive (false);
		}
		else if(other.tag == "Soda"){
			food += pointsPerSoda;
			other.gameObject.SetActive (false);
		}

	}

	protected override void OnCantMove <T> (T component){
		Wall hitWall = component as Wall;
		hitWall.DamageWall (wallDamage);
		animator.SetTrigger ("playerChop");
	}

	private void Restart (){
		SceneManager.LoadScene (0);
	}

	private void CheckIfGameOver(){
		Debug.Log ("check: food is: " + food);
		if (food <= 0)
			GameManager.instance.GameOver ();
	}
	public void LoseFood(int loss){
		animator.SetTrigger("playerHit");
		food -= loss;
		CheckIfGameOver();
	}
}
