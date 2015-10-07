using UnityEngine;
using System.Collections;

public class TouchAssistant : MonoBehaviour {

	//	Threshold to pass before no longer moving straight (in pixels)
	public const float moveTouchThreshold = 40f;
	public const float moveTouchMax = 70f;

	//	Threshold to pass before considered rotating
	public const float horizRotateTouchThreshold = 20f;
	public const float horizRotateMax = 100f;
	public const float vertRotatetouchThreshold = 30f;

	public PlayerController playerControl;
	public PlayerMover playerMover;
	
	public bool free;	//	If free to assign new touch

	public bool moveStart;				//	If can begin processing movement
	public Vector2 startMoveTouchPos;	//	Position of initial movement touch

	public bool rotateStart;			//	If can begin processing rotation
	public Vector2 startRotateTouchPos;	//	Position of initial rotation touch

	public bool pickupStart;				//	If can begin processing object pick up
	public Vector2 startPickupTouchPos;		//	Position of initial pickup touch
	public float pickupTouchDistance = 2f;	//	How far you can move finger to still be considered picking up
	public float pickupTime = 1f;			//	How long you have to hold to pick up
	public float pickupStartTime;			//	The time when you started the touch

	private Touch myTouch;
	private float startTime;	//	Time when touch first occured

	void Start() {
		free = true;
		moveStart = rotateStart = pickupStart = false;
		pickupStartTime = 0;
	}

	//	Update every new assignment or touch update
	void processTouch() {
		switch (myTouch.phase) {
		case TouchPhase.Began:

			//	To detect taps
			startTime = Time.time;

			//	To detect pickup
			pickupStart = true;
			startPickupTouchPos = myTouch.position;
			pickupStartTime = Time.time;

			//	To detect movement/rotation
			if(myTouch.position.x <= Screen.width/2f) {
				//	Movement
				moveStart = true;
				startMoveTouchPos = myTouch.position;
			}
			else {
				//	Rotation
				rotateStart = true;
				startRotateTouchPos = myTouch.position;
			}

			break;

		//	Same for both these two cases
		case TouchPhase.Moved:
		case TouchPhase.Stationary:

			if(moveStart) {
				//	Get vector from start pos to curr pos
				Vector2 moveVec = myTouch.position - startMoveTouchPos;

				//	Interpret x and y values
				float strafe = moveVec.x;
				float forward = moveVec.y;

				//	Clamp the max movement speed
				if(Mathf.Abs(strafe) > moveTouchMax)
					strafe = moveTouchMax * Mathf.Sign(strafe);
				if(Mathf.Abs(forward) > moveTouchMax)
					forward = moveTouchMax * Mathf.Sign(forward);

				//	Check threshold
				float sign = Mathf.Sign(strafe);
				if(Mathf.Abs(strafe) <= moveTouchThreshold) {
					//	Interpolate for smooth acceleration
					strafe = Mathf.Lerp(0, moveTouchThreshold, moveVec.x / moveTouchThreshold);
					strafe *= sign;
				}

				sign = Mathf.Sign(forward);
				if(Mathf.Abs(forward) <= moveTouchThreshold) {
					//	Interpolate for smooth acceleration
					forward = Mathf.Lerp(0, moveTouchThreshold, moveVec.y / moveTouchThreshold);
					forward *= sign;
				}

				playerMover.updateiOSMove(strafe, forward);
			}
			else if(rotateStart) {
				//	Get vector from start pos to curr pos
				Vector2 roteVec = myTouch.position - startRotateTouchPos;
				
				//	Interpret x and y values
				float leftRote = roteVec.x;
				float upRote = roteVec.y;

				//	Clamp the max horiz rotate speed
				if(Mathf.Abs(leftRote) > horizRotateMax)
					leftRote = horizRotateMax * Mathf.Sign(leftRote);

				//	Check threshold
				float sign = Mathf.Sign(leftRote);
				if(Mathf.Abs(leftRote) <= horizRotateTouchThreshold) {
					leftRote = Mathf.Lerp(0, horizRotateTouchThreshold, roteVec.x / horizRotateTouchThreshold);
					leftRote *= sign;
				}

				sign = Mathf.Sign(upRote);
				if(Mathf.Abs(upRote) <= vertRotatetouchThreshold) {
					upRote = Mathf.Lerp(0, vertRotatetouchThreshold, roteVec.y / vertRotatetouchThreshold);
					upRote *= sign;
				}

				playerMover.updateiOSRotation(leftRote, upRote);
			}

			if(pickupStart) {
				//	Check if held long enough
				if(Time.time - pickupStartTime >= pickupTime) {
					Debug.Log("Picking up now");
					playerControl.pickupObject(new Vector3(myTouch.position.x, myTouch.position.y, 0));
					pickupStart = false;
				}
				else {
					Vector2 moveDist = myTouch.position - startPickupTouchPos;
					if(Mathf.Abs (moveDist.x) > pickupTouchDistance || Mathf.Abs(moveDist.y) > pickupTouchDistance) {
						//	You done effed up
						pickupStart = false;
						Debug.Log("Move finger too far");
					}
				}
			}

			break;

		case TouchPhase.Ended:

			//	If it's a tap fire a bolt
			if (Time.time - startTime <= PlayerMover.tapTime && !GameEngine.pauseGame) {
				//	Convert touch position to a ray
				Vector2 touchPos = myTouch.position;
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(touchPos.x, touchPos.y, 0));
				Vector3 forceDir;
				//	Raycast and check if hit anything
				RaycastHit hitInfo;
				if(Physics.Raycast(ray.origin, ray.direction, out hitInfo)) {
					forceDir = hitInfo.point - playerControl.spawnPos.position;

					//	First check if hit a pickup and holding it
					if(hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Pickups")) {
						if(playerControl.pickupObj != null
						   && playerControl.pickupObj == hitInfo.transform.gameObject)
						{
							Debug.Log("Dropping object");
							playerControl.dropObject();
							break;
						}
					}
				}
				else {
					//	Get a world point 1000 units away instead
					Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, 1000f));
					forceDir = point - playerControl.spawnPos.position;
				}

				//	Instantiate energy bolt
				forceDir.Normalize();
				playerControl.fireEnergyBolt(forceDir);
			}
			else if(moveStart)
				playerMover.setIdleAnimation();

			free = true;
			moveStart = false;

			break;

		case TouchPhase.Canceled:

			break;
		}
	}

	//	Assign a new touch to track
	public void assignTouch(Touch touch) {
		myTouch = touch;
		free = false;

		processTouch ();
	}

	//	Return ID of current touch
	public int getTouchID() {
		return myTouch.fingerId;
	}

	//	Update the touch being tracked
	public void updateTouch(Touch touch) {
		myTouch = touch;
		processTouch ();
	}
}
