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
    unsafe class DragonU3DSDK_Network_API_Protocol_Mail_Binding_Types_Binding_MailInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo);
            args = new Type[]{};
            method = type.GetMethod("get_Title", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Title_0);
            args = new Type[]{};
            method = type.GetMethod("get_Message", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Message_1);


        }


        static StackObject* get_Title_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo)typeof(DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Title;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Message_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo instance_of_this_method = (DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo)typeof(DragonU3DSDK.Network.API.Protocol.Mail.Types.MailInfo).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Message;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
