using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{   
    public int comboPerforming = 0;
    private Move[] selectedCombo;
    public float comboCountinueTime = 0;
    public Move[] punchCombos;

    public float startComboCooldown;
    public float startComboCooldownCount;
    public bool canAttack;
    public Animator myAnim;
    private UnitState playerState;
    private bool isDead = false;
    public Rigidbody2D rigidbody2D;

    public bool testing;

    private List<UNITSTATE> AttackStates = new List<UNITSTATE> {
		UNITSTATE.IDLE, 
		UNITSTATE.WALK, 
		UNITSTATE.RUN, 
		UNITSTATE.JUMPING,
		UNITSTATE.ATTACK,        
		UNITSTATE.DEFEND,
	};

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        playerState = GetComponent<UnitState>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(!AttackStates.Contains(playerState.currentState) || isDead) return;
        if(startComboCooldownCount > 0)
        {
            startComboCooldownCount -= Time.deltaTime;
        }else
        {
            canAttack = true;
        }

        if(comboCountinueTime > 0)
        {
            comboCountinueTime-= Time.deltaTime;
        }    
        else if(selectedCombo != null)
        {
            EndCombo();                
        }

        if(Input.GetKeyDown(KeyCode.R) && canAttack && !testing)
        {   if(comboPerforming == 0 && comboCountinueTime <= 0){
                selectedCombo = punchCombos;
                StartCombo();
            }
            else
            {
                if(selectedCombo[comboPerforming].hit && comboPerforming < selectedCombo.Length - 1)
                {
                    comboPerforming++;
                    selectedCombo[comboPerforming].Perform();
                    selectedCombo[comboPerforming - 1].Intercrupt();
                    myAnim.SetBool(selectedCombo[comboPerforming].animationName, true);
                    comboCountinueTime = selectedCombo[comboPerforming].duration;
                    
                }
                    
            }
        }
    }

    public void StartCombo()
    {   
        playerState.SetState(UNITSTATE.ATTACK);
        selectedCombo[0].Perform();
        myAnim.SetBool(selectedCombo[0].animationName, true);
        comboCountinueTime = selectedCombo[0].duration;
    }

    public void EndCombo()
    {   
        playerState.SetState(UNITSTATE.IDLE);
        canAttack = false;
        comboPerforming = 0;
        startComboCooldownCount = startComboCooldown;
        for(int i = 0; i < selectedCombo.Length ; i++){
            myAnim.SetBool(selectedCombo[i].animationName, false);
        }
        selectedCombo = null;
    }

    public void HitByEnemies(Move MoveHitted)
    {
        playerState.SetState(UNITSTATE.HIT);
        gameObject.GetComponent<CharacterStats>().Hit(MoveHitted.power);
        myAnim.SetTrigger("Hit-1");
    }

}
