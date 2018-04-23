using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour {
	protected float timeEnteredState;

	public class State {
		public Action customUpdate = DoNothing;
		public Action enterState = DoNothing;
		public Action exitState = DoNothing;

		public Enum currentState;
	}

	public State state = new State();

	public Enum CurrentState {
		get { return state.currentState; }
		set {
			if(state.currentState == value)
				return;

			ChangingState();
			state.currentState = value;
			ConfigureCurrentState();
		}
	}

	[HideInInspector]
	public Enum lastState;

	private void ChangingState() {
		lastState = state.currentState;
		timeEnteredState = Time.time;
	}

	private void ConfigureCurrentState() {
		state.exitState?.Invoke();

		state.customUpdate = ConfigureDelegate<Action>("CustomUpdate", DoNothing);
		state.enterState = ConfigureDelegate<Action>("EnterState", DoNothing);
		state.exitState = ConfigureDelegate<Action>("ExitState", DoNothing);

		state.enterState?.Invoke();
	}

	Dictionary<Enum, Dictionary<string, Delegate>> _cache = new Dictionary<Enum, Dictionary<string, Delegate>>();

	T ConfigureDelegate<T>(string methodRoot, T Default) where T : class {
		Dictionary<string, Delegate> lookup;
		if(!_cache.TryGetValue(state.currentState, out lookup))
			_cache[state.currentState] = lookup = new Dictionary<string, Delegate>();

		Delegate returnValue;

		if(!lookup.TryGetValue(methodRoot, out returnValue)) {
			var mtd = GetType().GetMethod(
				state.currentState.ToString() + "_" + methodRoot,
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.NonPublic |
				System.Reflection.BindingFlags.InvokeMethod
			);

			if(mtd != null)
				returnValue = Delegate.CreateDelegate(typeof(T), this, mtd);
			else
				returnValue = Default as Delegate;

			lookup[methodRoot] = returnValue;
		}

		return returnValue as T;
	}

	public void CustomUpdate() {
		EarlyCustomUpdate();

		state.customUpdate();

		LateCustomUpdate();
	}

	protected virtual void EarlyCustomUpdate() { }

	protected virtual void LateCustomUpdate() { }

	public static void DoNothing() { }
}