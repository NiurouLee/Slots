namespace DragonPlus.SpineExtensions
{
    using System;
    using UnityEngine;
    using Spine.Unity;
#if UNITY_EDITOR
    using UnityEditor;
    using System.Reflection;

    
#endif
    [ExecuteInEditMode]
    public class SpineAnimationChange : MonoBehaviour
    {
        public SkeletonGraphic m_skeletonGraphic;
        public SkeletonAnimation m_SkeletonAnimation;

        public int AnimationIndex_Int;
        public bool AnimationLoop_Bool;
        // private int lastAnimationIndex_Int = -1;
        public string PlayName = "";
        // private Spine.TrackEntry currentTrackEntry;


        void OnEnable()
        {
            // AnimationIndex_Int = -1;
            // lastAnimationIndex_Int = -1;

            // Debug.LogError($"Arthur----->OnEnable,Name={gameObject.name},index={AnimationIndex_Int}");
            if(m_SkeletonAnimation == null)
                m_SkeletonAnimation = transform.GetComponent<SkeletonAnimation>();

            if (m_skeletonGraphic == null)
            {
                m_skeletonGraphic = transform.GetComponent<SkeletonGraphic>();
            }
            // CheckAnimationChange(true);

            DidApplyAnimation(true);
        }
        
        private void OnDidApplyAnimationProperties()
        {
            // CheckAnimationChange();

            DidApplyAnimation(false);
        }
        
        private string GetAnimationName(int animationIndex)
        {
            if (m_skeletonGraphic != null)
            {
                return m_skeletonGraphic.SkeletonData.Animations.Items[animationIndex].Name;
            }

            if (m_SkeletonAnimation != null)
            {
                return m_SkeletonAnimation.skeletonDataAsset.GetSkeletonData(true).Animations.Items[animationIndex]
                    .Name;
            }

            return String.Empty;
        }

        private int GetAnimationCount()
        {
            if (m_skeletonGraphic != null)
            {
                return m_skeletonGraphic.SkeletonData.Animations.Items.Length;
            }

            if (m_SkeletonAnimation != null)
            {
                return m_SkeletonAnimation.skeletonDataAsset.GetSkeletonData(true).Animations.Items.Length;
            }

            return 0;
        }

        private Spine.AnimationState GetAnimationState()
        {
            if (m_skeletonGraphic != null)
            {
                return m_skeletonGraphic.AnimationState;
            }

            if (m_SkeletonAnimation != null)
            {
                return m_SkeletonAnimation.AnimationState;
            }

            return null;
        }


        private void UpdateSkeletonAnimation(float deltaTime)
        {
            if (m_skeletonGraphic != null)
            {
                 m_skeletonGraphic.Update(deltaTime);
            }

            if (m_SkeletonAnimation != null)
            {
                 m_SkeletonAnimation.Update(deltaTime);
            }
        }

        private bool DidApplyAnimation(bool force)
        {
            if (AnimationIndex_Int < 0 || AnimationIndex_Int >= GetAnimationCount())
            {
                Debug.Log("DidApplyAnimation error AnimationIndex_Int: " + AnimationIndex_Int);

                return false;
            }

            var animationName = GetAnimationName(AnimationIndex_Int);
            if (string.IsNullOrEmpty(animationName))
            {
                Debug.Log("DidApplyAnimation error animationName: " + animationName);

                return false;
            }

            var state = GetAnimationState();
            if (state == null)
            {
                Debug.Log("DidApplyAnimation error state null animationName: " + animationName);
                return false;
            }

            if (!force)
            {
                var track = state.GetCurrent(0);
                if (track != null)
                {
                    if (track.Animation != null 
                        && track.Animation.Name == animationName 
                        && track.Loop == AnimationLoop_Bool)
                    {
                        Debug.Log("DidApplyAnimation same name: " + animationName);
                        return false;
                    }
                }
            }
            
            PlayName = animationName;

            GetAnimationState().ClearTracks();

            var skeleton = m_SkeletonAnimation != null ? m_SkeletonAnimation.Skeleton : null;

            skeleton?.SetToSetupPose();

            // currentTrackEntry = 
            GetAnimationState().SetAnimation(0, PlayName, AnimationLoop_Bool);
            // GetAnimationState().Update(0);

            skeleton?.UpdateWorldTransform();
            //m_SkeletonAnimation?.LateUpdate();
            UpdateSkeletonAnimation(0);
            
            

            Debug.Log("DidApplyAnimation animationName: " + animationName);

#if UNITY_EDITOR
            lastTime = 0;
#endif
            return true;
        }

        //检查是否切换动画
//         void CheckAnimationChange(bool forcePlay = false)
//         {
//             if (forcePlay)
//             {
//                 if (AnimationIndex_Int >= 0 && AnimationIndex_Int < GetAnimationCount())
//                 {
//                     PlayName = GetAnimationName(AnimationIndex_Int);

//                     if (currentTrackEntry != null && !currentTrackEntry.IsComplete)
//                     {
//                         GetAnimationState().ClearListenerNotifications();
//                         if (m_SkeletonAnimation)
//                         {
//                             m_SkeletonAnimation.ClearState();
//                         }
//                     }

//                     currentTrackEntry = GetAnimationState().SetAnimation(0, PlayName, AnimationLoop_Bool);
//                     GetAnimationState().Update(0);

//                     lastAnimationIndex_Int = AnimationIndex_Int;
// #if UNITY_EDITOR
//                     lastTime = 0;
// #endif
//                 }
//             }
//             else
//             {
//                 if (lastAnimationIndex_Int != AnimationIndex_Int)
//                 {
//                    if (AnimationIndex_Int >= 0 && AnimationIndex_Int < GetAnimationCount())
//                     {
//                         PlayName = GetAnimationName(AnimationIndex_Int);
//                         //第一次播动画
//                         if (lastAnimationIndex_Int < 0)
//                         {
//                             if (currentTrackEntry != null && !currentTrackEntry.IsComplete)
//                             {
//                                 GetAnimationState().ClearListenerNotifications();
                                
//                                 if (m_SkeletonAnimation)
//                                 {
//                                     m_SkeletonAnimation.ClearState();
//                                 }
//                             }
//                         }

//                         GetAnimationState().ClearTrack(0);
//                         currentTrackEntry = GetAnimationState().SetAnimation(0, PlayName, AnimationLoop_Bool);
//                         //第一次播动画，避免跳帧
//                     //    if (lastAnimationIndex_Int < 0)
//                         {
//                             GetAnimationState().Update(0);
//                         }

//                         lastAnimationIndex_Int = AnimationIndex_Int;
// #if UNITY_EDITOR
//                         lastTime = 0;
// #endif
//                     }
//                 }
//             }
//         }

#if UNITY_EDITOR

        #region Editor

        static FieldInfo animEditor;
        static PropertyInfo IsPlaying;
        static PropertyInfo Time;

        static EditorWindow animationWindowEditor;
        static object controlInterface;

        static Type AnimationKeyTimeType;
        static bool initEditor = false;
        private float lastTime = 0;

        public static bool InitInEditor()
        {
            if (initEditor) return false;
            animEditor = SpineEditorHelp.GetAnimEditor(ref animationWindowEditor);
            IsPlaying = SpineEditorHelp.P_IsPlaying(animEditor);
            Time = SpineEditorHelp.P_Time(animEditor);
            controlInterface = SpineEditorHelp.GetcontrolInterface(animEditor, animationWindowEditor);
            AnimationKeyTimeType = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.AnimationKeyTime");
            initEditor = true;
            return true;
        }

        public static bool ReleaseEditor()
        {
            if (!initEditor) return false;
            animEditor = null;
            IsPlaying = null;
            Time = null;
            controlInterface = null;
            AnimationKeyTimeType = null;
            initEditor = false;
            return true;
        }

        public void StartInEditor()
        {
            OnEnable();
        }

        //编辑器下的Update
        public void OnAnimationEditorUpdate()
        {
            //Debug.Log("Arthur----->OnAnimationEditorUpdate");
            if (animEditor != null)
            {
                PropertyInfo apInfo = AnimationKeyTimeType.GetProperty("time");
                object value = Time.GetValue(controlInterface);
                float time = ((float) apInfo.GetValue(value));
                float deltaTime = time - lastTime;
                Debug.Log($"Arthur----->OnAnimationEditorUpdate,time={time},deltaTime={deltaTime}");
                // CheckAnimationChange();

                DidApplyAnimation(false);

                if (deltaTime > 0)
                {
                    //倒着播会有点问题，因为设置的动画不一样
                    GetAnimationState().Update(deltaTime);
                }

                lastTime = time;

                if (m_skeletonGraphic != null)
                    m_skeletonGraphic.LateUpdate();

                else if (m_skeletonGraphic != null)
                    m_SkeletonAnimation.LateUpdate();
            }
        }

        #endregion

#endif
    }
}