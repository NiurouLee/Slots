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
    unsafe class Google_ilruntime_Protobuf_CodedOutputStream_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.CodedOutputStream);
            args = new Type[]{typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_0);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("WriteString", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteString_1);
            args = new Type[]{typeof(System.Byte), typeof(System.Byte)};
            method = type.GetMethod("WriteRawTag", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteRawTag_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteEnum", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteEnum_3);
            args = new Type[]{typeof(System.String)};
            method = type.GetMethod("ComputeStringSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeStringSize_4);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("ComputeEnumSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeEnumSize_5);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("WriteUInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt64_6);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("WriteBool", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteBool_7);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage)};
            method = type.GetMethod("WriteMessage", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteMessage_8);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("ComputeUInt64Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeUInt64Size_9);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.IMessage)};
            method = type.GetMethod("ComputeMessageSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeMessageSize_10);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("WriteUInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteUInt32_11);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("ComputeUInt32Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeUInt32Size_12);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("WriteInt64", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteInt64_13);
            args = new Type[]{typeof(System.Int64)};
            method = type.GetMethod("ComputeInt64Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeInt64Size_14);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("WriteFloat", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteFloat_15);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("WriteInt32", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteInt32_16);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("ComputeInt32Size", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeInt32Size_17);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.ByteString)};
            method = type.GetMethod("WriteBytes", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteBytes_18);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.ByteString)};
            method = type.GetMethod("ComputeBytesSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComputeBytesSize_19);


        }


        static StackObject* WriteRawTag_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte @b1 = (byte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(@b1);

            return __ret;
        }

        static StackObject* WriteString_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteString(@value);

            return __ret;
        }

        static StackObject* WriteRawTag_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte @b2 = (byte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Byte @b1 = (byte)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteRawTag(@b1, @b2);

            return __ret;
        }

        static StackObject* WriteEnum_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteEnum(@value);

            return __ret;
        }

        static StackObject* ComputeStringSize_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @value = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeStringSize(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ComputeEnumSize_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeEnumSize(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteUInt64_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt64(@value);

            return __ret;
        }

        static StackObject* WriteBool_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteBool(@value);

            return __ret;
        }

        static StackObject* WriteMessage_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.IMessage @value = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteMessage(@value);

            return __ret;
        }

        static StackObject* ComputeUInt64Size_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeUInt64Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* ComputeMessageSize_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.IMessage @value = (Google.ilruntime.Protobuf.IMessage)typeof(Google.ilruntime.Protobuf.IMessage).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeMessageSize(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteUInt32_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteUInt32(@value);

            return __ret;
        }

        static StackObject* ComputeUInt32Size_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 @value = (uint)ptr_of_this_method->Value;


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeUInt32Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteInt64_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @value = *(long*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteInt64(@value);

            return __ret;
        }

        static StackObject* ComputeInt64Size_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int64 @value = *(long*)&ptr_of_this_method->Value;


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeInt64Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteFloat_15(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteFloat(@value);

            return __ret;
        }

        static StackObject* WriteInt32_16(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteInt32(@value);

            return __ret;
        }

        static StackObject* ComputeInt32Size_17(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @value = ptr_of_this_method->Value;


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeInt32Size(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* WriteBytes_18(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.ByteString @value = (Google.ilruntime.Protobuf.ByteString)typeof(Google.ilruntime.Protobuf.ByteString).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream instance_of_this_method = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.WriteBytes(@value);

            return __ret;
        }

        static StackObject* ComputeBytesSize_19(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.ByteString @value = (Google.ilruntime.Protobuf.ByteString)typeof(Google.ilruntime.Protobuf.ByteString).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Google.ilruntime.Protobuf.CodedOutputStream.ComputeBytesSize(@value);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }



    }
}
