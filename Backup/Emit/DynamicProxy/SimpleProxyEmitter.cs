using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// SimpleProxyEmitter 生成的动态代理类型将继承TInterface接口，并且动态类型有一个类型为originType构造参数。
    /// 动态类型针对TInterface接口的所有实现将转发给从Ctor传入的originType实例完成。
    /// 注意：originType 必须是public修饰的
    /// 注意：TInterface只支持非泛型接口，但接口中可以包括泛型方法。
    /// zhuweisky 最后一次整理：2007.08.02
    /// </summary>
    public class SimpleProxyEmitter :BaseProxyEmitter
    {
        public SimpleProxyEmitter() : base() { }
        public SimpleProxyEmitter(bool save) : base(save) { }

        protected override string GetPostfixOfDynamicTypeName()
        {
            return "SimpleProxy";
        }

        #region EmitProxyType     
        protected override Type DoEmitProxyType(Type interfaceType ,Type originType)
        {         
            string typeName = base.GetDynamicTypeName(interfaceType, originType);
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.SetParent(typeof(MarshalByRefObject));
            typeBuilder.AddInterfaceImplementation(interfaceType);

            //定义成员，用于保存传入originType实例。
            FieldBuilder targetField = typeBuilder.DefineField("target", originType, FieldAttributes.Private);            

            #region Emit Ctor
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType });
            ILGenerator ctorGen = ctor.GetILGenerator();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { }));
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_1);
            ctorGen.Emit(OpCodes.Stfld, targetField);
            ctorGen.Emit(OpCodes.Ret);
            #endregion
           
            foreach (MethodInfo baseMethod in ESBasic.Helpers.ReflectionHelper.GetAllMethods(interfaceType))//interfaceType.GetMethods())
            {
                this.EmitMethod(originType, typeBuilder, baseMethod, targetField);
            }           

            Type target = typeBuilder.CreateType();

            return target;
        }
        #endregion

        #region EmitMethod
        private void EmitMethod(Type originType, TypeBuilder typeBuilder, MethodInfo baseMethod, FieldBuilder targetField)
        {
            Type[] argTypes = EmitHelper.GetParametersType(baseMethod);
            MethodInfo originMethod = ReflectionHelper.SearchMethod(originType, baseMethod.Name, argTypes);
            MethodBuilder methodBuilder = EmitHelper.DefineDerivedMethodSignature(typeBuilder, baseMethod);
             
            ILGenerator methodGen = methodBuilder.GetILGenerator();
           
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, targetField);

            int coventIndex = 0;
            foreach (ParameterInfo pi in originMethod.GetParameters())
            {
                EmitHelper.LoadArg(methodGen, coventIndex + 1);
                EmitHelper.ConvertTopArgType(methodGen, argTypes[coventIndex], pi.ParameterType);
                coventIndex++;
            }           

            methodGen.Emit(OpCodes.Callvirt, originMethod);           
            methodGen.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        } 
        #endregion      
    }

    
}
