using UnityEngine;
using System.Collections.Generic;

public static class EnemyList {

	public static List<EnemyRadius> enemyList = new List<EnemyRadius>();

	public static Vector2 GetEnemyPosition(Vector2 position) {
		List<EnemyRadius> enemiesInRadius = new List<EnemyRadius>();

		for(int i = 0; i < enemyList.Count; i++) {
			if(Mathf.Abs(GetDistanceFromEnemy(enemyList[i], position)) < enemyList[i].radius) {
				//Casts a ray from the hero to the enemy.
				RaycastHit2D tHit = Physics2D.Linecast(position, enemyList[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

				//If the ray doesn't hit anything, starts the dash ( Prevents the hero from clipping through blocks ).
				if(tHit.collider == null)
					enemiesInRadius.Add(enemyList[i]);
			}
		}

		//If the hero is within the radius of an enemy.
		if(enemiesInRadius.Count > 0) {
			//Grabs the first enemy in the list.
			ushort tEnemy = 0;

			//If the hero is within the radius of more than 1 enemy.
			//Parses through the rest of the enemies to find the one with the highest priority.
			//If there's multiple enemies with the same priority, grabs the closest one.
			for(ushort i = 1; i < enemiesInRadius.Count; i++) {
				if(enemiesInRadius[i].priority < enemiesInRadius[tEnemy].priority)
					tEnemy = i;
				else if(enemiesInRadius[i].priority == enemiesInRadius[tEnemy].priority) {
					if(GetDistanceFromEnemy(enemiesInRadius[i], position) < GetDistanceFromEnemy(enemiesInRadius[tEnemy], position))
						tEnemy = i;
				}
			}

			//Returns the highest priority / closest enemy.
			return enemiesInRadius[tEnemy].transform.position;
		}

		return Vector2.zero;
	}

	public static Vector2 GetDash(Vector2 position) {
		List<EnemyRadius> enemiesInRadius = new List<EnemyRadius>();

		for(int i = 0; i < enemyList.Count; i++) {
			if(Mathf.Abs(GetDistanceFromEnemy(enemyList[i], position)) < enemyList[i].radius) {
				float direction = LevelManager.Instance.player.GetComponent<Rigidbody2D>().velocity.x;
				if(enemyList[i].transform.position.x - position.x < 0 && direction < 0 || enemyList[i].transform.position.x - position.x > 0 && direction > 0) {
					//Casts a ray from the hero to the enemy.
					RaycastHit2D tHit = Physics2D.Linecast(position, enemyList[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

					//If the ray doesn't hit anything, starts the dash ( Prevents the hero from clipping through blocks ).
					if(tHit.collider == null)
						enemiesInRadius.Add(enemyList[i]);
				}
			}
		}

		//If the hero is within the radius of an enemy.
		if(enemiesInRadius.Count > 0) {
			//Grabs the first enemy in the list.
			ushort tEnemy = 0;

			//If the hero is within the radius of more than 1 enemy.
			//Parses through the rest of the enemies to find the one with the highest priority.
			//If there's multiple enemies with the same priority, grabs the closest one.
			for(ushort i = 1; i < enemiesInRadius.Count; i++) {
				if(enemiesInRadius[i].priority < enemiesInRadius[tEnemy].priority)
					tEnemy = i;
				else if(enemiesInRadius[i].priority == enemiesInRadius[tEnemy].priority) {
					if(GetDistanceFromEnemy(enemiesInRadius[i], position) < GetDistanceFromEnemy(enemiesInRadius[tEnemy], position))
						tEnemy = i;
				}
			}

			//If the enemy isn't invincible, destroy it once the hero finishes dashing ( temporary solution to destroy enemies ).
			if(!enemiesInRadius[tEnemy].isInvincible)
				Object.Destroy(enemiesInRadius[tEnemy].transform.parent.gameObject, (position - (Vector2)enemiesInRadius[tEnemy].transform.position).magnitude / 14f - 0.05f);

			//Returns the highest priority / closest enemy.
			return enemiesInRadius[tEnemy].transform.position;
		}

		return Vector2.zero;
	}

	//Add an enemy (EnemyRadius) to the enemyList.
	//Called from the Start function of all enemies.
	public static void AddEnemy(EnemyRadius enemy) {
		enemyList.Add(enemy);
	}

	//Remove an enemy (EnemyRadius) from the enemyList.
	//Called from the OnDestroy function when an enemy is destroyed.
	public static void DestroyEnemy(EnemyRadius enemy) {
		for(int i = 0; i < enemyList.Count; i++) {
			if(enemyList[i] == enemy) {
				enemyList.RemoveAt(i);
				break;
			}
		}
	}

	public static void UpdateAreaAlpha(Vector2 heroPosition) {
		for(int i = 0; i < enemyList.Count; i++) {
			float distanceFromArea = GetDistanceFromEnemy(enemyList[i], heroPosition) - enemyList[i].radius;

			if(distanceFromArea < enemyList[i].radius * 2f) {
				if(distanceFromArea > 0f)
					enemyList[i].SetAlpha(1f - distanceFromArea / (enemyList[i].radius * 2f));
				else
					enemyList[i].SetAlpha(1f);
			}
			else
				enemyList[i].SetAlpha(0f);
		}
	}

	//Returns the distance between an enemy and the hero.
	private static float GetDistanceFromEnemy(EnemyRadius enemy, Vector2 heroPosition) {
		return new Vector2(enemy.transform.position.x - heroPosition.x, enemy.transform.position.y - heroPosition.y).magnitude;
	}
}