using UnityEngine;

public class NewPlayerFlyingGlitch : NewPlayerGlitch {
	public NewPlayerFlyingGlitch(NewPlayerController controller, float timer) : base(controller, timer) {
		EndGlitch();
	}

	private async void EndGlitch() {
		await GlitchTimer();
	}
}