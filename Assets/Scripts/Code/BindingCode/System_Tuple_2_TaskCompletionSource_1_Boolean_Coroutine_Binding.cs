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
    unsafe class System_Tuple_2_TaskCompletionSource_1_Boolean_Coroutine_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>);
            args = new Type[]{};
            method = type.GetMethod("get_Item1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item1_0);
            args = new Type[]{};
            method = type.GetMethod("get_Item2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item2_1);

            args = new Type[]{typeof(System.Threading.Tasks.TaskCompletionSource<System.Boolean>), typeof(UnityEngine.Coroutine)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* get_Item1_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine> instance_of_this_method = (System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>)typeof(System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item1;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Item2_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine> instance_of_this_method = (System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>)typeof(System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Item2;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Coroutine @item2 = (UnityEngine.Coroutine)typeof(UnityEngine.Coroutine).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Threading.Tasks.TaskCompletionSource<System.Boolean> @item1 = (System.Threading.Tasks.TaskCompletionSource<System.Boolean>)typeof(System.Threading.Tasks.TaskCompletionSource<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new System.Tuple<System.Threading.Tasks.TaskCompletionSource<System.Boolean>, UnityEngine.Coroutine>(@item1, @item2);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
