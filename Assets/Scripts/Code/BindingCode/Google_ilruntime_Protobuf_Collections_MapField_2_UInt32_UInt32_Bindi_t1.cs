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
    unsafe class Google_ilruntime_Protobuf_Collections_MapField_2_UInt32_UInt32_Binding_Codec_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, System.UInt32>.Codec);

            args = new Type[]{typeof(Google.ilruntime.Protobuf.FieldCodec<System.UInt32>), typeof(Google.ilruntime.Protobuf.FieldCodec<System.UInt32>), typeof(System.UInt32)};
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
            Google.ilruntime.Protobuf.FieldCodec<System.UInt32> @valueCodec = (Google.ilruntime.Protobuf.FieldCodec<System.UInt32>)typeof(Google.ilruntime.Protobuf.FieldCodec<System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.FieldCodec<System.UInt32> @keyCodec = (Google.ilruntime.Protobuf.FieldCodec<System.UInt32>)typeof(Google.ilruntime.Protobuf.FieldCodec<System.UInt32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = new Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, System.UInt32>.Codec(@keyCodec, @valueCodec, @mapTag);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
