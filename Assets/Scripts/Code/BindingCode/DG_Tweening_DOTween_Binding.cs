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
    unsafe class DG_Tweening_DOTween_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DG.Tweening.DOTween);
            args = new Type[]{typeof(DG.Tweening.Core.DOGetter<System.Single>), typeof(DG.Tweening.Core.DOSetter<System.Single>), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_0);
            args = new Type[]{typeof(DG.Tweening.Core.DOGetter<System.Int64>), typeof(DG.Tweening.Core.DOSetter<System.Int64>), typeof(System.Int64), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_1);
            args = new Type[]{typeof(DG.Tweening.Core.DOSetter<System.Single>), typeof(System.Single), typeof(System.Single), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_2);
            args = new Type[]{typeof(System.Object), typeof(System.Boolean)};
            method = type.GetMethod("Kill", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Kill_3);
            args = new Type[]{typeof(DG.Tweening.Core.DOGetter<System.Double>), typeof(DG.Tweening.Core.DOSetter<System.Double>), typeof(System.Double), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_4);
            args = new Type[]{};
            method = type.GetMethod("Sequence", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Sequence_5);
            args = new Type[]{typeof(System.Collections.Generic.List<DG.Tweening.Tween>)};
            method = type.GetMethod("PlayingTweens", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayingTweens_6);
            args = new Type[]{typeof(DG.Tweening.Core.DOGetter<System.Int32>), typeof(DG.Tweening.Core.DOSetter<System.Int32>), typeof(System.Int32), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_7);
            args = new Type[]{typeof(DG.Tweening.Core.DOGetter<UnityEngine.Vector2>), typeof(DG.Tweening.Core.DOSetter<UnityEngine.Vector2>), typeof(UnityEngine.Vector2), typeof(System.Single)};
            method = type.GetMethod("To", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, To_8);


        }


        static StackObject* To_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @endValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DG.Tweening.Core.DOSetter<System.Single> @setter = (DG.Tweening.Core.DOSetter<System.Single>)typeof(DG.Tweening.Core.DOSetter<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOGetter<System.Single> @getter = (DG.Tweening.Core.DOGetter<System.Single>)typeof(DG.Tweening.Core.DOGetter<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@getter, @setter, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* To_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int64 @endValue = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DG.Tweening.Core.DOSetter<System.Int64> @setter = (DG.Tweening.Core.DOSetter<System.Int64>)typeof(DG.Tweening.Core.DOSetter<System.Int64>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOGetter<System.Int64> @getter = (DG.Tweening.Core.DOGetter<System.Int64>)typeof(DG.Tweening.Core.DOGetter<System.Int64>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@getter, @setter, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* To_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single @endValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single @startValue = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOSetter<System.Single> @setter = (DG.Tweening.Core.DOSetter<System.Single>)typeof(DG.Tweening.Core.DOSetter<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@setter, @startValue, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Kill_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @complete = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Object @targetOrId = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.Kill(@targetOrId, @complete);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* To_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Double @endValue = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DG.Tweening.Core.DOSetter<System.Double> @setter = (DG.Tweening.Core.DOSetter<System.Double>)typeof(DG.Tweening.Core.DOSetter<System.Double>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOGetter<System.Double> @getter = (DG.Tweening.Core.DOGetter<System.Double>)typeof(DG.Tweening.Core.DOGetter<System.Double>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@getter, @setter, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Sequence_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DG.Tweening.DOTween.Sequence();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* PlayingTweens_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.List<DG.Tweening.Tween> @fillableList = (System.Collections.Generic.List<DG.Tweening.Tween>)typeof(System.Collections.Generic.List<DG.Tweening.Tween>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.PlayingTweens(@fillableList);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* To_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @endValue = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DG.Tweening.Core.DOSetter<System.Int32> @setter = (DG.Tweening.Core.DOSetter<System.Int32>)typeof(DG.Tweening.Core.DOSetter<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOGetter<System.Int32> @getter = (DG.Tweening.Core.DOGetter<System.Int32>)typeof(DG.Tweening.Core.DOGetter<System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@getter, @setter, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* To_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @duration = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.Vector2 @endValue = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DG.Tweening.Core.DOSetter<UnityEngine.Vector2> @setter = (DG.Tweening.Core.DOSetter<UnityEngine.Vector2>)typeof(DG.Tweening.Core.DOSetter<UnityEngine.Vector2>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DG.Tweening.Core.DOGetter<UnityEngine.Vector2> @getter = (DG.Tweening.Core.DOGetter<UnityEngine.Vector2>)typeof(DG.Tweening.Core.DOGetter<UnityEngine.Vector2>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = DG.Tweening.DOTween.To(@getter, @setter, @endValue, @duration);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
