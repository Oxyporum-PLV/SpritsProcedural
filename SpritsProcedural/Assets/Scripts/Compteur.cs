﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compteur : MonoBehaviour
{
    public static Compteur Instance = null;

    public float currentCompteur = 0;
    public float _compteurmax = 6;
    Text text;
    
    
    public Room roomStart1, roomStart2 , roomEnd1, roomEnd2, roomSecret;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        text = GetComponent<Text>();
        
        currentCompteur = Random.Range(3, 5);

    }

    // Update is called once per frame
    void Update()
    {

        text.text = currentCompteur.ToString("0");
        CompteurStart();
    }

    public void CompteurStart()
    {
        currentCompteur -= Time.deltaTime;

        if (currentCompteur <= 0)
        {

            currentCompteur = _compteurmax;
            MapGenerator.Instance.AssignSecrectRoom();
        }
        

    }

    public void CompteurInscrease()
    {
        currentCompteur -= 0.5f;
        Debug.Log("-1");
    }
    
}