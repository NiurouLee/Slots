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
    unsafe class Google_ilruntime_Protobuf_Collections_MapField_2_UInt32_Adapt_IMessage_Binding_Adaptor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.CodedOutputStream), typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)};
            method = type.GetMethod("WriteTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteTo_0);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)};
            method = type.GetMethod("CalculateSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalculateSize_1);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.CodedInputStream), typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)};
            method = type.GetMethod("AddEntriesFrom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddEntriesFrom_2);

            args = new Type[]{};
            method = type.GetConstructor(flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Ctor_0);

        }


        static StackObject* WriteTo_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec @codec = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream @output = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteTo(@output, @codec);

            return __ret;
        }

        static StackObject* CalculateSize_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec @codec = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.CalculateSize(@codec);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* AddEntriesFrom_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec @codec = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>.Codec).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedInputStream @input = (Google.ilruntime.Protobuf.CodedInputStream)typeof(Google.ilruntime.Protobuf.CodedInputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddEntriesFrom(@input, @codec);

            return __ret;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Google.ilruntime.Protobuf.Collections.MapField<System.UInt32, global::Adapt_IMessage.Adaptor>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
