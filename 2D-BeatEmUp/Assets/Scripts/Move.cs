using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{   
    public BoxCollider2D hitBox;
    public float duration;
    public float durationCount = 0;
    public string animationName;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if(durationCount > 0)
        {
            durationCount -= Time.deltaTime;
        }else
        {
            gameObject.SetActive(false);
        }

      
    }

    public void Perform()
    {   
        durationCount = duration;
        gameObject.SetActive(true);
    }

    public void Intercrupt()
    {
        durationCount = 0;
        gameObject.SetActive(false);
    }
}
