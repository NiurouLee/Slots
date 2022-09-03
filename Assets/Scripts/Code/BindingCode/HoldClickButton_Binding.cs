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
    unsafe class HoldClickButton_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::HoldClickButton);

            field = type.GetField("onLongClick", flag);
            app.RegisterCLRFieldGetter(field, get_onLongClick_0);
            app.RegisterCLRFieldSetter(field, set_onLongClick_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onLongClick_0, AssignFromStack_onLongClick_0);


        }



        static object get_onLongClick_0(ref object o)
        {
            return ((global::HoldClickButton)o).onLongClick;
        }

        static StackObject* CopyToStack_onLongClick_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::HoldClickButton)o).onLongClick;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onLongClick_0(ref object o, object v)
        {
            ((global::HoldClickButton)o).onLongClick = (UnityEngine.Events.UnityEvent)v;
        }

        static StackObject* AssignFromStack_onLongClick_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Events.UnityEvent @onLongClick = (UnityEngine.Events.UnityEvent)typeof(UnityEngine.Events.UnityEvent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::HoldClickButton)o).onLongClick = @onLongClick;
            return ptr_of_this_method;
        }



    }
}
