using UnityEngine;
using System.Collections;

public class ProjectileController_Player : ProjectileController_Base {
	
	public override void OnCollisionEnter(Collision col) {
		if (col.gameObject != parent && col.gameObject.tag != "PlayerAttack") {
			//	Create the hit effect
			Instantiate (hitEffectFab, this.transform.position, Quaternion.identity);

			//	Kill if drone
			if(col.gameObject.tag == "Drone") {
				col.gameObject.GetComponent<EnemyController_Drone>().killed();
			}
			//	Damage if monster
			else if(col.gameObject.layer == LayerMask.NameToLayer("Creatures") && col.gameObject != null) {
				col.gameObject.GetComponent<EnemyController_Base>().takeDamage(attackDmg);
			}
		
			Destroy(this.gameObject);
		}
	}
}
