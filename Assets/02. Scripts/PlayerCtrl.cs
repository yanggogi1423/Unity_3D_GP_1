using System;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //  Animation
    private Animation anim;
    
    //  Properties
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    private void Start()
    {
        if (anim != null)
        {
            anim.Play("Idle");   
        }
    }
    
    private void Update()
    {
        //  Move
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(h, 0, v)* Time.deltaTime * moveSpeed);
        
        //  Animation
        if (h != 0 || v != 0)
        {
            if(v > 0) anim.CrossFade("RunF", 0.25f);
            else if(v < 0) anim.CrossFade("RunB", 0.25f);
            else if(h > 0) anim.CrossFade("RunR", 0.25f);
            else if (h < 0) anim.CrossFade("RunL", 0.25f);
        }
        else 
        {
            anim.CrossFade("Idle", 0.25f);
        }
        
        
        //  Rotate
        float r = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * r * rotateSpeed * Time.deltaTime);
    }
}
