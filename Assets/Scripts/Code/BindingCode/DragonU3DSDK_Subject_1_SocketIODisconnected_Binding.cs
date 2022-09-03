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
    unsafe class DragonU3DSDK_Subject_1_SocketIODisconnected_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Subject<DragonU3DSDK.SDKEvents.SocketIODisconnected>);
            args = new Type[]{};
            method = type.GetMethod("Trigger", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Trigger_0);


        }


        static StackObject* Trigger_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Subject<DragonU3DSDK.SDKEvents.SocketIODisconnected> instance_of_this_method = (DragonU3DSDK.Subject<DragonU3DSDK.SDKEvents.SocketIODisconnected>)typeof(DragonU3DSDK.Subject<DragonU3DSDK.SDKEvents.SocketIODisconnected>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Trigger();

            return __ret;
        }



    }
}
