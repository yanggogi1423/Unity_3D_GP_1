using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20f;
    public float force = 1500f;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.AddForce(transform.forward * force);
    }
}
