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

	public static bool IsDashValid(Vector2 position, float direction) {
		List<EnemyRadius> enemiesInRadius = GetValidDashEnemyList(position, direction);
		return enemiesInRadius.Count > 0;
	}

	public static GameObject GetDash(Vector2 position, float direction) {
		List<EnemyRadius> enemiesInRadius = GetValidDashEnemyList(position, direction);

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
			return enemiesInRadius[tEnemy].transform.parent.parent.gameObject;
		}

		return null;
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

	private static List<EnemyRadius> GetValidDashEnemyList(Vector2 position, float direction) {
		List<EnemyRadius> enemiesInRadius = new List<EnemyRadius>();

		for(int i = 0; i < Enemies.Count; i++) {
			if(Mathf.Abs(GetDistanceFromEnemy(Enemies[i], position)) < Enemies[i].radius) {
				//Verifies that the player is facing torwards the ennemy.
				if(Enemies[i].transform.position.x - position.x < 0 && direction < 0 || Enemies[i].transform.position.x - position.x > 0 && direction > 0) {
					//Casts a ray from the hero to the enemy.
					RaycastHit2D tHit = Physics2D.Linecast(position, Enemies[i].transform.position, 1 << LayerMask.NameToLayer("Ground"));

					//If the ennemy is in line of sight, add it to the list of potential ennemies inside the player radius.
					if(tHit.collider == null)
						enemiesInRadius.Add(Enemies[i]);
				}
			}
		}

		return enemiesInRadius;
	}
}