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
    unsafe class DragDropEventCustomHandler_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::DragDropEventCustomHandler);
            args = new Type[]{typeof(System.Action<UnityEngine.EventSystems.PointerEventData>)};
            method = type.GetMethod("BindingDragAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingDragAction_0);
            args = new Type[]{typeof(System.Action<UnityEngine.EventSystems.PointerEventData>)};
            method = type.GetMethod("BindingBeginDragAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingBeginDragAction_1);
            args = new Type[]{typeof(System.Action<UnityEngine.EventSystems.PointerEventData>)};
            method = type.GetMethod("BindingEndDragAction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindingEndDragAction_2);


        }


        static StackObject* BindingDragAction_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.EventSystems.PointerEventData> @dragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::DragDropEventCustomHandler instance_of_this_method = (global::DragDropEventCustomHandler)typeof(global::DragDropEventCustomHandler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingDragAction(@dragAction);

            return __ret;
        }

        static StackObject* BindingBeginDragAction_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.EventSystems.PointerEventData> @beginDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::DragDropEventCustomHandler instance_of_this_method = (global::DragDropEventCustomHandler)typeof(global::DragDropEventCustomHandler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingBeginDragAction(@beginDragAction);

            return __ret;
        }

        static StackObject* BindingEndDragAction_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<UnityEngine.EventSystems.PointerEventData> @endDragAction = (System.Action<UnityEngine.EventSystems.PointerEventData>)typeof(System.Action<UnityEngine.EventSystems.PointerEventData>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::DragDropEventCustomHandler instance_of_this_method = (global::DragDropEventCustomHandler)typeof(global::DragDropEventCustomHandler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindingEndDragAction(@endDragAction);

            return __ret;
        }



    }
}
