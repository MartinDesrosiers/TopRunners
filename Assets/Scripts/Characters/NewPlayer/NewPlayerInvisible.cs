using UnityEngine;

public class NewPlayerInvisible : NewPlayerGlitch {
	public NewPlayerInvisible(NewPlayerController controller, float timer) : base(controller, timer) {
		_controller.IsInvisible = true;
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		_controller.IsInvisible = false;
		DestroySelf();
	}
}