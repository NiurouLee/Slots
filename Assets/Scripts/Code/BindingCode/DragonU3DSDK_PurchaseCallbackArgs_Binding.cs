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
    unsafe class DragonU3DSDK_PurchaseCallbackArgs_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonU3DSDK.PurchaseCallbackArgs);

            field = type.GetField("isReplenishmentOrder", flag);
            app.RegisterCLRFieldGetter(field, get_isReplenishmentOrder_0);
            app.RegisterCLRFieldSetter(field, set_isReplenishmentOrder_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_isReplenishmentOrder_0, AssignFromStack_isReplenishmentOrder_0);
            field = type.GetField("extraInfo", flag);
            app.RegisterCLRFieldGetter(field, get_extraInfo_1);
            app.RegisterCLRFieldSetter(field, set_extraInfo_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_extraInfo_1, AssignFromStack_extraInfo_1);
            field = type.GetField("productId", flag);
            app.RegisterCLRFieldGetter(field, get_productId_2);
            app.RegisterCLRFieldSetter(field, set_productId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_productId_2, AssignFromStack_productId_2);
            field = type.GetField("isSuccess", flag);
            app.RegisterCLRFieldGetter(field, get_isSuccess_3);
            app.RegisterCLRFieldSetter(field, set_isSuccess_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_isSuccess_3, AssignFromStack_isSuccess_3);
            field = type.GetField("failureReason", flag);
            app.RegisterCLRFieldGetter(field, get_failureReason_4);
            app.RegisterCLRFieldSetter(field, set_failureReason_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_failureReason_4, AssignFromStack_failureReason_4);

            app.RegisterCLRCreateDefaultInstance(type, () => new DragonU3DSDK.PurchaseCallbackArgs());


        }

        static void WriteBackInstance(ILRuntime.Runtime.Enviorment.AppDomain __domain, StackObject* ptr_of_this_method, IList<object> __mStack, ref DragonU3DSDK.PurchaseCallbackArgs instance_of_this_method)
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
                        var instance_of_arrayReference = __mStack[ptr_of_this_method->Value] as DragonU3DSDK.PurchaseCallbackArgs[];
                        instance_of_arrayReference[ptr_of_this_method->ValueLow] = instance_of_this_method;
                    }
                    break;
            }
        }


        static object get_isReplenishmentOrder_0(ref object o)
        {
            return ((DragonU3DSDK.PurchaseCallbackArgs)o).isReplenishmentOrder;
        }

        static StackObject* CopyToStack_isReplenishmentOrder_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.PurchaseCallbackArgs)o).isReplenishmentOrder;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isReplenishmentOrder_0(ref object o, object v)
        {
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.isReplenishmentOrder = (System.Boolean)v;
            o = ins;
        }

        static StackObject* AssignFromStack_isReplenishmentOrder_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isReplenishmentOrder = ptr_of_this_method->Value == 1;
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.isReplenishmentOrder = @isReplenishmentOrder;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_extraInfo_1(ref object o)
        {
            return ((DragonU3DSDK.PurchaseCallbackArgs)o).extraInfo;
        }

        static StackObject* CopyToStack_extraInfo_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.PurchaseCallbackArgs)o).extraInfo;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_extraInfo_1(ref object o, object v)
        {
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.extraInfo = (System.Byte[])v;
            o = ins;
        }

        static StackObject* AssignFromStack_extraInfo_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Byte[] @extraInfo = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.extraInfo = @extraInfo;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_productId_2(ref object o)
        {
            return ((DragonU3DSDK.PurchaseCallbackArgs)o).productId;
        }

        static StackObject* CopyToStack_productId_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.PurchaseCallbackArgs)o).productId;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_productId_2(ref object o, object v)
        {
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.productId = (System.String)v;
            o = ins;
        }

        static StackObject* AssignFromStack_productId_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.productId = @productId;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_isSuccess_3(ref object o)
        {
            return ((DragonU3DSDK.PurchaseCallbackArgs)o).isSuccess;
        }

        static StackObject* CopyToStack_isSuccess_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.PurchaseCallbackArgs)o).isSuccess;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_isSuccess_3(ref object o, object v)
        {
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.isSuccess = (System.Boolean)v;
            o = ins;
        }

        static StackObject* AssignFromStack_isSuccess_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @isSuccess = ptr_of_this_method->Value == 1;
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.isSuccess = @isSuccess;
            o = ins;
            return ptr_of_this_method;
        }

        static object get_failureReason_4(ref object o)
        {
            return ((DragonU3DSDK.PurchaseCallbackArgs)o).failureReason;
        }

        static StackObject* CopyToStack_failureReason_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.PurchaseCallbackArgs)o).failureReason;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_failureReason_4(ref object o, object v)
        {
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.failureReason = (UnityEngine.Purchasing.PurchaseFailureReason)v;
            o = ins;
        }

        static StackObject* AssignFromStack_failureReason_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            UnityEngine.Purchasing.PurchaseFailureReason @failureReason = (UnityEngine.Purchasing.PurchaseFailureReason)typeof(UnityEngine.Purchasing.PurchaseFailureReason).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            DragonU3DSDK.PurchaseCallbackArgs ins =(DragonU3DSDK.PurchaseCallbackArgs)o;
            ins.failureReason = @failureReason;
            o = ins;
            return ptr_of_this_method;
        }



    }
}
