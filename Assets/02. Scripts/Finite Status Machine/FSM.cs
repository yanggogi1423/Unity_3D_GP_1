using UnityEngine;

/*
 * BaseState를 기반으로 현재 상태 혹은 상태 변경 시 호출되어야 할 메소드를 관리하는 클ㄹ래스
 */

public class FSM
{
    private BaseState _curState;
    
    public FSM(BaseState initState)
    {
        _curState = initState;
        
    }

    public void ChangeState(BaseState nextState)
    {
        if (nextState == _curState) return;
        
        if(_curState != null) _curState.OnStateExit();
        
        _curState = nextState;
        _curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if(_curState!=null) _curState.OnStateUpdate();
    }
}
