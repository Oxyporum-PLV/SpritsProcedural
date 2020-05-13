using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    private List<Vector2> liRoomPosition = new List<Vector2>(); //list roomPosition

    private int[] arrOrient = { 0, 1, 2, 3 }; // 0 = Right, 1 = left, 2 = top, 3 = bot

    Vector2 currentPos = Vector2.zero;
    int currentOrient = 0;

    private int totalRoom = 15;

    private int idRoom = 0;

    private bool isInPlayMode = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        isInPlayMode = true;
        liRoomPosition.Add(Vector2.zero); // startRoom
        idRoom++;
        RandomNextRoom();
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (!isInPlayMode) return;
        for (int i = 0; i < totalRoom; i++)
        {
            if (i == 0 || i == totalRoom - 1)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawSphere(liRoomPosition[i], 0.1f);

            if (i + 1 >= liRoomPosition.Count) return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(liRoomPosition[i], liRoomPosition[i + 1]);
        }
    }

    private void RandomNextRoom()
    {
        for (int i = idRoom ; i < totalRoom; i++)
        {
            AssignRoomPos();

            liRoomPosition.Add(currentPos);
            idRoom = i;
        }
    }

    private void AssignRoomPos()
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

        foreach (Vector2 pos in liRoomPosition)
        {
            if(pos == currentTestPos)
            {
                AssignRoomPos();
                return;
            }

        }
        currentPos = currentTestPos;


    }

}
