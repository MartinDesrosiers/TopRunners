using UnityEngine;

public class NewPlayerLag : NewPlayerGlitch {
	public NewPlayerLag(NewPlayerController controller, float timer) : base(controller, timer) {
		Time.timeScale = 0.5f;
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
		Time.timeScale = 1.0f;
		DestroySelf();
	}
}