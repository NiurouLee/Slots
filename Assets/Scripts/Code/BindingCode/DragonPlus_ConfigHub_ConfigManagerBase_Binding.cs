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
    unsafe class DragonPlus_ConfigHub_ConfigManagerBase_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonPlus.ConfigHub.ConfigManagerBase);
            args = new Type[]{};
            method = type.GetMethod("Release", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Release_0);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_1);

            field = type.GetField("MetaData", flag);
            app.RegisterCLRFieldGetter(field, get_MetaData_0);
            app.RegisterCLRFieldSetter(field, set_MetaData_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_MetaData_0, AssignFromStack_MetaData_0);
            field = type.GetField("IsRemote", flag);
            app.RegisterCLRFieldGetter(field, get_IsRemote_1);
            app.RegisterCLRFieldSetter(field, set_IsRemote_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_IsRemote_1, AssignFromStack_IsRemote_1);


        }


        static StackObject* Release_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonPlus.ConfigHub.ConfigManagerBase instance_of_this_method = (DragonPlus.ConfigHub.ConfigManagerBase)typeof(DragonPlus.ConfigHub.ConfigManagerBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Release();

            return __ret;
        }

        static StackObject* Init_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonPlus.ConfigHub.ConfigManagerBase instance_of_this_method = (DragonPlus.ConfigHub.ConfigManagerBase)typeof(DragonPlus.ConfigHub.ConfigManagerBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init();

            return __ret;
        }


        static object get_MetaData_0(ref object o)
        {
            return ((DragonPlus.ConfigHub.ConfigManagerBase)o).MetaData;
        }

        static StackObject* CopyToStack_MetaData_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonPlus.ConfigHub.ConfigManagerBase)o).MetaData;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_MetaData_0(ref object o, object v)
        {
            ((DragonPlus.ConfigHub.ConfigManagerBase)o).MetaData = (DragonPlus.ConfigHub.MetaData)v;
        }

        static StackObject* AssignFromStack_MetaData_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            DragonPlus.ConfigHub.MetaData @MetaData = (DragonPlus.ConfigHub.MetaData)typeof(DragonPlus.ConfigHub.MetaData).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonPlus.ConfigHub.ConfigManagerBase)o).MetaData = @MetaData;
            return ptr_of_this_method;
        }

        static object get_IsRemote_1(ref object o)
        {
            return ((DragonPlus.ConfigHub.ConfigManagerBase)o).IsRemote;
        }

        static StackObject* CopyToStack_IsRemote_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonPlus.ConfigHub.ConfigManagerBase)o).IsRemote;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static void set_IsRemote_1(ref object o, object v)
        {
            ((DragonPlus.ConfigHub.ConfigManagerBase)o).IsRemote = (System.Boolean)v;
        }

        static StackObject* AssignFromStack_IsRemote_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Boolean @IsRemote = ptr_of_this_method->Value == 1;
            ((DragonPlus.ConfigHub.ConfigManagerBase)o).IsRemote = @IsRemote;
            return ptr_of_this_method;
        }



    }
}
