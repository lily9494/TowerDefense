using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public LayerMask EnemiesLayer;
    public Enemy Target;
    public Transform TowerPivot;

    public float Damage;
    public float Firerate;
    public float Range;
    private float Delay;

    private IDamageMethode CurrentDamageMethodClass;
    
    // Start is called before the first frame update
    void Start()
    {
        CurrentDamageMethodClass= GetComponent<IDamageMethode>();
        if (CurrentDamageMethodClass == null)
        {
            Debug.LogError("not attached!");
        }

        else
        {
         CurrentDamageMethodClass.Init(Damage, Firerate);
        }
       
        Delay = 1/Firerate;
    }

    // Update is called once per frame
    public void Tick(){
        CurrentDamageMethodClass.DamageTick(Target);
        if (Target !=null) 
        {
            TowerPivot.transform.rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
        }
    }
}
