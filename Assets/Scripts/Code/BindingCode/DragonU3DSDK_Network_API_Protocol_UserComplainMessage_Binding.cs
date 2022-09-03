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
    unsafe class DragonU3DSDK_Network_API_Protocol_UserComplainMessage_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_Message", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Message_0);
            args = new Type[]{typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage.Types.MessageType)};
            method = type.GetMethod("set_MessageType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_MessageType_1);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("set_PlayerId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_PlayerId_2);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("set_CreatedAt", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_CreatedAt_3);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_Email", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Email_4);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("set_RevenueUsdCents", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_RevenueUsdCents_5);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("set_LastLevel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_LastLevel_6);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_DeviceType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_DeviceType_7);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_DeviceModel", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_DeviceModel_8);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_DeviceMemory", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_DeviceMemory_9);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_NetworkType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NetworkType_10);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_ResVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ResVersion_11);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("set_NativeVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_NativeVersion_12);
            args = new Type[]{typeof(DragonU3DSDK.Network.API.Protocol.Platform)};
            method = type.GetMethod("set_Platform", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Platform_13);
            args = new Type[]{};
            method = type.GetMethod("get_Message", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Message_14);
            args = new Type[]{};
            method = type.GetMethod("get_CreatedAt", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_CreatedAt_15);
            args = new Type[]{};
            method = type.GetMethod("get_MessageType", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_MessageType_16);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* set_Message_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Message = value;

            return __ret;
        }

        static StackObject* set_MessageType_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage.Types.MessageType @value = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage.Types.MessageType)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage.Types.MessageType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.MessageType = value;

            return __ret;
        }

        static StackObject* set_PlayerId_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.PlayerId = value;

            return __ret;
        }

        static StackObject* set_CreatedAt_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CreatedAt = value;

            return __ret;
        }

        static StackObject* set_Email_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Email = value;

            return __ret;
        }

        static StackObject* set_RevenueUsdCents_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RevenueUsdCents = value;

            return __ret;
        }

        static StackObject* set_LastLevel_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LastLevel = value;

            return __ret;
        }

        static StackObject* set_DeviceType_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DeviceType = value;

            return __ret;
        }

        static StackObject* set_DeviceModel_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DeviceModel = value;

            return __ret;
        }

        static StackObject* set_DeviceMemory_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DeviceMemory = value;

            return __ret;
        }

        static StackObject* set_NetworkType_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NetworkType = value;

            return __ret;
        }

        static StackObject* set_ResVersion_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ResVersion = value;

            return __ret;
        }

        static StackObject* set_NativeVersion_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.NativeVersion = value;

            return __ret;
        }

        static StackObject* set_Platform_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.Platform @value = (DragonU3DSDK.Network.API.Protocol.Platform)typeof(DragonU3DSDK.Network.API.Protocol.Platform).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Platform = value;

            return __ret;
        }

        static StackObject* get_Message_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Message;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_CreatedAt_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CreatedAt;

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* get_MessageType_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.UserComplainMessage instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.UserComplainMessage)typeof(DragonU3DSDK.Network.API.Protocol.UserComplainMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.MessageType;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new DragonU3DSDK.Network.API.Protocol.UserComplainMessage();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
