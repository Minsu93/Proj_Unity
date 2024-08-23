using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Data/StageData", order = int.MaxValue)]
public class StageData : ScriptableObject
{
    [SerializeField] private string stageName;
    public string StageName { get { return stageName; } }
    [SerializeField] private string sceneAddress;
    public string SceneAddress { get { return sceneAddress; } }

    [SerializeField] private Sprite stageImage;
    public Sprite StageImage { get { return stageImage; }}

    [TextArea]
    [SerializeField] private string stageDescription;
    public string StageDescription { get { return stageDescription; } }
}
