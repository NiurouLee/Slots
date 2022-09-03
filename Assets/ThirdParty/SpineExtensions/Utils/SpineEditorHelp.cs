
#if UNITY_EDITOR
namespace DragonPlus.SpineExtensions
{
    using System;
    using System.Reflection;
    using System.Text;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    public class SpineEditorHelp
    {
        public static FieldInfo GetAnimEditor(ref EditorWindow animationWindowEditor)
        {
            System.Type animationWindowType = null;
            animationWindowEditor = ShowAndReturnEditorWindow("AnimationWindow", ref animationWindowType);

            //Get animationWindow Type
            animationWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");



            //Get field m_AnimEditor
            FieldInfo animEditorFI = animationWindowType.GetField("m_AnimEditor", GetFullBinding());
            return animEditorFI;
        }
        public static PropertyInfo P_IsPlaying(FieldInfo animEditorFI)
        {
            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", GetFullBinding());

            //Get property i splaying or not
            PropertyInfo isPlaying = controlInterfacePI.PropertyType.GetProperty("playing", GetFullBinding());
            return isPlaying;
        }

        public static PropertyInfo P_Time(FieldInfo animEditorFI)
        {
            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", GetFullBinding());

            //Get property i splaying or not
            PropertyInfo AnimationKeyTimetime = controlInterfacePI.PropertyType.GetProperty("time", GetFullBinding());
            //PropertyInfo time=AnimationKeyTimetime.PropertyType.GetProperty("time", GetFullBinding());

            //Type type = Type.GetType("UnityEditorInternal.AnimationKeyTime")；
            //UnityEditorInternal.AnimationKeyTime

            return AnimationKeyTimetime;
        }
        public static object GetcontrolInterface(FieldInfo animEditorFI, EditorWindow eWin)
        {
            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", GetFullBinding());
            return controlInterfacePI.GetValue(animEditorFI.GetValue(eWin));
        }
        // public static PropertyInfo GetPropertyInfoFromType(Type type,string proName){
        //         type.GetProperty(proName)
        // }
        /// <summary>
        /// play button on animator
        /// </summary>
        public static void SetPlayButton()
        {
            //open Animation Editor Window
            System.Type animationWindowType = null;
            EditorWindow animationWindowEditor = ShowAndReturnEditorWindow("AnimationWindow", ref animationWindowType);

            //Get animationWindow Type
            animationWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");



            //Get field m_AnimEditor
            FieldInfo animEditorFI = animationWindowType.GetField("m_AnimEditor", GetFullBinding());

            //Get the propertue of animEditorFI
            PropertyInfo controlInterfacePI = animEditorFI.FieldType.GetProperty("controlInterface", GetFullBinding());

            //Get property i splaying or not
            PropertyInfo isPlaying = controlInterfacePI.PropertyType.GetProperty("playing", GetFullBinding());

            //get object controlInterface
            object controlInterface = controlInterfacePI.GetValue(animEditorFI.GetValue(animationWindowEditor));
            bool playing = (bool)isPlaying.GetValue(controlInterface);


            //Get property i splaying or not
            PropertyInfo position = controlInterfacePI.PropertyType.GetProperty("position", GetFullBinding());

            PropertyInfo[] propers = controlInterfacePI.PropertyType.GetProperties();

            for (int i = 0; i < propers.Length; i++)
            {
                UnityEngine.Debug.Log("Arthur------>PP[" + propers[i].PropertyType + "]=" + propers[i].Name);
            }


            var filds = controlInterfacePI.PropertyType.GetRuntimeFields();

            foreach (var f in filds)
            {
                UnityEngine.Debug.Log("Arthur------>ff=" + f.Name);
            }

            //   if (!playing)
            //   {
            //       MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StartPlayback", GetFullBinding());
            //        playMI.Invoke(controlInterface, new object[0]);
            //   }
            //   else
            //   {
            //       MethodInfo playMI = controlInterfacePI.PropertyType.GetMethod("StopPlayback", GetFullBinding());
            //       playMI.Invoke(controlInterface, new object[0]);
            //   }
        }

        public static System.Reflection.BindingFlags GetFullBinding()
        {
            return (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.NonPublic);
        }


        /// <summary>
        /// from a given name, return and open/show the editorWindow
        /// usage:
        /// System.Type animationWindowType = null;
        /// EditorWindow animationWindowEditor = ShowAndReturnEditorWindow(ExtReflexion.AllNameAssemblyKnown.AnimationWindow, ref animationWindowType);
        /// </summary>
        public static EditorWindow ShowAndReturnEditorWindow(string editorWindow, ref System.Type animationWindowType)
        {
            System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
            animationWindowType = GetTypeFromAssembly(editorWindow, editorAssembly);
            EditorWindow animationWindowEditor = EditorWindow.GetWindow(animationWindowType);

            return (animationWindowEditor);
        }

        /// <summary>
        /// System.Reflection.Assembly editorAssembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
        /// GetTypeFromAssembly("AnimationWindow", editorAssembly);
        /// </summary>
        /// <returns></returns>
        public static System.Type GetTypeFromAssembly(string typeName, System.Reflection.Assembly assembly, System.StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase)
        {
            if (assembly == null)
                return (null);

            System.Type[] types = assembly.GetTypes();
            foreach (System.Type type in types)
            {
                if (type.Name.Equals(typeName, ignoreCase) || type.Name.Contains('+' + typeName))
                    return (type);
            }
            return (null);
        }
    }
}
#endif