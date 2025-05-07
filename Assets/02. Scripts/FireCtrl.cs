using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//  Require AudioSource
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public GameObject bulletPrefab;

    public Transform firePos;

    [Header("Player MuzzleFlash Event")]
    public UnityEvent onPlayerFire = new UnityEvent();
    
    //  Audio
    public AudioClip fireSfx;
    private new AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //  Playing 상태가 아니거나 GameOver일 때 총이 발사되지 않도록 방지
        if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsGameOver) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            audio.PlayOneShot(fireSfx, 1.0f);
            onPlayerFire.Invoke();
        }
    }
    
}
