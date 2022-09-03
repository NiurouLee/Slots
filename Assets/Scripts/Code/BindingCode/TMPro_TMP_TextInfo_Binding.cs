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
    unsafe class TMPro_TMP_TextInfo_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(TMPro.TMP_TextInfo);

            field = type.GetField("linkInfo", flag);
            app.RegisterCLRFieldGetter(field, get_linkInfo_0);
            app.RegisterCLRFieldSetter(field, set_linkInfo_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_linkInfo_0, AssignFromStack_linkInfo_0);


        }



        static object get_linkInfo_0(ref object o)
        {
            return ((TMPro.TMP_TextInfo)o).linkInfo;
        }

        static StackObject* CopyToStack_linkInfo_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((TMPro.TMP_TextInfo)o).linkInfo;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_linkInfo_0(ref object o, object v)
        {
            ((TMPro.TMP_TextInfo)o).linkInfo = (TMPro.TMP_LinkInfo[])v;
        }

        static StackObject* AssignFromStack_linkInfo_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            TMPro.TMP_LinkInfo[] @linkInfo = (TMPro.TMP_LinkInfo[])typeof(TMPro.TMP_LinkInfo[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((TMPro.TMP_TextInfo)o).linkInfo = @linkInfo;
            return ptr_of_this_method;
        }



    }
}
