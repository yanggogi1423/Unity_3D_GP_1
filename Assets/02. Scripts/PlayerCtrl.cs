using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Player Properties")] 
    public int maxHp;
    public int curHp;

    [Header("Player UIs")] public UnityEvent onUpdateHpUI = new UnityEvent();
    
    //  Animation
    private Animation anim;
    
    //  Properties
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    
    //  Particles
    [Header("Muzzle Flash")]
    [SerializeField] private MeshRenderer muzzleFlash;
    
    // UI Elements For Player
    public Image playerHpBar;
    
    //  Message -> Monster
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        
        //  Disable MuzzleFlash
        muzzleFlash.enabled = false;
        
        //  Init Properties
        maxHp = curHp = 50;
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

    public void OnAttack()
    {
        curHp -= 1;

        if (curHp <= 0)
        {
            GameManager.Instance.IsGameOver = true;
            PlayerDie();
        }
        
        playerHpBar.fillAmount = curHp / (float)maxHp;
        onUpdateHpUI.Invoke();
    }

    public void ShowPlayerMuzzleFlash()
    {
        StartCoroutine(ShowMuzzleFlashCoroutine());
    }

    private IEnumerator ShowMuzzleFlashCoroutine()
    {
        muzzleFlash.enabled = true;
        
        yield return new WaitForSeconds(0.2f);
        
        muzzleFlash.enabled = false;
    }

    //  Using Delegate with monster
    private void PlayerDie()
    {
        OnPlayerDie();
        
        //  GameOver
        GameManager.Instance.IsGameOver = true;
        
        Destroy(gameObject);
    }
}
