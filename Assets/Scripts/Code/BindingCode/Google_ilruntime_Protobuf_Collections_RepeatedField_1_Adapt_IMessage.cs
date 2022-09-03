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
    unsafe class Google_ilruntime_Protobuf_Collections_RepeatedField_1_Adapt_IMessage_Binding_Adaptor_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.CodedOutputStream), typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)};
            method = type.GetMethod("WriteTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, WriteTo_0);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)};
            method = type.GetMethod("CalculateSize", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CalculateSize_1);
            args = new Type[]{typeof(Google.ilruntime.Protobuf.CodedInputStream), typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)};
            method = type.GetMethod("AddEntriesFrom", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddEntriesFrom_2);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("get_Item", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Item_3);
            args = new Type[]{};
            method = type.GetMethod("get_Count", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Count_4);
            args = new Type[]{typeof(global::Adapt_IMessage.Adaptor)};
            method = type.GetMethod("Add", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Add_5);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("RemoveAt", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, RemoveAt_6);
            args = new Type[]{};
            method = type.GetMethod("GetEnumerator", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetEnumerator_7);
            args = new Type[]{typeof(global::Adapt_IMessage.Adaptor[]), typeof(System.Int32)};
            method = type.GetMethod("CopyTo", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, CopyTo_8);
            args = new Type[]{typeof(System.Collections.Generic.IEnumerable<global::Adapt_IMessage.Adaptor>)};
            method = type.GetMethod("AddRange", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddRange_9);
            args = new Type[]{typeof(System.Int32), typeof(global::Adapt_IMessage.Adaptor)};
            method = type.GetMethod("set_Item", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_Item_10);

            field = type.GetField("count", flag);
            app.RegisterCLRFieldGetter(field, get_count_0);
            app.RegisterCLRFieldSetter(field, set_count_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_count_0, AssignFromStack_count_0);
            field = type.GetField("array", flag);
            app.RegisterCLRFieldGetter(field, get_array_1);
            app.RegisterCLRFieldSetter(field, set_array_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_array_1, AssignFromStack_array_1);

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
            Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor> @codec = (Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedOutputStream @output = (Google.ilruntime.Protobuf.CodedOutputStream)typeof(Google.ilruntime.Protobuf.CodedOutputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
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
            Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor> @codec = (Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
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
            Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor> @codec = (Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.FieldCodec<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.CodedInputStream @input = (Google.ilruntime.Protobuf.CodedInputStream)typeof(Google.ilruntime.Protobuf.CodedInputStream).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddEntriesFrom(@input, @codec);

            return __ret;
        }

        static StackObject* get_Item_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method[index];

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* get_Count_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Count;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Add_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::Adapt_IMessage.Adaptor @item = (global::Adapt_IMessage.Adaptor)typeof(global::Adapt_IMessage.Adaptor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Add(@item);

            return __ret;
        }

        static StackObject* RemoveAt_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.RemoveAt(@index);

            return __ret;
        }

        static StackObject* GetEnumerator_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetEnumerator();

            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* CopyTo_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @arrayIndex = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            global::Adapt_IMessage.Adaptor[] @array = (global::Adapt_IMessage.Adaptor[])typeof(global::Adapt_IMessage.Adaptor[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.CopyTo(@array, @arrayIndex);

            return __ret;
        }

        static StackObject* AddRange_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.Generic.IEnumerable<global::Adapt_IMessage.Adaptor> @values = (System.Collections.Generic.IEnumerable<global::Adapt_IMessage.Adaptor>)typeof(System.Collections.Generic.IEnumerable<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddRange(@values);

            return __ret;
        }

        static StackObject* set_Item_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            global::Adapt_IMessage.Adaptor @value = (global::Adapt_IMessage.Adaptor)typeof(global::Adapt_IMessage.Adaptor).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @index = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor> instance_of_this_method = (Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)typeof(Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method[index] = value;

            return __ret;
        }


        static object get_count_0(ref object o)
        {
            return ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).count;
        }

        static StackObject* CopyToStack_count_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).count;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_count_0(ref object o, object v)
        {
            ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).count = (System.Int32)v;
        }

        static StackObject* AssignFromStack_count_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @count = ptr_of_this_method->Value;
            ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).count = @count;
            return ptr_of_this_method;
        }

        static object get_array_1(ref object o)
        {
            return ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).array;
        }

        static StackObject* CopyToStack_array_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).array;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_array_1(ref object o, object v)
        {
            ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).array = (global::Adapt_IMessage.Adaptor[])v;
        }

        static StackObject* AssignFromStack_array_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            global::Adapt_IMessage.Adaptor[] @array = (global::Adapt_IMessage.Adaptor[])typeof(global::Adapt_IMessage.Adaptor[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>)o).array = @array;
            return ptr_of_this_method;
        }


        static StackObject* Ctor_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = new Google.ilruntime.Protobuf.Collections.RepeatedField<global::Adapt_IMessage.Adaptor>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }


    }
}
