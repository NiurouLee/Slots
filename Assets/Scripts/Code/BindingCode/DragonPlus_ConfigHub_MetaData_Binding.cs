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
    unsafe class DragonPlus_ConfigHub_MetaData_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonPlus.ConfigHub.MetaData);

            field = type.GetField("GroupId", flag);
            app.RegisterCLRFieldGetter(field, get_GroupId_0);
            app.RegisterCLRFieldSetter(field, set_GroupId_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_GroupId_0, AssignFromStack_GroupId_0);


        }



        static object get_GroupId_0(ref object o)
        {
            return ((DragonPlus.ConfigHub.MetaData)o).GroupId;
        }

        static StackObject* CopyToStack_GroupId_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonPlus.ConfigHub.MetaData)o).GroupId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_GroupId_0(ref object o, object v)
        {
            ((DragonPlus.ConfigHub.MetaData)o).GroupId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_GroupId_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @GroupId = ptr_of_this_method->Value;
            ((DragonPlus.ConfigHub.MetaData)o).GroupId = @GroupId;
            return ptr_of_this_method;
        }



    }
}
