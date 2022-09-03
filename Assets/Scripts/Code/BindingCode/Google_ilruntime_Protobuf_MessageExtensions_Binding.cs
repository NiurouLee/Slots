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
    unsafe class Google_ilruntime_Protobuf_MessageExtensions_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.MessageExtensions);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage), typeof(Google.ilruntime.Protobuf.ByteString)};
            method = type.GetMethod("MergeFrom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MergeFrom_0);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage)};
            method = type.GetMethod("ToByteString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ToByteString_1);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage)};
            method = type.GetMethod("ToByteArray", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ToByteArray_2);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage), typeof(System.Byte[])};
            method = type.GetMethod("MergeFrom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, MergeFrom_3);


        }


        static StackObject* MergeFrom_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.ByteString @data = (Google.ilruntime.Protobuf.ByteString)typeof(Google.ilruntime.Protobuf.ByteString).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.IMessage @message = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Google.ilruntime.Protobuf.MessageExtensions.MergeFrom(@message, @data);

            return __ret;
        }

        static StackObject* ToByteString_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.IMessage @message = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.ilruntime.Protobuf.MessageExtensions.ToByteString(@message);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* ToByteArray_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.IMessage @message = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.ilruntime.Protobuf.MessageExtensions.ToByteArray(@message);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* MergeFrom_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] @data = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.IMessage @message = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Google.ilruntime.Protobuf.MessageExtensions.MergeFrom(@message, @data);

            return __ret;
        }



    }
}
