using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrone : MonoBehaviour
{
    public int droneSlots { get; set; }
    public List<DroneItem> drones = new List<DroneItem>();

    //드론 아이템을 먹으면 생성 가능한지 물어본다. 슬롯 이하의 개수면 먹을 수 있다. bool 리턴
    public bool AddDrone(GameObject droneObj)
    {
        if(drones.Count < droneSlots)
        {
            GameObject drone = GameManager.Instance.poolManager.GetPoolObj(droneObj, 5);
            drone.transform.position = transform.position;

            DroneItem dronitem = drone.GetComponent<DroneItem>();
            dronitem.InitializeDrone();
            drones.Add(dronitem);

            //UI업데이트
            GameManager.Instance.playerManager.UpdateDroneUI();

            return true;

        }
        return false;
    }

    //특정 슬롯의 드론을 사용하도록 명령을 내린다. 
    public void UseDrone(int index)
    {
        if(drones.Count >= index + 1)
        {
            drones[index].UseDrone(GameManager.Instance.playerManager.playerBehavior.mousePos, Quaternion.identity);
            drones.RemoveAt(index);

            //UI업데이트
            GameManager.Instance.playerManager.UpdateDroneUI();
        }
    }

    //void RemoveDrone(int index)
    //{
    //    drones.RemoveAt(index);
    //    //UI업데이트
    //    GameManager.Instance.playerManager.UpdateDroneUI();
    //}
}
