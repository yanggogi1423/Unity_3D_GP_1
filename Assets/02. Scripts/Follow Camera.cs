using System;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public GameObject target;
    
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;

    public float height = 2f;
    public float targetOffset = 3f;

    private void Awake()
    {
        if (target == null)
        {
            target = GameObject.Find("Player");
        }
    }
    private void Start()
    {
        transform.LookAt(target.transform.position + target.transform.up * targetOffset);
        transform.position = GetMove();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, GetMove(), 0.1f);
        transform.LookAt(target.transform.position + target.transform.up * targetOffset);
    }

    private Vector3 GetMove()
    {
        return (target.transform.position + (Vector3.back * distance) + (Vector3.up * height));
    }
}
