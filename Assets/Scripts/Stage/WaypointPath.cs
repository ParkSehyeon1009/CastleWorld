using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적이 따라가는 경로 웨이포인트 목록.
/// 자식 Transform을 순서대로 waypoints에 등록하세요.
/// </summary>
public class WaypointPath : MonoBehaviour
{
    public static WaypointPath Instance { get; private set; }

    [SerializeField] private List<Transform> waypoints = new();

    void Awake()
    {
        Instance = this;
    }

    public int Count => waypoints.Count;

    public Vector3 GetWaypoint(int index) => waypoints[index].position;

    /// <summary>경로 전체 길이 (디버그/UI용)</summary>
    public float TotalLength()
    {
        float len = 0f;
        for (int i = 0; i < waypoints.Count - 1; i++)
            len += Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
        return len;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
        {
            if (wp != null)
                Gizmos.DrawSphere(wp.position, 0.2f);
        }
    }
#endif
}
