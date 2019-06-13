using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Character : MonoBehaviour
{
    [Header("Velocidad de movimiento:")] [SerializeField] private float speed;

    [Header("Fuerza de salto")] [Tooltip("La fuerza debe ser una magnitud grande")]public float jumpForce;
    
    private Rigidbody2D _rigidbody;
    
    [HideInInspector]
    public float _distGround;

    private bool isOnGround;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        isOnGround = false;
    }

    private void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Suelo"))
        {
            isOnGround = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hider"))
        {
            var obj = GameObject.FindWithTag("Texto");
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        else if (other.CompareTag("KillZone"))
        {
            Scene escena = SceneManager.GetActiveScene();
            SceneManager.LoadScene(escena.name);
        }
        else if (other.CompareTag("WinZone"))
        {
            SceneManager.LoadScene("Titulo");
        }   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float movHorizontal = Input.GetAxis("Horizontal");
        Vector2 movimiento = new Vector2(movHorizontal,0f);
        _rigidbody.AddForce(movimiento * speed);
        
        //saltar con un boton
        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            _rigidbody.AddForce(Vector2.up * jumpForce * 100f, ForceMode2D.Force);
        }
    }
}
