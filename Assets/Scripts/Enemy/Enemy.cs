using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private WaypointPath _waypointPath;
    [SerializeField] private int _initialWaypointIndex = 0;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _stoppingDistanceFromPlayer = 3f;

    private enum EnemyState
    {
        Wandering = 0,
        Chasing = 1,
        Attacking = 2
    }

    private int _targetWaypointIndex;
    private NavMeshAgent _agent;
    private EnemyState _currentState;
    private float _elapsedTimeSinceWaypointIsReached;

    public void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        Debug.Log(_agent.stoppingDistance);
        _agent.speed = _speed;
        _targetWaypointIndex = _initialWaypointIndex;
        _currentState = EnemyState.Wandering;
    }

    public void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Wandering:
                Wander();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }
    }

    public abstract void Attack();

    private void Wander()
    {
        _agent.stoppingDistance = 0f;
        Waypoint targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex).GetComponent<Waypoint>();
        _agent.SetDestination(targetWaypoint.transform.position);

        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    _elapsedTimeSinceWaypointIsReached += Time.deltaTime;

                    if (_elapsedTimeSinceWaypointIsReached >= targetWaypoint.WaitDuration)
                    {
                        TargetNextWaypoint();
                        _elapsedTimeSinceWaypointIsReached = 0;
                    }
                }
            }
        }
    }

    private void Chase()
    {
        _agent.stoppingDistance = _stoppingDistanceFromPlayer;
        _agent.SetDestination(GameObject.FindWithTag("Player").transform.position);
    }

    private void TargetNextWaypoint()
    {
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
    }
}
