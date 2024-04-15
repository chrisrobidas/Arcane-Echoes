using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Wandering = 0,
        Chasing = 1,
        Attacking = 2
    }

    [HideInInspector] public EnemyState CurrentState { get; private set; }

    [SerializeField] private WaypointPath _waypointPath;
    [SerializeField] private int _initialWaypointIndex = 0;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _stoppingDistanceFromPlayer = 3f;

    protected Animator _animator;

    private int _targetWaypointIndex;
    private NavMeshAgent _agent;
    private float _elapsedTimeSinceWaypointIsReached;

    public void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;
        _targetWaypointIndex = _initialWaypointIndex;
        CurrentState = EnemyState.Wandering;
    }

    public void Update()
    {
        _animator.SetFloat("Velocity", _agent.velocity.magnitude);

        switch (CurrentState)
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

    public void StartChasingOrAttack()
    {
        if (IsAtStoppingDistanceFromPlayer())
        {
            StartAttacking();
        }
        else
        {
            StartChasing();
        }
    }

    public void StartChasing()
    {
        CurrentState = EnemyState.Chasing;
    }

    public void StartAttacking()
    {
        CurrentState = EnemyState.Attacking;
    }

    protected bool IsAtStoppingDistanceFromPlayer()
    {
        float distance = Vector3.Distance(transform.position, GameObject.FindWithTag("Player").transform.position);

        if (distance > _stoppingDistanceFromPlayer)
        {
            return false;
        }

        return true;
    }

    protected void LookAtPlayer()
    {
        var lookPosition = GameObject.FindWithTag("Player").transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookPosition);
        lookRotation.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, lookRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = lookRotation;
    }

    private void Wander()
    {
        if (_waypointPath == null)
            return;

        _agent.stoppingDistance = 0.1f;
        Waypoint targetWaypoint = _waypointPath.GetWaypoint(_targetWaypointIndex).GetComponent<Waypoint>();
        _agent.SetDestination(targetWaypoint.transform.position);

        if (IsAgentDestinationReached())
        {
            _elapsedTimeSinceWaypointIsReached += Time.deltaTime;

            if (_elapsedTimeSinceWaypointIsReached >= targetWaypoint.WaitDuration)
            {
                TargetNextWaypoint();
                _elapsedTimeSinceWaypointIsReached = 0;
            }
        }
    }

    private void Chase()
    {
        _agent.stoppingDistance = _stoppingDistanceFromPlayer;
        _agent.SetDestination(GameObject.FindWithTag("Player").transform.position);

        if (IsAgentDestinationReached())
        {
            StartAttacking();
        }
    }

    private void TargetNextWaypoint()
    {
        _targetWaypointIndex = _waypointPath.GetNextWaypointIndex(_targetWaypointIndex);
    }

    private bool IsAgentDestinationReached()
    {
        if (!_agent.pathPending)
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
