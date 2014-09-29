using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Diagnostics;

namespace ESBasic.Helpers
{
    public static class MachineHelper
    {
        private static PerformanceCounter CpuPerformanceCounter = new PerformanceCounter("Process", "% Processor Time", "Idle");
        private static PerformanceCounter MemPerformanceCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use", "");//Available MBytes

        #region GetMacAddress 
        /// <summary>
        /// GetMacAddress ªÒ»°Õ¯ø®macµÿ÷∑
        /// </summary>        
        public static IList<string> GetMacAddress()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            IList<string> strArr = new List<string>();

            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                {
                    strArr.Add(mo["MacAddress"].ToString().Replace(":", ""));
                }
                mo.Dispose();
            }

            return strArr;
        }
        #endregion

        public static bool IsCurrentMachine(string macAddress)
        {
            IList<string> addList = MachineHelper.GetMacAddress();
            return addList.Contains(macAddress);
        }

        public static void GetPerformanceUsage(out float cpuAvailable, out float memoryUsage)
        {
            cpuAvailable = MachineHelper.CpuPerformanceCounter.NextValue();
            memoryUsage = MachineHelper.MemPerformanceCounter.NextValue();
        }
    }
}