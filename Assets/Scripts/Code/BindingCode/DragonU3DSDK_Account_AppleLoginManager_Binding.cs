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
    unsafe class DragonU3DSDK_Account_AppleLoginManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Account.AppleLoginManager);
            args = new Type[]{};
            method = type.GetMethod("IsLoggedIn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsLoggedIn_0);
            args = new Type[]{typeof(System.Action<System.String, Dlugin.PluginStructs.UserInfo, Dlugin.PluginStructs.SDKError>)};
            method = type.GetMethod("LogOut", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LogOut_1);


        }


        static StackObject* IsLoggedIn_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AppleLoginManager instance_of_this_method = (DragonU3DSDK.Account.AppleLoginManager)typeof(DragonU3DSDK.Account.AppleLoginManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsLoggedIn();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* LogOut_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.String, Dlugin.PluginStructs.UserInfo, Dlugin.PluginStructs.SDKError> @callback = (System.Action<System.String, Dlugin.PluginStructs.UserInfo, Dlugin.PluginStructs.SDKError>)typeof(System.Action<System.String, Dlugin.PluginStructs.UserInfo, Dlugin.PluginStructs.SDKError>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Account.AppleLoginManager instance_of_this_method = (DragonU3DSDK.Account.AppleLoginManager)typeof(DragonU3DSDK.Account.AppleLoginManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LogOut(@callback);

            return __ret;
        }



    }
}
