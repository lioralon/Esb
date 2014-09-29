using System;
using System.Drawing ;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ESBasic.Helpers
{
    public static class WindowsHelper
    {
        #region ShowQuery
        public static bool ShowQuery(string query)
        {
            if (DialogResult.Yes != MessageBox.Show(query, "提示", MessageBoxButtons.YesNo))
            {
                return false;
            }

            return true;
        }
        #endregion	

        #region DoWindowsEvents
        /// <summary>
        /// DoWindowsEvents 在UI线程中调用该方法将使UI线程处理windows事件。
        /// </summary>
        public static void DoWindowsEvents()
        {
            Application.DoEvents();
        } 
        #endregion

        #region GetMdiChildForm ,MdiChildIsExist
        public static Form GetMdiChildForm(Form parentForm, Type childFormType)
        {
            foreach (Form child in parentForm.MdiChildren)
            {
                if (child.GetType() == childFormType)
                {
                    return child;
                }
            }

            return null;
        } 

        public static bool MdiChildIsExist(Form parentForm, Type childFormType)
        {
           if(WindowsHelper.GetMdiChildForm(parentForm ,childFormType) != null)
           {
               return true ;
           }

            return false;
        } 
        #endregion

        #region GetCursorPosition
        public static Point GetCursorPosition()
        {
            return System.Windows.Forms.Cursor.Position;
        } 
        #endregion

        #region GetStartupDirectoryPath
        /// <summary>
        /// GetStartupDirectoryPath 获取当前应用程序所在的目录
        /// </summary>        
        public static string GetStartupDirectoryPath()
        {
            return Application.StartupPath; //AppDomain.CurrentDomain.BaseDirectory
        } 
        #endregion

        #region CaptureScreen
        /// <summary>
        /// CaptureScreen 截取全屏。
        /// </summary>  
        public static Bitmap CaptureScreen()
        {
            return WindowsHelper.CaptureScreen(new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));           
        }

        /// <summary>
        /// CaptureScreen 截取目标区域region内的屏幕。
        /// </summary>        
        public static Bitmap CaptureScreen(Rectangle region )
        {
            Bitmap image = new Bitmap(region.Width, region.Height);
            Graphics g = Graphics.FromImage(image);
            g.CopyFromScreen(region.Location, region.Location, region.Size);
            return image;
        } 
        #endregion
    
    }
}
