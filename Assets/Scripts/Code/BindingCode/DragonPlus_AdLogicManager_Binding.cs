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
    unsafe class DragonPlus_AdLogicManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonPlus.AdLogicManager);
            args = new Type[]{typeof(Dlugin.AD_Type), typeof(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonMonetizationAdEventFailedReason), typeof(DragonPlus.AdLogicManager.FailDelegate)};
            method = type.GetMethod("RegisterFailDelegate", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RegisterFailDelegate_0);
            args = new Type[]{typeof(System.String), typeof(System.Boolean)};
            method = type.GetMethod("ShouldShowRV", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShouldShowRV_1);
            args = new Type[]{typeof(System.String), typeof(System.Boolean)};
            method = type.GetMethod("ShouldShowInterstitial", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ShouldShowInterstitial_2);
            args = new Type[]{typeof(System.String), typeof(System.Action<System.Boolean, System.String>)};
            method = type.GetMethod("TryShowInterstitialInternal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, TryShowInterstitialInternal_3);
            args = new Type[]{typeof(System.String), typeof(System.Action<System.Boolean, System.String>)};
            method = type.GetMethod("TryShowRewardedVideoInternal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, TryShowRewardedVideoInternal_4);
            args = new Type[]{};
            method = type.GetMethod("get_specialOrder", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_specialOrder_5);


        }


        static StackObject* RegisterFailDelegate_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonPlus.AdLogicManager.FailDelegate @fail = (DragonPlus.AdLogicManager.FailDelegate)typeof(DragonPlus.AdLogicManager.FailDelegate).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonMonetizationAdEventFailedReason @reason = (DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonMonetizationAdEventFailedReason)typeof(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonMonetizationAdEventFailedReason).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Dlugin.AD_Type @type = (Dlugin.AD_Type)typeof(Dlugin.AD_Type).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RegisterFailDelegate(@type, @reason, @fail);

            return __ret;
        }

        static StackObject* ShouldShowRV_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @withBI = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @placementId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ShouldShowRV(@placementId, @withBI);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* ShouldShowInterstitial_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @withBI = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @placementId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ShouldShowInterstitial(@placementId, @withBI);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* TryShowInterstitialInternal_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean, System.String> @cb = (System.Action<System.Boolean, System.String>)typeof(System.Action<System.Boolean, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @placementId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TryShowInterstitialInternal(@placementId, @cb);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* TryShowRewardedVideoInternal_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean, System.String> @cb = (System.Action<System.Boolean, System.String>)typeof(System.Action<System.Boolean, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @placementId = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.TryShowRewardedVideoInternal(@placementId, @cb);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_specialOrder_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonPlus.AdLogicManager instance_of_this_method = (DragonPlus.AdLogicManager)typeof(DragonPlus.AdLogicManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.specialOrder;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
