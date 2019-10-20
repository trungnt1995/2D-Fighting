using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDamageCollider : StateMachineBehaviour
{   
    StateManager states;
    public HandleDamageCollider.DamageType damageType;
    public HandleDamageCollider.DCType dCType;
    public float delay;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        if(states == null)
        {
            states = animator.transform.GetComponentInParent<StateManager>();
        }

        states.handleDC.OpenCollider(dCType, delay, damageType);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        if(states == null)
        {
            states = animator.transform.GetComponentInParent<StateManager>();
        }
        states.handleDC.CloseCollider();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
