using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacter : MonoBehaviour
{   
    StateManager states;
    public StateManager enStates;

    public float changeStateTolerance = 3;

    public float normalRate = 1;
    float nrmTimer;

    public float closeRate = 0.5f;
    float clTimer;

    public float blockingRate = 1.5f;
    float blTimer;

    public float aiStateLife = 1;
    float aiTimer;

    bool initiateAI;
    bool closeCombat;

    bool gotRandom;
    float storeRandom;

    bool checkForBlocking;
    bool blocking;
    float blockMultipler;

    bool randomizeAttacks;
    int numberOfAttacks;
    int curNumAttacks;

    public float JumpRate = 1;
    float jRate;
    bool jump;
    float jtimer;

    public AttackPatterns[] attacksPatterns;

    public enum AIState
    {
        closeState,
        normalState,
        resetAI
    }

    public AIState aiStates;

    // Start is called before the first frame update
    void Start()
    {
        states = GetComponent<StateManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
        States();
        AIAgent();
    }

    void States()
    {
        switch (aiStates)
        {
            case AIState.closeState:
               CloseState();
               break; 
            case AIState.normalState:
               NormalState();
               break; 
            case AIState.resetAI:
               ResetAI();
               break;       
        }

        //Blocking()
        Jumping();
    }
    
    void CloseState()
    {
        clTimer += Time.deltaTime;
        if(clTimer > closeRate)
        {
            clTimer = 0;
            initiateAI = true;
        }
    }

    void NormalState()
    {
        nrmTimer += Time.deltaTime;

        if(nrmTimer > normalRate)
        {
            initiateAI = true;
            nrmTimer = 0;
        }
    }

    void AIAgent()
    {
        if(initiateAI)
        {
            aiStates = AIState.resetAI;
            float multiplier = 0;

            if(!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            if(!closeCombat)
            {
                multiplier += 30;
            }else
            {
                multiplier -= 30;
            }

            if(storeRandom + multiplier < 50)
            {
                Attack();
            }else
            {
                Movement();
            }
        }
    }

    void ResetAI()
    {
        aiTimer += Time.deltaTime;

        if(aiTimer > aiStateLife)
        {
            initiateAI = false;
            states.horizontal = 0;
            states.vertical = 0;
            aiTimer = 0;

            gotRandom = false;

            storeRandom = ReturnRandom();
            if(storeRandom < 50)
            {
                aiStates = AIState.normalState;
            }else
            {
                aiStates = AIState.closeState;
            }
            
            curNumAttacks = 1;
            randomizeAttacks = false;
        }
    }

    void CheckDistance() 
    {
        float distance = Vector3.Distance(transform.position, enStates.transform.position);

        if(distance < changeStateTolerance)
        {
            if(aiStates != AIState.resetAI)
            {
                aiStates = AIState.closeState;
            }
            
            closeCombat = true;
        }else
        {
            if(aiStates != AIState.resetAI)
            {
                aiStates = AIState.normalState;
            }

            if(closeCombat)
            {
                if(!gotRandom)
                {
                    storeRandom = ReturnRandom();
                    gotRandom = true;
                }

                if(storeRandom < 60)
                {
                    Movement();
                }
            }
        }
    }

    void Blocking()
    {
        if(states.gettingHit)
        {
            if(!gotRandom)
            {
                storeRandom = ReturnRandom();
                gotRandom = true;
            }

            if(storeRandom < 50)
            {
                blocking = true;
                states.gettingHit = false;
                //states.blocking = true;
            }
        }

        if(blocking)
        {
            blTimer += Time.deltaTime;

            if(blTimer > blockingRate)
            {
                //states.blocking = false;
                blTimer = 0;
            }
        }
    }

    void Jumping()
    {   

        if(!enStates.onGround)
        {
            float ranValue = ReturnRandom();
            if(ranValue < 50)
            {
                jump = true;
            }
        }

        if(jump)
        {
            states.vertical = 1;
            jump = false;
        }else
        {
            states.vertical = 0;
        }

        jtimer += Time.deltaTime;
        if(jtimer > JumpRate*10)
        {
            jRate = ReturnRandom();
            
            if(jRate < 50)
            {
                jump = true;
            }else
            {
                jump = false;
            }

            jtimer = 0;
        }
    }

    void Attack()
    {
        if(!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        // if(storeRandom < 75)
        // {

            if(!randomizeAttacks)
            {
                numberOfAttacks = (int)Random.Range(1, 4);
                randomizeAttacks = true;
            }

            if(curNumAttacks < numberOfAttacks)
            {
                int attackNumber = Random.Range(0, attacksPatterns.Length);

                StartCoroutine(OpenAttack(attacksPatterns[attackNumber], 0));

                curNumAttacks++;
            }
        // }else
        // {
        //     if(curNumAttacks < 1)
        //     {
        //         //states.SpecialAttack = true;
        //         curNumAttacks++
        //     }
        // }
    }

    void Movement()
    {
        if(!gotRandom)
        {
            storeRandom = ReturnRandom();
            gotRandom = true;
        }

        if(storeRandom < 90)
        {
            if(enStates.transform.position.x < transform.position.x)
            {
                states.horizontal = -1;
            }else
            {
                states.horizontal = 1;
            }
        }else
        {
            if(enStates.transform.position.x < transform.position.x)
            {
                states.horizontal = 1;
            }else
            {
                states.horizontal = -1;
            }
        }
    }

    float ReturnRandom()
    {
        float retVal = Random.Range(0, 101);
        return retVal;
    }

    IEnumerator OpenAttack(AttackPatterns a, int i)
    {
        int index = i;
        float delay = a.attacks[index].delay;
        states.attack1 = a.attacks[index].attack1;
        states.attack2 = a.attacks[index].attack2;
        yield return new WaitForSeconds(delay);

        states.attack1 = false;
        states.attack2 = false;

        if(index < a.attacks.Length - 1)
        {
            index++;
            StartCoroutine(OpenAttack(a, index));
        }
    }

    [System.Serializable]

    public class AttackPatterns
    {
        public AttackBase[] attacks;
    }
    
    [System.Serializable]

    public class AttackBase
    {
        public bool attack1;
        public bool attack2;
        public float delay;
    }
}