using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    // ���ʸ� �����͸� �����ϴ� �Լ�
    public static void Save<T>(T data, string fileName)
    {
        // �����͸� JSON �������� ����ȭ
        string jsonData = JsonUtility.ToJson(data,true);

        // ���� ��� ����
        //string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string filePath = Path.Combine(Application.dataPath + "/Data", fileName);
        
        // JSON �����͸� ���Ϸ� ����
        File.WriteAllText(filePath, jsonData);

        Debug.Log($"Data saved to {filePath}");
    }

    // ���ʸ� �����͸� �ε��ϴ� �Լ�
    public static T Load<T>(string fileName)
    {
        // ���� ��� ����
        //string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string filePath = Path.Combine(Application.dataPath + "/Data", fileName);


        // ������ �����ϴ��� Ȯ��
        if (File.Exists(filePath))
        {
            // ���Ͽ��� JSON �����͸� �б�
            string jsonData = File.ReadAllText(filePath);

            // JSON �����͸� ��ü�� ������ȭ
            T data = JsonUtility.FromJson<T>(jsonData);

            Debug.Log($"Data loaded from {filePath}");
            return data;
        }
        else
        {
            Debug.LogWarning($"File not found: {filePath}");
            return default;
        }
    }
}
