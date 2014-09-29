using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy.Aop
{
    /// <summary>
    /// AopProxyEmitter 用于发射AOP Proxy。AOP动态代理支持预处理、后处理，以及Around处理。分别由IMethodInterceptor和IAroundInterceptor支持。
    /// 注意，该AopProxyEmitter发射的AOP代理最后对截获的目标方法的调用是经过反射调用的，所以可能会影响性能。
    /// 所以，如果是截获异常，直接使用ExceptionFilterProxyEmitter效率更高。
    /// zhuweisky 2008.05.20 支持ref/out参数
    /// </summary>
    public sealed class AopProxyEmitter : BaseProxyEmitter
    {
        public AopProxyEmitter() : base() { }
        public AopProxyEmitter(bool save) : base(save) { }

        protected override string GetPostfixOfDynamicTypeName()
        {
            return "AopProxy";
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
            FieldBuilder methodInterceptorField = typeBuilder.DefineField("methodInterceptor", typeof(IMethodInterceptor), FieldAttributes.Private);
            FieldBuilder aroundInterceptorField = typeBuilder.DefineField("aroundInterceptor", typeof(IAroundInterceptor), FieldAttributes.Private);
            ConstructorInfo emptyMethodInterceptorCtorInfo = typeof(EmptyMethodInterceptor).GetConstructor(new Type[] { });
            ConstructorInfo emptyAroundInterceptorCtorInfo = typeof(EmptyAroundInterceptor).GetConstructor(new Type[] { });


            #region Emit Ctor
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType, typeof(IMethodInterceptor), typeof(IAroundInterceptor) });
            ILGenerator ctorGen = ctor.GetILGenerator();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { }));
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_1);
            ctorGen.Emit(OpCodes.Stfld, targetField);

            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_2);
            ctorGen.Emit(OpCodes.Stfld, methodInterceptorField);

            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_3);
            ctorGen.Emit(OpCodes.Stfld, aroundInterceptorField);

            Label setAroundInterceptorFieldLable = ctorGen.DefineLabel() ;
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldfld, methodInterceptorField);
            ctorGen.Emit(OpCodes.Ldnull);
            ctorGen.Emit(OpCodes.Ceq);
            ctorGen.Emit(OpCodes.Brfalse_S, setAroundInterceptorFieldLable);
            ctorGen.Emit(OpCodes.Nop);
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Newobj, emptyMethodInterceptorCtorInfo);
            ctorGen.Emit(OpCodes.Stfld, methodInterceptorField);
            ctorGen.MarkLabel(setAroundInterceptorFieldLable);

            Label retLable = ctorGen.DefineLabel();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldfld, aroundInterceptorField);
            ctorGen.Emit(OpCodes.Ldnull);
            ctorGen.Emit(OpCodes.Ceq);
            ctorGen.Emit(OpCodes.Brfalse_S, retLable);
            ctorGen.Emit(OpCodes.Nop);
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Newobj, emptyAroundInterceptorCtorInfo);
            ctorGen.Emit(OpCodes.Stfld, aroundInterceptorField);
            ctorGen.MarkLabel(retLable);

            ctorGen.Emit(OpCodes.Nop);
            ctorGen.Emit(OpCodes.Ret);
            #endregion

            foreach (MethodInfo baseMethod in ESBasic.Helpers.ReflectionHelper.GetAllMethods(interfaceType))//interfaceType.GetMethods())
            {
                this.EmitMethod(interfaceType, originType, typeBuilder, baseMethod, targetField, methodInterceptorField, aroundInterceptorField);
            }

            Type target = typeBuilder.CreateType();

            return target;
        }
        #endregion

        #region EmitMethod
        private void EmitMethod(Type interfaceType, Type originType, TypeBuilder typeBuilder, MethodInfo baseMethod, FieldBuilder targetField ,FieldBuilder methodInterceptorField ,FieldBuilder aroundInterceptorField)
        {
            Type[] argTypes = EmitHelper.GetParametersType(baseMethod);
            MethodInfo originMethod = ReflectionHelper.SearchMethod(originType, baseMethod.Name, argTypes);
            MethodBuilder methodBuilder = EmitHelper.DefineDerivedMethodSignature(typeBuilder, baseMethod);            
           
            MethodInfo getMethodMethodInfo = typeof(Type).GetMethod("GetMethod", new Type[] {typeof(string) , typeof(Type[]) });           

            ILGenerator methodGen = methodBuilder.GetILGenerator(); 
            methodGen.Emit(OpCodes.Nop);
            //调用 Type interfaceType = typeof(IComputer);
            LocalBuilder interfaceTypeLocalBuilder = methodGen.DeclareLocal(typeof(Type));
            EmitHelper.LoadType(methodGen, interfaceType);           

            methodGen.Emit(OpCodes.Stloc, interfaceTypeLocalBuilder);

            //调用 Type[] paraTypes = new Type[] { typeof(int), typeof(int) }
            ParameterInfo[] paras = originMethod.GetParameters();
            LocalBuilder paraTypesLocalBuilder = methodGen.DeclareLocal(typeof(Type[]));       
            methodGen.Emit(OpCodes.Ldc_I4, paras.Length);
            methodGen.Emit(OpCodes.Newarr, typeof(Type));
            methodGen.Emit(OpCodes.Stloc, paraTypesLocalBuilder);
            for (int i = 0; i < argTypes.Length; i++)
            {
                methodGen.Emit(OpCodes.Ldloc, paraTypesLocalBuilder);
                methodGen.Emit(OpCodes.Ldc_I4, i);
                EmitHelper.LoadType(methodGen, argTypes[i]);              
                
                methodGen.Emit(OpCodes.Stelem_Ref);
            }           

            //调用 MethodInfo method = ESBasic.Helpers.ReflectionHelper.SearchMethod(interfaceType ,"Add", new Type[] { typeof(int), typeof(int), typeof(T) });
            MethodInfo searchMethodMethodInfo = typeof(ReflectionHelper).GetMethod("SearchMethod", new Type[] { typeof(Type),typeof(string) , typeof(Type[]) });
            LocalBuilder methodLocalBuilder = methodGen.DeclareLocal(typeof(MethodInfo));
            methodGen.Emit(OpCodes.Ldloc, interfaceTypeLocalBuilder);
            methodGen.Emit(OpCodes.Ldstr, originMethod.Name);
            methodGen.Emit(OpCodes.Ldloc, paraTypesLocalBuilder);
            methodGen.Emit(OpCodes.Call, searchMethodMethodInfo);
            methodGen.Emit(OpCodes.Stloc, methodLocalBuilder);
            methodGen.Emit(OpCodes.Nop);

            #region 处理泛型参数
            //处理泛型参数 Type[] typeArguments = new Type[] { typeof(T) }; info = info.MakeGenericMethod(typeArguments);            
            if (baseMethod.IsGenericMethod)
            {
                LocalBuilder genericParaTypesLocalBuilder = methodGen.DeclareLocal(typeof(Type[]));
                Type[] genericParaTypes = baseMethod.GetGenericArguments();
                methodGen.Emit(OpCodes.Ldc_I4, genericParaTypes.Length);
                methodGen.Emit(OpCodes.Newarr, typeof(Type));
                methodGen.Emit(OpCodes.Stloc, genericParaTypesLocalBuilder);

                for (int i = 0; i < genericParaTypes.Length; i++)
                {
                    methodGen.Emit(OpCodes.Ldloc, genericParaTypesLocalBuilder);
                    methodGen.Emit(OpCodes.Ldc_I4, i);
                    EmitHelper.LoadType(methodGen, genericParaTypes[i]);                 

                    methodGen.Emit(OpCodes.Stelem_Ref);
                }

                MethodInfo makeGenericMethodMethodInfo = typeof(MethodInfo).GetMethod("MakeGenericMethod", new Type[] { typeof(Type[]) });
                methodGen.Emit(OpCodes.Ldloc, methodLocalBuilder);
                methodGen.Emit(OpCodes.Ldloc, genericParaTypesLocalBuilder);
                methodGen.Emit(OpCodes.Callvirt, makeGenericMethodMethodInfo);
                methodGen.Emit(OpCodes.Stloc, methodLocalBuilder);
                methodGen.Emit(OpCodes.Nop);
            } 
            #endregion
            
            //调用 object[] arguments = new object[] {a ,b };   
            LocalBuilder paramsLocalBuilder = methodGen.DeclareLocal(typeof(object[]));
            methodGen.Emit(OpCodes.Ldc_I4, paras.Length);
            methodGen.Emit(OpCodes.Newarr, typeof(object));
            methodGen.Emit(OpCodes.Stloc, paramsLocalBuilder);
            for (int i = 0; i < paras.Length; i++)
            {
                methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
                methodGen.Emit(OpCodes.Ldc_I4, i);
                methodGen.Emit(OpCodes.Ldarg, i + 1);

                if (paras[i].ParameterType.IsByRef) //如果是ref/out参数，则传递地址
                {
                    EmitHelper.Ldind(methodGen, paras[i].ParameterType); 
                    methodGen.Emit(OpCodes.Box, paras[i].ParameterType.GetElementType());
                }
                else if (paras[i].ParameterType.IsValueType)
                {
                    methodGen.Emit(OpCodes.Box, paras[i].ParameterType);
                }

                methodGen.Emit(OpCodes.Stelem_Ref);
            }

            //调用 InterceptedMethod interceptedMethod = new InterceptedMethod(this.target ,method ,arguments);
            LocalBuilder interceptedMethodLocalBuilder = methodGen.DeclareLocal(typeof(InterceptedMethod));         
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, targetField);
            methodGen.Emit(OpCodes.Ldloc, methodLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
            methodGen.Emit(OpCodes.Newobj, typeof(InterceptedMethod).GetConstructor(new Type[] { typeof(object), typeof(MethodInfo), typeof(object[]) }));
            methodGen.Emit(OpCodes.Stloc, interceptedMethodLocalBuilder);

            //调用this.methodInterceptor.PreProcess(interceptedMethod);
            MethodInfo preProcessMethodInfo = typeof(IMethodInterceptor).GetMethod("PreProcess" ,new Type[]{ typeof(InterceptedMethod)}) ;
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, methodInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, preProcessMethodInfo);

            //调用object returnVal = this.aroundInterceptor.AroundCall(interceptedMethod);            
            MethodInfo aroundCallMethodInfo = typeof(IAroundInterceptor).GetMethod("AroundCall", new Type[] { typeof(InterceptedMethod) });
            LocalBuilder returnValLocalBuilder = methodGen.DeclareLocal(typeof(object));
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aroundInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, aroundCallMethodInfo);
            methodGen.Emit(OpCodes.Stloc, returnValLocalBuilder);

            //调用this.methodInterceptor.PostProcess(interceptedMethod, returnVal);
            MethodInfo postProcessMethodInfo = typeof(IMethodInterceptor).GetMethod("PostProcess", new Type[] { typeof(InterceptedMethod), typeof(object) });
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, methodInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, returnValLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, postProcessMethodInfo);

            //为ref/out参数赋值
            for (int i = 0; i < argTypes.Length; i++)
            {
                if (argTypes[i].IsByRef)//如果是ref/out参数，则存储地址
                {                   
                    methodGen.Emit(OpCodes.Ldarg, i + 1);

                    methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
                    methodGen.Emit(OpCodes.Ldc_I4, i);
                    methodGen.Emit(OpCodes.Ldelem_Ref);

                    Type elementType = argTypes[i].GetElementType() ;
                    if (elementType.IsValueType)
                    {
                        methodGen.Emit(OpCodes.Unbox_Any, elementType);
                    }

                    EmitHelper.Stind(methodGen, elementType);
                }                 
            }  

            //返回值
            if (originMethod.ReturnType != typeof(void))
            {
                methodGen.Emit(OpCodes.Ldloc, returnValLocalBuilder);
                if (originMethod.ReturnType.IsValueType)
                {
                    methodGen.Emit(OpCodes.Unbox_Any, originMethod.ReturnType);
                }
            }
           
            methodGen.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }
        #endregion      
    }
}
