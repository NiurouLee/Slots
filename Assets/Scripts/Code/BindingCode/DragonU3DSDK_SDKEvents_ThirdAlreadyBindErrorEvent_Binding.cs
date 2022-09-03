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
    unsafe class DragonU3DSDK_SDKEvents_ThirdAlreadyBindErrorEvent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent);

            field = type.GetField("type", flag);
            app.RegisterCLRFieldGetter(field, get_type_0);
            app.RegisterCLRFieldSetter(field, set_type_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_type_0, AssignFromStack_type_0);
            field = type.GetField("token", flag);
            app.RegisterCLRFieldGetter(field, get_token_1);
            app.RegisterCLRFieldSetter(field, set_token_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_token_1, AssignFromStack_token_1);


        }



        static object get_type_0(ref object o)
        {
            return ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).type;
        }

        static StackObject* CopyToStack_type_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).type;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_type_0(ref object o, object v)
        {
            ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).type = (System.String)v;
        }

        static StackObject* AssignFromStack_type_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @type = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).type = @type;
            return ptr_of_this_method;
        }

        static object get_token_1(ref object o)
        {
            return ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).token;
        }

        static StackObject* CopyToStack_token_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).token;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_token_1(ref object o, object v)
        {
            ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).token = (System.String)v;
        }

        static StackObject* AssignFromStack_token_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.String @token = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)o).token = @token;
            return ptr_of_this_method;
        }



    }
}
