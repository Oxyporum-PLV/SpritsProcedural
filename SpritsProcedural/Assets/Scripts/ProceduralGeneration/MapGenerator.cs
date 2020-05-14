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
    public List<GameObject> LiStartRoom = new List<GameObject>();
    public List<GameObject> LiEndRoom = new List<GameObject>();
    private List<Room> LiScRoom = new List<Room>();
    public GameObject KeyRoom;
    private List<Room> liTriRoom = new List<Room>();

    public List<Room> liAllRoom = new List<Room>();

    public GameObject SecretRoom;

    private int lastSecondaryRoom = 0;

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
        liIdStartSecondaryRoom.Add(Random.Range(1, liIdLockedRoom[0]));
        if (lockedRoomCount == 2)
        {
            liIdLockedRoom.Add(Random.Range(liIdLockedRoom[0] + 2, totalPrincipalRoom));
            liIdStartSecondaryRoom.Add(Random.Range(liIdLockedRoom[0], liIdLockedRoom[1]));
            Debug.Log(liIdStartSecondaryRoom[1]);
        }
        //idKeyRoom = Random.Range(1, idLockedRoom);

        liPrincipalRoomPosition.Add(Vector2.zero); // startRoom
        idRoom++;
        RandomPrincipalNextRoom();

        AddPrincipalRooms();
        for (int i = 0; i < lockedRoomCount; i++)
        {
            totalSecondaryRoom = Random.Range(minSecondaryRoomCount, maxSecondaryRoomCount + 1);
            RandomSecondaryNextRoom(liPrincipalRoomPosition[liIdStartSecondaryRoom[i]]);
            AddSecondaryRoom(totalSecondaryRoom, i);
            liSecondaryRoomPosition.Clear();
           // lastSecondaryRoom = totalSecondaryRoom;

        }

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


    int v = 0;
    private void AddPrincipalRooms()
    {
        List<Room> liRoomUse = new List<Room>();

        GameObject startRoom = Instantiate(LiStartRoom[Random.Range(0, LiStartRoom.Count - 1)]) as GameObject;
        liRoomUse.Add(startRoom.GetComponent<Room>());
        liAllRoom.Add(liRoomUse[liRoomUse.Count - 1]);

        startRoom.transform.position = liPrincipalRoomPosition[0];

        for (int i = 1; i < liPrincipalRoomPosition.Count; i++)
        {
            if(i == liPrincipalRoomPosition.Count - 1)
            {
                GameObject room = Instantiate(LiEndRoom[Random.Range(0, LiEndRoom.Count - 1)]) as GameObject;
                liRoomUse.Add(room.GetComponent<Room>());
                liAllRoom.Add(liRoomUse[liRoomUse.Count - 1]);
                room.transform.position = liPrincipalRoomPosition[i];
            }
            else
            {
                GameObject room = Instantiate(LiScRoom[Random.Range(0, LiScRoom.Count)].gameObject) as GameObject;
                liRoomUse.Add(room.GetComponent<Room>());
                liAllRoom.Add(liRoomUse[liRoomUse.Count - 1]);
                room.transform.position = liPrincipalRoomPosition[i];
            }

            for (v = 0; v < liIdStartSecondaryRoom.Count; v++)
            {
                if (i == liIdStartSecondaryRoom[v])
                {
                    liTriRoom.Add(liRoomUse[liRoomUse.Count - 1]);
                    Debug.Log(liIdStartSecondaryRoom[v]);
                }
            }


            Vector2 nextOffset = liPrincipalRoomPosition[i - 1] - liPrincipalRoomPosition[i];
            
            if (nextOffset.x == 11)
            {

                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i - 1].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);

                foreach (int id in liIdLockedRoom)
                {
                    if (id == i)
                    {
                        liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.Wall);
                        liRoomUse[i - 1].LiScDoor[1].scDoor.SetState(Door.STATE.Wall);
                    }
                }
            }
            else if (nextOffset.x == -11)
            {
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN); 
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i -1].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);

                foreach (int id in liIdLockedRoom)
                {
                    if (id == i)
                    {
                        liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.Wall);
                        liRoomUse[i - 1].LiScDoor[0].scDoor.SetState(Door.STATE.Wall);
                    }
                }
            }
            else if (nextOffset.y == 9)
            {
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN); 
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i -1].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);

                foreach (int id in liIdLockedRoom)
                {
                    if (id == i)
                    {
                        liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.Wall);
                        liRoomUse[i - 1].LiScDoor[3].scDoor.SetState(Door.STATE.Wall);
                    }
                }
            }
            else if (nextOffset.y == -9)
            {
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);

                liRoomUse[i - 1].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN);

                foreach (int id in liIdLockedRoom)
                {
                    if (id == i)
                    {
                        liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.Wall);
                        liRoomUse[i - 1].LiScDoor[2].scDoor.SetState(Door.STATE.Wall);
                    }
                }
            }
        }
    }

    private void AddSecondaryRoom(int totalRoom, int j)
    {
        List<Room> liRoomUse = new List<Room>();
        for (int i = 0; i < totalRoom; i++)
        {
            if(i == totalRoom - 1)
            {
                GameObject keyRoom = Instantiate(KeyRoom) as GameObject; 
                liRoomUse.Add(keyRoom.GetComponent<Room>());
                liAllRoom.Add(liRoomUse[liRoomUse.Count - 1]);
                keyRoom.transform.position = liSecondaryRoomPosition[i];
                // keyRoom
            }
            else
            {
                GameObject room = Instantiate(LiScRoom[Random.Range(0, LiScRoom.Count)].gameObject) as GameObject;
                liRoomUse.Add(room.GetComponent<Room>());
                liAllRoom.Add(liRoomUse[liRoomUse.Count - 1]);
                room.transform.position = liSecondaryRoomPosition[i];
            }
            Vector2 nextOffset;
            if(i == 0)
            {
                nextOffset = liPrincipalRoomPosition[liIdStartSecondaryRoom[j]] - liSecondaryRoomPosition[i];
            }
            else
            {
                nextOffset = liSecondaryRoomPosition[i - 1] - liSecondaryRoomPosition[i];
            }

            if (nextOffset.x == 11)
            {

                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);
                if (i == 0)
                {
                    liTriRoom[j].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);
                }
                else
                    liRoomUse[i - 1].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.x == -11)
            {
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);
                if (i == 0)
                {
                    liTriRoom[j].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
                }
                else
                    liRoomUse[i - 1].LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.y == 9)
            {
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.WALL);
                if (i == 0)
                {
                    liTriRoom[j].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
                }
                else
                    liRoomUse[i - 1].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
            }
            else if (nextOffset.y == -9)
            {
                liRoomUse[i].LiScDoor[3].scDoor.SetState(Door.STATE.OPEN);
                liRoomUse[i].LiScDoor[1].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[0].scDoor.SetState(Door.STATE.WALL);
                liRoomUse[i].LiScDoor[2].scDoor.SetState(Door.STATE.WALL);
                if (i == 0)
                {
                    liTriRoom[j].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN);
                }
                else
                    liRoomUse[i - 1].LiScDoor[2].scDoor.SetState(Door.STATE.OPEN);
            }
        }
    }

    private void RandomPrincipalNextRoom()
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
    }
    
    public void AssignSecrectRoom()
    {
        currentOrient = arrOrient[Random.Range(0, arrOrient.Length)];
        int idRandomRoom = Random.Range(0, liAllRoom.Count - 1);
        Room testRoom = liAllRoom[idRandomRoom];

        Vector2 currentTestPos = new Vector2();
        currentTestPos = new Vector2(testRoom.position.x + 11, testRoom.position.y);

        for (int i = 0; i < liAllRoom.Count; i++)
        {
            if(i!= idRandomRoom)
            {
                if (currentTestPos == new Vector2(liAllRoom[i].transform.position.x, liAllRoom[i].transform.position.y))
                {
                    AssignSecrectRoom();
                    return;
                }
            }
        }

        GameObject secretRoom = Instantiate(SecretRoom);
        secretRoom.transform.position = currentTestPos;
        Room currentRoom = secretRoom.GetComponent<Room>();
        currentRoom.LiScDoor[1].scDoor.SetState(Door.STATE.OPEN);
        testRoom.LiScDoor[0].scDoor.SetState(Door.STATE.OPEN);
    }

}

//[System.Serializable]
//public class Rooms
//{
//    public enum RoomState { startRoom = 0, endRoom, normalRoom, keyRoom, lockedRoom, secretRoom}
//    public RoomState roomState = RoomState.startRoom;
//    public List<GameObject> LiRoom = new List<GameObject>();
//}
