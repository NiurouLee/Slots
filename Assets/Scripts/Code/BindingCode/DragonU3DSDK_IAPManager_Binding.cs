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
    unsafe class DragonU3DSDK_IAPManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.IAPManager);
            args = new Type[]{};
            method = type.GetMethod("ResetIapState", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ResetIapState_0);
            args = new Type[]{};
            method = type.GetMethod("IsInitialized", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsInitialized_1);
            args = new Type[]{};
            method = type.GetMethod("GetAllProductInfo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAllProductInfo_2);
            args = new Type[]{typeof(System.String), typeof(System.Action<System.Boolean, System.Byte[]>)};
            method = type.GetMethod("FulfillPaymentWithServer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, FulfillPaymentWithServer_3);
            args = new Type[]{typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>), typeof(System.String)};
            method = type.GetMethod("RequestSpecifiedUnfulfilledPayment", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RequestSpecifiedUnfulfilledPayment_4);
            args = new Type[]{typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<System.String>), typeof(System.Collections.Generic.List<System.String>), typeof(System.Boolean)};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_5);
            args = new Type[]{typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>), typeof(System.String)};
            method = type.GetMethod("VerifyUnfulfilledPayment", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, VerifyUnfulfilledPayment_6);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("IsProductAlreadyOwned", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsProductAlreadyOwned_7);
           // args = new Type[]{typeof(System.String), typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>), typeof(System.String), typeof(System.String)};
           // method = type.GetMethod("SandBoxPurchaseProduct", flag, null, args, null);
           // app.RegisterCLRMethodRedirection(method, SandBoxPurchaseProduct_8);
            args = new Type[]{typeof(System.String), typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>), typeof(System.String), typeof(System.String)};
            method = type.GetMethod("PurchaseProduct", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PurchaseProduct_9);


        }


        static StackObject* ResetIapState_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResetIapState();

            return __ret;
        }

        static StackObject* IsInitialized_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsInitialized();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetAllProductInfo_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetAllProductInfo();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* FulfillPaymentWithServer_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean, System.Byte[]> @cb = (System.Action<System.Boolean, System.Byte[]>)typeof(System.Action<System.Boolean, System.Byte[]>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.FulfillPaymentWithServer(@productId, @cb);

            return __ret;
        }

        static StackObject* RequestSpecifiedUnfulfilledPayment_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @checkProductId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<DragonU3DSDK.PurchaseCallbackArgs> @cb = (System.Action<DragonU3DSDK.PurchaseCallbackArgs>)typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RequestSpecifiedUnfulfilledPayment(@cb, @checkProductId);

            return __ret;
        }

        static StackObject* Init_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @autoFulfillPayment = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Collections.Generic.List<System.String> @subProductIds = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Collections.Generic.List<System.String> @nonconsumableProductIds = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Collections.Generic.List<System.String> @consumableProductIds = (System.Collections.Generic.List<System.String>)typeof(System.Collections.Generic.List<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init(@consumableProductIds, @nonconsumableProductIds, @subProductIds, @autoFulfillPayment);

            return __ret;
        }

        static StackObject* VerifyUnfulfilledPayment_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<DragonU3DSDK.PurchaseCallbackArgs> @cb = (System.Action<DragonU3DSDK.PurchaseCallbackArgs>)typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.VerifyUnfulfilledPayment(@cb, @productId);

            return __ret;
        }

        static StackObject* IsProductAlreadyOwned_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsProductAlreadyOwned(@productId);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        // static StackObject* SandBoxPurchaseProduct_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        // {
        //     ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
        //     StackObject* ptr_of_this_method;
        //     StackObject* __ret = ILIntepreter.Minus(__esp, 5);
        //
        //     ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
        //     System.String @userData = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //     __intp.Free(ptr_of_this_method);
        //
        //     ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
        //     System.String @attribution = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //     __intp.Free(ptr_of_this_method);
        //
        //     ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
        //     System.Action<DragonU3DSDK.PurchaseCallbackArgs> @cb = (System.Action<DragonU3DSDK.PurchaseCallbackArgs>)typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //     __intp.Free(ptr_of_this_method);
        //
        //     ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
        //     System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //     __intp.Free(ptr_of_this_method);
        //
        //     ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
        //     DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
        //     __intp.Free(ptr_of_this_method);
        //
        //     instance_of_this_method.SandBoxPurchaseProduct(@productId, @cb, @attribution, @userData);
        //
        //     return __ret;
        // }

        static StackObject* PurchaseProduct_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @userData = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @attribution = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<DragonU3DSDK.PurchaseCallbackArgs> @cb = (System.Action<DragonU3DSDK.PurchaseCallbackArgs>)typeof(System.Action<DragonU3DSDK.PurchaseCallbackArgs>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.String @productId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            DragonU3DSDK.IAPManager instance_of_this_method = (DragonU3DSDK.IAPManager)typeof(DragonU3DSDK.IAPManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PurchaseProduct(@productId, @cb, @attribution, @userData);

            return __ret;
        }



    }
}
