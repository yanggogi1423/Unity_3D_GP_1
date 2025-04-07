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
        Die
    }

    private State _curState;
    private FSM _fsm;
    private Coroutine _fsmCoroutine;
    
    public Transform target;
    public NavMeshAgent agent;
    public Animator anim;

    [Header("Animation Properties")]
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;

    [Header("Monster Properties")] public int hp = 10;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        _curState = State.Idle;
        _fsm = new FSM(new IdleState(this));

        _fsmCoroutine = StartCoroutine(FSMCoroutine());
    }

    private IEnumerator FSMCoroutine()
    {
        while (true)
        {
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
        }
    }

    private bool CheckTraceTarget()
    {
        float dist = Vector3.Distance(transform.position, target.position);
        Debug.Log("Now Distance : " + dist);

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

    public void Die()
    {
        StopCoroutine(_fsmCoroutine);
        Destroy(gameObject, 1.0f);
        // StartCoroutine(DieAnimationCoroutine());
    }
    
    //  Die Animation With Coroutine
    // private IEnumerator DieAnimationCoroutine()
    // {
    //     int cnt = 100;
    //     
    //     while (cnt >= 0)
    //     {
    //         gameObject.transform.localScale *= 0.95f;
    //         
    //         yield return new WaitForSeconds(0.03f);
    //         cnt--;
    //     }
    //     
    //     Destroy(this.gameObject);
    // }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BULLET")
        {
            //  Hp가 추가로 감소하는 것은 States에서 Collider를 해제함.
            
            hp--;
            
            if(hp <= 0) ChangeState(State.Die);
            else anim.SetTrigger("Hit");
            
            Destroy(collision.gameObject);
        }
    }
    
}

//  기본 클래스 원형 ver
// public class MonsterCtrl : MonoBehaviour
// {
//     private Transform target;
//     private NavMeshAgent agent;
//     private Animator anim;
//
//     [Header("Animation Properties")] 
//     public float traceDist = 10.0f;
//     public float attackDist = 2.0f;
//     
//
//     private void Awake()
//     {
//         target = GameObject.FindGameObjectWithTag("Player").transform;
//         agent = GetComponent<NavMeshAgent>();
//         
//         anim = GetComponent<Animator>();
//     }
//
//     private void Start()
//     {
//         StartCoroutine(SetStatusCoroutine());
//     }
//
//     private void FixedUpdate()
//     {
//         agent.SetDestination(target.position);
//     }
//
//     private IEnumerator SetStatusCoroutine()
//     {
//         while (true)
//         {
//             float distance = Vector3.Distance(transform.position, target.position);
//
//             if (distance < traceDist)
//             {
//                 anim.SetBool("IsTrace", true);
//                 
//                 if (distance < attackDist)  
//                 {
//                     anim.SetBool("IsAttack", true); //  Attack
//                 }
//                 else
//                 {   
//                     anim.SetBool("IsAttack", false);    //  Only Trace
//                 }
//             }
//             else
//             {
//                 agent.isStopped = true;
//                 anim.SetBool("IsTrace", false); //  Idle
//             }
//         }
//     }
//     
// }
