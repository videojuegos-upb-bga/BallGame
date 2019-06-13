using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    public bool onGround;

    public float jumpHeight;
    
    // Start is called before the first frame update
    void Start()
    {
        jumpHeight = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        var jumpRay = new Ray(gameObject.transform.position, Vector3.down);
        onGround = Physics.Raycast(jumpRay, out hit, jumpHeight);
        Debug.DrawRay(gameObject.transform.position, Vector3.down, Color.red);
    }
}
