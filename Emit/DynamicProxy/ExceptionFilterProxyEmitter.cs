using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using ESBasic.Helpers;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// ExceptionFilterProxyEmitter ����Ķ�̬��������ת�����ò��ػ�������������쳣����IExceptionFilter���д���
    /// ֱ�ӵ���ԭʼĿ��ķ����������Ǿ���������ã�����Ч�ʸ�һЩ��֧��ref/out������֧�ַ��ͷ�����
    /// �����SimpleAopProxyEmitter���ԣ�ExceptionFilterProxyEmitter�ܼ�¼���������ľ������ơ�
    /// </summary>
    public class ExceptionFilterProxyEmitter :BaseProxyEmitter
    {
        public ExceptionFilterProxyEmitter() : base() { }
        public ExceptionFilterProxyEmitter(bool save) : base(save) { }        

        protected override string GetPostfixOfDynamicTypeName()
        {
            return "EFProxy";
        }

        #region EmitProxyType
        /// <summary>
        /// GenerateDynamicType ���ɵĶ�̬���ͽ��̳�TInterface�ӿڣ����Ҷ�̬������һ������ΪoriginType���������
        /// ��̬�������TInterface�ӿڵ�����ʵ�ֽ�ת������Ctor�����originTypeʵ����ɡ�
        /// ע�⣺originType ������public���ε�
        /// </summary>
        /// <typeparam name="TInterface">ֻ֧�ַǷ��ͽӿڣ����ӿ��п��԰������ͷ���</typeparam>        
        protected override Type DoEmitProxyType(Type interfaceType ,Type originType)
        {            
            string typeName = base.GetDynamicTypeName(interfaceType, originType);
            TypeBuilder typeBuilder = base.moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class);
            typeBuilder.SetParent(typeof(MarshalByRefObject));
            typeBuilder.AddInterfaceImplementation(interfaceType);

            //�����Ա�����ڱ��洫��originTypeʵ����
            FieldBuilder targetField = typeBuilder.DefineField("target", originType, FieldAttributes.Private);
            FieldBuilder exceptionFilterField = typeBuilder.DefineField("exceptionFilter", typeof(IExceptionFilter), FieldAttributes.Private);            

            #region Emit Ctor
            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType, typeof(IExceptionFilter) });
            ILGenerator ctorGen = ctor.GetILGenerator();
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[] { }));
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_1);
            ctorGen.Emit(OpCodes.Stfld, targetField);
            ctorGen.Emit(OpCodes.Ldarg_0);
            ctorGen.Emit(OpCodes.Ldarg_2);
            ctorGen.Emit(OpCodes.Stfld, exceptionFilterField);
            ctorGen.Emit(OpCodes.Ret);
            #endregion

            foreach (MethodInfo baseMethod in ReflectionHelper.GetAllMethods(interfaceType))//interfaceType.GetMethods())
            {
                this.EmitMethod(originType, typeBuilder, baseMethod, targetField, exceptionFilterField);
            }           

            Type target = typeBuilder.CreateType();           
            
            return target;
        }
        #endregion

        #region EmitMethod
        private void EmitMethod(Type originType, TypeBuilder typeBuilder, MethodInfo baseMethod, FieldBuilder targetField, FieldBuilder exceptionFilterField)
        {
            Type[] argTypes = EmitHelper.GetParametersType(baseMethod);
            MethodInfo originMethod = ReflectionHelper.SearchMethod(originType, baseMethod.Name, argTypes);
            MethodBuilder methodBuilder = EmitHelper.DefineDerivedMethodSignature(typeBuilder, baseMethod);

            ILGenerator methodGen = methodBuilder.GetILGenerator();
            LocalBuilder exceptionLocalBuilder = methodGen.DeclareLocal(typeof(Exception));

            LocalBuilder retLocalBuilder = null; //�Ƿ��з���ֵ
            if (originMethod.ReturnType != typeof(void))
            {
                retLocalBuilder = methodGen.DeclareLocal(originMethod.ReturnType);
            }

            Label beginExceptionLabel = methodGen.BeginExceptionBlock();

            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, targetField);

            int converIndex = 0;
            foreach (ParameterInfo pi in originMethod.GetParameters())
            {
                EmitHelper.LoadArg(methodGen, converIndex + 1);
                EmitHelper.ConvertTopArgType(methodGen, argTypes[converIndex], pi.ParameterType);
                converIndex++;
            }

            methodGen.Emit(OpCodes.Callvirt, originMethod);//����Ŀ�귽��

            if (retLocalBuilder != null)
            {
                methodGen.Emit(OpCodes.Stloc_1);
            }

            methodGen.BeginCatchBlock(typeof(Exception));
            methodGen.Emit(OpCodes.Stloc_0); //�洢Exception��local

            string methodPath = originMethod.DeclaringType.ToString() + "." + originMethod.Name;// ESBasic.Helpers.TypeHelper.GetClassSimpleName(originMethod.DeclaringType) + "." + originMethod.Name;
            MethodInfo filterMethodInfo = typeof(IExceptionFilter).GetMethod("Filter", new Type[] { typeof(Exception), typeof(string), typeof(string) });

            #region ����methodParaInfo����
            //����methodParaInfo string
            MethodInfo contactStringMethodInfo = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });
            LocalBuilder methodParaInfoLocalBuilder = methodGen.DeclareLocal(typeof(string));
            methodGen.Emit(OpCodes.Ldstr, "<Parameters>");
            methodGen.Emit(OpCodes.Stloc, methodParaInfoLocalBuilder);
            int paraIndex = 0;
            foreach (ParameterInfo pi in originMethod.GetParameters())
            {
                bool isByRef = false;
                Type paraType = pi.ParameterType;
                if (paraType.IsByRef)
                {
                    paraType = paraType.GetElementType();
                    isByRef = true;
                }

                Label continueLabel = methodGen.DefineLabel();
                MethodInfo toStringMethodInfo = paraType.GetMethod("ToString", new Type[] { }); //������typeof(object).GetMethod("ToString", new Type[] { });
                if (toStringMethodInfo == null) //���paraTypeΪ�ӿ����ͣ���toStringMethodInfoΪnull
                {
                    toStringMethodInfo = typeof(object).GetMethod("ToString", new Type[] { });                   
                }

                if (!paraType.IsValueType)
                {
                    EmitHelper.LoadArg(methodGen, paraIndex + 1);
                    if (isByRef)
                    {
                        methodGen.Emit(OpCodes.Ldind_Ref);
                    }
                    methodGen.Emit(OpCodes.Ldnull);
                    methodGen.Emit(OpCodes.Ceq);
                    methodGen.Emit(OpCodes.Brtrue, continueLabel);
                }

                methodGen.Emit(OpCodes.Ldloc, methodParaInfoLocalBuilder);
                methodGen.Emit(OpCodes.Ldstr, string.Format("<{0}>", pi.Name));
                methodGen.Emit(OpCodes.Call, contactStringMethodInfo);

                #region ����ToString()����
                if (paraType.IsEnum)
                {
                    EmitHelper.LoadArg(methodGen, paraIndex + 1);
                    if (isByRef)
                    {
                        EmitHelper.Ldind(methodGen, paraType);
                    }
                    methodGen.Emit(OpCodes.Box, paraType);
                    methodGen.Emit(OpCodes.Callvirt, toStringMethodInfo);
                }
                else if (paraType.IsValueType)
                {
                    if (isByRef)
                    {
                        EmitHelper.LoadArg(methodGen, paraIndex + 1);
                    }
                    else
                    {
                        methodGen.Emit(OpCodes.Ldarga_S, paraIndex + 1);
                    }

                    methodGen.Emit(OpCodes.Call, toStringMethodInfo);
                }
                else
                {
                    EmitHelper.LoadArg(methodGen, paraIndex + 1);
                    if (isByRef)
                    {
                        EmitHelper.Ldind(methodGen, paraType);
                    }

                    methodGen.Emit(OpCodes.Callvirt, toStringMethodInfo);
                }
                #endregion

                methodGen.Emit(OpCodes.Call, contactStringMethodInfo);

                methodGen.Emit(OpCodes.Ldstr, string.Format("</{0}>", pi.Name));
                methodGen.Emit(OpCodes.Call, contactStringMethodInfo);
                methodGen.Emit(OpCodes.Stloc, methodParaInfoLocalBuilder);

                methodGen.MarkLabel(continueLabel);
                ++paraIndex;
            }

            methodGen.Emit(OpCodes.Ldloc, methodParaInfoLocalBuilder);
            methodGen.Emit(OpCodes.Ldstr, "</Parameters>");
            methodGen.Emit(OpCodes.Call, contactStringMethodInfo);
            methodGen.Emit(OpCodes.Stloc, methodParaInfoLocalBuilder);

            #endregion

            methodGen.Emit(OpCodes.Ldarg_0);
            methodGen.Emit(OpCodes.Ldfld, exceptionFilterField);
            methodGen.Emit(OpCodes.Ldloc_0); //����Exception
            methodGen.Emit(OpCodes.Ldstr, methodPath);
            methodGen.Emit(OpCodes.Ldloc, methodParaInfoLocalBuilder);
            methodGen.Emit(OpCodes.Callvirt, filterMethodInfo);


            methodGen.Emit(OpCodes.Nop);
            methodGen.Emit(OpCodes.Rethrow);
            methodGen.Emit(OpCodes.Nop);
            methodGen.EndExceptionBlock();

            if (retLocalBuilder != null)
            {
                methodGen.Emit(OpCodes.Ldloc_1);
            }
            methodGen.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        } 
        #endregion       
    }

    
}
