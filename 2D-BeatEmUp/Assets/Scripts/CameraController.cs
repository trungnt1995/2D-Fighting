using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{   
    [SerializeField]
    public GameObject target;

    public float leftLimit;
    public float rightLimit;
    public float bottomLimit;
    public float topLimit;


    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        if(target != null)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x , leftLimit, rightLimit),
            Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
            transform.position.z
            );
        }  
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(leftLimit,topLimit), new Vector2(rightLimit,topLimit));
        Gizmos.DrawLine(new Vector2(leftLimit,bottomLimit), new Vector2(rightLimit,bottomLimit));
        Gizmos.DrawLine(new Vector2(leftLimit,topLimit), new Vector2(leftLimit,bottomLimit));
        Gizmos.DrawLine(new Vector2(rightLimit,topLimit), new Vector2(rightLimit,bottomLimit));

    }
}
