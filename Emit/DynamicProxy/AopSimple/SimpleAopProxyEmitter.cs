using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy.AopSimple
{
    /// <summary>
    /// SimpleProxyEmitter ���ɵĶ�̬�������ͽ��̳�TInterface�ӿڣ����Ҷ�̬������һ������ΪoriginType��������� 
    /// ��̬�������TInterface�ӿڵ�����ʵ�ֽ�ת������Ctor�����originTypeʵ����ɡ�
    /// ע�⣺originType ������public���ε�
    /// ע�⣺TInterfaceֻ֧�ַǷ��ͽӿڣ����ӿ��п��԰������ͷ�����
    /// ��AopProxyEmitter��ȣ���Ҫ��SimpleProxyEmitter���ɵĶ�̬������ֱ�ӵ��ñ��ػ�ķ����ģ���AopProxyEmitter���ɵĶ�̬������ͨ��������ñ��ػ�ķ����ġ�
    /// zhuweisky ���һ������2007.08.02
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

            //�����Ա�����ڱ��洫��originTypeʵ����
            FieldBuilder targetField = typeBuilder.DefineField("target", originType, FieldAttributes.Private);
            FieldBuilder aopInterceptorField = typeBuilder.DefineField("aopInterceptor", typeof(IAopInterceptor), FieldAttributes.Private);

            #region Emit Ctor
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType, typeof(IAopInterceptor) });
            ILGenerator ctorGen = ctor.GetILGenerator();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { })); //���û���Ĺ��캯��
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
            //���� object[] paras = new object[] {a ,b };   
            LocalBuilder paramsLocalBuilder = methodGen.DeclareLocal(typeof(object[]));
            methodGen.Emit(OpCodes.Ldc_I4, paras.Length);
            methodGen.Emit(OpCodes.Newarr, typeof(object));
            methodGen.Emit(OpCodes.Stloc, paramsLocalBuilder);
            for (int i = 0; i < paras.Length; i++)
            {
                methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
                methodGen.Emit(OpCodes.Ldc_I4, i);
                methodGen.Emit(OpCodes.Ldarg, i + 1);

                if (paras[i].ParameterType.IsByRef) //�����ref/out��������ȥ��ַ
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

            //����  InterceptedMethod method = new InterceptedMethod(this.target, "InsertMuch", paras);
            LocalBuilder interceptedMethodLocalBuilder = methodGen.DeclareLocal(typeof(InterceptedMethod));
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, targetField);
            methodGen.Emit(OpCodes.Ldstr, originMethod.Name);
            methodGen.Emit(OpCodes.Ldloc, paramsLocalBuilder);
            methodGen.Emit(OpCodes.Newobj, typeof(InterceptedMethod).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(object[]) }));
            methodGen.Emit(OpCodes.Stloc, interceptedMethodLocalBuilder);

            //����this.aopInterceptor.PreProcess(interceptedMethod);
            MethodInfo preProcessMethodInfo = typeof(IAopInterceptor).GetMethod("PreProcess", new Type[] { typeof(InterceptedMethod) });
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aopInterceptorField);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, preProcessMethodInfo);

            //���� IArounder arounder = this.aopInterceptor.NewArounder();
            LocalBuilder arounderLocalBuilder = methodGen.DeclareLocal(typeof(IArounder));
            MethodInfo newArounderMethodInfo = typeof(IAopInterceptor).GetMethod("NewArounder") ;
            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, aopInterceptorField);
            methodGen.Emit(OpCodes.Callvirt, newArounderMethodInfo);
            methodGen.Emit(OpCodes.Stloc, arounderLocalBuilder);

            //���� arounder.BeginAround(method);
            MethodInfo beginAroundMethodInfo = typeof(IArounder).GetMethod("BeginAround");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, beginAroundMethodInfo);

            //���� object returnVal = null;
            LocalBuilder retLocalBuilder = null; //�Ƿ��з���ֵ
            if (originMethod.ReturnType != typeof(void))
            {
                retLocalBuilder = methodGen.DeclareLocal(originMethod.ReturnType);
            }

            LocalBuilder exceptionLocalBuilder = methodGen.DeclareLocal(typeof(Exception));

            // try
            Label beginExceptionLabel = methodGen.BeginExceptionBlock(); 

            //����Ŀ�귽��
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
            methodGen.Emit(OpCodes.Stloc, exceptionLocalBuilder); //�洢Exception��local

            //���� arounder.OnException(interceptedMethod ,exception);
            MethodInfo onExceptionMethodInfo = typeof(IArounder).GetMethod("OnException");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, interceptedMethodLocalBuilder);
            methodGen.Emit(OpCodes.Ldloc, exceptionLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, onExceptionMethodInfo);           

            methodGen.Emit(OpCodes.Nop);
            methodGen.Emit(OpCodes.Rethrow);
            methodGen.Emit(OpCodes.Nop);
            methodGen.EndExceptionBlock();

            //���� arounder.EndAround(returnVal);
            MethodInfo endAroundMethodInfo = typeof(IArounder).GetMethod("EndAround");
            methodGen.Emit(OpCodes.Ldloc, arounderLocalBuilder);
            if (retLocalBuilder != null)
            {
                if (originMethod.ReturnType.IsValueType) //����ֵ�����ֵ���ͣ���װ����ٵ���EndAround
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

            //����this.aopInterceptor.PostProcess(method ,returnVal);
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
