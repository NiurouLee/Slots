using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntimeAdapters
{   
    public class ConfigManagerBaseAdapter : CrossBindingAdaptor
    {
      
        public override Type BaseCLRType
        {
            get
            {
                return typeof(DragonPlus.ConfigHub.ConfigManagerBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : DragonPlus.ConfigHub.ConfigManagerBase, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            protected CrossBindingFunctionInfo<System.String> mget_Guid_0 = new CrossBindingFunctionInfo<System.String>("get_Guid");
            protected CrossBindingFunctionInfo<System.Int32> mget_VersionMinIOS_1 = new CrossBindingFunctionInfo<System.Int32>("get_VersionMinIOS");
            protected CrossBindingFunctionInfo<System.Int32> mget_VersionMinAndroid_2 = new CrossBindingFunctionInfo<System.Int32>("get_VersionMinAndroid");
            protected CrossBindingFunctionInfo<System.Collections.Generic.List<System.String>> mget_SubModules_3 = new CrossBindingFunctionInfo<System.Collections.Generic.List<System.String>>("get_SubModules");
            protected CrossBindingFunctionInfo<System.Collections.Hashtable, System.Boolean> mCheckTable_4 = new CrossBindingFunctionInfo<System.Collections.Hashtable, System.Boolean>("CheckTable");
            protected CrossBindingFunctionInfo<System.Int32, System.Boolean> mHasGroup_5 = new CrossBindingFunctionInfo<System.Int32, System.Boolean>("HasGroup");
            //static CrossBindingFunctionInfo<DragonPlus.ConfigHub.CacheOperate, System.Int64, System.Collections.Generic.List<DragonPlus.ConfigHub.ConfigManagerBase.T>> mGetConfig_6 = new CrossBindingFunctionInfo<DragonPlus.ConfigHub.CacheOperate, System.Int64, System.Collections.Generic.List<DragonPlus.ConfigHub.ConfigManagerBase.T>>("GetConfig");
            protected CrossBindingMethodInfo<DragonPlus.ConfigHub.MetaData, System.String> mInitConfig_7 = new CrossBindingMethodInfo<DragonPlus.ConfigHub.MetaData, System.String>("InitConfig");
            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            protected override System.Boolean CheckTable(System.Collections.Hashtable table)
            {
                return mCheckTable_4.Invoke(this.instance, table);
            }

            protected override System.Boolean HasGroup(System.Int32 groupId)
            {
                return mHasGroup_5.Invoke(this.instance, groupId);
            }

            public override System.Collections.Generic.List<T> GetConfig<T>(DragonPlus.ConfigHub.CacheOperate cacheOp, System.Int64 cacheDuration)
            {
                return null;
            }

            public override void InitConfig(DragonPlus.ConfigHub.MetaData metaData, System.String jsonData)
            {
                mInitConfig_7.Invoke(this.instance, metaData, jsonData);
            }

            public override System.String Guid
            {
            get
            {
                return mget_Guid_0.Invoke(this.instance);

            }
            }

            public override System.Int32 VersionMinIOS
            {
            get
            {
                return mget_VersionMinIOS_1.Invoke(this.instance);

            }
            }

            public override System.Int32 VersionMinAndroid
            {
            get
            {
                return mget_VersionMinAndroid_2.Invoke(this.instance);

            }
            }

            protected override System.Collections.Generic.List<System.String> SubModules
            {
            get
            {
                return mget_SubModules_3.Invoke(this.instance);

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

