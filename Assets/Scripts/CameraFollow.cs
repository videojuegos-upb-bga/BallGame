using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objeto a seguir con la camara")]
    public GameObject followObject;

    public float offsetX;

    private Vector3 camPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        camPosition = new Vector3(followObject.transform.position.x + offsetX,
            0f,
            -10f);
        transform.position = camPosition;
    }
}
