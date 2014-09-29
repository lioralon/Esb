using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy
{
    public abstract class BaseProxyEmitter :IProxyEmitter
    {
        private bool saveFile = false;
        private string assemblyName = "DynamicProxyAssembly";
        private string theFileName;        
        private AssemblyBuilder dynamicAssembly;              
        protected ModuleBuilder moduleBuilder;
        private IDictionary<string, Type> proxyTypeDictionary = new Dictionary<string, Type>();//DynamicTypeName -- DynamicType

        #region Ctor
        public BaseProxyEmitter() :this(false)
        {
        }

        public BaseProxyEmitter(bool save)
        {
            this.saveFile = save;
            this.theFileName = this.assemblyName + ".dll";

            AssemblyBuilderAccess assemblyBuilderAccess = this.saveFile ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            this.dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(this.assemblyName), assemblyBuilderAccess);
            if (this.saveFile)
            {
                this.moduleBuilder = dynamicAssembly.DefineDynamicModule("MainModule", this.theFileName);
            }
            else
            {
                this.moduleBuilder = dynamicAssembly.DefineDynamicModule("MainModule");
            }
        } 
        #endregion             

        #region EmitProxyType
        public Type EmitProxyType(Type interfaceType, Type originType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new Exception("TInterface must be interface type !");
            }

            if (interfaceType.ContainsGenericParameters)
            {
                throw new Exception("TInterface can't be generic !");
            }

            string dynamicTypeName = this.GetDynamicTypeName(interfaceType, originType);
            if (this.proxyTypeDictionary.ContainsKey(dynamicTypeName))
            {
                return this.proxyTypeDictionary[dynamicTypeName];
            }

            Type target = this.DoEmitProxyType(interfaceType, originType);
            this.proxyTypeDictionary.Add(dynamicTypeName, target);

            return target;
        } 

        public Type EmitProxyType<TInterface>(Type originType)
        {
            Type interfaceType = typeof(TInterface);
            return this.EmitProxyType(interfaceType, originType);
        } 
        #endregion

        #region GetDynamicTypeName
        /// <summary>
        /// GetDynamicTypeName 获取要动态生成的类型的名称。注意，子类一定要使用本方法来得到动态类型的名称。
        /// </summary>    
        protected string GetDynamicTypeName(Type interfaceType, Type originType)
        {
            string postfix = this.GetPostfixOfDynamicTypeName();
            string typeName = string.Format("{0}.{1}_{2}_{3}", this.assemblyName ,originType.ToString(), interfaceType.ToString(), postfix);
            return typeName;
        }
        #endregion

        #region Save
        public void Save()
        {
            if (this.saveFile)
            {
                this.dynamicAssembly.Save(theFileName);
            }
        }
        #endregion

        /// <summary>
        /// GetPostfixOfDynamicTypeName 获取要动态生成的类型名称的后缀
        /// </summary>  
        protected abstract string GetPostfixOfDynamicTypeName();

        protected abstract Type DoEmitProxyType(Type interfaceType, Type originType);             
    }    
}
