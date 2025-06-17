using UnityEngine;
using System;
using System.IO;

namespace UIToolkitDemo
{
    // ���ڽ� SaveData ��д�����̵���
    public class FileManager
    {
        // ���ļ�����д��ָ���ļ������ļ�
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
                // ��¼д���ļ�ʧ�ܵĴ�����Ϣ
                Debug.LogError($"�޷�д�� {fullPath}���쳣��Ϣ: {e}");
                return false;
            }
        }

        // ��ָ���ļ������ļ��м�������
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
                // ��¼��ȡ�ļ�ʧ�ܵĴ�����Ϣ
                Debug.LogError($"�޷���ȡ {fullPath}���쳣��Ϣ: {e}");
                result = "";
                return false;
            }
        }

    }
}