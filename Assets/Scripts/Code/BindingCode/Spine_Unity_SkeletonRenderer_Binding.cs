using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class Spine_Unity_SkeletonRenderer_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Spine.Unity.SkeletonRenderer);

            field = type.GetField("maskInteraction", flag);
            app.RegisterCLRFieldGetter(field, get_maskInteraction_0);
            app.RegisterCLRFieldSetter(field, set_maskInteraction_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_maskInteraction_0, AssignFromStack_maskInteraction_0);


        }



        static object get_maskInteraction_0(ref object o)
        {
            return ((Spine.Unity.SkeletonRenderer)o).maskInteraction;
        }

        static StackObject* CopyToStack_maskInteraction_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Spine.Unity.SkeletonRenderer)o).maskInteraction;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_maskInteraction_0(ref object o, object v)
        {
            ((Spine.Unity.SkeletonRenderer)o).maskInteraction = (UnityEngine.SpriteMaskInteraction)v;
        }

        static StackObject* AssignFromStack_maskInteraction_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.SpriteMaskInteraction @maskInteraction = (UnityEngine.SpriteMaskInteraction)typeof(UnityEngine.SpriteMaskInteraction).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Spine.Unity.SkeletonRenderer)o).maskInteraction = @maskInteraction;
            return ptr_of_this_method;
        }



    }
}
