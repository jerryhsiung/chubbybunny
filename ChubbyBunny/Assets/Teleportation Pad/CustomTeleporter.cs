using UnityEngine;
using System.Collections;

/*
 * Code edited by team ChubbyBunny
 * */

public class CustomTeleporter : MonoBehaviour
{
	//one or more destination pads
	public Transform[] destinationPad;
	//height offset
	public float teleportationHeightOffset = 1;
	//a private float counting down the time
	private float curTeleportTime;
	//private bool checking if you entered the trigger
	private bool inside;
	//check to wait for arrived object to leave before enabling teleporation again
	[HideInInspector]
	public bool arrived;
	//gameobjects inside the pad
	private Transform subject;
	//add a sound component if you want the teleport playing a sound when teleporting
	//	public AudioSource teleportSound;
	//add a sound component for the teleport pad, vibrating for example, or music if you want :D
	//also to make it more apparent when the pad is off, stop this component playing with "teleportPadSound.Stop();"
	//PS the distance is set to 10 units, so you only hear it standing close, not from the other side of the map
	//	public AudioSource teleportPadSound;
	//simple enable/disable function in case you want the teleport not working at some point
	//without disabling the entire script, so receiving objects still works
	public bool teleportPadOn = true;
	
	static int count = 0;
	
	//connect to the sector engines
	public GameObject se2;
	SectorEngine engine2;
	
	string name;
	
	void Start () {
		engine2 = se2.GetComponent<SectorEngine> ();
	}

	void Update () {
		if(inside) {
			//if that object hasnt just arrived from another pad, teleport it
			if(!arrived && teleportPadOn)
				Teleport();
		}
	}
	
	void Teleport()
	{
		//set arrived to true in that array, so it doesnt teleport the subject back
		destinationPad[0].GetComponent<CustomTeleporter>().arrived = true;
		//and teleport the subject
		subject.transform.position = destinationPad[0].transform.position + new Vector3(0,teleportationHeightOffset,0);
		//play teleport sound
		//				teleportSound.Play();
		
		//for sector 2
		if(this.gameObject.name == "Teleporter Pad 1" && name=="Player"){
			count = 1;
		}
		else if (this.gameObject.name == "Teleporter Pad 2" && count==1 && name=="Player"){
			count++;
		}
		else if(this.gameObject.name == "Teleporter Pad 3" && count==2 && name=="Player"){
			count++;
		}
		else if(this.gameObject.name == "Teleporter Pad 4" && count==3 && name=="Player"){
			count++;
		}
		else{
			count = 0;
		}
		
		if(count==4){
			for(int i=0; i<destinationPad.Length; i++){
				destinationPad[i].GetComponent<CustomTeleporter>().gameObject.SetActive(false);
			}
			engine2.donePuzzle();
		}
	}
	
	void OnTriggerEnter(Collider trig)
	{
		if (trig.gameObject.tag != "Player")
			return;
		
		name = trig.gameObject.name;
		subject = trig.transform;
		inside = true;
	}
	
	void OnTriggerExit(Collider trig)
	{
		//set that the object left
		inside = false;
		//if the object that left the trigger is the same object as the subject
		if(trig.transform == subject)
		{
			//set arrived to false, so that the pad can be used again
			arrived = false;
		}
		//remove the subject from the pads memory
		subject = null;
	}
}
