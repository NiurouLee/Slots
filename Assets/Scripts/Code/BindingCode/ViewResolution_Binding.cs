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
    unsafe class ViewResolution_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ViewResolution);
            args = new Type[]{typeof(UnityEngine.Camera), typeof(System.Boolean)};
            method = type.GetMethod("GetCameraPositionByViewResolution", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCameraPositionByViewResolution_0);

            field = type.GetField("referenceResolutionLandscape", flag);
            app.RegisterCLRFieldGetter(field, get_referenceResolutionLandscape_0);
            app.RegisterCLRFieldSetter(field, set_referenceResolutionLandscape_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_referenceResolutionLandscape_0, AssignFromStack_referenceResolutionLandscape_0);
            field = type.GetField("referenceResolutionPortrait", flag);
            app.RegisterCLRFieldGetter(field, get_referenceResolutionPortrait_1);
            app.RegisterCLRFieldSetter(field, set_referenceResolutionPortrait_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_referenceResolutionPortrait_1, AssignFromStack_referenceResolutionPortrait_1);
            field = type.GetField("designSize", flag);
            app.RegisterCLRFieldGetter(field, get_designSize_2);
            app.RegisterCLRFieldSetter(field, set_designSize_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_designSize_2, AssignFromStack_designSize_2);


        }


        static StackObject* GetCameraPositionByViewResolution_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @isPortrait = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Camera @camera = (UnityEngine.Camera)typeof(UnityEngine.Camera).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::ViewResolution.GetCameraPositionByViewResolution(@camera, @isPortrait);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static object get_referenceResolutionLandscape_0(ref object o)
        {
            return global::ViewResolution.referenceResolutionLandscape;
        }

        static StackObject* CopyToStack_referenceResolutionLandscape_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ViewResolution.referenceResolutionLandscape;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_referenceResolutionLandscape_0(ref object o, object v)
        {
            global::ViewResolution.referenceResolutionLandscape = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_referenceResolutionLandscape_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @referenceResolutionLandscape = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::ViewResolution.referenceResolutionLandscape = @referenceResolutionLandscape;
            return ptr_of_this_method;
        }

        static object get_referenceResolutionPortrait_1(ref object o)
        {
            return global::ViewResolution.referenceResolutionPortrait;
        }

        static StackObject* CopyToStack_referenceResolutionPortrait_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ViewResolution.referenceResolutionPortrait;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_referenceResolutionPortrait_1(ref object o, object v)
        {
            global::ViewResolution.referenceResolutionPortrait = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_referenceResolutionPortrait_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @referenceResolutionPortrait = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::ViewResolution.referenceResolutionPortrait = @referenceResolutionPortrait;
            return ptr_of_this_method;
        }

        static object get_designSize_2(ref object o)
        {
            return global::ViewResolution.designSize;
        }

        static StackObject* CopyToStack_designSize_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ViewResolution.designSize;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_designSize_2(ref object o, object v)
        {
            global::ViewResolution.designSize = (UnityEngine.Vector2)v;
        }

        static StackObject* AssignFromStack_designSize_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Vector2 @designSize = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::ViewResolution.designSize = @designSize;
            return ptr_of_this_method;
        }



    }
}
