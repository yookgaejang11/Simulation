using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum UnitType
{
    Player,
    Enemy
}
public enum UnitInfo
{
    none,
    follow,
    chase,
    attack,
}
public enum PlayerUnitType
{
    warrior,
    archor,
    mage
}

public class GameManager : MonoBehaviour
{
    
    private static GameManager instance;
    public GameObject leader;
    public CinemachineVirtualCamera camera;
    public List<Unit> unitsList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Unit[] units;
        units = GameObject.FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.GetComponent<Unit>().unitType == UnitType.Player)
            {
                unitsList.Add(unit);
            }
        }

        unitsList[0].isLeader = true;
        camera.Follow = unitsList[0].transform;
        leader = unitsList[0].gameObject;
        leader.GetComponent<Unit>().cr = leader.AddComponent<CharacterController>();
        leader.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);


    }

    // Update is called once per frame
    void Update()
    {
        
        ChangeLeader();
    }

    void ChangeLeader()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

           leader.GetComponent<Unit>().isLeader = false;
            Destroy(leader.GetComponent<CharacterController>());
            unitsList[0].isLeader = true;
            camera.Follow = unitsList[0].transform;
            leader = unitsList[0].gameObject;
            leader.GetComponent<Unit>().cr =leader.AddComponent<CharacterController>();
            leader.GetComponent<CharacterController>().center = new Vector3(0,1,0);


        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            leader.GetComponent<Unit>().isLeader = false;
            Destroy(leader.GetComponent<CharacterController>());
            Destroy(leader.GetComponent<CharacterController>());
            unitsList[1].isLeader = true;
            camera.Follow = unitsList[1].transform;
            leader = unitsList[1].gameObject;
            leader.GetComponent<Unit>().cr = leader.AddComponent<CharacterController>();
            leader.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            leader.GetComponent<Unit>().isLeader = false;
            Destroy(leader.GetComponent<CharacterController>());
            unitsList[2].isLeader = true;
            camera.Follow = unitsList[2].transform;
            leader = unitsList[2].gameObject;
            leader.GetComponent<Unit>().cr = leader.AddComponent<CharacterController>();
            leader.GetComponent<CharacterController>().center = new Vector3(0, 1, 0);

        }
    }

    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }

}
