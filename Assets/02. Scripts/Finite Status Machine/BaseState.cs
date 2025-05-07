using UnityEngine;

/*
 * 이 클래스는 각 상태를 구현하기 위한 필수적인 내용을 미리 정의하는 추상 클래스이다.
 * FSM 클래스가 MonsterCtrl 행동을 제어하기 위한 용도로만 사용된다는 가정 하에 내부 변수에 GameObject를 저장한다.
 */
public abstract class BaseState
{
    protected MonsterCtrl monsterCtrl;

    protected BaseState(MonsterCtrl input)
    {
        monsterCtrl = input;
    }

    //  TODO : 원래는 HandleInput을 통해 받아야 하는게 맞음.
    // public abstract BaseState HandleInput();

    public abstract void OnStateEnter();
    public abstract void OnStateUpdate();
    public abstract void OnStateExit();
}
