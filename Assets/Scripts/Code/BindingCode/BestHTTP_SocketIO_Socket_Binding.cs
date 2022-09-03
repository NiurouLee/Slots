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
    unsafe class BestHTTP_SocketIO_Socket_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(BestHTTP.SocketIO.Socket);
            args = new Type[]{};
            method = type.GetMethod("get_IsOpen", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_IsOpen_0);
            args = new Type[]{typeof(BestHTTP.SocketIO.SocketIOEventTypes), typeof(BestHTTP.SocketIO.Events.SocketIOCallback)};
            method = type.GetMethod("On", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, On_1);
            args = new Type[]{typeof(System.String), typeof(BestHTTP.SocketIO.Events.SocketIOCallback)};
            method = type.GetMethod("On", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, On_2);
            args = new Type[]{typeof(System.String), typeof(BestHTTP.SocketIO.Events.SocketIOAckCallback), typeof(System.Object[])};
            method = type.GetMethod("Emit", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Emit_3);


        }


        static StackObject* get_IsOpen_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            BestHTTP.SocketIO.Socket instance_of_this_method = (BestHTTP.SocketIO.Socket)typeof(BestHTTP.SocketIO.Socket).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.IsOpen;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* On_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            BestHTTP.SocketIO.Events.SocketIOCallback @callback = (BestHTTP.SocketIO.Events.SocketIOCallback)typeof(BestHTTP.SocketIO.Events.SocketIOCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketIOEventTypes @type = (BestHTTP.SocketIO.SocketIOEventTypes)typeof(BestHTTP.SocketIO.SocketIOEventTypes).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            BestHTTP.SocketIO.Socket instance_of_this_method = (BestHTTP.SocketIO.Socket)typeof(BestHTTP.SocketIO.Socket).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.On(@type, @callback);

            return __ret;
        }

        static StackObject* On_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            BestHTTP.SocketIO.Events.SocketIOCallback @callback = (BestHTTP.SocketIO.Events.SocketIOCallback)typeof(BestHTTP.SocketIO.Events.SocketIOCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @eventName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            BestHTTP.SocketIO.Socket instance_of_this_method = (BestHTTP.SocketIO.Socket)typeof(BestHTTP.SocketIO.Socket).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.On(@eventName, @callback);

            return __ret;
        }

        static StackObject* Emit_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object[] @args = (System.Object[])typeof(System.Object[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.Events.SocketIOAckCallback @callback = (BestHTTP.SocketIO.Events.SocketIOAckCallback)typeof(BestHTTP.SocketIO.Events.SocketIOAckCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.String @eventName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            BestHTTP.SocketIO.Socket instance_of_this_method = (BestHTTP.SocketIO.Socket)typeof(BestHTTP.SocketIO.Socket).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Emit(@eventName, @callback, @args);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
