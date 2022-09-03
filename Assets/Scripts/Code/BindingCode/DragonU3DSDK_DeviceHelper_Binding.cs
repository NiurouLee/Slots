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
    unsafe class DragonU3DSDK_DeviceHelper_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.DeviceHelper);
            args = new Type[]{};
            method = type.GetMethod("GetPlatform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetPlatform_0);
            args = new Type[]{};
            method = type.GetMethod("GetAppVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetAppVersion_1);
            args = new Type[]{};
            method = type.GetMethod("GetUserAgent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetUserAgent_2);
            args = new Type[]{};
            method = type.GetMethod("GetDeviceType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDeviceType_3);
            args = new Type[]{};
            method = type.GetMethod("GetDeviceModel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetDeviceModel_4);
            args = new Type[]{};
            method = type.GetMethod("GetTotalMemory", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetTotalMemory_5);
            args = new Type[]{};
            method = type.GetMethod("GetNetworkStatus", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetNetworkStatus_6);


        }


        static StackObject* GetPlatform_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetPlatform();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetAppVersion_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetAppVersion();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetUserAgent_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetUserAgent();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetDeviceType_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetDeviceType();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetDeviceModel_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetDeviceModel();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetTotalMemory_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetTotalMemory();

            __ret->ObjectType = ObjectTypes.Long;
            *(long*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* GetNetworkStatus_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = DragonU3DSDK.DeviceHelper.GetNetworkStatus();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
