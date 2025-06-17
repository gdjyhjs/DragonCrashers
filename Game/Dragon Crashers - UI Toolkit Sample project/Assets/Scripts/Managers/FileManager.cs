using UnityEngine;
using System;
using System.IO;

namespace UIToolkitDemo
{
    // 用于将 SaveData 读写到磁盘的类
    public class FileManager
    {
        // 将文件内容写入指定文件名的文件
        public static bool WriteToFile(string fileName, string fileContents)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);

            try
            {
                File.WriteAllText(fullPath, fileContents);
                return true;
            }
            catch (Exception e)
            {
                // 记录写入文件失败的错误信息
                Debug.LogError($"无法写入 {fullPath}，异常信息: {e}");
                return false;
            }
        }

        // 从指定文件名的文件中加载内容
        public static bool LoadFromFile(string fileName, out string result)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, "");
            }
            try
            {
                result = File.ReadAllText(fullPath);
                return true;
            }
            catch (Exception e)
            {
                // 记录读取文件失败的错误信息
                Debug.LogError($"无法读取 {fullPath}，异常信息: {e}");
                result = "";
                return false;
            }
        }

    }
}