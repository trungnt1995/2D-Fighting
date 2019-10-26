using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{   
    public BoxCollider2D hitBox;
    public float duration;
    public float durationCount = 0;
    public string animationName;
    public int power; 

    public bool hit = false;
    // Start is called before the first frame update
    void Start()
    {
        hitBox = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if(durationCount > 0)
        {
            durationCount -= Time.deltaTime;
            if(durationCount < duration * .8)
            {
                hitBox.enabled = true;
            }
        }else
        {
            hitBox.enabled = false;
            hit = false;
        }
    }

    public void Perform()
    {   
        durationCount = duration;
        
    }

    public void Intercrupt()
    {
        durationCount = 0;
        hitBox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player")
        {
            hit = true;
            other.SendMessageUpwards("HitByEnemies", this);
        }
    }

    // private void OnTriggerStay(Collider other) {
    //     if(other.tag == "Player")
    //     {
    //         hit = true;
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other) {
    //     if(other.tag == "Player")
    //     {
    //         hit = false;
    //     }
    // }
}
