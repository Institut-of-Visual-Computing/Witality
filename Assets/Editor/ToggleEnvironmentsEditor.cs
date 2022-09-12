
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ToggleEnvironments))]
public class ToggleEnvironmentsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ToggleEnvironments t = (ToggleEnvironments)target;

        if (GUILayout.Button("Weinkeller"))
        {
            t.activate(0);
        }
        if (GUILayout.Button("Sensoriklabor"))
        {
            t.activate(1);
        }
        if (GUILayout.Button("Konferenzraum"))
        {
            t.activate(2);
        }
        if (GUILayout.Button("Vinothek"))
        {
            t.activate(3);
        }

    }
}