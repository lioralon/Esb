using System;
using System.Net.Sockets;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace ESBasic
{
    //delegate ∂®“Â
    public delegate void CbSimple();
    public delegate void CbGeneric<T>(T obj);
    public delegate void CbSimpleInt(int val);
    public delegate void CbSimpleBool(bool val);
    public delegate void CbSimpleStr(string str);  
    public delegate void CbSimpleObj(object obj);
    public delegate void CbStream(byte[] stream);
    public delegate void CbDataRow(DataRow row);
    public delegate void CbDateTime(DateTime dt) ;
    public delegate void CbException(Exception ex);   
    public delegate void CbNetworkStream(NetworkStream stream) ;

    public delegate void CbSimpleStrInt(string str, int val);
    public delegate void CbProgress(int val, int total);

    public delegate void Action<T1,T2>(T1 t1,T2 t2) ;
    public delegate void Action<T1, T2 ,T3>(T1 t1, T2 t2 ,T3 t3);
    public delegate TResult Func<T, TResult>(T source); 

    public interface IExceptionHandler
    {
        void HanleException(Exception ee);
    }
}
