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
    unsafe class DragonU3DSDK_Storage_StorageCommon_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Storage.StorageCommon);
            args = new Type[]{};
            method = type.GetMethod("get_PlayerId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_PlayerId_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_FacebookId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_FacebookId_1);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_AppleAccountId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AppleAccountId_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_DeviceId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_DeviceId_3);
            args = new Type[]{};
            method = type.GetMethod("get_AdsPredictUserGroup", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AdsPredictUserGroup_4);
            args = new Type[]{};
            method = type.GetMethod("get_RevenueUSDCents", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_RevenueUSDCents_5);
            args = new Type[]{};
            method = type.GetMethod("get_ResVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_ResVersion_6);


        }


        static StackObject* get_PlayerId_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.PlayerId;

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_FacebookId_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.FacebookId = value;

            return __ret;
        }

        static StackObject* set_AppleAccountId_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AppleAccountId = value;

            return __ret;
        }

        static StackObject* set_DeviceId_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DeviceId = value;

            return __ret;
        }

        static StackObject* get_AdsPredictUserGroup_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AdsPredictUserGroup;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_RevenueUSDCents_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.RevenueUSDCents;

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_ResVersion_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageCommon instance_of_this_method = (DragonU3DSDK.Storage.StorageCommon)typeof(DragonU3DSDK.Storage.StorageCommon).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.ResVersion;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
