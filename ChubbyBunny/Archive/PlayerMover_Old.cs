/*
 * 	REPURPOSED BY TEAM CHUBBYBUNNY
 */

// Mecanimのアニメーションデータが、原点で移動しない場合の Rigidbody付きコントローラ
// サンプル
// 2014/03/13 N.Kobyasahi

using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]
public class PlayerMover : MonoBehaviour
{
	
	public float animSpeed = 1.5f;				//	Animation play speed
	public float lookSmoother = 3.0f;			//	A smoothing setting for camera motion
	public bool useCurves = false;				//	For resetting the collider idk why
	
	/***********	CHUBBYBUNNY EDIT	***********/
	[HideInInspector]
	public bool canMove = true;
	/**********************************************/

	//	Movement speeds
	public float forwardSpeed;
	public float backwardSpeed;
	public float rotateSpeed;

	private CapsuleCollider collider;
	private Rigidbody rb;

	private Vector3 velocity;

	//	Height and center of collider
	private float orgColHeight;
	private Vector3 orgVectColCenter;
	
	private Animator animator;
	private AnimatorStateInfo currentBaseState;
	
	private GameObject cameraObject;
	
	//	State hashes
	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");
	static int restState = Animator.StringToHash("Base Layer.Rest");
	
	//VARIABLES FOR TOUCH INPUT
	public Vector2 startPos;
	public Vector2 direction;
	public bool directionChosen;
	public bool rotationChosen;
	
	//VARIABLES FOR SWIPE
	public float minMovement = 20.0f;
	public bool sendUpMessage = true;
	public GameObject MessageTarget = null;
	private Vector2 StartPos;
	private int SwipeID = -1;
	
	float h = 0;
	float v = 0;

	void Start ()
	{
		//	Get relevant components
		animator = GetComponent<Animator>();
		collider = GetComponent<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();

		cameraObject = GameObject.FindWithTag("MainCamera");
		orgColHeight = collider.height;
		orgVectColCenter = collider.center;
	}

	void FixedUpdate ()
	{
		/***********	CHUBBYBUNNY EDIT	***********/
		if (!canMove)
			return;
		/**********************************************/
		
		/**********   TOUCH EDIT **************/
		// Track a single touch as a direction control.
		
		Touch[] myTouches = Input.touches;
		int nbTouches = Input.touchCount;
		if (nbTouches > 0) {
			for (int i = 0; i < nbTouches; i++) {
				Touch touch = Input.GetTouch(i);
				// check if touch is on left half of screen
				if (touch.position.x > Screen.width/2) {
					rightSide(touch);
				}
				if(touch.position.x < Screen.width/2){
					leftSide(touch);
				}
			}
		}
		/**************************************/
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		animator.SetFloat("Speed", v);			//	Changes Mecanim state to Locomotion/WalkBack
		animator.SetFloat("Direction", h);
		animator.speed = animSpeed;
		currentBaseState = animator.GetCurrentAnimatorStateInfo(0);
		rb.useGravity = true;

		velocity = new Vector3 (0, 0, v);
		velocity = transform.TransformDirection(velocity);

		if (v > 0.1) {
			velocity *= forwardSpeed;
		} else if (v < -0.1) {
			velocity *= backwardSpeed;
		}

		transform.localPosition += velocity * Time.fixedDeltaTime;
		transform.Rotate(0, h * rotateSpeed, 0);	

		if (currentBaseState.nameHash == locoState){
			if(useCurves){
				resetCollider();
			}
		}

		//	Idle state
		else if (currentBaseState.nameHash == idleState)
		{
			if(useCurves){
				resetCollider();
			}
		}

		//	Rest state
		else if (currentBaseState.nameHash == restState)
		{
			if(!animator.IsInTransition(0))
			{
				animator.SetBool("Rest", false);
			}
		}
	}

	void resetCollider()
	{
		collider.height = orgColHeight;
		collider.center = orgVectColCenter;
	}
	
	void leftSide(Touch touch) 
	{
		// Handle finger movements based on touch phase.
		switch (touch.phase) {
			// Record initial touch position.
		case TouchPhase.Began:
			startPos = touch.position;
			directionChosen = false;
			break;
			
			// Determine direction by comparing the current touch position with the initial one.
		case TouchPhase.Moved:
			direction = touch.position - startPos;
			directionChosen = true;
			break;
			
			// Report that a direction has been chosen when the finger is lifted.
		case TouchPhase.Ended:
			directionChosen = false;
			break;
		}
		
		if (directionChosen) {
			//something that uses the chosen direction 
			if (direction.y > 0) {
				v = 0.5f;
			}
			else if (direction.y < 0) {
				v = -0.5f;
			}
		}
		else {
			v = 0;
		}
	}
	
	void rightSide(Touch touch) {
		
		switch (touch.phase) {
			// Record initial touch position.
		case TouchPhase.Began:
			startPos = touch.position;
			rotationChosen = false;
			break;
			
			// Determine direction by comparing the current touch position with the initial one.
		case TouchPhase.Moved:
			direction = touch.position - startPos;
			rotationChosen = true;
			break;
			
			// Report that a direction has been chosen when the finger is lifted.
		case TouchPhase.Ended:
			rotationChosen = false;
			break;
		}
		
		if (rotationChosen) {
			if (direction.x > 0) {
				h = 0.1f;
			}
			else if (direction.x < 0) {
				h = -0.1f;
			}
		} 
		else {
			h = 0;
		}
	}
}
