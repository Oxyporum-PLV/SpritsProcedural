using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    private List<Vector2> liPrincipalRoomPosition = new List<Vector2>(); //list roomPosition

    private int[] arrOrient = { 0, 1, 2, 3 }; // 0 = Right, 1 = left, 2 = top, 3 = bot
    Vector2 currentPos = Vector2.zero;
    int currentOrient = 0;

    [SerializeField] private int maxRoomCount = 15;
    [SerializeField] private int minRoomCount = 10;
    private int totalPrincipalRoom = 0;

    private int maxSecondaryRoomCount = 3;
    private int minSecondaryRoomCount = 1;
    private int totalSecondaryRoom = 0;

    private int idRoom = 0;
    private List<int> liIdLockedRoom = new List<int>();
    private int lockedRoomCount = 1;

    private List<int> liIdStartSecondaryRoom = new List<int>();
    private List<Vector2> liSecondaryRoomPosition = new List<Vector2>();
    private List<int> liIdKeyRoom = new List<int>();

    public List<GameObject> LiRoomGameObject = new List<GameObject>();
    private List<Room> LiScRoom = new List<Room>();

    private bool isInPlayMode = false;


    #region Unity Methode
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        foreach(GameObject room in LiRoomGameObject)
        {
            LiScRoom.Add(room.GetComponent<Room>());
        }

        totalPrincipalRoom = Random.Range(minRoomCount, maxRoomCount + 1);
        lockedRoomCount = Random.Range(1, 3);

        liIdLockedRoom.Add(Random.Range(2, (totalPrincipalRoom / 2) + 1));
        liIdStartSecondaryRoom.Add(Random.Range(liIdLockedRoom[0] - 2, liIdLockedRoom[0] - 1));
        if (lockedRoomCount == 2)
        {
            liIdLockedRoom.Add(Random.Range(liIdLockedRoom[0] + 2, totalPrincipalRoom));
            liIdStartSecondaryRoom.Add(Random.Range(liIdLockedRoom[1] - 2, liIdLockedRoom[1]));
        }
        //idKeyRoom = Random.Range(1, idLockedRoom);

        liPrincipalRoomPosition.Add(Vector2.zero); // startRoom
        idRoom++;
        RandomNextRoom();

        for (int i = 0; i < lockedRoomCount; i++)
        {
            totalSecondaryRoom = Random.Range(minSecondaryRoomCount, maxSecondaryRoomCount + 1);
            RandomSecondaryNextRoom(liPrincipalRoomPosition[liIdStartSecondaryRoom[i]]);
        }
        AddRooms();

        isInPlayMode = true;
    }

    private void OnDrawGizmos()
    {
        if (!isInPlayMode) return;

        for (int i = 0; i < totalPrincipalRoom; i++)
        {
            if (i == 0 || i == totalPrincipalRoom - 1)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            foreach(int idLockedRoom in liIdLockedRoom)
            {
                if (i == idLockedRoom)
                    Gizmos.color = Color.black;
            }

            Gizmos.DrawSphere(liPrincipalRoomPosition[i], 0.1f);

            if (i + 1 >= liPrincipalRoomPosition.Count) break;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(liPrincipalRoomPosition[i], liPrincipalRoomPosition[i + 1]);
        }

        for (int i = 0; i < liSecondaryRoomPosition.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(liSecondaryRoomPosition[i], 0.1f);
        }
    }
    #endregion

    private void AddRooms()
    {
        List<GameObject> liStartRoom = new List<GameObject>();
        List<GameObject> liEndRoom = new List<GameObject>();

        foreach (Room room in LiScRoom)
        {
            if (room.isStartRoom)
            {
                liStartRoom.Add(room.gameObject);
            }
            else if (room.isEndRoom)
            {
                liEndRoom.Add(room.gameObject);
            }
        }

        List<Room> liRoomUse = new List<Room>();

        GameObject startRoom = Instantiate(liStartRoom[Random.Range(0, liStartRoom.Count - 1)]) as GameObject;
        liRoomUse.Add(startRoom.GetComponent<Room>());
        startRoom.transform.position = liPrincipalRoomPosition[0];

        for (int i = 1; i < liPrincipalRoomPosition.Count -1; i++)
        {
            GameObject room = Instantiate(LiScRoom[Random.Range(0, liPrincipalRoomPosition.Count)].gameObject) as GameObject;
            liRoomUse.Add(room.GetComponent<Room>());
            room.transform.position = liPrincipalRoomPosition[i];

            if(i == liPrincipalRoomPosition.Count - 2)
            {
                GameObject endRoom = Instantiate(liEndRoom[Random.Range(0, liEndRoom.Count - 1)]) as GameObject;
                liRoomUse.Add(endRoom.GetComponent<Room>());
                endRoom.transform.position = liPrincipalRoomPosition[liPrincipalRoomPosition.Count - 1];
            }

            Vector2 nextOffset = liPrincipalRoomPosition[i - 1] - liPrincipalRoomPosition[i];
            Debug.Log("room : " + i + "Offset : " + nextOffset);
            if (nextOffset.x == 11)
            {
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i - 1].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.x == -11)
            {
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN); 
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i -1].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.y == 9)
            {
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN); 
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i -1].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.y == -9)
            {
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i - 1].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN);
            }
        }
    }

    private void RandomNextRoom()
    {
        for (int i = idRoom ; i < totalPrincipalRoom; i++)
        {
            AssignPrincipalRoomPos();

            liPrincipalRoomPosition.Add(currentPos);
            idRoom = i;
        }
    }

    private void AssignPrincipalRoomPos()
    {
        currentOrient = arrOrient[Random.Range(0, arrOrient.Length)];

        Vector2 currentTestPos = new Vector2();
        currentTestPos = currentPos;

        if (currentOrient == 0)
            currentTestPos.x += 11;
        else if (currentOrient == 1)
            currentTestPos.x += -11;
        else if (currentOrient == 2)
            currentTestPos.y += 9;
        else
            currentTestPos.y += -9;

        foreach (Vector2 pos in liPrincipalRoomPosition)
        {
            if(pos == currentTestPos)
            {
                AssignPrincipalRoomPos();
                return;
            }
        }
        currentPos = currentTestPos;
    }

    private void RandomSecondaryNextRoom(Vector2 branch)
    {
        currentPos = branch;
        for (int i = 0; i < totalSecondaryRoom; i++)
        {
            AssignSecondaryRoomPos(currentPos);
            liSecondaryRoomPosition.Add(currentPos);
        }
    }

    private void AssignSecondaryRoomPos(Vector2 branch)
    {
        currentOrient = arrOrient[Random.Range(0, arrOrient.Length)];
        Vector2 currentTestPos = new Vector2();
        currentTestPos = branch;

        if (currentOrient == 0)
            currentTestPos.x += 11;
        else if (currentOrient == 1)
            currentTestPos.x += -11;
        else if (currentOrient == 2)
            currentTestPos.y += 9;
        else
            currentTestPos.y += -9;

        foreach (Vector2 pos in liPrincipalRoomPosition)
        {
            if (pos == currentTestPos)
            {
                AssignSecondaryRoomPos(branch);
                return;
            }
        }
        foreach (Vector2 pos in liSecondaryRoomPosition)
        {
            if (pos == currentTestPos)
            {
                AssignSecondaryRoomPos(branch);
                return;
            }
        }
        currentPos = currentTestPos;
        Debug.Log(currentPos);
    }

}

//[System.Serializable]
//public class Rooms
//{
//    public enum RoomState { startRoom = 0, endRoom, normalRoom, keyRoom, lockedRoom, secretRoom}
//    public RoomState roomState = RoomState.startRoom;
//    public List<GameObject> LiRoom = new List<GameObject>();
//}
