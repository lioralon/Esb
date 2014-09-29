using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy.AopSimple
{
    /// <summary>
    /// SimpleProxyEmitter 生成的动态代理类型将继承TInterface接口，并且动态类型有一个类型为originType构造参数。 
    /// 动态类型针对TInterface接口的所有实现将转发给从Ctor传入的originType实例完成。
    /// 注意：originType 必须是public修饰的
    /// 注意：TInterface只支持非泛型接口，但接口中可以包括泛型方法。
    /// 与AopProxyEmitter相比，主要是SimpleProxyEmitter生成的动态代理是直接调用被截获的方法的，而AopProxyEmitter生成的动态代理是通过反射调用被截获的方法的。
    /// zhuweisky 最后一次整理：2007.08.02
    /// </summary>
    public class SimpleAopProxyEmitter :BaseProxyEmitter
    {
        public SimpleAopProxyEmitter() : base() { }
        public SimpleAopProxyEmitter(bool save) : base(save) { }

        protected override string GetPostfixOfDynamicTypeName()
        {
            return "SimpleAopProxy";
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
            FieldBuilder aopInterceptorField = typeBuilder.DefineField("aopInterceptor", typeof(IAopInterceptor), FieldAttributes.Private);

            #region Emit Ctor
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType, typeof(IAopInterceptor) });
            ILGenerator ctorGen = ctor.GetILGenerator();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { })); //调用基类的构造函数
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_1);
            ctorGen.Emit(OpCodes.Stfld, targetField);

            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_2);
            ctorGen.Emit(OpCodes.Stfld, aopInterceptorField);
           
            ctorGen.Emit(OpCodes.Ret);
            #endregion
           
            foreach (MethodInfo baseMethod in ESBasic.Helpers.ReflectionHelper.GetAllMethods(interfaceType))//interfaceType.GetMethods())
            {
                this.EmitMethod(originType, typeBuilder, baseMethod, targetField, aopInterceptorField);
            }           

            Type target = typeBuilder.CreateType();

            return target;
        }
        #endregion

        #region EmitMethod
        private void EmitMethod(Type originType, TypeBuilder typeBuilder, MethodInfo baseMethod, FieldBuilder targetField ,FieldBuilder aopInterceptorField)
        {
            Type[] argTypes = EmitHelper.GetParametersType(baseMethod);
            MethodInfo originMethod = ReflectionHelper.SearchMethod(originType, baseMethod.Name, argTypes);
            MethodBuilder methodBuilder = EmitHelper.DefineDerivedMethodSignature(typeBuilder, baseMethod);
             
            ILGenerator methodGen = methodBuilder.GetILGenerator();

            ParameterInfo[] paras = originMethod.GetParameters();
            //调用 object[] paras = new object[] {a ,b };   
            LocalBuilder paramsLocalBuilder = methodGen.DeclareLocal(typeof(object[]));
            methodGen.Emit(OpCodes.Ldc_I4, paras.Length);
            methodGen.Emit(OpCodes.Newarr, typeof(object));
            methodGen.Emit(OpCodes.Stloc, paramsLocalBuilder);
            for (int i = 0; i < paras.Length; i++)
            {
                methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
                methodGen.Emit(OpCodes.Ldc_I4, i);
                methodGen.Emit(OpCodes.Ldarg, i + 1);

                if (paras[i].ParameterType.IsByRef) //如果是ref/out参数，则去地址
                {
                    EmitHelper.Ldind(methodGen, paras[i].ParameterType);
                    methodGen.Emit(OpCodes.Box, paras[i].ParameterType.GetElementType());  
                }
                else if (paras[i].ParameterType.IsValueType)
                {
                    methodGen.Emit(OpCodes.Box, paras[i].ParameterType);
                }
                else
                {
                }

                methodGen.Emit(OpCodes.Stelem_Ref);
            }

            //调用  InterceptedMethod method = new InterceptedMethod(this.target, "InsertMuch", paras);
            LocalBuilder interceptedMethodLocalBuilder = methodGen.DeclareLocal(typeof(InterceptedMethod));
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, targetField);
            methodGen.Emit(OpCodes.Ldstr, originMethod.Name);
            methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
            methodGen.Emit(OpCodes.Newobj, typeof(InterceptedMethod).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(object[]) }));
            methodGen.Emit(OpCodes.Stloc, interceptedMethodLocalBuilder);

            //调用this.aopInterceptor.PreProcess(interceptedMethod);
            MethodInfo preProcessMethodInfo = typeof(IAopInterceptor).GetMethod("PreProcess", new Type[] { typeof(InterceptedMethod) });
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aopInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, preProcessMethodInfo);

            //调用 IArounder arounder = this.aopInterceptor.NewArounder();
            LocalBuilder arounderLocalBuilder = methodGen.DeclareLocal(typeof(IArounder));
            MethodInfo newArounderMethodInfo = typeof(IAopInterceptor).GetMethod("NewArounder") ;
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aopInterceptorField);
            methodGen.Emit(OpCodes.Callvirt, newArounderMethodInfo);
            methodGen.Emit(OpCodes.Stloc, arounderLocalBuilder);

            //调用 arounder.BeginAround(method);
            MethodInfo beginAroundMethodInfo = typeof(IArounder).GetMethod("BeginAround");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, beginAroundMethodInfo);

            //调用 object returnVal = null;
            LocalBuilder retLocalBuilder = null; //是否有返回值
            if (originMethod.ReturnType != typeof(void))
            {
                retLocalBuilder = methodGen.DeclareLocal(originMethod.ReturnType);
            }

            LocalBuilder exceptionLocalBuilder = methodGen.DeclareLocal(typeof(Exception));

            // try
            Label beginExceptionLabel = methodGen.BeginExceptionBlock(); 

            //调用目标方法
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
            if (retLocalBuilder != null)
            {
                methodGen.Emit(OpCodes.Stloc, retLocalBuilder);
            }

            // catch
            methodGen.BeginCatchBlock(typeof(Exception));
            methodGen.Emit(OpCodes.Stloc, exceptionLocalBuilder); //存储Exception到local

            //调用 arounder.OnException(interceptedMethod ,exception);
            MethodInfo onExceptionMethodInfo = typeof(IArounder).GetMethod("OnException");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, exceptionLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, onExceptionMethodInfo);           

            methodGen.Emit(OpCodes.Nop);
            methodGen.Emit(OpCodes.Rethrow);
            methodGen.Emit(OpCodes.Nop);
            methodGen.EndExceptionBlock();

            //调用 arounder.EndAround(returnVal);
            MethodInfo endAroundMethodInfo = typeof(IArounder).GetMethod("EndAround");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            if (retLocalBuilder != null)
            {
                if (originMethod.ReturnType.IsValueType) //返回值如果是值类型，则装箱后再调用EndAround
                {
                    methodGen.Emit(OpCodes.Ldloc, retLocalBuilder);
                    methodGen.Emit(OpCodes.Box, originMethod.ReturnType);
                }
                else
                {
                    methodGen.Emit(OpCodes.Ldloc, retLocalBuilder);
                }
            }
            else
            {
                methodGen.Emit(OpCodes.Ldnull);
            }
            methodGen.Emit(OpCodes.Callvirt, endAroundMethodInfo);
            methodGen.Emit(OpCodes.Nop);

            //调用this.aopInterceptor.PostProcess(method ,returnVal);
            MethodInfo postProcessMethodInfo = typeof(IAopInterceptor).GetMethod("PostProcess", new Type[] { typeof(InterceptedMethod),typeof(object) });
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aopInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            if (retLocalBuilder != null)
            {
                if (originMethod.ReturnType.IsValueType)
                {
                    methodGen.Emit(OpCodes.Ldloc, retLocalBuilder);
                    methodGen.Emit(OpCodes.Box, originMethod.ReturnType);
                }
                else
                {
                    methodGen.Emit(OpCodes.Ldloc, retLocalBuilder);
                }
            }
            else
            {
                methodGen.Emit(OpCodes.Ldnull);
            }
            methodGen.Emit(OpCodes.Callvirt, postProcessMethodInfo);
            methodGen.Emit(OpCodes.Nop);

            if (retLocalBuilder != null)
            {
                methodGen.Emit(OpCodes.Ldloc, retLocalBuilder);
            }
            methodGen.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        } 
        #endregion      
    }

    
}
