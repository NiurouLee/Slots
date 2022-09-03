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
    unsafe class BestHTTP_SocketIO_SocketOptions_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(BestHTTP.SocketIO.SocketOptions);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_AutoConnect", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AutoConnect_0);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_Reconnection", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Reconnection_1);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("set_ReconnectionAttempts", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ReconnectionAttempts_2);
            args = new Type[]{typeof(BestHTTP.SocketIO.Transports.TransportTypes)};
            method = type.GetMethod("set_ConnectWith", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_ConnectWith_3);
            args = new Type[]{typeof(PlatformSupport.Collections.ObjectModel.ObservableDictionary<System.String, System.String>)};
            method = type.GetMethod("set_AdditionalQueryParams", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_AdditionalQueryParams_4);
            args = new Type[]{};
            method = type.GetMethod("get_AdditionalQueryParams", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_AdditionalQueryParams_5);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* set_AutoConnect_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AutoConnect = value;

            return __ret;
        }

        static StackObject* set_Reconnection_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Reconnection = value;

            return __ret;
        }

        static StackObject* set_ReconnectionAttempts_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ReconnectionAttempts = value;

            return __ret;
        }

        static StackObject* set_ConnectWith_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            BestHTTP.SocketIO.Transports.TransportTypes @value = (BestHTTP.SocketIO.Transports.TransportTypes)typeof(BestHTTP.SocketIO.Transports.TransportTypes).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ConnectWith = value;

            return __ret;
        }

        static StackObject* set_AdditionalQueryParams_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            PlatformSupport.Collections.ObjectModel.ObservableDictionary<System.String, System.String> @value = (PlatformSupport.Collections.ObjectModel.ObservableDictionary<System.String, System.String>)typeof(PlatformSupport.Collections.ObjectModel.ObservableDictionary<System.String, System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AdditionalQueryParams = value;

            return __ret;
        }

        static StackObject* get_AdditionalQueryParams_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            BestHTTP.SocketIO.SocketOptions instance_of_this_method = (BestHTTP.SocketIO.SocketOptions)typeof(BestHTTP.SocketIO.SocketOptions).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.AdditionalQueryParams;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new BestHTTP.SocketIO.SocketOptions();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
