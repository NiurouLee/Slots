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
    unsafe class System_Action_2_RepeatedField_1_Adapt_IMessage_Binding_Adaptor_Action_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.Action<Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>, System.Action>);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>), typeof(System.Action)};
            method = type.GetMethod("Invoke", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Invoke_0);


        }


        static StackObject* Invoke_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @arg2 = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> @arg1 = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>, System.Action> instance_of_this_method = (System.Action<Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>, System.Action>)typeof(System.Action<Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>, System.Action>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Invoke(@arg1, @arg2);

            return __ret;
        }



    }
}
