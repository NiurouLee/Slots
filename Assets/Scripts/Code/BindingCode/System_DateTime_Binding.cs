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
    unsafe class System_DateTime_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(System.DateTime);
            args = new Type[]{};
            method = type.GetMethod("get_UtcNow", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_UtcNow_0);
            args = new Type[]{typeof(System.DateTime), typeof(System.DateTime)};
            method = type.GetMethod("op_Subtraction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, op_Subtraction_1);
            args = new Type[]{typeof(System.Double)};
            method = type.GetMethod("AddDays", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddDays_2);
            args = new Type[]{};
            method = type.GetMethod("get_Year", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Year_3);
            args = new Type[]{};
            method = type.GetMethod("get_Month", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Month_4);
            args = new Type[]{};
            method = type.GetMethod("get_Day", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Day_5);
            args = new Type[]{};
            method = type.GetMethod("ToUniversalTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ToUniversalTime_6);
            args = new Type[]{};
            method = type.GetMethod("get_Now", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Now_7);
            args = new Type[]{typeof(System.Double)};
            method = type.GetMethod("AddMilliseconds", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddMilliseconds_8);
            args = new Type[]{};
            method = type.GetMethod("ToString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ToString_9);

            app.RegisterCLRCreateDefaultInstance(type, () => new System.DateTime());

            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.DateTimeKind)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.DateTimeKind)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_2);
            args = new Type[]{typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32), typeof(System.Int32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_3);

        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref System.DateTime instance_of_this_method)
        {
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            switch(ptr_of_this_method->ObjectType)
            {
                case ObjectTypes.Object:
                    {
                        __mStack[ptr_of_this_method->Value] = instance_of_this_method;
                    }
                    break;
                case ObjectTypes.FieldReference:
                    {
                        var ___obj = __mStack[ptr_of_this_method->Value];
                        if(___obj is ILTypeInstance)
                        {
                            ((ILTypeInstance)___obj)[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            var t = __domain.GetType(___obj.GetType()) as CLRType;
                            t.SetFieldValue(ptr_of_this_method->ValueLow, ref ___obj, instance_of_this_method);
                        }
                    }
                    break;
                case ObjectTypes.StaticFieldReference:
                    {
                        var t = __domain.GetType(ptr_of_this_method->Value);
                        if(t is ILType)
                        {
                            ((ILType)t).StaticInstance[ptr_of_this_method->ValueLow] = instance_of_this_method;
                        }
                        else
                        {
                            ((CLRType)t).SetStaticFieldValue(ptr_of_this_method->ValueLow, instance_of_this_method);
                        }
                    }
                    break;
                 case ObjectTypes.ArrayReference:
                    {
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as System.DateTime[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }

        static StackObject* get_UtcNow_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = System.DateTime.UtcNow;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* op_Subtraction_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.DateTime @d2 = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.DateTime @d1 = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = d1 - d2;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddDays_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @value = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.AddDays(@value);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Year_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.Year;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Month_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.Month;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_Day_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.Day;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ToUniversalTime_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.ToUniversalTime();

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Now_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = System.DateTime.Now;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddMilliseconds_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Double @value = *(double*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.AddMilliseconds(@value);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ToString_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.GetObjectAndResolveReference(ptr_of_this_method);
            System.DateTime instance_of_this_method = (System.DateTime)typeof(System.DateTime).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));

            var result_of_this_method = instance_of_this_method.ToString();

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            WriteBackInstance(__domain, ptr_of_this_method, __mStack, ref instance_of_this_method);

            __intp.Free(ptr_of_this_method);
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 7);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.DateTimeKind @kind = (System.DateTimeKind)typeof(System.DateTimeKind).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @second = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @minute = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @hour = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Int32 @day = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            System.Int32 @month = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            System.Int32 @year = ptr_of_this_method->Value;


            var result_of_this_method = new System.DateTime(@year, @month, @day, @hour, @minute, @second, @kind);

            if(!isNewObj)
            {
                __ret--;
                WriteBackInstance(__domain, __ret, __mStack, ref result_of_this_method);
                return __ret;
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Ctor_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 8);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.DateTimeKind @kind = (System.DateTimeKind)typeof(System.DateTimeKind).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @millisecond = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @second = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @minute = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Int32 @hour = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            System.Int32 @day = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            System.Int32 @month = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 8);
            System.Int32 @year = ptr_of_this_method->Value;


            var result_of_this_method = new System.DateTime(@year, @month, @day, @hour, @minute, @second, @millisecond, @kind);

            if(!isNewObj)
            {
                __ret--;
                WriteBackInstance(__domain, __ret, __mStack, ref result_of_this_method);
                return __ret;
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Ctor_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @day = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @month = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @year = ptr_of_this_method->Value;


            var result_of_this_method = new System.DateTime(@year, @month, @day);

            if(!isNewObj)
            {
                __ret--;
                WriteBackInstance(__domain, __ret, __mStack, ref result_of_this_method);
                return __ret;
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Ctor_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 7);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @millisecond = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @second = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @minute = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @hour = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.Int32 @day = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            System.Int32 @month = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 7);
            System.Int32 @year = ptr_of_this_method->Value;


            var result_of_this_method = new System.DateTime(@year, @month, @day, @hour, @minute, @second, @millisecond);

            if(!isNewObj)
            {
                __ret--;
                WriteBackInstance(__domain, __ret, __mStack, ref result_of_this_method);
                return __ret;
            }

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
