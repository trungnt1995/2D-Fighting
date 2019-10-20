using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StateManager : MonoBehaviour
{   
    public int health = 100;

    public float horizontal;
    public float vertical;
    public bool attack1;
    public bool attack2;
    public bool attack3;
    public bool crouch;

    public bool canAttack;
    public bool gettingHit;
    public bool currentlyAttacking;

    public bool dontMove;
    public bool onGround;
    public bool lookRight;

    public Slider healthSlider;
    SpriteRenderer sRenderer;

    public HandleDamageCollider handleDC;
    public HandleAnimations handleAnim;
    public HandleMovement handleMovement;

    public GameObject[] movementColliders;
    
    ParticleSystem blood;

    // Start is called before the first frame update
    void Start()
    {
        handleDC = GetComponent<HandleDamageCollider>();
        handleAnim = GetComponent<HandleAnimations>();
        handleMovement = GetComponent<HandleMovement>();
        sRenderer = GetComponentInChildren<SpriteRenderer>();
        blood = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate() {
        sRenderer.flipX = lookRight;
        onGround = isOnGround();

        if(healthSlider != null)
        {
            healthSlider.value = health * 0.01f;
        }

        if(health <= 0)
        {
            if(LevelManager.GetInstance().countdown)
            {
                LevelManager.GetInstance().EndTurnFunction();
                StartCoroutine("PlayDead");
                
            }
        }
    }
    IEnumerator PlayDead()
    {   
        yield return new WaitForSeconds(0.2f);
        handleAnim.anim.SetBool("Dead", true);
        handleAnim.anim.Play("Dead");
    }

    bool isOnGround()
    {
        bool retVal = false;

        LayerMask layer = ~(1 << gameObject.layer | 1 << 3);
        retVal = Physics2D.Raycast(transform.position, -Vector2.up, 0.1f, layer);

        return retVal;
    }

    public void ResetStateInput()
    {
        horizontal = 0;
        vertical = 0;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        crouch = false;
        gettingHit = false;
        currentlyAttacking = false;
        dontMove = false;
    }

    public void CloseMovementCollider(int index)
    {
        movementColliders[index].SetActive(false);
    }

    public void OpenMovementCollider(int index)
    {
        movementColliders[index].SetActive(true);
    }

    public void TakeDamage(int damage, HandleDamageCollider.DamageType damageType)
    {
        if(!gettingHit)
        {   
            switch (damageType)
            {
                case HandleDamageCollider.DamageType.light:
                    StartCoroutine(CloseImmortality(0.3f));
                    break;
                case HandleDamageCollider.DamageType.heavy:
                    handleMovement.AddVelocityOnCharacter(
                        ((!lookRight) ? Vector3.right * 1 : Vector3.right * -1) + Vector3.up
                        , 0.5f
                        );   
                    StartCoroutine(CloseImmortality(1f));
                    break;
            }
            if(blood != null)
            {
                blood.Emit(30);
            }
            health -= damage;
            gettingHit = true;
        }
    }

    IEnumerator CloseImmortality(float timer)
    {
        yield return new WaitForSeconds(timer);
        gettingHit = false;
    }
}
