using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace ESBasic.Helpers
{
    public static class PublicHelper
    {
        #region CompressBitmapToJpg
        /// <summary>
        /// CompressBitmapToJpg ��λͼѹ��ΪJPG��ʽ
        /// </summary>       
        public static byte[] CompressBitmapToJpg(System.Drawing.Bitmap bm)
        {
            MemoryStream memStream = new MemoryStream();
            bm.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bResult = memStream.ToArray();
            memStream.Close();

            return bResult;
        }
        //byte[] bImage = this.CompressBitmapToJpg(bm) ;
        //this.pictureBox1.Image = Image.FromStream(new MemoryStream(bImage)) ;
        #endregion

        #region CopyData
        /// <summary>
        /// CopyData ��������������
        /// </summary>      
        public static void CopyData(byte[] source, byte[] dest, int destOffset)
        {
            Buffer.BlockCopy(source, 0, dest, destOffset, source.Length);           
        }
        #endregion
    }
}
