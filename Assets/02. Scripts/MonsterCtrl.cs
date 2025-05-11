using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
 * FSM으로 구성했었지만, 과제 제출을 위해 파일을 하나로 통합, Animator만 사용함
 */

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        Idle,
        Trace,
        Attack,
        Die,
        PlayerDieState
    }
    
    public Transform target;
    public NavMeshAgent agent;
    public Animator anim;

    [Header("Animation Properties")]
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    [Header("Monster Properties")]
    public int maxHp = 100;
    public int curHp;
    private bool isDead;
    
    //  Effects
    private GameObject bloodEffect;
    
    //  State
    public State _curState = State.Idle;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        
        anim = GetComponent<Animator>();
        
        //  Health
        curHp = maxHp;
        isDead = false;
        ChangeState(State.Idle);
        
        //  Find Effect From Resources
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }

    private void Start()
    {
        StartCoroutine(MonsterBehaviorCoroutine());
    }

    private void OnEnable()
    {
        Awake();
        Start();
        
        //  Collider Enable
        agent.isStopped = false;
        GetComponent<CapsuleCollider>().enabled = true;

        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }

    private void OnDisable()
    {
        GameManager.Instance.MonsterDisable();
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    private IEnumerator MonsterBehaviorCoroutine()
    {
        while (true)
        {
            //  Coroutine이 종료될 때 까지 Continue;
            if (isDead)
            {
                Debug.Log("Coroutine Continue");
                continue;
            }
            
            switch (_curState)
            {
                case State.Idle:
                    if (CheckTraceTarget())
                    {
                        if (CheckAttackTarget())
                        {
                            ChangeState(State.Attack);
                        }
                        else
                        {
                            ChangeState(State.Trace);
                        }
                    }

                    break;
                case State.Trace:
                    if (CheckTraceTarget())
                    {
                        if (CheckAttackTarget())
                        {
                            ChangeState(State.Attack);
                        }
                    }
                    else
                    {
                        ChangeState(State.Idle);
                    }

                    break;
                case State.Attack:
                    if (CheckTraceTarget())
                    {
                        if (!CheckAttackTarget())
                        {
                            ChangeState(State.Trace);
                        }
                    }
                    else
                    {
                        ChangeState(State.Trace);
                    }

                    break;
                case State.Die:
                    ChangeState(State.Die);
                    break;
                
                //  Hit은 OnCollision으로 관리함
            }
            
            //  0.3초 간격으로 업데이트
            UpdateState();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void ChangeState(State newState)
    {
        ExitState();
        Debug.Log("New State: " + newState);
        _curState = newState;
        EnterState();
    }

    #region State Handler
    private void EnterState()
    {
        switch (_curState)
        {
            case State.Idle:
                agent.isStopped = true;
                anim.SetBool("IsTrace", false);
                break;
            case State.Trace:
                agent.isStopped = false;
                anim.SetBool("IsTrace", true);
                MoveAndLook();
                break;
            case State.Attack:
                anim.SetBool("IsAttack", true);
                break;
            case State.Die:
                Debug.Log("Monster died" + name);
                
                anim.SetTrigger("Die");
                BuffDie();
                break;
            case State.PlayerDieState:
                Debug.Log("Player died");
        
                anim.SetTrigger("PlayerDie");
                break;
            default:
                Debug.LogError($"Invalid state: {_curState}");
                break;
        }
    }

    private void UpdateState()
    {
        switch (_curState)
        {
            case State.Idle:

                break;
            case State.Trace:
                Debug.Log("Trace Updating");
                MoveAndLook();
                break;
            case State.Attack:
                //  Monster의 Rotation 보정
                transform.LookAt(target.transform.position);
        
                // TODO : 플레이어 공격 관련 보정 필요
                target.GetComponent<PlayerCtrl>().OnAttack();
                Debug.Log("Player is under attack!");
                break;
            case State.Die:
                
                break;
            case State.PlayerDieState:
                
                break;
            default:
                Debug.LogError($"Invalid state: {_curState}");
                break;
        }
    }

    private void ExitState()
    {
        if (_curState == State.Attack)
        {
            anim.SetBool("IsAttack", false);
        }
    }
    
    #endregion

    private bool CheckTraceTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < traceDist) return true;
        
        //  else
        return false;
    }
    
    private bool CheckAttackTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        
        if (dist < attackDist) return true;
        
        //  else
        return false;
    }

    //  Handled By States
    public void MoveAndLook()
    {
        agent.SetDestination(target.position);
    }

    public void BuffDie()
    {
        isDead = true;
        StopAllCoroutines();
        
        //  Agent, Collider Disabled
        agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        
        //  Handler variable disable
        anim.SetBool("IsTrace", false);
        anim.SetBool("IsAttack", false);
        
        
        //  Score + 50 In GameManager
        GameManager.Instance.DisplayScore(50);
        
        Invoke("Die", 3f);
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public void OnPlayerDie()
    {
        ChangeState(State.PlayerDieState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BULLET")
        {
            //  Hp가 추가로 감소하는 것은 States에서 Collider를 해제함.

            //  GetDamageNum : 10
            curHp -= 10;
            
            if(curHp <= 0) ChangeState(State.Die);
            
            else
            {
                anim.SetTrigger("Hit");

                StartCoroutine(ShowBloodEffectCoroutine());
            }
            
            Destroy(collision.gameObject);
        }
    }
    
    //  Show Blood Effects
    private IEnumerator ShowBloodEffectCoroutine()
    {
        Debug.Log("Show Blood Effect");
        bloodEffect.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        bloodEffect.SetActive(false);
    }

    //  Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, traceDist);
    }
}

