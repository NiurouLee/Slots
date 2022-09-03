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
    unsafe class DragonU3DSDK_Network_API_APIEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.API.APIEntry);

            field = type.GetField("uri", flag);
            app.RegisterCLRFieldGetter(field, get_uri_0);
            app.RegisterCLRFieldSetter(field, set_uri_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_uri_0, AssignFromStack_uri_0);
            field = type.GetField("method", flag);
            app.RegisterCLRFieldGetter(field, get_method_1);
            app.RegisterCLRFieldSetter(field, set_method_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_method_1, AssignFromStack_method_1);
            field = type.GetField("scheme", flag);
            app.RegisterCLRFieldGetter(field, get_scheme_2);
            app.RegisterCLRFieldSetter(field, set_scheme_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_scheme_2, AssignFromStack_scheme_2);
            field = type.GetField("timeout", flag);
            app.RegisterCLRFieldGetter(field, get_timeout_3);
            app.RegisterCLRFieldSetter(field, set_timeout_3);
            app.RegisterCLRFieldBinding(field, CopyToStack_timeout_3, AssignFromStack_timeout_3);
            field = type.GetField("gzip", flag);
            app.RegisterCLRFieldGetter(field, get_gzip_4);
            app.RegisterCLRFieldSetter(field, set_gzip_4);
            app.RegisterCLRFieldBinding(field, CopyToStack_gzip_4, AssignFromStack_gzip_4);
            field = type.GetField("ignoreAuth", flag);
            app.RegisterCLRFieldGetter(field, get_ignoreAuth_5);
            app.RegisterCLRFieldSetter(field, set_ignoreAuth_5);
            app.RegisterCLRFieldBinding(field, CopyToStack_ignoreAuth_5, AssignFromStack_ignoreAuth_5);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static object get_uri_0(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).uri;
        }

        static StackObject* CopyToStack_uri_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).uri;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_uri_0(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).uri = (System.String)v;
        }

        static StackObject* AssignFromStack_uri_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @uri = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.Network.API.APIEntry)o).uri = @uri;
            return ptr_of_this_method;
        }

        static object get_method_1(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).method;
        }

        static StackObject* CopyToStack_method_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).method;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_method_1(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).method = (System.String)v;
        }

        static StackObject* AssignFromStack_method_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @method = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.Network.API.APIEntry)o).method = @method;
            return ptr_of_this_method;
        }

        static object get_scheme_2(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).scheme;
        }

        static StackObject* CopyToStack_scheme_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).scheme;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_scheme_2(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).scheme = (System.String)v;
        }

        static StackObject* AssignFromStack_scheme_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @scheme = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.Network.API.APIEntry)o).scheme = @scheme;
            return ptr_of_this_method;
        }

        static object get_timeout_3(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).timeout;
        }

        static StackObject* CopyToStack_timeout_3(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).timeout;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_timeout_3(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).timeout = (System.Int32)v;
        }

        static StackObject* AssignFromStack_timeout_3(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @timeout = ptr_of_this_method->Value;
            ((DragonU3DSDK.Network.API.APIEntry)o).timeout = @timeout;
            return ptr_of_this_method;
        }

        static object get_gzip_4(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).gzip;
        }

        static StackObject* CopyToStack_gzip_4(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).gzip;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_gzip_4(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).gzip = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_gzip_4(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @gzip = ptr_of_this_method->Value == 1;
            ((DragonU3DSDK.Network.API.APIEntry)o).gzip = @gzip;
            return ptr_of_this_method;
        }

        static object get_ignoreAuth_5(ref object o)
        {
            return ((DragonU3DSDK.Network.API.APIEntry)o).ignoreAuth;
        }

        static StackObject* CopyToStack_ignoreAuth_5(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Network.API.APIEntry)o).ignoreAuth;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_ignoreAuth_5(ref object o, object v)
        {
            ((DragonU3DSDK.Network.API.APIEntry)o).ignoreAuth = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_ignoreAuth_5(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @ignoreAuth = ptr_of_this_method->Value == 1;
            ((DragonU3DSDK.Network.API.APIEntry)o).ignoreAuth = @ignoreAuth;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new DragonU3DSDK.Network.API.APIEntry();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
