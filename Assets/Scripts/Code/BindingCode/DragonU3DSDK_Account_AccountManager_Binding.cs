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
    unsafe class DragonU3DSDK_Account_AccountManager_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(DragonU3DSDK.Account.AccountManager);
            args = new Type[]{};
            method = type.GetMethod("Init", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Init_0);
            args = new Type[]{};
            method = type.GetMethod("get_HasLogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_HasLogin_1);
            args = new Type[]{};
            method = type.GetMethod("HasBindFacebook", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HasBindFacebook_2);
            args = new Type[]{};
            method = type.GetMethod("HasBindApple", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HasBindApple_3);
            args = new Type[]{};
            method = type.GetMethod("get_RefreshToken", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_RefreshToken_4);
            args = new Type[]{typeof(System.String), typeof(System.Action<System.Boolean>)};
            method = type.GetMethod("Relogin", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Relogin_5);
            args = new Type[]{typeof(System.Action<System.Boolean>), typeof(System.Action), typeof(System.Boolean)};
            method = type.GetMethod("BindFacebook", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindFacebook_6);
            args = new Type[]{typeof(System.Action<System.Boolean>), typeof(System.Action), typeof(System.Boolean)};
            method = type.GetMethod("BindApple", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, BindApple_7);
            args = new Type[]{typeof(System.Action<System.Boolean>), typeof(System.Action)};
            method = type.GetMethod("Login", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Login_8);
            args = new Type[]{};
            method = type.GetMethod("get_Token", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Token_9);
            args = new Type[]{};
            method = type.GetMethod("OnTokenExpire", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnTokenExpire_10);
            args = new Type[]{};
            method = type.GetMethod("OnRefreshTokenExpire", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, OnRefreshTokenExpire_11);
            args = new Type[]{};
            method = type.GetMethod("GetFacebookId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetFacebookId_12);
            args = new Type[]{};
            method = type.GetMethod("Clear", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Clear_13);

            field = type.GetField("loginStatus", flag);
            app.RegisterCLRFieldGetter(field, get_loginStatus_0);
            app.RegisterCLRFieldSetter(field, set_loginStatus_0);
            app.RegisterCLRFieldBinding(field, CopyToStack_loginStatus_0, AssignFromStack_loginStatus_0);


        }


        static StackObject* Init_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Init();

            return __ret;
        }

        static StackObject* get_HasLogin_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HasLogin;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* HasBindFacebook_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HasBindFacebook();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* HasBindApple_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.HasBindApple();

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* get_RefreshToken_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.RefreshToken;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Relogin_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<System.Boolean> @callback = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @refreshToken = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Relogin(@refreshToken, @callback);

            return __ret;
        }

        static StackObject* BindFacebook_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @postEventInAlreadyBinded = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action @cancelCallback = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<System.Boolean> @callback = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindFacebook(@callback, @cancelCallback, @postEventInAlreadyBinded);

            return __ret;
        }

        static StackObject* BindApple_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @postEventInAlreadyBinded = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action @cancelCallback = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<System.Boolean> @callback = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.BindApple(@callback, @cancelCallback, @postEventInAlreadyBinded);

            return __ret;
        }

        static StackObject* Login_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action @cancelCallback = (System.Action)typeof(System.Action).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<System.Boolean> @callback = (System.Action<System.Boolean>)typeof(System.Action<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Login(@callback, @cancelCallback);

            return __ret;
        }

        static StackObject* get_Token_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.Token;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* OnTokenExpire_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnTokenExpire();

            return __ret;
        }

        static StackObject* OnRefreshTokenExpire_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.OnRefreshTokenExpire();

            return __ret;
        }

        static StackObject* GetFacebookId_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetFacebookId();

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* Clear_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            DragonU3DSDK.Account.AccountManager instance_of_this_method = (DragonU3DSDK.Account.AccountManager)typeof(DragonU3DSDK.Account.AccountManager).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Clear();

            return __ret;
        }


        static object get_loginStatus_0(ref object o)
        {
            return ((DragonU3DSDK.Account.AccountManager)o).loginStatus;
        }

        static StackObject* CopyToStack_loginStatus_0(ref object o, ILIntepreter __intp, StackObject* __ret, IList<object> __mStack)
        {
            var result_of_this_method = ((DragonU3DSDK.Account.AccountManager)o).loginStatus;
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static void set_loginStatus_0(ref object o, object v)
        {
            ((DragonU3DSDK.Account.AccountManager)o).loginStatus = (DragonU3DSDK.Account.LoginStatus)v;
        }

        static StackObject* AssignFromStack_loginStatus_0(ref object o, ILIntepreter __intp, StackObject* ptr_of_this_method, IList<object> __mStack)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            DragonU3DSDK.Account.LoginStatus @loginStatus = (DragonU3DSDK.Account.LoginStatus)typeof(DragonU3DSDK.Account.LoginStatus).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            ((DragonU3DSDK.Account.AccountManager)o).loginStatus = @loginStatus;
            return ptr_of_this_method;
        }



    }
}
