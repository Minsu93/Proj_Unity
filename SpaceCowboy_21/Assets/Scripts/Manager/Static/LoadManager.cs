using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    // 제너릭 데이터를 저장하는 함수
    public static void Save<T>(T data, string fileName)
    {
        // 데이터를 JSON 형식으로 직렬화
        string jsonData = JsonUtility.ToJson(data,true);

        // 파일 경로 설정
        //string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string filePath = Path.Combine(Application.dataPath + "/Data", fileName);
        
        // JSON 데이터를 파일로 저장
        File.WriteAllText(filePath, jsonData);

        Debug.Log($"Data saved to {filePath}");
    }

    // 제너릭 데이터를 로드하는 함수
    public static T Load<T>(string fileName)
    {
        // 파일 경로 설정
        //string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string filePath = Path.Combine(Application.dataPath + "/Data", fileName);


        // 파일이 존재하는지 확인
        if (File.Exists(filePath))
        {
            // 파일에서 JSON 데이터를 읽기
            string jsonData = File.ReadAllText(filePath);

            // JSON 데이터를 객체로 역직렬화
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
