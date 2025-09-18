using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class Status : ScriptableObject
{
    public int level;
    public float maxExp;
    public float curExp;
    public float MaxHp;
    public float curHp;
    public float MaxMp;
    public float curMp;
    public float attack;
    public float attackSpeed;
    public float attackDistance;
    public float invenRan;
    public float critical;
}
