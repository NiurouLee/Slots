#if UNITY_EDITOR
using UnityEngine;
using UnityEditor.UI;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(RoundCornerRawImage))]
public class RoundCornerRawImageEditor : RawImageEditor
{
    private SerializedProperty _segment;
    private SerializedProperty _topLeftRadius;
    private SerializedProperty _topRightRadius;
    private SerializedProperty _bottomLeftRadius;
    private SerializedProperty _bottomRightRadius;
    private SerializedProperty _unifiedRadius;
    private SerializedProperty _maximumRadius;
    private RectTransform _rectTransform;

    protected override void OnEnable()
    {
        base.OnEnable();
        _segment = serializedObject.FindProperty("_segment");
        _topLeftRadius = serializedObject.FindProperty("_topLeftRadius");
        _topRightRadius = serializedObject.FindProperty("_topRightRadius");
        _bottomLeftRadius = serializedObject.FindProperty("_bottomLeftRadius");
        _bottomRightRadius = serializedObject.FindProperty("_bottomRightRadius");
        _unifiedRadius = serializedObject.FindProperty("_unifiedRadius");
        _maximumRadius = serializedObject.FindProperty("_maximumRadius");
        _rectTransform = (target as RawImage).rectTransform;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
        EditorGUI.EndDisabledGroup();
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(_segment);
        EditorGUILayout.PropertyField(_unifiedRadius);
        if (_unifiedRadius.boolValue == false)
        {
            EditorGUILayout.PropertyField(_topLeftRadius);
            EditorGUILayout.PropertyField(_topRightRadius);
            EditorGUILayout.PropertyField(_bottomLeftRadius);
            EditorGUILayout.PropertyField(_bottomRightRadius);
        }
        else
        {
            EditorGUILayout.PropertyField(_maximumRadius);
            if (_maximumRadius.boolValue == true)
            {
                float size = Mathf.Min(_rectTransform.rect.width, _rectTransform.rect.height);
                _topLeftRadius.doubleValue = size * 0.5f;
            }
            EditorGUILayout.PropertyField(_topLeftRadius, new GUIContent("Radius"));
            _topRightRadius.doubleValue = _topLeftRadius.doubleValue;
            _bottomLeftRadius.doubleValue = _topLeftRadius.doubleValue;
            _bottomRightRadius.doubleValue = _topLeftRadius.doubleValue;
        }
        _topLeftRadius.doubleValue = _topLeftRadius.doubleValue < 0 ? 0 : _topLeftRadius.doubleValue;
        _topRightRadius.doubleValue = _topRightRadius.doubleValue < 0 ? 0 : _topRightRadius.doubleValue;
        _bottomLeftRadius.doubleValue = _bottomLeftRadius.doubleValue < 0 ? 0 : _bottomLeftRadius.doubleValue;
        _bottomRightRadius.doubleValue = _bottomRightRadius.doubleValue < 0 ? 0 : _bottomRightRadius.doubleValue;
        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/UI/Round Corner Raw Image", false, -100)]
    private static void Create()
    {
        UGUIEditorUtil.CreateUGUIObject<RoundCornerRawImage>("RoundCornerRawImage");
    }
}
#endif