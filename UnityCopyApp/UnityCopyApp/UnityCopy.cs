using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CopyUnityDependApp
{
    internal class UnityCopy
    {
        public UnityCopy()
        {
        }

        async public static void CopyDirectoryDeep(string sourcePath, string destinationPath)
        {
            Console.WriteLine($"资源目录复制开始:{sourcePath} -> {destinationPath}");
            await Task.Delay(1000);
            CopyDirectoryRecursively(sourcePath, destinationPath);

            await Task.Delay(1000);
            Console.WriteLine($"开始获取Meta列表");
            List<string> metaFiles = GetFilesRecursively(destinationPath, (f) => f.ToLower().EndsWith(".meta"));
            List<(string originalGuid, string newGuid)> guidTable = new List<(string originalGuid, string newGuid)>();
            Console.WriteLine($"开始处理Meta列表...");
            await Task.Delay(1000);
            foreach (string metaFile in metaFiles)
            {
                //Console.WriteLine("开始添加处理：" + metaFile);
                //await Task.Delay(1);
                StreamReader file = new StreamReader(metaFile);
                file.ReadLine();
                string guidLine = file.ReadLine();
                file.Close();
                string originalGuid = guidLine.Substring(6, guidLine.Length - 6);
                string newGuid = System.Guid.NewGuid().ToString().Replace("-", ""); 
                guidTable.Add((originalGuid, newGuid));
            }




            List<string> allFiles = GetFilesRecursively(destinationPath);

            Console.WriteLine($"所有Guild数量：" + guidTable.Count);
            Console.WriteLine($"所有文件数量：" + allFiles.Count);
            await Task.Delay(3000);

            for (int j = 0; j < allFiles.Count; j++)
            //foreach (string fileToModify in allFiles)
            {
                string fileToModify = allFiles[j];
                bool binfile = CheckForBinary(fileToModify);
                if (binfile)
                {
                    Console.WriteLine("<color=#111111>二进制文件不用修改引用</color>：" + fileToModify);
                    continue;
                }
                Console.WriteLine($"开始处理（{j}/{allFiles.Count}）：" + fileToModify);
                await Task.Delay(1);
                //string newname = fileToModify.ToLower();
                //if (newname.EndsWith(".png"))
                //    continue;
                //if (newname.EndsWith(".jpg"))
                //    continue;
                //if (newname.EndsWith(".bmp"))
                //    continue;
                //if (newname.EndsWith(".tif"))
                //    continue;
                //if (newname.EndsWith(".tga"))
                //    continue;
                //if (newname.EndsWith(".psd"))
                //    continue;
                string content = File.ReadAllText(fileToModify);
                for (int i = 0; i < guidTable.Count; i++)
                //foreach (var guidPair in guidTable)
                {
                    var guidPair = guidTable[i];
                    string oldstr = guidPair.originalGuid;
                    string newstr = guidPair.newGuid;
                    Console.WriteLine($"（{j}/{allFiles.Count}）（{i}/{guidTable.Count}）：" + fileToModify + ",size:" + content.Length + "," + oldstr + "->" + newstr);
                    //await Task.Delay(1);
                    content = content.Replace(oldstr, newstr);
                    //Console.WriteLine(content.Length );
                }
                Console.WriteLine("修改文件引用：" + fileToModify);
                File.WriteAllText(fileToModify, content);


            }

            Console.WriteLine($"-----------------------------------");
            Console.WriteLine($"目录文件复制完毕:{sourcePath} -> {destinationPath}。");
            
        }

        /// <summary>
        /// This method checks whether selected file is Binary file or not.
        /// </summary>     
        static bool CheckForBinary(string file)
        {

            Stream objStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            bool bFlag = true;

            // Iterate through stream & check ASCII value of each byte.
            for (int nPosition = 0; nPosition < objStream.Length; nPosition++)
            {
                int a = objStream.ReadByte();

                if (!(a >= 0 && a <= 127))
                {
                    break;            // Binary File
                }
                else if (objStream.Position == (objStream.Length))
                {
                    bFlag = false;    // Text File
                }
            }
            objStream.Dispose();

            return bFlag;
        }

        private static void CopyDirectoryRecursively(string sourceDirName, string destDirName)
        {
            string ResCopyWin = ".cs;.shader;.inputactions";
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            List<string> efile = new List<string>(ResCopyWin.Split(';'));
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {

                string expname = file.Extension.Trim().ToLower();
                if (efile.IndexOf(expname) != -1)
                {
                    Console.WriteLine("这个文件被排除了类型：" + file.FullName + "");
                    continue;
                }
                if (expname == ".meta")
                {
                    string newname = file.FullName.Replace(".meta", "");
                    expname = Path.GetExtension(newname).ToLower();
                    if (efile.IndexOf(expname) != -1)
                    {
                        Console.WriteLine("这个文件被排除了类型：" + file.FullName + "");
                        continue;
                    }
                }
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
                Console.WriteLine("复制文件：" + temppath + "");
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectoryRecursively(subdir.FullName, temppath);
            }
        }

        private static List<string> GetFilesRecursively(string path, Func<string, bool> criteria = null, List<string> files = null)
        {
            if (files == null)
            {
                files = new List<string>();
            }

            files.AddRange(Directory.GetFiles(path).Where(f => criteria == null || criteria(f)));

            foreach (string directory in Directory.GetDirectories(path))
            {
                GetFilesRecursively(directory, criteria, files);
            }

            return files;
        }
    }
}

    
