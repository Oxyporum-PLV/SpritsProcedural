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


    [SerializeField] private List<Rooms> liRooms = new List<Rooms>();

    private bool isInPlayMode = false;


    #region Unity Methode
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        totalPrincipalRoom = Random.Range(minRoomCount, maxRoomCount + 1);
        lockedRoomCount = Random.Range(1, 3);

        liIdLockedRoom.Add(Random.Range(3, (totalPrincipalRoom / 2) + 1));
        liIdStartSecondaryRoom.Add(Random.Range(liIdLockedRoom[0] - 2, liIdLockedRoom[0] - 1));
        if (lockedRoomCount == 2)
        {
            liIdLockedRoom.Add(Random.Range(liIdLockedRoom[0] + 2, totalPrincipalRoom));
            liIdStartSecondaryRoom.Add(Random.Range(liIdLockedRoom[0] - 2, liIdLockedRoom[0] - 1));
        }
        //idKeyRoom = Random.Range(1, idLockedRoom);

        liPrincipalRoomPosition.Add(Vector2.zero); // startRoom
        idRoom++;
        RandomNextRoom();

        totalSecondaryRoom = Random.Range(minSecondaryRoomCount, maxSecondaryRoomCount + 1);
        RandomSecondaryNextRoom(liPrincipalRoomPosition[liIdStartSecondaryRoom[0]]);
        if (lockedRoomCount == 2)
        {
            totalSecondaryRoom = Random.Range(minSecondaryRoomCount, maxSecondaryRoomCount + 1);
            RandomSecondaryNextRoom(liPrincipalRoomPosition[liIdStartSecondaryRoom[0]]);
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

            if (i + 1 >= liPrincipalRoomPosition.Count) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(liPrincipalRoomPosition[i], liPrincipalRoomPosition[i + 1]);
        }
    }
    #endregion

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
            currentTestPos.x += 1;
        else if (currentOrient == 1)
            currentTestPos.x += -1;
        else if (currentOrient == 2)
            currentTestPos.y += 1;
        else
            currentTestPos.y += -1;

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
        for (int i = 0; i < totalSecondaryRoom; i++)
        {
            AssignSecondaryRoomPos(branch);
            liSecondaryRoomPosition.Add(currentPos);
            //idRoom = i;
        }
    }

    private void AssignSecondaryRoomPos(Vector2 branch)
    {
        currentOrient = arrOrient[Random.Range(0, arrOrient.Length)];
        Vector2 currentTestPos = new Vector2();
        currentTestPos = branch;

        if (currentOrient == 0)
            currentTestPos.x += 1;
        else if (currentOrient == 1)
            currentTestPos.x += -1;
        else if (currentOrient == 2)
            currentTestPos.y += 1;
        else
            currentTestPos.y += -1;

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

}

[System.Serializable]
public class Rooms
{
    public enum RoomState { startRoom = 0, endRoom, normalRoom, keyRoom, lockedRoom, secretRoom}
    public RoomState roomState = RoomState.startRoom;
    public List<GameObject> LiRoom = new List<GameObject>();
}
