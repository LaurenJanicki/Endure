﻿using UnityEngine;
using System.Collections;

public class EnemyFullAI : MonoBehaviour {

	//Basic AI Stats	
	public float speed = 3f;
	public float aggro = 5f;
	public float deAggro = 6f; 
	
	//For idle movement
	public float moveDuration = 0.8f;
	public float idleDuration = 0.6f;
	
	//Basic AI stats that player shouldn't reaaaally care about
	private bool active = false;
	private Rigidbody2D rb2d;
	private PlayerController player;
	private Vector3 lastPlayerPos;
	private Vector3 heading;
	private Vector3 targetHeading;
	//FOr idle movement (Privately set)
	private Animator animator;
	private Vector3 oldPosition;
	private float animationTime = 0f;
	
	//For weapons
	//private MeleeAttacker melee;
	//private RangedAttacker ranged;
	[System.Serializable]
	public class Melee {
		public bool isMelee = false;
		public MeleeAttacker meleeWeapon;
	}
	public Melee melee;

	[System.Serializable]
	public class Ranged {
		public bool isRanged = false;
		public float rangedDistance = 4f;
		public RangedAttacker rangedWeapon;
	}
	public Ranged ranged;

	[System.Serializable]
	public class Coward {
		public bool isCoward = false;
		public float cowardDistance = 2f;
		public float maxRunDistance = 6f;
		private bool feared = false;
	}
	public Coward coward;

	// Use this for initialization
	void Start () {
		this.player = PlayerController.instance;
		this.rb2d = this.GetComponent<Rigidbody2D> ();
		this.melee.meleeWeapon = this.GetComponent<MeleeAttacker> ();
		this.animator = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.active) {
			this.targetHeading = this.player.transform.position - this.transform.position;
			//print (heading.magnitude + "|" + this.aggro);
			if(this.targetHeading.magnitude < this.aggro){
				this.lastPlayerPos = player.transform.position;
				this.MeleeAttack ();
			} else if (this.targetHeading.magnitude < this.deAggro){
				this.MeleeAttack ();
			} else if(this.targetHeading.magnitude >= this.deAggro){
				this.IdleMovement ();
			}
			
			// Make sure enemy is on the right layer
			if(this.transform.position.z != (float)(this.transform.position.y + 16)){
				this.transform.position = new Vector3(this.transform.position.x,
				                                      this.transform.position.y,
				                                      (float)(this.transform.position.y + 16));
			}

			//Animation for player movement
			if (this.rb2d.velocity.x > 0) {
				animator.SetBool("moving", true);
				transform.localScale = new Vector3(1f, 1f, 1f);
			} else if (this.rb2d.velocity.x < 0) {
				animator.SetBool("moving", true);
				transform.localScale = new Vector3(-1f, 1f, 1f);
			} else {
				animator.SetBool("moving", false);
			}
		}
		
	}
	//moveTo should be a vector2 of position
	public void MoveTo (Vector3 moveTo){
		this.heading = moveTo - this.transform.position;
		this.rb2d.velocity = this.heading.normalized * this.speed;
	}

	void MeleeAttack (){
		//heading = this.lastPlayerPos - this.transform.position;
		this.oldPosition = this.transform.position;
		if (!this.melee.meleeWeapon.Locked) {
			this.MoveTo (this.lastPlayerPos);
		} else {
			this.rb2d.velocity = Vector2.zero;
		}
		
		if (this.targetHeading.magnitude < this.melee.meleeWeapon.range + 0.5f) {
			Vector3 n = this.targetHeading.normalized;
			if (n.x > Mathf.Sqrt (2) / 2) {
				this.melee.meleeWeapon.AttackEast ();
			} else if (n.x < - Mathf.Sqrt (2) / 2) {
				this.melee.meleeWeapon.AttackWest ();
			} else if (n.y > Mathf.Sqrt (2) / 2) {
				this.melee.meleeWeapon.AttackNorth ();
			} else if (n.y < -Mathf.Sqrt (2) / 2) {
				this.melee.meleeWeapon.AttackSouth ();
			}
		}
	}
	void RangedAttack (){

	}
	void CowardRun (){

	}
	void IdleMovement (){
		if(this.oldPosition == Vector3.zero){
			this.oldPosition = this.transform.position;
		}
		
		this.animationTime += Time.deltaTime;
		
		//print (this.oldPosition);
		
		if(this.animationTime < this.idleDuration){
			this.rb2d.velocity = Vector2.zero;
		} else if (this.animationTime >= this.idleDuration){
			if(this.animationTime > this.moveDuration + this.idleDuration){
				this.rb2d.velocity = Vector2.zero;
				this.animationTime = 0;
			} else if (this.rb2d.velocity == Vector2.zero){
				float rx = Random.Range (-1f, 1f);
				float ry = Random.Range (-1f, 1f);
				Vector3 idleHeading = this.oldPosition - this.transform.position;
				this.rb2d.velocity = speed * (idleHeading - new Vector3(rx, ry)).normalized;
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D collided){
		if(collided.tag == "Player"){
			//print ("Activated Enemy");
			this.active = true;
		}
	}
	
	void OnTriggerExit2D (Collider2D collided) {
		CircleCollider2D playerCollider = player.GetComponent<CircleCollider2D> ();
		this.targetHeading = player.transform.position - this.transform.position;
		//print ("collider radius: " + playerCollider.radius + " headingmag: " + heading.magnitude);
		if (collided.tag == "Player" && this.targetHeading.magnitude >= playerCollider.radius) {
			//print ("Deactivated Enemy");
			this.active = false;
		}
	}
	
	
	public void enemyActive (bool a){
		this.active = a;
	}
}
