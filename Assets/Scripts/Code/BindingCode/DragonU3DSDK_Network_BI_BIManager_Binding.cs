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
    unsafe class DragonU3DSDK_Network_BI_BIManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.BI.BIManager);
            args = new Type[]{typeof(System.Exception), typeof(System.Int32), typeof(System.String)};
            method = type.GetMethod("SendException", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendException_0);
            args = new Type[]{typeof(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType), typeof(System.String[])};
            method = type.GetMethod("SendCommonGameEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendCommonGameEvent_1);
            args = new Type[]{typeof(System.String), typeof(System.Byte[])};
            method = type.GetMethod("SendEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SendEvent_2);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("onThirdPartyTracking", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, onThirdPartyTracking_3);


        }


        static StackObject* SendException_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @protocol = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @depth = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Exception @e = (System.Exception)typeof(System.Exception).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DragonU3DSDK.Network.BI.BIManager instance_of_this_method = (DragonU3DSDK.Network.BI.BIManager)typeof(DragonU3DSDK.Network.BI.BIManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendException(@e, @depth, @protocol);

            return __ret;
        }

        static StackObject* SendCommonGameEvent_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String[] @args = (System.String[])typeof(System.String[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType @commonGameEventType = (DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType)typeof(DragonU3DSDK.Network.API.Protocol.BiEventCommon.Types.CommonGameEventType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.Network.BI.BIManager instance_of_this_method = (DragonU3DSDK.Network.BI.BIManager)typeof(DragonU3DSDK.Network.BI.BIManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendCommonGameEvent(@commonGameEventType, @args);

            return __ret;
        }

        static StackObject* SendEvent_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] @payload = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @payloadType = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.Network.BI.BIManager instance_of_this_method = (DragonU3DSDK.Network.BI.BIManager)typeof(DragonU3DSDK.Network.BI.BIManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SendEvent(@payloadType, @payload);

            return __ret;
        }

        static StackObject* onThirdPartyTracking_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @gameEventType = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.BI.BIManager instance_of_this_method = (DragonU3DSDK.Network.BI.BIManager)typeof(DragonU3DSDK.Network.BI.BIManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.onThirdPartyTracking(@gameEventType);

            return __ret;
        }



    }
}
