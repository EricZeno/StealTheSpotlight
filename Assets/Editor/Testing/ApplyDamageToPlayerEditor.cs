using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ApplyDamageToPlayer))]
public class ApplyDamageToPlayerEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        ApplyDamageToPlayer script = (ApplyDamageToPlayer)target;
        if (GUILayout.Button("Apply Damage")) {
            script.ApplyDamage();
        }
    }
}
