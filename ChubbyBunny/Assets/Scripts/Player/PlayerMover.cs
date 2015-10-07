/*
 * 	Code included in Zenrin Co. package
 * 	Heavily modified for ChubbyBunny
 */ 

using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour {

	//	Max time for a tap (seconds)
	public const float tapTime = 0.15f;

	//	Max rotation for looking up/down
	public const float maxDownRotation = 15f;
	public const float maxUpRotation = 50f;

	[SerializeField]
	GameObject QueryObject;

	//	Supports up to 2 touches simultaneously
	public GameObject touchAssistantFab;
	private TouchAssistant touchAssistant1;
	private TouchAssistant touchAssistant2;

	//	Movement attributes
	public float moveSpeed;
	public float rotateSpeed;

	[HideInInspector]
	public bool canMove;

	//	Rotation stuff
	private float vertRote;
	private float horizRote;

	//	Flying animation states
	QueryAnimationController.QueryChanAnimationType nowFlyingState;
	QueryAnimationController.QueryChanAnimationType previousFlyingState;

	private CharacterController controller;

	//	Current speed from gravity
	private float gravity;

	void Start () {
		canMove = true;
		vertRote = horizRote = gravity = 0;
		controller = GetComponent<CharacterController>();

		//	Set initial state/animation
		nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_IDLE;
		previousFlyingState = nowFlyingState;
		QueryObject.GetComponent<QueryAnimationController>()
			.ChangeAnimation(QueryAnimationController.QueryChanAnimationType.FLY_IDLE);

		//	Spawn touch assistants
		GameObject go = Instantiate (touchAssistantFab, Vector3.zero, Quaternion.identity) as GameObject;
		touchAssistant1 = go.GetComponent<TouchAssistant> ();
		touchAssistant1.playerControl = this.GetComponent<PlayerController>();
		touchAssistant1.playerMover = this;

		go = Instantiate (touchAssistantFab, Vector3.zero, Quaternion.identity) as GameObject;
		touchAssistant2 = go.GetComponent<TouchAssistant> ();
		touchAssistant2.playerControl = this.GetComponent<PlayerController>();
		touchAssistant2.playerMover = this;
	}

	void Update () {
		if (canMove) {
			//	Windows/Mac controls
			#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
			updatePCRotation ();
			updatePCMove ();
			#endif

			//	iOS touch controls
			#if UNITY_IOS
			processiOS ();
			#endif
		}

		//	Add force of gravity
		gravity += Physics.gravity.y * Time.deltaTime * 0.02f;
		controller.Move (new Vector3(0, gravity, 0));
		if (controller.isGrounded)
			gravity = 0;
	}

	//	For reading and assigning touches
	void processiOS() {
		int nbTouches = Input.touchCount;
		if (nbTouches > 0) {
			for (int i = 0; i < nbTouches; i++) {
				Touch touch = Input.GetTouch(i);
				
				//	Check if update to currently tracked touch
				if(!touchAssistant1.free && touch.fingerId == touchAssistant1.getTouchID()) {
					touchAssistant1.updateTouch(touch);
				}
				else if(!touchAssistant2.free && touch.fingerId == touchAssistant2.getTouchID()) {
					touchAssistant2.updateTouch(touch);
				}
				else {
					//	Not owned by either assistant, check if any are free
					if(touchAssistant1.free)
						touchAssistant1.assignTouch(touch);
					else if(touchAssistant2.free)
						touchAssistant2.assignTouch(touch);
					
					//	If none are free, touch is ignored
				}
			}
		}
	}

	public void updateiOSRotation(float leftRote, float upRote) {
		//	Rotate at constant speed
		horizRote += leftRote * rotateSpeed * Time.deltaTime;

		vertRote -= upRote * rotateSpeed * Time.deltaTime * 0.7f;
		vertRote = Mathf.Clamp(vertRote, -maxUpRotation, maxDownRotation);

		this.transform.localRotation = Quaternion.Euler(vertRote, horizRote, 0);
	}

	//	Called by touch assistants
	public void updateiOSMove(float strafe, float forward) {
		//	Animation based on strafe values
		if (strafe > 0)
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_TORIGHT;
		else if (strafe < 0)
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_TOLEFT;
		else
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_STRAIGHT;

		//	Normalize and factor move speed and delta time
		Vector3 strafeVec = transform.right*strafe;
		strafeVec *= moveSpeed*Time.deltaTime;

		Vector3 forwardVec = transform.forward*forward;
		forwardVec.y = 0;
		forwardVec *= moveSpeed*Time.deltaTime;

		//	Dampen the vectors so you don't move so fast
		strafeVec /= 10f;
		forwardVec /= 10f;

		//	Take average of 2 vectors
		Vector3 moveDirection = (strafeVec + forwardVec) / 2f;
		
		// Move Forward
		controller.Move (moveDirection);
		
		//	Idle if not moving or moving straight backwards
		moveDirection.Normalize();
		if (moveDirection == Vector3.zero || moveDirection == -this.transform.forward) {
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_IDLE;
		}
		
		//	Change flight animation
		if (previousFlyingState != nowFlyingState) {
			QueryObject.GetComponent<QueryAnimationController>().ChangeAnimation(nowFlyingState);
		}
		
		previousFlyingState = nowFlyingState;
	}

	void updatePCRotation() {
		//	Rotate about y axis
		Vector3 rotation = new Vector3(0, Input.GetAxis("Rotate")*rotateSpeed*Time.deltaTime*100f, 0);
		this.transform.Rotate(rotation);
	}
	
	void updatePCMove() {
		Vector3 moveDirection = Vector3.zero;
		Vector3 strafe = Vector3.zero;
		Vector3 forward = Vector3.zero;
		
		//	Strafe left/right
		if (Input.GetAxis ("Horizontal") > 0) {
			strafe = this.transform.right * moveSpeed;
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_TORIGHT;
		}
		else if (Input.GetAxis("Horizontal") < 0) {
			strafe = -this.transform.right * moveSpeed;
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_TOLEFT;
		}
		else
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_STRAIGHT;
		
		//	Move forward/backward
		if (Input.GetAxis("Vertical") > 0) {
			forward = this.transform.forward * moveSpeed;

		}
		else if (Input.GetAxis("Vertical") < 0) {
			forward = -this.transform.forward * moveSpeed;
		}

		strafe *= 10f;
		forward *= 10f;

		//	Take average of 2 vectors
		moveDirection = (strafe + forward) / 2f;
		
		// Move Forward
		controller.Move (moveDirection*Time.deltaTime);

		//	Idle if not moving or moving straight backwards
		moveDirection.Normalize();
		if (moveDirection == Vector3.zero || moveDirection == -this.transform.forward) {
			nowFlyingState = QueryAnimationController.QueryChanAnimationType.FLY_IDLE;
		}
		
		//	Change flight animation
		if (previousFlyingState != nowFlyingState) {
			QueryObject.GetComponent<QueryAnimationController>().ChangeAnimation(nowFlyingState);
		}
		
		previousFlyingState = nowFlyingState;
	}

	//	Called by touch assistants
	public void setIdleAnimation() {
		QueryObject.GetComponent<QueryAnimationController>()
			.ChangeAnimation(QueryAnimationController.QueryChanAnimationType.FLY_IDLE);
	}
}
