using UnityEngine;

public class NewPlayerStatistics {
	public ushort health;
	public ushort strength;
	public float jumpForce;
	public float walkingSpeed;
	public float runningSpeed;
	public float sprintingSpeed;
	public float stamina;
	public float staminaUseSpeed;

	public NewPlayerStatistics() {
		health = 3;
		strength = 4;
		jumpForce = 6f;
		walkingSpeed = 2f;
		runningSpeed = 10f;
		sprintingSpeed = 15f;
		stamina = 10f;
		staminaUseSpeed = 5f;
	}
}