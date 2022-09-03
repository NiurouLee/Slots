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
    unsafe class UnityEngine_SpriteMask_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.SpriteMask);
            args = new Type[]{};
            method = type.GetMethod("get_backSortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_backSortingOrder_0);
            args = new Type[]{};
            method = type.GetMethod("get_frontSortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_frontSortingOrder_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_frontSortingLayerID", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_frontSortingLayerID_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_frontSortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_frontSortingOrder_3);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_backSortingLayerID", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_backSortingLayerID_4);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_backSortingOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_backSortingOrder_5);


        }


        static StackObject* get_backSortingOrder_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.backSortingOrder;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_frontSortingOrder_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.frontSortingOrder;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_frontSortingLayerID_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.frontSortingLayerID = value;

            return __ret;
        }

        static StackObject* set_frontSortingOrder_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.frontSortingOrder = value;

            return __ret;
        }

        static StackObject* set_backSortingLayerID_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.backSortingLayerID = value;

            return __ret;
        }

        static StackObject* set_backSortingOrder_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.SpriteMask instance_of_this_method = (UnityEngine.SpriteMask)typeof(UnityEngine.SpriteMask).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.backSortingOrder = value;

            return __ret;
        }



    }
}
