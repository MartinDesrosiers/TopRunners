using UnityEngine;

public class TimedGlitch : MonoBehaviour {
	public string glitchType;
	public float timerDelay;

	public void GlitchInfo(out string glitch, out float timer) {
		glitch = glitchType;
		timer = timerDelay;
	}
}