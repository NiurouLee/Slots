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
    unsafe class DG_Tweening_Tween_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DG.Tweening.Tween);

            field = type.GetField("onComplete", flag);
            app.RegisterCLRFieldGetter(field, get_onComplete_0);
            app.RegisterCLRFieldSetter(field, set_onComplete_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_onComplete_0, AssignFromStack_onComplete_0);
            field = type.GetField("target", flag);
            app.RegisterCLRFieldGetter(field, get_target_1);
            app.RegisterCLRFieldSetter(field, set_target_1);
            app.RegisterCLRFieldBinding(field, CopyToStack_target_1, AssignFromStack_target_1);
            field = type.GetField("intId", flag);
            app.RegisterCLRFieldGetter(field, get_intId_2);
            app.RegisterCLRFieldSetter(field, set_intId_2);
            app.RegisterCLRFieldBinding(field, CopyToStack_intId_2, AssignFromStack_intId_2);


        }



        static object get_onComplete_0(ref object o)
        {
            return ((DG.Tweening.Tween)o).onComplete;
        }

        static StackObject* CopyToStack_onComplete_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DG.Tweening.Tween)o).onComplete;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_onComplete_0(ref object o, object v)
        {
            ((DG.Tweening.Tween)o).onComplete = (DG.Tweening.TweenCallback)v;
        }

        static StackObject* AssignFromStack_onComplete_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            DG.Tweening.TweenCallback @onComplete = (DG.Tweening.TweenCallback)typeof(DG.Tweening.TweenCallback).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DG.Tweening.Tween)o).onComplete = @onComplete;
            return ptr_of_this_method;
        }

        static object get_target_1(ref object o)
        {
            return ((DG.Tweening.Tween)o).target;
        }

        static StackObject* CopyToStack_target_1(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DG.Tweening.Tween)o).target;
            object obj_result_of_this_method = result_of_this_method;
            if(obj_result_of_this_method is CrossBindingAdaptorType)
            {    
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance, true);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method, true);
        }

        static void set_target_1(ref object o, object v)
        {
            ((DG.Tweening.Tween)o).target = (System.Object)v;
        }

        static StackObject* AssignFromStack_target_1(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Object @target = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DG.Tweening.Tween)o).target = @target;
            return ptr_of_this_method;
        }

        static object get_intId_2(ref object o)
        {
            return ((DG.Tweening.Tween)o).intId;
        }

        static StackObject* CopyToStack_intId_2(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DG.Tweening.Tween)o).intId;
            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static void set_intId_2(ref object o, object v)
        {
            ((DG.Tweening.Tween)o).intId = (System.Int32)v;
        }

        static StackObject* AssignFromStack_intId_2(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            System.Int32 @intId = ptr_of_this_method->Value;
            ((DG.Tweening.Tween)o).intId = @intId;
            return ptr_of_this_method;
        }



    }
}
