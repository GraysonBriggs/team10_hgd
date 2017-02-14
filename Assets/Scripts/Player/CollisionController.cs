using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	public PlayerController player;
	public GameController game;

	void Start() {
		player = GetComponent<PlayerController> ();
		game = GameObject.Find ("Game").GetComponent<GameController> ();
	}

	//For objects that aren't triggers
	void OnCollisionStay2D(Collision2D other) {
		switch (other.gameObject.tag) {

		case "LaserBeam":
			player.applyDamage (10f);
			break;
		case "Lava":
			player.applyDamage (20f);
			break;
		case "Slime":
			if(!player.powerUp.Equals("jump"))
			player.jumpForce = 600f;
			break;
		}
	}

	//For objects set as triggers
	void OnTriggerStay2D(Collider2D other) {
		switch (other.gameObject.tag) {

		case "Obstacle_Damage":
			player.applyDamage (10f);
			break;
		case "Saw":
			player.applyDamage (30f);
			break;
		case "Ladder":
			player.onLadder = true;
			break;
		case "EndFlag":
			game.endPlayerPhase ();
			break;
		case "Spike":
			player.applyDamage (10f);
			break;
		case "OutOfBounds":
			player.applyDamage (1000f);
			break;
		}
	}

	void OnCollisionExit2D(Collision2D col)
	{
		player.resetAttributesToDefault ();
	}

	void OnTriggerExit2D(Collider2D col)
	{
		player.resetAttributesToDefault ();
	}
}
