using UnityEngine;

/*
 *  BaseState를 상속받은 클래스는 다른 BaseState 자식 클래스들에 대해 알 수 없고,
 *  오직 어떤 행동을 수행해야 하는지에 대한 내용만을 구현한다.
 *  상태 변경에 대한 책임은 이를 사용하는 클래스에 있다.
 */

public class IdleState : BaseState
{
    public IdleState(MonsterCtrl input) : base(input)
    {
        
    }

    public override void OnStateEnter()
    {
        monsterCtrl.agent.isStopped = true;
        monsterCtrl.anim.SetBool("IsTrace", false);
    }

    public override void OnStateUpdate() { }
    
    public override void OnStateExit() { }
}

public class TraceState : BaseState
{
    public TraceState(MonsterCtrl input) : base(input)
    {
        monsterCtrl.agent.isStopped = false;
        monsterCtrl.anim.SetBool("IsTrace", true);
    }

    public override void OnStateEnter()
    {
        monsterCtrl.MoveAndLook();
    }

    public override void OnStateUpdate()
    {
        Debug.Log("Trace Updating");
        monsterCtrl.MoveAndLook();
    }
    
    public override void OnStateExit() { }
}

public class AttackState : BaseState
{
    public AttackState(MonsterCtrl input) : base(input){ }

    public override void OnStateEnter()
    {
        monsterCtrl.anim.SetBool("IsAttack", true);
    }

    public override void OnStateUpdate()
    {
        //  Monster의 Rotation 보정
        monsterCtrl.transform.LookAt(monsterCtrl.target.transform.position);
        
        // TODO : 플레이어 공격 관련 보정 필요
        monsterCtrl.target.GetComponent<PlayerCtrl>().OnAttack();
        Debug.Log("Player is under attack!");
    }

    public override void OnStateExit()
    {
        monsterCtrl.anim.SetBool("IsAttack", false);
    }
}

public class DieState : BaseState
{
    public DieState(MonsterCtrl input) : base(input)
    {
        
    }

    public override void OnStateEnter()
    {
        Debug.Log("Monster died");
        
        monsterCtrl.anim.SetTrigger("Die");
        monsterCtrl.BuffDie();
    }

    public override void OnStateUpdate() { }
    
    public override void OnStateExit() { }
}

//  TODO : p.35까지 했음
public class PlayerDieState : BaseState
{
    public PlayerDieState(MonsterCtrl input) : base(input)
    {
        
    }

    public override void OnStateEnter()
    {
        Debug.Log("Player died");
        
        monsterCtrl.anim.SetTrigger("PlayerDie");
    }

    public override void OnStateUpdate() { }
    
    public override void OnStateExit() { }
}
