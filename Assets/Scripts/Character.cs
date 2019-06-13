using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Character : MonoBehaviour
{
    [Header("Velocidad de movimiento:")] [SerializeField] private float speed;

    [Header("Fuerza de salto")] [Tooltip("La fuerza debe ser una magnitud grande")]public float jumpForce;

    //[Header("Audio Assets")] public AudioClip[] clips;

    public AudioClip clipSaltar;
    public AudioClip clipPerder;
    public AudioClip clipGanar;
    
    private Rigidbody2D _rigidbody;
    private AudioSource _audio;
    
    [HideInInspector]
    public float _distGround;

    private bool isOnGround;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
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
            StartCoroutine(Perder());
        }
        else if (other.CompareTag("WinZone"))
        {
            StartCoroutine(Ganar());
        }   
    }

    private IEnumerator Ganar()
    {
        _audio.clip = clipGanar;
        _audio.Play();
        yield return new WaitWhile(() => _audio.isPlaying);
        SceneManager.LoadScene("Titulo");
    }

    private IEnumerator Perder()
    {
        _audio.clip = clipPerder;
        _audio.Play();
        yield return new WaitWhile(() => _audio.isPlaying);
        Scene escena = SceneManager.GetActiveScene();
        Debug.Log(escena.name);
        SceneManager.LoadScene(escena.name);
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
            _audio.Play();
        }
        /*else
        {
            if (Input.GetButtonUp("Jump"))
            {
                _audio.Stop();
            }
        }*/
    }
}
