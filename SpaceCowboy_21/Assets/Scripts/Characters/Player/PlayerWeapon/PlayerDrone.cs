using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public int droneSlots { get; set; }
    public List<DroneItem> drones = new List<DroneItem>();

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
        }
    }

    //void RemoveDrone(int index)
    //{
    //    drones.RemoveAt(index);
    //    //UI������Ʈ
    //    GameManager.Instance.playerManager.UpdateDroneUI();
    //}
}
