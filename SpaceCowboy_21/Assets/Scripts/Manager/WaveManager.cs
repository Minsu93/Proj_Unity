using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public float gameTime { get; set; } //���� �ð�
    public float waveTime { get; set; } //���� ���̺갡 ���۵� �ð�
    [SerializeField] private AnimationCurve waveTimeCurve;
    public int waveIndex { get; set; }   //���� ��ȣ 0 ~ 3

    private void Awake()
    {
        instance = this; 
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if(gameTime >= waveTime)
        {
            gameTime = 0f;
            NextLevel();
        }

    }

    void NextLevel()
    {
        waveIndex ++;
        waveTime = waveTimeCurve.Evaluate(waveIndex);
        Debug.Log("This wave is : " + waveIndex + " and NextLevel is : " + waveTime);
    }
}
