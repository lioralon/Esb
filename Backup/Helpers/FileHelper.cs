using System;
using System.Windows.Forms ;
using System.IO ;

namespace ESBasic.Helpers
{
	/// <summary>
	/// FileHelper ���ڼ����ļ���ز�����
	/// ���ߣ���ΰ sky.zhuwei@163.com 
	/// 2004.03.26
	/// </summary>
	public static class FileHelper
	{	
		#region GenerateFile 
        /// <summary>
        /// GenerateFile ���ַ���д���ļ�
        /// </summary>       
        public static void GenerateFile(string filePath, string text)
		{           
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
			{
                Directory.CreateDirectory(directoryPath);
			}

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);			

			StreamWriter sw = new StreamWriter(fs) ;

			sw.Write(text) ;
			sw.Flush() ;

			sw.Close() ;
			fs.Close() ;			
		}	
		#endregion

		#region GetFileContent
        /// <summary>
        /// GetFileContent ��ȡ�ı��ļ�������
        /// </summary>       
		public static string GetFileContent(string file_path)
		{
			if(! File.Exists(file_path))
			{
				return null ;
			}
			
			StreamReader reader = new StreamReader(file_path ,System.Text.Encoding.Default ) ;
			string content = reader.ReadToEnd() ;
			reader.Close() ;

			return content ;
		}
		#endregion

		#region WriteBuffToFile 
        /// <summary>
        /// WriteBuffToFile ������������д���ļ���
        /// </summary>    
		public static void WriteBuffToFile(byte[] buff ,int offset ,int len ,string filePath )
		{
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
			{
                Directory.CreateDirectory(directoryPath);
			}

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(buff, offset, len);
            bw.Flush();

            bw.Close();
            fs.Close();
		}

        /// <summary>
        /// WriteBuffToFile ������������д���ļ���
        /// </summary>   
        public static void WriteBuffToFile(byte[] buff, string filePath)
		{
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(buff);
            bw.Flush();

            bw.Close();
            fs.Close();
		}
		#endregion

		#region ReadFileReturnBytes
        /// <summary>
        /// ReadFileReturnBytes ���ļ��ж�ȡ����������
        /// </summary>      
		public static byte[] ReadFileReturnBytes(string filePath)
		{
            if (!File.Exists(filePath))
			{
				return null ;
			}

            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);			

			BinaryReader br = new BinaryReader(fs) ;

			byte[] buff = br.ReadBytes((int)fs.Length) ;	

			br.Close() ;
			fs.Close() ;
			
			return buff ;
		}
		#endregion

		#region GetFileToOpen 
        /// <summary>
        /// GetFileToOpen ��ȡҪ�򿪵��ļ�·��
        /// </summary>        
		public static string GetFileToOpen(string title)
		{
			OpenFileDialog openDlg = new OpenFileDialog();
			openDlg.Filter  = "All Files (*.*)|*.*";
			openDlg.FileName = "" ;		
			if(title != null)
			{
				openDlg.Title = title ;
			}

			openDlg.CheckFileExists = true;
			openDlg.CheckPathExists = true;

			DialogResult res =openDlg.ShowDialog ();
			if(res == DialogResult.OK)
			{
				return openDlg.FileName ;
			}
			
			return null ;
		}

        /// <summary>
        /// GetFileToOpen ��ȡҪ�򿪵��ļ�·��
        /// </summary>      
		public static string GetFileToOpen(string title ,string extendName ,string iniDir)
		{
			OpenFileDialog openDlg = new OpenFileDialog();
			openDlg.Filter  = string.Format("The Files (*{0})|*{0}" ,extendName);
			openDlg.FileName = "" ;
            openDlg.InitialDirectory = iniDir;
			if(title != null)
			{
				openDlg.Title = title ;
			}

			openDlg.CheckFileExists = true;
			openDlg.CheckPathExists = true;

			DialogResult res =openDlg.ShowDialog ();
			if(res == DialogResult.OK)
			{
				return openDlg.FileName ;
			}
			
			return null ;
		}
		#endregion

		#region GetFolderToOpen 
        /// <summary>
        /// GetFolderToOpen ��ȡҪ�򿪵��ļ���
        /// </summary>      
		public static string GetFolderToOpen(bool newFolderButton)
		{
			FolderBrowserDialog folderDialog = new FolderBrowserDialog() ;
			folderDialog.ShowNewFolderButton = newFolderButton ;
			DialogResult res = folderDialog.ShowDialog() ;
			if(res == DialogResult.OK)
			{
				return folderDialog.SelectedPath ;
			}
			
			return null ;
		}
		#endregion

		#region GetPathToSave 
        /// <summary>
        /// GetPathToSave ��ȡҪ������ļ���·�� 
        /// </summary>       
        public static string GetPathToSave(string title, string defaultName ,string iniDir)
		{
            string extendName = Path.GetExtension(defaultName);            
			SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = string.Format("The Files (*{0})|*{0}", extendName);
            saveDlg.FileName = defaultName;
            saveDlg.InitialDirectory = iniDir;            
			saveDlg.OverwritePrompt = false ;
			if(title != null)
			{
				saveDlg.Title = title ;
			}
			
			DialogResult res = saveDlg.ShowDialog ();
			if(res == DialogResult.OK)
			{
				return saveDlg.FileName ;
			}

			return null ;	
		}
		#endregion	

		#region GetFileNameNoPath
        /// <summary>
        /// GetFileNameNoPath ��ȡ������·�����ļ���
        /// </summary>      
		public static string GetFileNameNoPath(string filePath)
		{
            return Path.GetFileName(filePath);			
		}
		#endregion

		#region GetFileSize
        /// <summary>
        /// GetFileSize ��ȡĿ���ļ��Ĵ�С
        /// </summary>        
		public static int GetFileSize(string filePath)
		{
			FileStream fs = new FileStream(filePath ,FileMode.Open ,FileAccess.Read ,FileShare.Read) ;
			int size = (int)fs.Length ;
			fs.Close() ;

			return size ;
		}
		#endregion

		#region ReadFileData 
        /// <summary>
        /// ReadFileData ���ļ����ж�ȡָ����С������
        /// </summary>       
		public static void ReadFileData(FileStream fs, byte[] buff, int count ,int offset)
		{
			int readCount = 0;
			while (readCount < count)
			{
				int read = fs.Read(buff, offset + readCount, count - readCount);
				readCount += read;
			}

			return;
		}
		#endregion

        #region GetFileDirectory
        /// <summary>
        /// GetFileDirectory ��ȡ�ļ����ڵ�Ŀ¼·��
        /// </summary>       
        public static string GetFileDirectory(string filePath)
		{
            return Path.GetDirectoryName(filePath);
		}
		#endregion

		#region DeleteFile 
        /// <summary>
        /// DeleteFile ɾ���ļ�
        /// </summary>
        /// <param name="filePath"></param>
		public static void DeleteFile(string filePath)
		{
            if (File.Exists(filePath))
			{
                File.Delete(filePath);					
			}
		}
		#endregion

		#region EnsureExtendName 
        /// <summary>
        /// EnsureExtendName ȷ����չ����ȷ
        /// </summary>       
		public static string EnsureExtendName(string origin_path ,string extend_name)
		{
			if(Path.GetExtension(origin_path) != extend_name)
			{
				origin_path += extend_name ;
			}

			return origin_path ;
		}
		#endregion

        #region ClearDirectory
        public static void ClearDirectory(string dirPath)
        {
            string[] filePaths = Directory.GetFiles(dirPath);
            foreach (string file in filePaths)
            {
                File.Delete(file);
            }
        } 
        #endregion
	}


}