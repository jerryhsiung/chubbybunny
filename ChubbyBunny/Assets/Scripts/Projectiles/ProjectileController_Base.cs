using UnityEngine;
using System.Collections;

public class ProjectileController_Base : MonoBehaviour {

	public GameObject hitEffectFab;

	[HideInInspector]
	public GameObject parent;

	protected float attackDmg;

	public virtual void OnCollisionEnter (Collision col) {}

	public void setAttackDmg(float dmg) {
		attackDmg = dmg;
	}
}
