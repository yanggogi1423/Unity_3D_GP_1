using System;
using UnityEngine;
   
   public class FollowCamera : MonoBehaviour
   {
       [Header("Target")]
       public GameObject target;
       
       [Header("Properties")]
       [Range(2.0f, 20.0f)]
       public float distance = 10.0f;
       
       [Range(2.0f, 20.0f)]
       public float height = 2f;
       
       public float targetOffset = 5f;
       
       //   SmoothDamp
       Vector3 velocity = Vector3.zero;
   
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
   
       private void LateUpdate()
       {
           if (target == null) return;
           
           transform.position = Vector3.SmoothDamp(transform.position, GetMove(), ref velocity, 0.1f);
           // transform.position = Vector3.Lerp(transform.position, GetMove(), 0.1f);
           transform.LookAt(target.transform.position + Vector3.up * targetOffset);
       }
   
       private Vector3 GetMove()
       {    
           //   Player Die State
           if (target == null)
           {
               return transform.position;
           }
           
           return (target.transform.position - (target.transform.forward * distance) + (Vector3.up * height));
       }
   }

// using System;
// using UnityEngine;
//
// public class FollowCamera : MonoBehaviour
// {
//     [Header("Target")]
//     public GameObject target;
//     
//     [Header("Properties")]
//     [Range(2.0f, 20.0f)]
//     [SerializeField] private float distance = 7.0f;
//     
//     [Range(2.0f, 20.0f)]
//     [SerializeField] private float height = 3f;
//     
//     [SerializeField] private float targetOffset = 3f;
//     
//     //  Buff Value
//     Vector3 velocity = Vector3.zero;
//
//     private void Awake()
//     {
//         if (target == null)
//         {
//             target = GameObject.Find("Player");
//         }
//     }
//     private void Start()
//     {
//         transform.LookAt(target.transform.position + target.transform.up * targetOffset);
//         transform.position = GetMove();
//     }
//
//     private void LateUpdate()
//     {
//         transform.position = Vector3.SmoothDamp(transform.position, GetMove(), ref velocity, 0.2f);
//
//         Vector3 lookTarget = target.transform.position + Vector3.up * targetOffset;
//         transform.LookAt(lookTarget);
//     }
//
//     
//     // TODO : 카메라가 Player 시점에 맞추어 회전할 수 있도록 세팅 -> 미완성
//     private void CameraRotation()
//     {
//         Vector3 targetR = target.transform.rotation.eulerAngles;
//         transform.rotation = Quaternion.Euler(targetR);
//     }
//
//     private Vector3 GetMove()
//     {
//         return (target.transform.position + (Vector3.back * distance) + (Vector3.up * height));
//     }
// }