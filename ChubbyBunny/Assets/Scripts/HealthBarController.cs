using UnityEngine;
using System.Collections;

public class HealthBarController : MonoBehaviour {

	public GameObject barQuadFab;
	public float maxWidth;

	private GameObject healthBar;
	private Transform healthBarPos;
	private EnemyController_Base controller;

	// Use this for initialization
	void Start () {
		//	Get position to place health bar
		healthBarPos = this.transform.FindChild ("Health Bar Pos");
		healthBar = Instantiate (barQuadFab, healthBarPos.position, Quaternion.identity) as GameObject;

		controller = this.gameObject.GetComponent<EnemyController_Base> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (healthBar == null)
			return;

		//	Update position
		healthBar.transform.position = healthBarPos.position;

		//	Update size based on damage
		float fraction = controller.health / controller.maxHealth;

		//	Destroy if enemy died
		if (fraction <= 0)
			Destroy (healthBar);

		healthBar.transform.localScale = new Vector3 (maxWidth*fraction, 0.3f, 0.3f);
	}

	//	Called by monster gens in case of premature death
	public void destroyMe() {
		Destroy (healthBar);
	}
}
