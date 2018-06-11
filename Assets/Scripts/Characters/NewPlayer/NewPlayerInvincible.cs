using UnityEngine;

public class NewPlayerInvincible : NewPlayerGlitch {
	public NewPlayerInvincible(NewPlayerController controller, float timer) : base(controller, timer) {
		_controller.IsInvincible = true;
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		_controller.IsInvincible = false;
		DestroySelf();
	}
}