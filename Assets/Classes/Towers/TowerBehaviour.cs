using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public Transform TowerPivot;

    public float Damage;
    public float FireRate;
    public float Range;
    private float Delay;
    
    // Start is called before the first frame update
    void Start()
    {
        Delay = 1/FireRate;
    }

    // Update is called once per frame
    public void Tick(){
        if (Target !=null) 
        {
            TowerPivot.transform.rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        }
    }
}
