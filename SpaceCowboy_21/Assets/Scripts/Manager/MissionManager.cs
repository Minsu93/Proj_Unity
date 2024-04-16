using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    /// <summary>
    /// ���� ���������� �ִ� �̼� ����.
    /// 1. Total Objective : �̴ϸʿ� ǥ�õ� ��ǥ�� �Ǵ� �༺(or ������) ǥ�ø� ����
    /// 2. ��ü �̼��� ��, �޼��� �̼��� �� : ���൵ ǥ�ø� ���� 
    /// 3. �����ִ� ������ �� : ���� �̼� ������ ����(����)
    /// </summary>
    /// 
    public static MissionManager instance;
    public GameObject[] totalObjectives;
    public List<GameObject> unCompletedObjectives = new List<GameObject>();
    public List<GameObject> completedObjectives = new List<GameObject>();

    public MissionUI missionUI;
    public MinimapIndicator minimapIndicator;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        foreach (GameObject obj in totalObjectives) { unCompletedObjectives.Add(obj); }
    }

    //�������� ���� �� ����
    public void UpdateTotalObjectives(GameObject[] objectives)
    {
        //totalObjectives = new GameObject[objectives.Length];
        totalObjectives = objectives;

        completedObjectives.Clear() ;
        unCompletedObjectives.Clear() ;
        foreach (GameObject obj in totalObjectives) { unCompletedObjectives.Add(obj); }

        missionUI.UpdateMissionUI(totalObjectives.Length, 0);
        minimapIndicator.SetMissionObjectives(totalObjectives);
    }

    //������Ʈ �Ϸ� �� ���� 
    public void CompleteMissionObjectives(GameObject completedObj)
    {
        //������Ʈ �Ϸ� ǥ�� 
        unCompletedObjectives.Remove(completedObj);
        completedObjectives.Add(completedObj);
        missionUI.UpdateMissionUI(totalObjectives.Length, completedObjectives.Count);
        //������Ʈ�� ��� �Ϸ������� GameManager���� Win
    }
}
