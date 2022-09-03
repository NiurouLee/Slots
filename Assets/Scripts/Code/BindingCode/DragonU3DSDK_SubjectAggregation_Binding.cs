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
    unsafe class DragonU3DSDK_SubjectAggregation_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.SubjectAggregation);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(DragonU3DSDK.SDKEvents.SocketIOConnected)};
            if (genericMethods.TryGetValue("Trigger", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.SDKEvents.SocketIOConnected)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, Trigger_0);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DragonU3DSDK.SDKEvents.SocketIODisconnected)};
            if (genericMethods.TryGetValue("Trigger", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.SDKEvents.SocketIODisconnected)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, Trigger_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DragonU3DSDK.SDKEvents.SocketIOError)};
            if (genericMethods.TryGetValue("Trigger", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.SDKEvents.SocketIOError)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, Trigger_2);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent)};
            if (genericMethods.TryGetValue("Subscribe", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.IEventHandler<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>), typeof(System.Action<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, Subscribe_3);

                        break;
                    }
                }
            }


        }


        static StackObject* Trigger_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.SubjectAggregation instance_of_this_method = (DragonU3DSDK.SubjectAggregation)typeof(DragonU3DSDK.SubjectAggregation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Trigger<DragonU3DSDK.SDKEvents.SocketIOConnected>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Trigger_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.SubjectAggregation instance_of_this_method = (DragonU3DSDK.SubjectAggregation)typeof(DragonU3DSDK.SubjectAggregation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Trigger<DragonU3DSDK.SDKEvents.SocketIODisconnected>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Trigger_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.SubjectAggregation instance_of_this_method = (DragonU3DSDK.SubjectAggregation)typeof(DragonU3DSDK.SubjectAggregation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Trigger<DragonU3DSDK.SDKEvents.SocketIOError>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Subscribe_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent> @func = (System.Action<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>)typeof(System.Action<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.SubjectAggregation instance_of_this_method = (DragonU3DSDK.SubjectAggregation)typeof(DragonU3DSDK.SubjectAggregation).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Subscribe<DragonU3DSDK.SDKEvents.ThirdAlreadyBindErrorEvent>(@func);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
