using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tech Cube", menuName = "Weapon/TechCube", order = int.MaxValue)]
public class AlienTechCube : ScriptableObject
{
    //ID
    [SerializeField] private string cubeID;
    public string CubeID { get { return cubeID; } }

    //�̹���
    [SerializeField] private Sprite cubeSprite;
    public Sprite CubeSprite { get {  return cubeSprite; } }

    //����
    [SerializeField] private string cubeName;
    [SerializeField][Multiline(3)] private string description;

    //����. ����Ǵ� float��ġ�� �ۼ�Ʈ. int ��ġ�� ����/����
    [SerializeField] private WeaponStats bonusStats;
    public WeaponStats BonusStats { get {  return bonusStats; } }

    //�ɷ�
    //�߻� ��
    public virtual void OnShootEvent()
    {
        Debug.Log("A");
    }
    //�̵� �� 
    //public virtual void OnMoveEvent(float moveDist)
    //{
    //    Debug.Log("B");
    //}
    //Ÿ�� ��
    public virtual void OnImpactEvent()
    {
        Debug.Log("C");
    }
}
