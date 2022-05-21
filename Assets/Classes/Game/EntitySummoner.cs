using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySummoner : MonoBehaviour
{
    public static List<Enemy> EnemiesInGame;
    public static List<Transform> EnemiesInGameTransform;
    public static Dictionary<int, GameObject> EnemyPrefabs;
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;

    private static bool IsInitialized;

    public static void Init()
    {
        if(!IsInitialized){
        EnemyPrefabs= new Dictionary<int, GameObject>();
        EnemyObjectPools= new Dictionary<int, Queue<Enemy>>();
        EnemiesInGameTransform= new List<Transform>();
        EnemiesInGame =new List<Enemy>();

        EnemySummonData[] Enemies = Resources.LoadAll<EnemySummonData>("Enemies");
        Debug.Log(Enemies[0].name);
        foreach(EnemySummonData enemy in Enemies){
        EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
        EnemyObjectPools.Add(enemy.EnemyID , new Queue<Enemy>());
            }  
        IsInitialized=true;
        }
        else
        {
            {
                Debug.Log("ENTITYSUMMONER: THIS CLASS IS ALREADY INTIALIZED");
            }
        }
     
    }

    public static Enemy SummonEnemy(int EnemyID){
Enemy SummonedEnemy=null;
if(EnemyPrefabs.ContainsKey(EnemyID)){
Queue<Enemy> RefrencedQueue= EnemyObjectPools[EnemyID];

        if(RefrencedQueue.Count > 0){
                SummonedEnemy=RefrencedQueue.Dequeue();
                SummonedEnemy.Init();
                SummonedEnemy.gameObject.SetActive(true);
        }
        else{
            GameObject NewEnemy= Instantiate(EnemyPrefabs[EnemyID], GameLoopManager.NodePositions[0],Quaternion.identity) ;
            SummonedEnemy=NewEnemy.GetComponent<Enemy>();
            SummonedEnemy.Init();
        }
}
    else{

    return null;
    }
    EnemiesInGameTransform.Add(SummonedEnemy.transform);
    EnemiesInGame.Add(SummonedEnemy);
    SummonedEnemy.ID=EnemyID;
    return SummonedEnemy;
    }
   
    public static void RemoveEnemy(Enemy EnemyToRemove)
    {
    EnemyObjectPools[EnemyToRemove.ID].Enqueue(EnemyToRemove);
    EnemyToRemove.gameObject.SetActive(false);
     EnemiesInGameTransform.Remove(EnemyToRemove.transform);
    EnemiesInGame.Remove(EnemyToRemove);
    }
}
