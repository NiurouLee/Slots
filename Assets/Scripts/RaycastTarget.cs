using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class RaycastTarget : MaskableGraphic
{

    protected RaycastTarget()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }

    void OnDrawGizmosSelected()
    {
        if (isActiveAndEnabled == true && raycastTarget == true)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            for (int i = 0; i < 4; i++)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
            Gizmos.DrawLine(corners[0], corners[2]);
            Gizmos.DrawLine(corners[1], corners[3]);

        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RaycastTarget))]
public class RaycastTargetEditor : Editor
{
    private SerializedProperty _color;
    private SerializedProperty _raycastTarget;

    void OnEnable()
    {
        _color = serializedObject.FindProperty("m_Color");
        _raycastTarget = serializedObject.FindProperty("m_RaycastTarget");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_color);
        EditorGUILayout.PropertyField(_raycastTarget);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif