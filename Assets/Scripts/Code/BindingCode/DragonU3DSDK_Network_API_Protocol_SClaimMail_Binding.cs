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
    unsafe class DragonU3DSDK_Network_API_Protocol_SClaimMail_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.API.Protocol.SClaimMail);
            args = new Type[]{};
            method = type.GetMethod("get_Content", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Content_0);


        }


        static StackObject* get_Content_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.SClaimMail instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.SClaimMail)typeof(DragonU3DSDK.Network.API.Protocol.SClaimMail).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Content;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
