using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class AiVisionBehavior : MonoBehaviour
{
    #region Parameters

    [Header("Parameters")]
    [SerializeField] private float _visionRadius;
    [Range(0f, 360f)]
    [SerializeField] private float _visionAngle;
    [SerializeField] private float _visionRange;
    [SerializeField] private GameObject _eyePosition;
    [SerializeField] private bool _inChase = false;
    [SerializeField] private bool _shouldSeePlayer;
    [SerializeField] private float _loopDelay;
    private float _defaultVisionAngle;
    public static bool PlayerInSight { get; set; }
    public LayerMask PlayerMask;
    public LayerMask ObstacleMask;

    [Header("Components")]
    private NavMeshAgent _enemyAgent;
    private EnemyAI _enemyAi;

    //Debug Parameters
    [Header("Debug")]
    public bool WithinVisionSphere;

    #endregion

    #region Methods

    private void Start()
    {
        _enemyAi = GetComponent<EnemyAI>();
        _enemyAgent = GetComponent<NavMeshAgent>();
        _defaultVisionAngle = _visionAngle;
        StartCoroutine("FindPlayer", _loopDelay);
    }

    IEnumerator FindPlayer(float delay)
    {
        while (_shouldSeePlayer)
        {
            yield return new WaitForSeconds(delay);
            
            ObjectsInRadius();
        }
    }

    private void ObjectsInRadius()
    {
        Collider[] objectsInViewRadius = Physics.OverlapSphere(_enemyAgent.transform.position, _visionRadius, PlayerMask);

        WithinVisionSphere = objectsInViewRadius.Length > 0;

        for (int i = 0; i < objectsInViewRadius.Length; i++)
        {
            var detectedObject = objectsInViewRadius[i].transform;
            Vector3 directionToObject = (detectedObject.position - _enemyAgent.transform.position).normalized;

            if (Vector3.Angle(_enemyAgent.transform.forward, directionToObject) < _visionAngle / 2)
            {
                float distanceToObject = Vector3.Distance(_enemyAgent.transform.position, detectedObject.position);

                if (distanceToObject > _visionRange)
                {
                    PlayerInSight = false;
                    _inChase = false;
                    return;
                }

                if (!Physics.Raycast(_eyePosition.transform.position, directionToObject, distanceToObject, ObstacleMask))
                {
                    PlayerInSight = true;
                    DetectionParameterToggle();
                    ChasePlayer();
                }
                else
                {
                    PlayerInSight = false;
                    _inChase = false;
                }
            }
            else
            {
                PlayerInSight = false;
                _inChase = false;
            }
        }
    }

    private void ChasePlayer()
    {
        if (!_inChase && PlayerInSight)
        {
            _enemyAi.SetState("Chase");
            _inChase = true;
        }
    }

    private void ReturnToPatrol()
    {
        if (_inChase && !PlayerInSight)
        {
            _enemyAi.SetState("Patrol");
        }
    }

    private void DetectionParameterToggle()
    {
        if (_inChase)
        {
            _visionRange *= 2f;
            _visionAngle = 360f;
        }

        else
        {
            _visionRange /= 2f;
            _visionAngle = _defaultVisionAngle;
        }
    }

    #endregion

    // Editor Debug
    public Vector3 DirectionFromAngle(float angleDegrees, bool globalAngle)
    {
        if (!globalAngle)
            angleDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
    }
}
