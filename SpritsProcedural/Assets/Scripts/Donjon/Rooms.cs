using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Rooms : MonoBehaviour
{
    public enum RoomState { startRoom = 0, endRoom, normalRoom, keyRoom, lockedRoom, secretRoom }
    public RoomState roomState = RoomState.startRoom;
    public List<GameObject> LiRoom = new List<GameObject>();

    public void ChoiseRooms()
    {
      
            roomState = (RoomState)Random.Range(0, 5);
            Debug.Log(roomState);

    }
}
