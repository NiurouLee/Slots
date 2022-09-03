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
    unsafe class ILRuntimeHelp_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ILRuntimeHelp);

            field = type.GetField("appdomain", flag);
            app.RegisterCLRFieldGetter(field, get_appdomain_0);
            app.RegisterCLRFieldSetter(field, set_appdomain_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_appdomain_0, AssignFromStack_appdomain_0);


        }



        static object get_appdomain_0(ref object o)
        {
            return global::ILRuntimeHelp.appdomain;
        }

        static StackObject* CopyToStack_appdomain_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = global::ILRuntimeHelp.appdomain;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_appdomain_0(ref object o, object v)
        {
            global::ILRuntimeHelp.appdomain = (ILRuntime.Runtime.Enviorment.AppDomain)v;
        }

        static StackObject* AssignFromStack_appdomain_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            ILRuntime.Runtime.Enviorment.AppDomain @appdomain = (ILRuntime.Runtime.Enviorment.AppDomain)typeof(ILRuntime.Runtime.Enviorment.AppDomain).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            global::ILRuntimeHelp.appdomain = @appdomain;
            return ptr_of_this_method;
        }



    }
}
