using UnityEngine;


//public enum BossStateType
//{
//    Normal, PatternA, PatternB
//}

//public abstract class BossState
//{
//    protected Boss_0 _boss;
//    protected Boss0_FSM _fsm;
//    public BossStateType StateType { get; }

//    public BossState(Boss_0 boss, Boss0_FSM fsm)
//    {
//        _boss = boss;
//        _fsm = fsm;
//    }

//    public abstract void Enter();
//    public abstract void Exit();
//    public abstract void Tick();
//}

//public class BossNormalState : BossState
//{
//    float _duration;
//    float _timer;

//    public BossNormalState(Boss_0 boss, Boss0_FSM fsm, float duration) : base(boss, fsm)
//    {
//        _duration = duration;
//    }

//    public override void Enter()
//    {
//        // 일반 상태 시작 시 해야 할 함수 호출

//        //_timer = 0;
//    }

//    public override void Exit()
//    {
//        //_timer = 0;
//    }

//    public override void Tick()
//    {
//        // 일반 상태 매 프레임 실행해야 함수

//        // _boss.Follow();


//        //_timer += Time.deltaTime;
//        //if(_timer > _duration )
//        //{
//        //    // 상태 변경
//        //}
//    }
//}



public abstract class Boss0_States 
{
    protected Player player;
    protected Boss_0 boss;
    protected Boss0_FSM fsm;    

    public Boss0_States(Boss_0 boss, Boss0_FSM fsm)
    {        
        this.boss = boss;
        this.fsm = fsm;
        this.player = boss.Player;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Tick(float deltaTime)
    {

    }
}
