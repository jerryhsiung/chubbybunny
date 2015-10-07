using UnityEngine;
using System.Collections;

public class ProjectileController_Necromancer : ProjectileController_Base {

	public override void OnCollisionEnter(Collision col) {
		if (col.gameObject != parent && col.gameObject.tag != "NecroAttack") {
			//	Create the hit effect
			Instantiate (hitEffectFab, this.transform.position, Quaternion.identity);

			//	Damage if creature or player
			if(col.gameObject != null) {
				if(col.gameObject.tag == "Player")
					col.gameObject.GetComponent<PlayerController>().takeDamage(attackDmg, 0);
				else if(col.gameObject.layer == LayerMask.NameToLayer("Creatures"))
					col.gameObject.GetComponent<EnemyController_Base>().takeDamage(attackDmg);
			}

			Destroy(this.gameObject);
		}
	}
}