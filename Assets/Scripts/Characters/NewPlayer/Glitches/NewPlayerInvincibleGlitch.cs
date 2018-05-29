using UnityEngine;

public class NewPlayerInvincibleGlitch : NewPlayerGlitch {
	public NewPlayerInvincibleGlitch(NewPlayerController controller, float timer) : base(controller, timer) {
		_controller.IsInvincible = true;
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		_controller.IsInvincible = false;
		DestroySelf();
	}
}