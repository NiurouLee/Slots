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
    unsafe class ILRuntime_Reflection_ILRuntimeWrapperType_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(ILRuntime.Reflection.ILRuntimeWrapperType);
            args = new Type[]{};
            method = type.GetMethod("get_RealType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_RealType_0);
            args = new Type[]{};
            method = type.GetMethod("get_CLRType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CLRType_1);


        }


        static StackObject* get_RealType_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ILRuntime.Reflection.ILRuntimeWrapperType instance_of_this_method = (ILRuntime.Reflection.ILRuntimeWrapperType)typeof(ILRuntime.Reflection.ILRuntimeWrapperType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.RealType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_CLRType_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ILRuntime.Reflection.ILRuntimeWrapperType instance_of_this_method = (ILRuntime.Reflection.ILRuntimeWrapperType)typeof(ILRuntime.Reflection.ILRuntimeWrapperType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CLRType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
