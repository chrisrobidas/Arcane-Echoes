public class ReversingWaypointPath : WaypointPath
{
    private bool _isReversed;

    public override int GetNextWaypointIndex(int currentWaypointIndex)
    {
        int nextWaypointIndex = currentWaypointIndex + (_isReversed ? -1 : 1);

        if (nextWaypointIndex == transform.childCount - 1 || nextWaypointIndex == 0)
        {
            _isReversed = !_isReversed;
        }

        return nextWaypointIndex;
    }
}
