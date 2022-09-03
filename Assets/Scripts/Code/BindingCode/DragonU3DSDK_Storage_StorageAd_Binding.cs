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
    unsafe class DragonU3DSDK_Storage_StorageAd_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Storage.StorageAd);
            args = new Type[]{};
            method = type.GetMethod("get_AdVideoWatchCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AdVideoWatchCount_0);
            args = new Type[]{};
            method = type.GetMethod("get_AdResetTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AdResetTime_1);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("set_AdResetTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AdResetTime_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_AdSettingWatchCount", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AdSettingWatchCount_3);
            args = new Type[]{};
            method = type.GetMethod("get_AdVideoWatchTime", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AdVideoWatchTime_4);


        }


        static StackObject* get_AdVideoWatchCount_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageAd instance_of_this_method = (DragonU3DSDK.Storage.StorageAd)typeof(DragonU3DSDK.Storage.StorageAd).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AdVideoWatchCount;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_AdResetTime_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageAd instance_of_this_method = (DragonU3DSDK.Storage.StorageAd)typeof(DragonU3DSDK.Storage.StorageAd).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AdResetTime;

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_AdResetTime_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @value = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageAd instance_of_this_method = (DragonU3DSDK.Storage.StorageAd)typeof(DragonU3DSDK.Storage.StorageAd).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AdResetTime = value;

            return __ret;
        }

        static StackObject* set_AdSettingWatchCount_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageAd instance_of_this_method = (DragonU3DSDK.Storage.StorageAd)typeof(DragonU3DSDK.Storage.StorageAd).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AdSettingWatchCount = value;

            return __ret;
        }

        static StackObject* get_AdVideoWatchTime_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageAd instance_of_this_method = (DragonU3DSDK.Storage.StorageAd)typeof(DragonU3DSDK.Storage.StorageAd).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AdVideoWatchTime;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
