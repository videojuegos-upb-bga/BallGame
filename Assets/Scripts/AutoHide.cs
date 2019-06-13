using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHide : MonoBehaviour
{

    [Header("Time out:")] public float tiempo;
    public GameObject textHider;
    
    private void OnEnable()
    {
        StartCoroutine(textKiller());
    }

    private IEnumerator textKiller()
    {
        yield return new  WaitForSeconds(tiempo);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}
