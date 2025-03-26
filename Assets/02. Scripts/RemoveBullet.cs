using System;
using System.Transactions;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    [Header("Spark Effect")]
    public GameObject sparkEffect;
    
    public void OnCollisionEnter(Collision other)
    {
        ContactPoint cp = other.GetContact(0);
        Quaternion rot = Quaternion.LookRotation(-cp.normal);   //  법선
        
        if (other.collider.CompareTag("BULLET"))
        {
            Destroy(other.gameObject);
            
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            Destroy(spark, 0.5f);
        }
    }
}
