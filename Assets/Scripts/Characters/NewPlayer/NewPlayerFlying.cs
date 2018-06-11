using UnityEngine;

public class NewPlayerFlying : NewPlayerGlitch {
	public NewPlayerFlying(NewPlayerController controller, float timer) : base(controller, timer) {
		_controller.ToggleGravity(false);
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		_controller.ToggleGravity(true);
		DestroySelf();
	}
}