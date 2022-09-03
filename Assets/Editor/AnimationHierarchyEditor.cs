#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;

public class AnimationHierarchyEditor : EditorWindow {
	private static int columnWidth = 300;
	
	private Animator animatorObject;
	private GameObject destAnimatorObject;
	private List<AnimationClip> animationClips;
	private ArrayList pathsKeys;
	private Hashtable paths;
	private Dictionary<string, string> dicFromPaths;
	private Dictionary<string, string> dicToPaths;
	

	private Vector2 scrollPos = Vector2.zero;
	
	[MenuItem("Tools/改变Animator GameObject")]
	static void ShowWindow() {
		GetWindow<AnimationHierarchyEditor>();
	}


	public AnimationHierarchyEditor(){
		animationClips = new List<AnimationClip>();
		dicFromPaths = new Dictionary<string, string>();
		dicToPaths = new Dictionary<string, string>();
	}

	void OnGUI() {
		scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("源 Animator:", GUILayout.Width(columnWidth));
		animatorObject = ((Animator)EditorGUILayout.ObjectField(
				animatorObject,
				typeof(Animator),
				true,
				GUILayout.Width(columnWidth))
			);
		//destAnimatorObject =
				
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("目标 Animator:", GUILayout.Width(columnWidth));
		destAnimatorObject = ((GameObject)EditorGUILayout.ObjectField(
				destAnimatorObject,
				typeof(GameObject),
				true,
				GUILayout.Width(columnWidth))
			);
		if (destAnimatorObject && destAnimatorObject.GetComponent<Animator>() == null)
		{
			destAnimatorObject.gameObject.AddComponent<Animator>();
		}
				
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(20);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("改变Animator依附的节点")) {
			if (animatorObject && destAnimatorObject)
			{
				if (animatorObject && destAnimatorObject)
				{
					animationClips.Clear();
					dicFromPaths.Clear();
					dicToPaths.Clear();
					GetAllChildPath(animatorObject.transform, in dicFromPaths, "", 0);
					GetAllChildPath(destAnimatorObject.transform, in dicToPaths, "", 0);
					var animClips = AnimationUtility.GetAnimationClips(animatorObject.gameObject);
					for (int i = 0; i < animClips.Length; i++)
					{
						animationClips.Add(animClips[i]);
					}
					ReplaceRoot();
					destAnimatorObject.GetComponent<Animator>().runtimeAnimatorController =
						animatorObject.runtimeAnimatorController;
					DestroyImmediate(animatorObject.GetComponent<Animator>());
				}
			}
		}

		EditorGUILayout.EndHorizontal();

		GUILayout.Space(40);
		GUILayout.EndScrollView();
	}
	
	private void GetAllChildPath(Transform transform, in Dictionary<string, string> dictPath, string path, int depth)
	{
		string newPath = "";
		if (depth>0)
		{
			newPath = path+transform.name;
			if (!dictPath.ContainsKey(transform.name))
			{
				dictPath.Add(transform.name, newPath);
			}	
		}

		var childCount = transform.childCount;
		if (childCount>0)
		{
			if (depth>0)
			{
				newPath = newPath + "/";	
			}
			for (int i = 0; i < childCount; i++)
			{
				GetAllChildPath(transform.GetChild(i), in dictPath, newPath, depth+1);	
			}	
		}
	}

	void OnInspectorUpdate() {
		this.Repaint();
	}
	
	void FillModel() {
		paths = new Hashtable();
		pathsKeys = new ArrayList();

		foreach ( AnimationClip animationClip in animationClips )
		{
			FillModelWithCurves(AnimationUtility.GetCurveBindings(animationClip));
			FillModelWithCurves(AnimationUtility.GetObjectReferenceCurveBindings(animationClip));
		}
	}
	
	private void FillModelWithCurves(EditorCurveBinding[] curves) {
		foreach (EditorCurveBinding curveData in curves) {
			string key = curveData.path;
			if (paths.ContainsKey(key)) {
				((ArrayList)paths[key]).Add(curveData);
			} else {
				ArrayList newProperties = new ArrayList();
				newProperties.Add(curveData);
				paths.Add(key, newProperties);
				pathsKeys.Add(key);
			}
		}
	}

	void ReplaceRoot()
	{
		FillModel();
		float fProgress = 0.0f;

		AssetDatabase.StartAssetEditing();
		
		for ( int iCurrentClip = 0; iCurrentClip < animationClips.Count; iCurrentClip++ )
		{
			AnimationClip animationClip =  animationClips[iCurrentClip];
			Undo.RecordObject(animationClip, "Animation Hierarchy Root Change");
			for ( int iCurrentPath = 0; iCurrentPath < pathsKeys.Count; iCurrentPath ++)
			{
				string path = pathsKeys[iCurrentPath] as string;
				ArrayList curves = (ArrayList)paths[path];
				for (int i = 0; i < curves.Count; i++) 
				{
					
					EditorCurveBinding binding = (EditorCurveBinding)curves[i];

					string nameTransform = path.Substring(path.LastIndexOf('/')+1);
					if (dicFromPaths.ContainsKey(nameTransform) && dicToPaths.ContainsKey(nameTransform))
					{
						string sNewPath = Regex.Replace(path, "^"+dicFromPaths[nameTransform], dicToPaths[nameTransform] );
						AnimationCurve curve = AnimationUtility.GetEditorCurve(animationClip, binding);
						if ( curve != null )
						{
							AnimationUtility.SetEditorCurve(animationClip, binding, null);				
							binding.path = sNewPath;
							AnimationUtility.SetEditorCurve(animationClip, binding, curve);
						}
						else
						{
							ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(animationClip, binding);
							if (objectReferenceCurve != null && objectReferenceCurve.Length>0)
							{
								AnimationUtility.SetObjectReferenceCurve(animationClip, binding, null);
								binding.path = sNewPath;
								AnimationUtility.SetObjectReferenceCurve(animationClip, binding, objectReferenceCurve);	
							}
						}
					}
				}
				
				// Update the progress meter
				float fChunk = 1f / animationClips.Count;
				fProgress = (iCurrentClip * fChunk) + fChunk * ((float) iCurrentPath / (float) pathsKeys.Count);				
				
				EditorUtility.DisplayProgressBar(
					"Animation Hierarchy Progress", 
					"How far along the animation editing has progressed.",
					fProgress);
			}

		}
		EditorUtility.SetDirty(animatorObject);
		EditorUtility.SetDirty(destAnimatorObject);
		EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
		AssetDatabase.StopAssetEditing();
		EditorUtility.ClearProgressBar();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		FillModel();
		this.Repaint();
	}
}

#endif