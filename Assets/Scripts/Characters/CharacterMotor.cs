using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour {
    protected Rigidbody2D rg;
    protected Vector2 propulseAfterTouch;
    [SerializeField]protected Animator animator;
    protected bool _isWalled;
    protected bool _isGrounded;
    protected float horiAxes;
    protected float vertAxis;

    public bool GetIsGrounded { get { return _isGrounded; } }
    public Rigidbody2D getRigid { get { return rg; } }

    private void Start() {
        rg = transform.GetComponent<Rigidbody2D>();
	}

    protected void Movement(float x, float y){
        rg.velocity = new Vector2(x, y);
    }
    //Reset every animation to false and set the current one to true
    protected void SetAnimation(string name, bool state)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                animator.SetBool(parameter.name, false);
        }
        animator.SetBool(name, state);
    }
    //It flips stuff but only on the x axis
    public void Flip()
    {
        Vector3 tScale = transform.localScale;
        tScale.x = -tScale.x;
        transform.localScale = tScale;        
    }

    /*private void Update()
    {
        if (!LevelManager.Instance.isPaused)
        {
        }
    }*/

    /*private IEnumerator OnTriggerEnter2D(Collider2D col) {
		if(!tEnter) {
			if(col.name.Contains("Water")) {
				tEnter = true;
				yield return new WaitForEndOfFrame();
				tEnter = false;

				_isSwimming = true;
			}
		}
	}

	private IEnumerator OnTriggerStay2D(Collider2D col) {
		if(!tStay) {
			if(col.name.Contains("Water")) {
				tStay = true;
				yield return new WaitForEndOfFrame();
				tStay = false;

				_isSwimming = true;
			}
		}
	}*/
}

