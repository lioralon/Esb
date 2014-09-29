using System;
using System.Collections.Generic;
using System.Text;

namespace ESBasic.Emit.DynamicProxy
{
    /// <summary>
    /// IProxyEmitter ���ڷ���ָ�����͵Ķ�̬�������͡�
    /// TInterface �����Ƿ��ͽӿڣ�����TInterface���԰������ͷ���(���Ͳ���������Լ��)��֧��ref/out������ 
    /// </summary>
    public interface IProxyEmitter
    {
        /// <summary>
        /// EmitProxyType ����originTypeû��ʵ��TInterface�������
        /// </summary>       
        Type EmitProxyType<TInterface>(Type originType);

        void Save();
    }
}
