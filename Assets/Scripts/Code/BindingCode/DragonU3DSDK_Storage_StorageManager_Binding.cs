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
    unsafe class DragonU3DSDK_Storage_StorageManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Storage.StorageManager);
            args = new Type[]{};
            method = type.GetMethod("SaveToLocal", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SaveToLocal_0);
            args = new Type[]{};
            method = type.GetMethod("get_LocalVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_LocalVersion_1);
            args = new Type[]{typeof(System.UInt64)};
            method = type.GetMethod("set_LocalVersion", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_LocalVersion_2);
            args = new Type[]{typeof(System.Boolean)};
            method = type.GetMethod("set_SyncForceRemote", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_SyncForceRemote_3);
            args = new Type[]{typeof(System.Action<System.Boolean>)};
            method = type.GetMethod("GetOrCreateProfile", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetOrCreateProfile_4);
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
            args = new Type[]{typeof(DragonU3DSDK.Storage.StorageCommon)};
            if (genericMethods.TryGetValue("GetStorage", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.Storage.StorageCommon)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetStorage_5);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(DragonU3DSDK.Storage.StorageAd)};
            if (genericMethods.TryGetValue("GetStorage", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.MatchGenericParameters(args, typeof(DragonU3DSDK.Storage.StorageAd)))
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, GetStorage_6);

                        break;
                    }
                }
            }


        }


        static StackObject* SaveToLocal_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SaveToLocal();

            return __ret;
        }

        static StackObject* get_LocalVersion_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.LocalVersion;

            __ret->ObjectType = ObjectTypes.Long;
            *(ulong*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* set_LocalVersion_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt64 @value = *(ulong*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.LocalVersion = value;

            return __ret;
        }

        static StackObject* set_SyncForceRemote_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @value = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.SyncForceRemote = value;

            return __ret;
        }

        static StackObject* GetOrCreateProfile_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean> @cb = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.GetOrCreateProfile(@cb);

            return __ret;
        }

        static StackObject* GetStorage_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetStorage<DragonU3DSDK.Storage.StorageCommon>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* GetStorage_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Storage.StorageManager instance_of_this_method = (DragonU3DSDK.Storage.StorageManager)typeof(DragonU3DSDK.Storage.StorageManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetStorage<DragonU3DSDK.Storage.StorageAd>();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
