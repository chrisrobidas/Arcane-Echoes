using UnityEngine;

public abstract class WaypointPath : MonoBehaviour
{
    public GameObject GetWaypoint(int waypointIndex)
    {
        return transform.GetChild(waypointIndex).gameObject;
    }

    public abstract int GetNextWaypointIndex(int currentWaypointIndex);
}
