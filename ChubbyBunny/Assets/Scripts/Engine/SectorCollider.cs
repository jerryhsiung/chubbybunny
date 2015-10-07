using UnityEngine;
using System.Collections;

public class SectorCollider : MonoBehaviour {

	public bool activateCollider;

	public GameObject parentSectorObject;
	private SectorEngine parentSector;

	public GameObject partnerCollider;

	[HideInInspector]
	public bool sector1 = false;
	[HideInInspector]
	public bool sector2 = false;
	[HideInInspector]
	public bool onHighway = true;

	void Start() {
		parentSector = parentSectorObject.GetComponent<SectorEngine> ();
	}

	void OnTriggerEnter(Collider col) {
		//	Check if player
		if (col.tag == "Player") {
			if(activateCollider) {
				//	Activate the sector
				parentSector.activateSector();
				if(parentSector.name == "Sector2Engine") {
					this.sector2 = true;
					partnerCollider.GetComponent<SectorCollider>().sector2 = true;
				}
				else if(parentSector.name == "Sector1Engine") {
					this.sector1 = true;
					partnerCollider.GetComponent<SectorCollider>().sector1 = true;
				}

				onHighway = false;
			}
			else {
				//	Deactivate the sector
				parentSector.deactivateSector();
				if(parentSector.name == "Sector2Engine") {
					this.sector2 = false;
					partnerCollider.GetComponent<SectorCollider>().sector2 = false;
				}
				else if(parentSector.name == "Sector1Engine") {
					sector1 = false;
					partnerCollider.GetComponent<SectorCollider>().sector1 = false;
				}

				onHighway = true;
			}

			//	Deactivate this and activate partner collider
			this.gameObject.SetActive(false);
			partnerCollider.SetActive(true);
		}
	}
	/*
	void OnTriggerExit(Collider col) {
		if(col.tag == "Player") {
			GameObject player = col.gameObject;
			Vector3 vec = this.collider.ClosestPointOnBounds(player.transform.position) - enterLoc;
			float dot = Mathf.Abs(Vector3.Dot(vec.normalized, this.transform.forward.normalized));

			//	Check if vector from enterLoc to exitLoc is parallel to forward
			if((dot < 0.99f || dot > 1.01f) && vec.magnitude > 0.5f) {
				//	Player crossed the boundary
				if(parentSector.isActive) {
					//	Leaving parent sector
					parentSector.deactivateSector();
					//change bool for dialog box when leaving
					if(this.gameObject.name == "Wall1"){
						sector1 = false;
						onHighway = true;
					}
					else if(this.gameObject.name == "Wall2"){
						sector2 = false;
						onHighway = true;
					}
				}
				else {
					//	Entering parent sector
					parentSector.activateSector();
					//trigger dialog box when first entered
					if(this.gameObject.name == "Wall1"){
						sector1 = true;
						onHighway = false;
					}
					else if(this.gameObject.name == "Wall2"){
						sector2 = true;
						onHighway = false;
					}
				}
			}
		}
	}*/
}
