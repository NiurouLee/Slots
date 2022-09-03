#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

public class RaycastTargetChecker : EditorWindow
{
    private MaskableGraphic[] _graphics;
    private bool _showBorders = true;
    private Color _borderColor = Color.magenta;
    private Color _selectedBorderColor = Color.blue;
    private Vector2 _scrollPosition = Vector2.zero;
    private static RaycastTargetChecker _instance = null;

    [MenuItem("Tools/RaycastTarget Checker")]
    private static void Open()
    {
        _instance = _instance ?? EditorWindow.GetWindow<RaycastTargetChecker>("RaycastTargets");
        _instance.Show();
    }

    void OnEnable() { _instance = this; }

    void OnDisable() { _instance = null; }

    void OnGUI()
    {
        _showBorders = EditorGUILayout.Toggle("Show Border", _showBorders, GUILayout.Width(200.0f));
        _borderColor = EditorGUILayout.ColorField("Border Color", _borderColor);
        _selectedBorderColor = EditorGUILayout.ColorField("Selected Border Color", _selectedBorderColor);

        GUILayout.Space(12.0f);
        Rect rect = GUILayoutUtility.GetLastRect();
        GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.25f);
        GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 4.0f), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(0.0f, rect.yMin + 6.0f, Screen.width, 1.0f), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(0.0f, rect.yMin + 9.0f, Screen.width, 1.0f), EditorGUIUtility.whiteTexture);
        GUI.color = Color.white;

        _graphics = GameObject.FindObjectsOfType<MaskableGraphic>();

        using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scrollViewScope.scrollPosition;
            using (GUILayout.HorizontalScope horizontalScope = new GUILayout.HorizontalScope())
            {
                using (GUILayout.VerticalScope verticalScope = new GUILayout.VerticalScope(GUILayout.Width(Screen.width * 0.39f)))
                {
                    EditorGUILayout.LabelField("Actived");
                    for (int i = 0; i < _graphics.Length; i++)
                    {
                        MaskableGraphic graphic = _graphics[i];
                        if (graphic.raycastTarget == true)
                        {
                            using (EditorGUILayout.HorizontalScope elementScope = new EditorGUILayout.HorizontalScope())
                            {
                                Undo.RecordObject(graphic, "Modify RaycastTarget");
                                EditorGUI.BeginDisabledGroup(true);
                                graphic.raycastTarget = EditorGUILayout.Toggle(graphic.raycastTarget, GUILayout.Width(20));
                                EditorGUILayout.ObjectField(graphic, typeof(MaskableGraphic), true);
                                EditorGUI.EndDisabledGroup();
                                if (GUILayout.Button(">>", GUILayout.Width(50)) == true)
                                {
                                    graphic.raycastTarget = false;
                                }
                            }
                        }
                    }
                }
                using (GUILayout.VerticalScope verticalScope = new GUILayout.VerticalScope(GUILayout.Width(Screen.width * 0.39f)))
                {
                    EditorGUILayout.LabelField("Inactived");
                    for (int i = 0; i < _graphics.Length; i++)
                    {
                        MaskableGraphic graphic = _graphics[i];
                        if (graphic.raycastTarget == false)
                        {
                            using (EditorGUILayout.HorizontalScope elementScope = new EditorGUILayout.HorizontalScope())
                            {
                                Undo.RecordObject(graphic, "Modify RaycastTarget");
                                if (GUILayout.Button("<<", GUILayout.Width(50)) == true)
                                {
                                    graphic.raycastTarget = true;
                                }
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.ObjectField(graphic, typeof(MaskableGraphic), true);
                                graphic.raycastTarget = EditorGUILayout.Toggle(graphic.raycastTarget, GUILayout.Width(20));
                                EditorGUI.EndDisabledGroup();
                            }
                        }
                    }
                }
            }
        }
        foreach (var item in _graphics)
        {
            EditorUtility.SetDirty(item);
        }
        Repaint();
    }

    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawGizmos(MaskableGraphic source, GizmoType gizmoType)
    {
        if (_instance != null && _instance._showBorders == true && source.raycastTarget == true)
        {
            Vector3[] corners = new Vector3[4];
            source.rectTransform.GetWorldCorners(corners);
            Color lastColor = Gizmos.color;
            if (Selection.activeGameObject == source.gameObject)
            {
                Gizmos.color = _instance._selectedBorderColor;
                Gizmos.DrawLine(corners[0], corners[2]);
                Gizmos.DrawLine(corners[1], corners[3]);
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
                }
            }
            else
            {
                Gizmos.color = _instance._borderColor;
                for (int i = 0; i < 4; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
                }
            }
            Gizmos.color = lastColor;
        }
        SceneView.RepaintAll();
    }
}
#endif