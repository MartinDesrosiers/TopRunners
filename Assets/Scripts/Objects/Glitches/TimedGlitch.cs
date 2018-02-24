using UnityEngine;

public class TimedGlitch : MonoBehaviour 
{
	public string glitchType;
	public float timerDelay;

    public void GlitchInfo(ref string glitch, ref float timer)
    {
        glitch = glitchType;
        timer = timerDelay;
        /*Debug.Log("Please, don't destroy the pad");
        if(!gameObject.name.Contains("AccelerationPad(Clone)"))
            DestroyObj(gameObject);*/
    }
}