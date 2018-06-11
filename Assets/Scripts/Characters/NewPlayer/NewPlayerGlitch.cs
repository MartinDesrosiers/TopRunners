using UnityEngine;
using System;
using System.Threading.Tasks;

public abstract class NewPlayerGlitch {
	protected NewPlayerController _controller;
	protected float _timer;
	protected float _timeStarted;

	public void ResetTimer() {
		_timeStarted = Time.time;
	}

	protected NewPlayerGlitch() { }

	protected NewPlayerGlitch(NewPlayerController controller, float timer) {
		_controller = controller;
		_timer = timer;
		_timeStarted = Time.time;
	}

	protected async Task GlitchTimer() {
		while(true) {
			await Task.Delay(TimeSpan.FromSeconds(_timer - (Time.time - _timeStarted)));
			if(Time.time - _timeStarted >= _timer)
				return;
		}
	}

	protected void DestroySelf() {
		_controller.RemoveGlitch(this);
	}
}