using UnityEngine;
using System.Collections.Generic;

public static class EnemyList {
	private static List<EnemyRadius> _enemyList = null;
	public static List<EnemyRadius> Enemies {
		get {
			if(_enemyList == null)
				_enemyList = new List<EnemyRadius>();
			return _enemyList;
		}
		set {
			_enemyList = value;
		}
	}

	public static void Clear() {
		Enemies.Clear();
		Enemies = null;
	}

	public static Vector2 GetEnemyPosition(Vector2 position) {
		List<EnemyRadius> enemiesInRadius = new List<EnemyRadius>();

		for(int i = 0; i < Enemies.Count; i++) {
			if(Mathf.Abs(GetDistanceFromEnemy(Enemies[i], position)) < Enemies[i].radius) {
				//Casts a ray from the hero to the enemy.
				RaycastHit2D tHit = Physics2D.Linecast(position, Enemies[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

				//If the ray doesn't hit anything, starts the dash ( Prevents the hero from clipping through blocks ).
				if(tHit.collider == null)
					enemiesInRadius.Add(Enemies[i]);
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

		for(int i = 0; i < Enemies.Count; i++) {
			if(Mathf.Abs(GetDistanceFromEnemy(Enemies[i], position)) < Enemies[i].radius) {
				float direction = LevelManager.Instance.player.GetComponent<Rigidbody2D>().velocity.x;
				if(Enemies[i].transform.position.x - position.x < 0 && direction < 0 || Enemies[i].transform.position.x - position.x > 0 && direction > 0) {
					//Casts a ray from the hero to the enemy.
					RaycastHit2D tHit = Physics2D.Linecast(position, Enemies[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

					//If the ray doesn't hit anything, starts the dash ( Prevents the hero from clipping through blocks ).
					if(tHit.collider == null)
						enemiesInRadius.Add(Enemies[i]);
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
		Enemies.Add(enemy);
	}

	//Remove an enemy (EnemyRadius) from the enemyList.
	//Called from the OnDestroy function when an enemy is destroyed.
	public static void DestroyEnemy(EnemyRadius enemy) {
		for(int i = 0; i < Enemies.Count; i++) {
			if(Enemies[i] == enemy) {
				Enemies.RemoveAt(i);
				break;
			}
		}
	}

	public static void UpdateAreaAlpha(Vector2 heroPosition) {
		for(int i = 0; i < Enemies.Count; i++) {
			float distanceFromArea = GetDistanceFromEnemy(Enemies[i], heroPosition) - Enemies[i].radius;

			if(distanceFromArea < Enemies[i].radius * 2f) {
				if(distanceFromArea > 0f)
					Enemies[i].SetAlpha(1f - distanceFromArea / (Enemies[i].radius * 2f));
				else
					Enemies[i].SetAlpha(1f);
			}
			else
				Enemies[i].SetAlpha(0f);
		}
	}

	public static void SetBodyType(RigidbodyType2D bodyType) {
		foreach(EnemyRadius enemy in Enemies)
			enemy.transform.parent.parent.GetComponent<Rigidbody2D>().bodyType = bodyType;
	}

	//Returns the distance between an enemy and the hero.
	private static float GetDistanceFromEnemy(EnemyRadius enemy, Vector2 heroPosition) {
		return new Vector2(enemy.transform.position.x - heroPosition.x, enemy.transform.position.y - heroPosition.y).magnitude;
	}
}