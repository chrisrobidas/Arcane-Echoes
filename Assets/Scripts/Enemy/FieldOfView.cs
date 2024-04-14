using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class FieldOfView : MonoBehaviour
{
    public float ViewRadius;

    [Range(0, 360)]
    public float ViewAngle;

    [SerializeField]
    protected bool _drawFieldOfView;

    [HideInInspector]
    public List<GameObject> VisibleTargets = new List<GameObject>();

    [SerializeField]
    private LayerMask _targetMask;

    [SerializeField]
    private LayerMask _obstacleMask;

    [SerializeField]
    private float _meshResolution;

    [SerializeField]
    private int _edgeResolveIterations;

    [SerializeField]
    private float _edgeDistanceThreshold;

    [SerializeField]
    private MeshFilter _viewMeshFilter;

    private Mesh _viewMesh;

    protected virtual void Start()
    {
        _viewMesh = new Mesh
        {
            name = "View Mesh"
        };
        _viewMeshFilter.mesh = _viewMesh;

        StartCoroutine(nameof(FindTargetsWithDelay), 0.2f);
    }

    protected virtual void Update()
    {
        if (_drawFieldOfView)
        {
            DrawFieldOfView();
        }
        else
        {
            _viewMesh.Clear();
        }
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        VisibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, ViewRadius, _targetMask);

        foreach (Collider targetInViewRadius in targetsInViewRadius)
        {
            Vector3 directionToTarget = (targetInViewRadius.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToTarget) < ViewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetInViewRadius.transform.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    VisibleTargets.Add(targetInViewRadius.gameObject);
                }
            }
        }
    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * _meshResolution);
        float stepAngleSize = ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.Distance - newViewCast.Distance) > _edgeDistanceThreshold;
                if (oldViewCast.Hit != newViewCast.Hit || (oldViewCast.Hit && newViewCast.Hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.Point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.Angle;
        float maxAngle = maxViewCast.Angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.Distance - newViewCast.Distance) > _edgeDistanceThreshold;
            if (newViewCast.Hit == minViewCast.Hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.Point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.Point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, ViewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        return new ViewCastInfo(false, transform.position + direction * ViewRadius, ViewRadius, globalAngle);
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private readonly struct ViewCastInfo
    {
        public readonly bool Hit;
        public readonly Vector3 Point;
        public readonly float Distance;
        public readonly float Angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            Hit = hit;
            Point = point;
            Distance = distance;
            Angle = angle;
        }
    }

    private readonly struct EdgeInfo
    {
        public readonly Vector3 PointA;
        public readonly Vector3 PointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
