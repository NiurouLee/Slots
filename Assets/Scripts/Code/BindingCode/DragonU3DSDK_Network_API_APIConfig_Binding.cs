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
    unsafe class DragonU3DSDK_Network_API_APIConfig_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.API.APIConfig);

            field = type.GetField("APIEntries", flag);
            app.RegisterCLRFieldGetter(field, get_APIEntries_0);
            app.RegisterCLRFieldSetter(field, set_APIEntries_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_APIEntries_0, AssignFromStack_APIEntries_0);


        }



        static object get_APIEntries_0(ref object o)
        {
            return DragonU3DSDK.Network.API.APIConfig.APIEntries;
        }

        static StackObject* CopyToStack_APIEntries_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = DragonU3DSDK.Network.API.APIConfig.APIEntries;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_APIEntries_0(ref object o, object v)
        {
            DragonU3DSDK.Network.API.APIConfig.APIEntries = (System.Collections.Generic.Dictionary<System.String, DragonU3DSDK.Network.API.APIEntry>)v;
        }

        static StackObject* AssignFromStack_APIEntries_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Collections.Generic.Dictionary<System.String, DragonU3DSDK.Network.API.APIEntry> @APIEntries = (System.Collections.Generic.Dictionary<System.String, DragonU3DSDK.Network.API.APIEntry>)typeof(System.Collections.Generic.Dictionary<System.String, DragonU3DSDK.Network.API.APIEntry>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            DragonU3DSDK.Network.API.APIConfig.APIEntries = @APIEntries;
            return ptr_of_this_method;
        }



    }
}
