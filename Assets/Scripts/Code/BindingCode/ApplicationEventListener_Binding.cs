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
    unsafe class ApplicationEventListener_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::ApplicationEventListener);
            args = new Type[]{typeof(System.Action<System.Boolean>)};
            method = type.GetMethod("set_OnApplicationFocusAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnApplicationFocusAction_0);
            args = new Type[]{typeof(System.Action<System.Boolean>)};
            method = type.GetMethod("set_OnApplicationPauseAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnApplicationPauseAction_1);
            args = new Type[]{typeof(System.Action)};
            method = type.GetMethod("set_OnApplicationQuitAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnApplicationQuitAction_2);
            args = new Type[]{typeof(System.Action<System.String>)};
            method = type.GetMethod("set_OnDeepLinkActivateAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_OnDeepLinkActivateAction_3);


        }


        static StackObject* set_OnApplicationFocusAction_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean> @value = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::ApplicationEventListener.OnApplicationFocusAction = value;

            return __ret;
        }

        static StackObject* set_OnApplicationPauseAction_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean> @value = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::ApplicationEventListener.OnApplicationPauseAction = value;

            return __ret;
        }

        static StackObject* set_OnApplicationQuitAction_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @value = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::ApplicationEventListener.OnApplicationQuitAction = value;

            return __ret;
        }

        static StackObject* set_OnDeepLinkActivateAction_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String> @value = (System.Action<System.String>)typeof(System.Action<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::ApplicationEventListener.OnDeepLinkActivateAction = value;

            return __ret;
        }



    }
}
