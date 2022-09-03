#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class UGUIEditorUtil
{
    private static Canvas CreateCanvas()
    {
        GameObject objectCanvas = new GameObject("Canvas");
        Canvas canvas = objectCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        objectCanvas.AddComponent<CanvasScaler>();
        objectCanvas.AddComponent<GraphicRaycaster>();

        EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject objectEventSystem = new GameObject("EventSystem");
            objectEventSystem.transform.position = Vector3.zero;
            objectEventSystem.AddComponent<EventSystem>();
            objectEventSystem.AddComponent<StandaloneInputModule>();
        }
        return canvas;
    }

    public static void CreateUGUIObject<T>(string objectName) where T : UIBehaviour
    {
        Transform transformRoot;
        if (Selection.activeTransform != null)
        {
            Canvas canvas = Selection.activeTransform.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                transformRoot = Selection.activeTransform;
            }
            else
            {
                canvas = GameObject.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    canvas = UGUIEditorUtil.CreateCanvas();
                }
                transformRoot = canvas.transform;
            }
        }
        else
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                transformRoot = canvas.transform;
            }
            else
            {
                transformRoot = UGUIEditorUtil.CreateCanvas().transform;
            }
        }

        GameObject newObject = new GameObject(objectName);
        newObject.AddComponent<RectTransform>();
        newObject.AddComponent<T>();

        newObject.transform.SetParent(transformRoot);
        newObject.transform.localScale = Vector3.one;
        newObject.transform.localPosition = Vector3.zero;

        newObject.layer = LayerMask.NameToLayer("UI");
        Selection.activeGameObject = newObject;
    }
}
#endif