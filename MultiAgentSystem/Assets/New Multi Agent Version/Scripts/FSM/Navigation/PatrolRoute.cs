using UnityEngine;

/// <summary>
/// Handles patrol logic for an agent. Supports two modes:
/// 1. Area-based: random points within a defined area (uses your existing Area component)
/// 2. Waypoint-based: cycles through predefined waypoint transforms
/// 
/// PatrolState calls this to get destinations. This doesn't control movement directly —
/// it just provides WHERE to go. NavigationController handles HOW.
/// </summary>
public class PatrolRoute : MonoBehaviour
{
    public enum PatrolMode
    {
        AreaRandom,
        Waypoints
    }

    [Header("Patrol Mode")]
    public PatrolMode mode = PatrolMode.AreaRandom;

    [Header("Area Mode (random points within area)")]
    [Tooltip("Assign your existing Area component here")]
    public Area patrolArea;

    [Header("Waypoint Mode (cycle through points)")]
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Wait Time Between Patrol Points")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 4f;

    /// <summary>
    /// Returns the next patrol destination based on current mode.
    /// </summary>
    public Vector3 GetNextDestination()
    {
        switch (mode)
        {
            case PatrolMode.AreaRandom:
                return GetRandomAreaPoint();

            case PatrolMode.Waypoints:
                return GetNextWaypoint();

            default:
                return transform.position;
        }
    }

    /// <summary>
    /// Returns a random wait duration between patrol points.
    /// </summary>
    public float GetRandomWaitTime()
    {
        return Random.Range(minWaitTime, maxWaitTime);
    }

    private Vector3 GetRandomAreaPoint()
    {
        if (patrolArea == null)
        {
            Debug.LogWarning($"[PATROL] {gameObject.name}: No patrol area assigned!");
            return transform.position;
        }
        return patrolArea.GetRandomPoint();
    }

    private Vector3 GetNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning($"[PATROL] {gameObject.name}: No waypoints assigned!");
            return transform.position;
        }

        Vector3 point = waypoints[currentWaypointIndex].position;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        return point;
    }
}