using CreativeSpore.SuperTilemapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour {

    public bool isStartRoom = false;
    public bool isEndRoom = false;
    public bool isSecretRoom = false;
	public Vector2Int position = Vector2Int.zero;
    //public bool up, down, left, right, openUp,openDown ,openLeft,openRight;
    //public int type;
    // Compteur :
    public GameObject compteur;

    private TilemapGroup _tilemapGroup;
    

    [Tooltip("Element0 = Right, Element1 = Left, Element2 = Top, Element3 = Bot")]
    public List<door> LiScDoor = new List<door>();

	public static List<Room> allRooms = new List<Room>();

    void Awake()
    {
        
        _tilemapGroup = GetComponentInChildren<TilemapGroup>();
		allRooms.Add(this);
	}

	private void OnDestroy()
	{
		allRooms.Remove(this);
	}

	void Start () {

        
 
        if (isStartRoom)
        {
            OnEnterRoom();
            compteur.SetActive(false);
        }
       

        if (isSecretRoom)
        {
            compteur.SetActive(false);
        }
        

        if (isEndRoom)
        {
            compteur.SetActive(false);
        }
       

    }


    public void OnEnterRoom()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        Bounds cameraBounds = _GetWorldRoomBounds();
        cameraFollow.SetBounds(cameraBounds);
		Player.Instance.EnterRoom(this);
        compteur.SetActive(true);
       
    }


    private Bounds _GetLocalRoomBounds()
    {
		Bounds roomBounds = new Bounds(Vector3.zero, Vector3.zero);
		if (_tilemapGroup == null)
			return roomBounds;

		foreach (STETilemap tilemap in _tilemapGroup.Tilemaps)
		{
			Bounds bounds = tilemap.MapBounds;
			roomBounds.Encapsulate(bounds);
		}
		return roomBounds;
    }

    private Bounds _GetWorldRoomBounds()
    {
        Bounds result = _GetLocalRoomBounds();
        result.center += transform.position;
        return result;
    }

	public bool Contains(Vector3 position)
	{
		position.z = 0;
		return (_GetWorldRoomBounds().Contains(position));
	}

    
}

[System.Serializable]
public class door
{
    public bool itNeedKey = false;
    public bool isOpen = false;
    public Door scDoor; 
}
