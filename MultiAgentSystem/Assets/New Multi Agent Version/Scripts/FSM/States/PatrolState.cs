using UnityEngine;

public class PatrolState : BaseState<AgentState>
{
    private AgentFSM fsm;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private float currentWaitDuration = 0f;
    private float stuckTimer = 0f;
    private float maxTimeToReachPoint = 10f;

    public PatrolState(AgentFSM fsm) : base(AgentState.Patrol)
    {
        this.fsm = fsm;
    }

    public override void EnterState()
    {
        isWaiting = false;
        waitTimer = 0f;
        stuckTimer = 0f;
        SetNextPatrolDestination();
        Debug.Log($"[STATE] {fsm.gameObject.name} → PATROL");
    }

    public override void UpdateState()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= currentWaitDuration)
            {
                isWaiting = false;
                stuckTimer = 0f;
                SetNextPatrolDestination();
            }
            return;
        }

        // Stuck timeout — if taking too long to reach point, pick a new one
        stuckTimer += Time.deltaTime;
        if (stuckTimer > maxTimeToReachPoint)
        {
            stuckTimer = 0f;
            SetNextPatrolDestination();
            return;
        }

        // No path — pick new destination
        if (!fsm.Navigation.HasPath())
        {
            SetNextPatrolDestination();
            return;
        }

        if (fsm.Navigation.HasReachedDestination())
        {
            isWaiting = true;
            waitTimer = 0f;
            currentWaitDuration = fsm.Patrol.GetRandomWaitTime();
            fsm.Navigation.Stop();
        }
    }

    public override void ExitState()
    {
        isWaiting = false;
        fsm.Navigation.Stop();
    }

    public override AgentState GetNextState()
    {
        AgentState evaluated = fsm.AwarenessModel.EvaluateState();
        if (evaluated != AgentState.Patrol)
            return evaluated;
        return AgentState.Patrol;
    }

    private void SetNextPatrolDestination()
    {
        if (fsm.Patrol == null) return;
        Vector3 destination = fsm.Patrol.GetNextDestination();
        fsm.Navigation.PatrolTo(destination);
    }
}