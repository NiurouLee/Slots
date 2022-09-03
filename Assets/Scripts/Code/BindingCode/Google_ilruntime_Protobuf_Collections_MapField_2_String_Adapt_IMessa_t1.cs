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
    unsafe class Google_ilruntime_Protobuf_Collections_MapField_2_String_Adapt_IMessage_Binding_Adaptor_Binding_Codec_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.Collections.MapField<System.String, global::Adapt_IMessage.Adaptor>.Codec);

            args = new Type[]{typeof(Google.ilruntime.Protobuf.FieldCodec<System.String>), typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>), typeof(System.UInt32)};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }



        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @mapTag = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor> @valueCodec = (Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.FieldCodec<System.String> @keyCodec = (Google.ilruntime.Protobuf.FieldCodec<System.String>)typeof(Google.ilruntime.Protobuf.FieldCodec<System.String>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new Google.ilruntime.Protobuf.Collections.MapField<System.String, global::Adapt_IMessage.Adaptor>.Codec(@keyCodec, @valueCodec, @mapTag);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
