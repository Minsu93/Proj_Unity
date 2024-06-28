using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tech Cube", menuName = "Weapon/TechCube", order = int.MaxValue)]
public class AlienTechCube : ScriptableObject
{
    //ID
    [SerializeField] private string cubeID;
    public string CubeID { get { return cubeID; } }

    //이미지
    [SerializeField] private Sprite cubeSprite;
    public Sprite CubeSprite { get {  return cubeSprite; } }

    //설명
    [SerializeField] private string cubeName;
    [SerializeField][Multiline(3)] private string description;

    //스텟. 적용되는 float수치는 퍼센트. int 수치는 증가/감소
    [SerializeField] private WeaponStats bonusStats;
    public WeaponStats BonusStats { get {  return bonusStats; } }

    //능력
    //발사 시
    public virtual void OnShootEvent()
    {
        Debug.Log("A");
    }
    //이동 시 
    //public virtual void OnMoveEvent(float moveDist)
    //{
    //    Debug.Log("B");
    //}
    //타격 시
    public virtual void OnImpactEvent()
    {
        Debug.Log("C");
    }
}
