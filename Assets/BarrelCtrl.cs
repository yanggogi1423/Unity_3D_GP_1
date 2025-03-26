using System;
using System.Collections;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    // [Header("Radius")]
    private float radius = 5f;

    [Header("Explosion Effect")] public GameObject explosionEffect;
    //  Hp
    private int hpCnt;

    private void Awake()
    {
        hpCnt = 3;
    }

    public void OnCollisionEnter(Collision other)
    {
        ContactPoint cp = other.GetContact(0);
        Quaternion rot = Quaternion.LookRotation(-cp.normal);   //  법선
        
        if (other.collider.CompareTag("BULLET"))
        {
            hpCnt--;
            
            if (hpCnt <= 0)
            {
                IndirectDamage(transform.position);
                
                GameObject explosion = Instantiate(explosionEffect, cp.point, rot);
                Destroy(explosion, 5f);
                
                //  추가적인 폭발 방지를 위한 코드
                GetComponent<CapsuleCollider>().enabled = false;
                
                //  3초 후 제거
                Destroy(gameObject, 3f);
                // StartCoroutine(DestroyCoroutine());
            }
        }
    }

    // private IEnumerator DestroyCoroutine()
    // {
    //     yield return new WaitForSeconds(5f);
    //     Destroy(gameObject, 3f);
    // }
    
    //  Explosion Radius Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, radius, 1 << 3);
        
        Rigidbody rb;
        foreach (var coll in colliders)
        {
            rb = coll.GetComponent<Rigidbody>();

            rb.mass = 1.0f; //  Barrel Mass Lighter
            
            rb.constraints = RigidbodyConstraints.None;
            
            rb.AddExplosionForce(1500f, pos, radius, 1200f);
        }
    }
}
