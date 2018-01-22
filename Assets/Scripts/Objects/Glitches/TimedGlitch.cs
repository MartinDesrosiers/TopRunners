using UnityEngine;

public class TimedGlitch : TriggerDestroy
{
	public string glitchType;
	public float timerDelay;

    public void GlitchInfo(ref string glitch, ref float timer)
    {
        glitch = glitchType;
        timer = timerDelay;
        if(!gameObject.name.Contains("AccelerationPad"))
            DestroyObj(gameObject);
    }
}