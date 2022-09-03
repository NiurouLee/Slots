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
    unsafe class DG_Tweening_DOVirtual_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DG.Tweening.DOVirtual);
            args = new Type[]{typeof(System.Single), typeof(System.Single), typeof(System.Single), typeof(DG.Tweening.Ease)};
            method = type.GetMethod("EasedValue", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EasedValue_0);
            args = new Type[]{typeof(System.Single), typeof(DG.Tweening.TweenCallback), typeof(System.Boolean)};
            method = type.GetMethod("DelayedCall", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DelayedCall_1);


        }


        static StackObject* EasedValue_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DG.Tweening.Ease @easeType = (DG.Tweening.Ease)typeof(DG.Tweening.Ease).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @lifetimePercentage = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @to = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Single @from = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = DG.Tweening.DOVirtual.EasedValue(@from, @to, @lifetimePercentage, @easeType);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* DelayedCall_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @ignoreTimeScale = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DG.Tweening.TweenCallback @callback = (DG.Tweening.TweenCallback)typeof(DG.Tweening.TweenCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @delay = *(float*)&ptr_of_this_method->Value;


            var result_of_this_method = DG.Tweening.DOVirtual.DelayedCall(@delay, @callback, @ignoreTimeScale);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
