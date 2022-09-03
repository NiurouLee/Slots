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
    unsafe class APIManagerBridge_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(global::APIManagerBridge);
            args = new Type[]{typeof(Google.Protobuf.IMessage), typeof(System.Action<Google.Protobuf.IMessage>), typeof(System.Action<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.Protobuf.IMessage>)};
            method = type.GetMethod("Send", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Send_0);
            args = new Type[]{};
            method = type.GetMethod("ClearBridgeRequest", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearBridgeRequest_1);


        }


        static StackObject* Send_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.Protobuf.IMessage> @onError = (System.Action<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.Protobuf.IMessage>)typeof(System.Action<DragonU3DSDK.Network.API.Protocol.ErrorCode, System.String, Google.Protobuf.IMessage>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<Google.Protobuf.IMessage> @onResponse = (System.Action<Google.Protobuf.IMessage>)typeof(System.Action<Google.Protobuf.IMessage>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.Protobuf.IMessage @iMessage = (Google.Protobuf.IMessage)typeof(Google.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            global::APIManagerBridge.Send(@iMessage, @onResponse, @onError);

            return __ret;
        }

        static StackObject* ClearBridgeRequest_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            global::APIManagerBridge.ClearBridgeRequest();

            return __ret;
        }



    }
}
