using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

public class InputScript
{
    public float[] axisXY = { 0.0f, 0.0f };
    float pressedTimerA = 0f;
    float pressedTimerD = 0f;
    float delay = 0.2f;
    bool sprinting = false;
    bool aPressed = false;
    bool dPressed = false;
    bool sPressed = false;
    bool wPressed = false;

    public bool GetSprint { get { return sprinting; } set { sprinting = value; } }
    /*public List<int> horiPressed = new List<int>();
    public List<int> vertiPressed = new List<int>();
    public List<float> horiTimePressed = new List<float>();
    public List<float> vertiTimePressed = new List<float>();
    float reference = 0f;
    int i = 0;
    int j = 0;
    int k = 0;
    int l = 0;*/

    /*public void Reset()
    {
        aPressed = false;
        dPressed = false;
        sPressed = false;
        wPressed = false;
        reference = 0f;
        horiPressed.Clear();
        vertiPressed.Clear();
        horiTimePressed.Clear();
        vertiTimePressed.Clear();
        i = 0;
        j = 0;
    }
    void SetReference()
    {
        reference = Time.time;
    }*/
    public float[] PlayerInput(bool player)
    {
#if UNITY_ANDROID || UNITY_IOS
            axisXY[0] = CrossPlatformInputManager.GetAxis("Horizontal");
            if (axisXY[0] < 0)
            {
                axisXY[0] = -1f;
                aPressed = true;
                if (Time.time - pressedTimerA < delay)
                    sprinting = true;
            }
            else if (axisXY[0] > 0)
            {
                axisXY[0] = 1f;
                dPressed = true;
                if (Time.time - pressedTimerD < delay)
                    sprinting = true;
            }
            else
            {
                axisXY[0] = 0f;
                if (aPressed)
                {
                    pressedTimerA = Time.time;
                    aPressed = false;
                    sprinting = false;
                }
                else if (dPressed)
                {
                    pressedTimerD = Time.time;
                    dPressed = false;
                    sprinting = false;
                }
            }
            axisXY[1] = CrossPlatformInputManager.GetAxis("Vertical");
            if (axisXY[1] < 0)
            {
                axisXY[1] = -1f;
            }
            else if (axisXY[1] > 0)
            {
                axisXY[1] = 1f;
            }
            else
            {
                axisXY[1] = 0f;
            }
            return axisXY;
#elif UNITY_EDITOR || UNITY_STANDALONE
		/*if (aPressed || dPressed || wPressed || sPressed && reference == 0)
        {
            RuntimeUI.GetStartTimer = true;
            SetReference();
        }*/
		//Move forward or backward

		axisXY[0] = 0;
		axisXY[1] = 0;

		if(Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow)) {
			if(Time.time - pressedTimerA < delay)
				sprinting = true;
			axisXY[0] -= 1;
		}
		if(Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow)) {
			if(Time.time - pressedTimerD < delay)
				sprinting = true;
			axisXY[0] += 1;
		}
		if(Input.GetKeyUp(KeyCode.A) | Input.GetKeyUp(KeyCode.LeftArrow)) {
			pressedTimerA = Time.time;
			sprinting = false;
		}
		if(Input.GetKeyUp(KeyCode.D) | Input.GetKeyUp(KeyCode.RightArrow)) {
			pressedTimerD = Time.time;
			sprinting = false;
		}
		if(Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow) | Input.GetKey(KeyCode.Space))
			axisXY[1] += 1;
		if(Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow))
			axisXY[1] -= 1;
		if(axisXY[0] == 0)
			sprinting = false;

		//if (Input.GetKey("a") && !Input.GetKey("d"))
        //    {
        //        if (Time.time - pressedTimerA < delay)
        //            sprinting = true;
		//
        //        axisXY[0] = -1f;
        //        /*if (!aPressed)
        //        {
        //            aPressed = true;
        //            horiTimePressed.Add(Time.time - reference);
        //            horiPressed.Add(-1);
        //        }*/
        //    }
		//
        //    if (Input.GetKey("d") && !Input.GetKey("a"))
        //    {
        //        if (Time.time - pressedTimerD < delay)
        //            sprinting = true;
		//
        //        axisXY[0] = 1f;
        //        /*if (!dPressed)
        //        {
        //            dPressed = true;
        //            horiTimePressed.Add(Time.time - reference);
        //            horiPressed.Add(1);
        //        }*/
        //    }
		//
        //    if (Input.GetKeyUp("a"))
        //    {
        //        axisXY[0] = 0f;
        //        pressedTimerA = Time.time;
        //        sprinting = false;
        //        /*if (aPressed)
        //        {
        //            aPressed = false;
        //            horiTimePressed.Add(Time.time - reference);
        //        }*/
        //    }
		//
        //    if (Input.GetKeyUp("d"))
        //    {
        //        axisXY[0] = 0f;
        //        pressedTimerD = Time.time;
        //        sprinting = false;
        //        /*if (dPressed)
        //        {
        //            dPressed = false;
        //            horiTimePressed.Add(Time.time - reference);
        //        }*/
        //    }
		//
        //    //Jump or roll
        //    if (Input.GetKey("w") && !Input.GetKey("s"))
        //    {
        //        axisXY[1] = 1f;
        //        /*if (!wPressed)
        //        {
        //            wPressed = true;
        //            vertiTimePressed.Add(Time.time - reference);
        //            vertiPressed.Add(1);
        //        }*/
        //    }
		//
        //    if (Input.GetKey("s") && !Input.GetKey("w"))
        //    {
        //        axisXY[1] = -1f;
        //        /*if (!sPressed)
        //        {
        //            sPressed = true;
        //            vertiTimePressed.Add(Time.time - reference);
        //            vertiPressed.Add(-1);
        //        }*/
        //    }
		//
        //    if (Input.GetKeyUp("w"))
        //    {
        //        axisXY[1] = 0f;
        //        /*if (wPressed)
        //        {
        //            wPressed = false;
        //            vertiTimePressed.Add(Time.time - reference);
        //        }*/
        //    }
		//
        //    if (Input.GetKeyUp("s"))
        //    {
        //        axisXY[1] = 0f;
        //        /*if (sPressed)
        //        {
        //            sPressed = false;
        //            vertiTimePressed.Add(Time.time - reference);
        //        }*/
        //    }
            /*else
            {
                if (RuntimeUI.GetStartTimer)
                {
                    if (LevelManager.Instance.ghostReplay.hori.Count > k)
                    {
                        if (Time.time - reference > LevelManager.Instance.ghostReplay.horiTime[i] && Time.time - reference < LevelManager.Instance.ghostReplay.horiTime[i + 1])
                        {
                            axisXY[0] = LevelManager.Instance.ghostReplay.hori[k];
                        }
                        else if (Time.time - reference > LevelManager.Instance.ghostReplay.horiTime[i + 1])
                        {
                            axisXY[0] = 0;
                            i += 2;
                            k++;
                        }
                    }
                    if (LevelManager.Instance.ghostReplay.verti.Count > l)
                    {
                        if (Time.time - reference > LevelManager.Instance.ghostReplay.vertiTime[j] && Time.time - reference < LevelManager.Instance.ghostReplay.vertiTime[j + 1])
                        {
                            axisXY[1] = LevelManager.Instance.ghostReplay.verti[l];
                        }
                        else if (Time.time - reference > LevelManager.Instance.ghostReplay.vertiTime[j + 1])
                        {
                            axisXY[1] = 0;
                            j += 2;
                            l++;
                        }
                    }
                }
            }*/
            return axisXY;
#endif
        }
    }

