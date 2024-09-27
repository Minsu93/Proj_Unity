using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public int droneSlots { get; set; }
    public List<DroneItem> drones = new List<DroneItem>();
    [SerializeField] private Vector2[] droneSlotPos = new Vector2[3];

    //��� �������� ������ ���� �������� �����. ���� ������ ������ ���� �� �ִ�. bool ����
    public bool AddDrone(GameObject droneObj)
    {
        if(drones.Count < droneSlots)
        {
            GameObject drone = GameManager.Instance.poolManager.GetPoolObj(droneObj, 5);
            drone.transform.position = transform.position;

            DroneItem dronitem = drone.GetComponent<DroneItem>();
            dronitem.InitializeDrone();
            drones.Add(dronitem);

            //UI������Ʈ
            GameManager.Instance.playerManager.UpdateDroneUI();
            RepositionDrones();
            return true;

        }
        return false;
    }

    //Ư�� ������ ����� ����ϵ��� ����� ������. 
    public void UseDrone(int index)
    {
        if(drones.Count >= index + 1)
        {
            drones[index].UseDrone(GameManager.Instance.playerManager.playerBehavior.mousePos, Quaternion.identity);
            drones.RemoveAt(index);

            //UI������Ʈ
            GameManager.Instance.playerManager.UpdateDroneUI();
            RepositionDrones();
        }
    }


    //���� �ֱٿ� ���� ����� �����Ѵ�. 
    public bool RemoveDrone()
    {
        if (drones.Count > 0)
        {
            int index = drones.Count - 1;
            drones[index].EndUseDrone();
            drones.RemoveAt(index);
            //UI������Ʈ
            GameManager.Instance.playerManager.UpdateDroneUI();
            RepositionDrones();
            return true;
        }
        else return false;
        
    }

    void RepositionDrones()
    {
        for(int i = 0; i < drones.Count; i++)
        {
            drones[i].dronePos = droneSlotPos[i];
        }
    }
}
