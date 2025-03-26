using System;
using System.Collections;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform firePos;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        }
    }
    
}
