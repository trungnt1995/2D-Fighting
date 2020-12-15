using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleDamageCollider : MonoBehaviour
{   
    public GameObject[] damageCollidersLeft;
    public GameObject[] damageCollidersRight;

    public enum DamageType
    {
        light
        , heavy
    }

    public enum DCType
    {
        bottom
        , up
    }

    StateManager states;

    // Start is called before the first frame update
    void Start()
    {
        states = GetComponent<StateManager>();
        CloseCollider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenCollider(DCType type, float delay, DamageType damageType)
    {
        if(!states.lookRight)
        {
            switch (type)
            {
                case DCType.bottom:
                    StartCoroutine(OpenCollider(damageCollidersLeft, 0, delay, damageType));
                    break;
                case DCType.up:
                    StartCoroutine(OpenCollider(damageCollidersLeft, 1, delay, damageType));
                    break;
            }
        }else
        {
            switch (type)
            {
                case DCType.bottom:
                    StartCoroutine(OpenCollider(damageCollidersRight, 0, delay, damageType));
                    break;
                case DCType.up:
                    StartCoroutine(OpenCollider(damageCollidersRight, 1, delay, damageType));
                    break;
            }
        }
    }

    IEnumerator OpenCollider(GameObject[] array, int index, float delay, DamageType damageType)
    {
        yield return new WaitForSeconds(delay);
        array[index].SetActive(true);
        array[index].GetComponent<DoDamage>().damageType = damageType;
    }

    public void CloseCollider()
    {
        for(int i = 0; i < damageCollidersLeft.Length; i ++)
        {
            damageCollidersLeft[i].SetActive(false);
            damageCollidersRight[i].SetActive(false);

        }
    }
}
