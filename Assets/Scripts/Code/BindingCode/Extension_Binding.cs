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
    unsafe class Extension_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::Extension);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("GetCommaFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaFormat_0);
            args = new Type[]{typeof(UnityEngine.UI.Text), typeof(System.String)};
            method = type.GetMethod("SetText", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetText_1);
            args = new Type[]{typeof(System.Object), typeof(System.Int32), typeof(System.Boolean), typeof(System.Func<System.Double, System.Double>)};
            method = type.GetMethod("GetAbbreviationFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAbbreviationFormat_2);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("GetCommaFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaFormat_3);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("GetCommaFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaFormat_4);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("IsNumeric", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsNumeric_5);
            args = new Type[]{typeof(UnityEngine.Animator), typeof(System.String), typeof(System.Int32)};
            method = type.GetMethod("HasState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HasState_6);
            args = new Type[]{typeof(System.Double)};
            method = type.GetMethod("GetCommaFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaFormat_7);
            args = new Type[]{typeof(UnityEngine.TextMesh), typeof(System.String)};
            method = type.GetMethod("SetText", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetText_8);
            args = new Type[]{typeof(System.Int64), typeof(System.Int32)};
            method = type.GetMethod("GetCommaOrSimplify", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaOrSimplify_9);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetCommaFormat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetCommaFormat_10);


        }


        static StackObject* GetCommaFormat_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @num = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaFormat(@num);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetText_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.Text @tex = (UnityEngine.UI.Text)typeof(UnityEngine.UI.Text).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::Extension.SetText(@tex, @text);

            return __ret;
        }

        static StackObject* GetAbbreviationFormat_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Func<System.Double, System.Double> @roundingFunction = (System.Func<System.Double, System.Double>)typeof(System.Func<System.Double, System.Double>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Boolean @needZeroPlaceHolder = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @decimalsNum = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Object @inNum = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::Extension.GetAbbreviationFormat(@inNum, @decimalsNum, @needZeroPlaceHolder, @roundingFunction);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetCommaFormat_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @num = (uint)ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaFormat(@num);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetCommaFormat_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @num = *(ulong*)&ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaFormat(@num);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsNumeric_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @obj = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::Extension.IsNumeric(@obj);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* HasState_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @layer = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @stateName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityEngine.Animator @anim = (UnityEngine.Animator)typeof(UnityEngine.Animator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = global::Extension.HasState(@anim, @stateName, @layer);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetCommaFormat_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @num = *(double*)&ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaFormat(@num);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* SetText_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @text = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.TextMesh @textMesh = (UnityEngine.TextMesh)typeof(UnityEngine.TextMesh).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::Extension.SetText(@textMesh, @text);

            return __ret;
        }

        static StackObject* GetCommaOrSimplify_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @length = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int64 @num = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaOrSimplify(@num, @length);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetCommaFormat_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @num = ptr_of_this_method->Value;


            var result_of_this_method = global::Extension.GetCommaFormat(@num);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
