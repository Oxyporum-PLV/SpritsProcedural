using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compteur : MonoBehaviour
{
    public static Compteur Instance = null;

    public float currentCompteur = 0;
    public float _compteurmax = 6;
    Text text;
    private bool IsActivateSecretRoom = false;
    
    
    public Room roomStart1, roomStart2 , roomEnd1, roomEnd2, roomSecret;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        text = GetComponent<Text>();
        
        currentCompteur = Random.Range(4, 6);
    }

    // Update is called once per frame
    void Update()
    {

        text.text = currentCompteur.ToString("0");
        
    }

    public void CompteurStart()
    {
        if (IsActivateSecretRoom)
        {
            return;
        }
        currentCompteur -= 1;

        if (currentCompteur <= 0)
        {
            IsActivateSecretRoom = true;
            DesactiveText();
            currentCompteur = _compteurmax;
            MapGenerator.Instance.AssignSecrectRoom();
        }

        
    }
    public void DesactiveText()
    {
        text.text = "";
    }

}
