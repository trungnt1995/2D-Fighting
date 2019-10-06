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
    public bool isAtacking;
    public Animator myAnim;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {   

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

        if(Input.GetKeyDown(KeyCode.R) && canAttack)
        {   if(comboPerforming == 0 && comboCountinueTime <= 0){
                selectedCombo = punchCombos;
                StartCombo();
            }
            else
            {
                comboPerforming++;
                if(comboPerforming < selectedCombo.Length)
                {
                
                selectedCombo[comboPerforming].Perform();
                selectedCombo[comboPerforming - 1].Intercrupt();
                myAnim.SetBool(selectedCombo[comboPerforming].animationName, true);
                comboCountinueTime = selectedCombo[comboPerforming].duration;
                }
            }
        }
    }

    public void StartCombo()
    {   isAtacking = true;
        selectedCombo[0].Perform();
        myAnim.SetBool(selectedCombo[0].animationName, true);
        comboCountinueTime = selectedCombo[0].duration;
    }

    public void EndCombo()
    {
        canAttack = false;
        isAtacking = false;
        comboPerforming = 0;
        startComboCooldownCount = startComboCooldown;
        for(int i = 0; i < selectedCombo.Length ; i++){
            myAnim.SetBool(selectedCombo[i].animationName, false);
        }
        selectedCombo = null;
    }
}
