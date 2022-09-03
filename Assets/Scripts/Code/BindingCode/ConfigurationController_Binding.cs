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
    unsafe class ConfigurationController_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(global::ConfigurationController);
            args = new Type[]{};
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[]{};
            method = type.GetMethod("get_APIServerSecret", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_APIServerSecret_1);
            args = new Type[]{};
            method = type.GetMethod("get_APIServerURL", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_APIServerURL_2);
            args = new Type[]{};
            method = type.GetMethod("get_SocketIOEnabled", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_SocketIOEnabled_3);
            args = new Type[]{};
            method = type.GetMethod("get_APIServerTimeout", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_APIServerTimeout_4);

            field = type.GetField("PrivacyPolicyURL", flag);
            app.RegisterCLRFieldGetter(field, get_PrivacyPolicyURL_0);
            app.RegisterCLRFieldSetter(field, set_PrivacyPolicyURL_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_PrivacyPolicyURL_0, AssignFromStack_PrivacyPolicyURL_0);
            field = type.GetField("API_Server_URL_Beta", flag);
            app.RegisterCLRFieldGetter(field, get_API_Server_URL_Beta_1);
            app.RegisterCLRFieldSetter(field, set_API_Server_URL_Beta_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_API_Server_URL_Beta_1, AssignFromStack_API_Server_URL_Beta_1);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);


            var result_of_this_method = global::ConfigurationController.Instance;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_APIServerSecret_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ConfigurationController instance_of_this_method = (global::ConfigurationController)typeof(global::ConfigurationController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.APIServerSecret;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_APIServerURL_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ConfigurationController instance_of_this_method = (global::ConfigurationController)typeof(global::ConfigurationController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.APIServerURL;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_SocketIOEnabled_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ConfigurationController instance_of_this_method = (global::ConfigurationController)typeof(global::ConfigurationController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.SocketIOEnabled;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_APIServerTimeout_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::ConfigurationController instance_of_this_method = (global::ConfigurationController)typeof(global::ConfigurationController).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.APIServerTimeout;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }


        static object get_PrivacyPolicyURL_0(ref object o)
        {
            return ((global::ConfigurationController)o).PrivacyPolicyURL;
        }

        static StackObject* CopyToStack_PrivacyPolicyURL_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ConfigurationController)o).PrivacyPolicyURL;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_PrivacyPolicyURL_0(ref object o, object v)
        {
            ((global::ConfigurationController)o).PrivacyPolicyURL = (System.String)v;
        }

        static StackObject* AssignFromStack_PrivacyPolicyURL_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @PrivacyPolicyURL = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ConfigurationController)o).PrivacyPolicyURL = @PrivacyPolicyURL;
            return ptr_of_this_method;
        }

        static object get_API_Server_URL_Beta_1(ref object o)
        {
            return ((global::ConfigurationController)o).API_Server_URL_Beta;
        }

        static StackObject* CopyToStack_API_Server_URL_Beta_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((global::ConfigurationController)o).API_Server_URL_Beta;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_API_Server_URL_Beta_1(ref object o, object v)
        {
            ((global::ConfigurationController)o).API_Server_URL_Beta = (System.String)v;
        }

        static StackObject* AssignFromStack_API_Server_URL_Beta_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @API_Server_URL_Beta = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((global::ConfigurationController)o).API_Server_URL_Beta = @API_Server_URL_Beta;
            return ptr_of_this_method;
        }



    }
}
