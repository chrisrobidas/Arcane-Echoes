using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView), true)]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fieldOfView = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360, fieldOfView.ViewRadius);
        Vector3 viewAngleA = fieldOfView.DirectionFromAngle(-fieldOfView.ViewAngle / 2, false);
        Vector3 viewAngleB = fieldOfView.DirectionFromAngle(fieldOfView.ViewAngle / 2, false);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleA * fieldOfView.ViewRadius);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + viewAngleB * fieldOfView.ViewRadius);

        Handles.color = Color.black;
        foreach (GameObject visibleTarget in fieldOfView.VisibleTargets)
        {
            Handles.DrawLine(fieldOfView.transform.position, visibleTarget.transform.position);
        }
    }
}
