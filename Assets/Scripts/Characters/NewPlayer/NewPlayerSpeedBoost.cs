using UnityEngine;

public class NewPlayerSpeedBoost : NewPlayerGlitch {
	public NewPlayerSpeedBoost(NewPlayerController controller, float timer) : base(controller, timer) {
		//_controller.SpeedBoost = 1.5f;
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		//_controller.SpeedBoost = 1.0f;
		DestroySelf();
	}
}