using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//  FSM 기술을 반영한 ver
public class MonsterCtrl : MonoBehaviour
{
    //  For Using FSM
    private enum State
    {
        Idle,
        Trace,
        Attack,
        Die,
        PlayerDieState
    }

    private State _curState;
    private FSM _fsm;
    // private Coroutine _fsmCoroutine;
    
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

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        
        anim = GetComponent<Animator>();
        
        //  Health
        curHp = maxHp;
        isDead = false;
        
        //  Find Effect From Resources
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }

    private void Start()
    {
        _curState = State.Idle;
        _fsm = new FSM(new IdleState(this));
        
        StartCoroutine(FSMCoroutine());
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

    private IEnumerator FSMCoroutine()
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
                // case State.Hit:
                //     break;
            }
            
            //  0.3초 간격으로 업데이트
            _fsm.UpdateState();
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void ChangeState(State newState)
    {
        _curState = newState;
        switch (_curState)
        {
            case State.Idle:
                _fsm.ChangeState(new IdleState(this));
                break;
            case State.Trace:
                _fsm.ChangeState(new TraceState(this));
                break;
            case State.Attack:
                _fsm.ChangeState(new AttackState(this));
                break;
            case State.Die:
                _fsm.ChangeState(new DieState(this));
                break;
            case State.PlayerDieState:
                _fsm.ChangeState(new PlayerDieState(this));
                break;
        }
    }

    private bool CheckTraceTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        // Debug.Log("Now Distance : " + dist);

        if (dist < traceDist) return true;
        else return false;
    }
    
    private bool CheckAttackTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        
        if (dist < attackDist) return true;
        else return false;
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
        
        //  Collider Disabled
        agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        
        //  Score ++ 50 In GameManager
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

    private IEnumerator ShowBloodEffectCoroutine()
    {
        bloodEffect.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        bloodEffect.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, traceDist);
    }
}

