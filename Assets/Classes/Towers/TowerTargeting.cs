using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class TowerTargeting 
{
    public enum TargetType
    {
        First,
        Last,
        Close

    }
    public static Enemy GetTarget(TowerBehaviour CurrentTower, TargetType TargetMethod)
    {
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position,CurrentTower.Range,CurrentTower.EnemiesLayer);
        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length,Allocator.TempJob);
        NativeArray<Vector3> NodePositions = new NativeArray<Vector3>(GameLoopManager.NodePositions,Allocator.TempJob);
        NativeArray<float> NodeDistances= new NativeArray<float>(GameLoopManager.NodeDistances, Allocator.TempJob);
        NativeArray<int> EnemyToIndex= new NativeArray<int>(new int[]{1},Allocator.TempJob);
        int EnemyIndexToReturn= -1;

        for(int i=0; i<EnemiesToCalculate.Length;i++)
        {
            Enemy CurrentEnemy =EnemiesInRange[i].transform.parent.GetComponent<Enemy>();
           int EnemyIndexInList =EntitySummoner.EnemiesInGame.FindIndex(x=> x==CurrentEnemy);
           
            EnemiesToCalculate[i]= new EnemyData(CurrentEnemy.transform.position, CurrentEnemy.NodeIndex, CurrentEnemy.Health, EnemyIndexInList);
        }
        SearchForEnemy EnemySearchJob =new SearchForEnemy
        {
            _EnemiesToCalculate=EnemiesToCalculate,
            _NodePositions= NodePositions,
            _NodeDistances= NodeDistances,
            _EnemyToIndex = EnemyToIndex ,
            CompareValue = Mathf.Infinity,
            TargetingType = (int) TargetMethod,
            TowerPosition= CurrentTower.transform.position
        };
        switch((int)TargetMethod)
        {
            case 0:
                EnemySearchJob.CompareValue= Mathf.Infinity;
                break;
            case 1:
                EnemySearchJob.CompareValue= Mathf.NegativeInfinity;
                break;
            case 2:
            goto case 0;

        }

        JobHandle dependency= new JobHandle();
        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length,dependency);

        SearchJobHandle.Complete();
        Debug.Log(EnemyToIndex[0]);
        Debug.Log(EnemiesToCalculate.Length);
        if(EnemiesToCalculate.Length>EnemyToIndex[0])
        EnemyIndexToReturn =EnemiesToCalculate[EnemyToIndex[0]].EnemyIndex;
        else if(EnemiesToCalculate.Length<=EnemyToIndex[0])
        EnemyIndexToReturn =-1;

        EnemiesToCalculate.Dispose();
        NodeDistances.Dispose();
        NodePositions.Dispose();
        EnemyToIndex.Dispose();

        if(EnemyIndexToReturn==-1)
        {
            return null;
        }
        return EntitySummoner.EnemiesInGame[EnemyIndexToReturn];
    }
    struct EnemyData{
        public EnemyData(Vector3 position, int nodeindex , float hp , int enemyindex )
        {
            EnemyPosition = position;
            NodeIndex=nodeindex;
            Health=hp;
            EnemyIndex=enemyindex;
        }

    public Vector3 EnemyPosition;
    public int NodeIndex;
    public int EnemyIndex;
    public float Health;
    }

    struct SearchForEnemy :IJobFor
    {
        public  NativeArray<EnemyData> _EnemiesToCalculate;
        public NativeArray<Vector3> _NodePositions ;
        public NativeArray<float> _NodeDistances;
        public NativeArray<int> _EnemyToIndex;
        public Vector3 TowerPosition;
        public float CompareValue;
        public int TargetingType;
        public void Execute(int index)
        {
             float CurrentEnemyDistanceToEnd=0 ;
             float DistanceToEnemy=0;
            switch(TargetingType){
                case 0: //first
               CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                if(CurrentEnemyDistanceToEnd<CompareValue)
                {
                    _EnemyToIndex[0]=index;
                    CompareValue =CurrentEnemyDistanceToEnd;
                }
                 break;

                case 1: //last
                CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                if(CurrentEnemyDistanceToEnd>CompareValue)
                {
                    _EnemyToIndex[0]=index;
                    CompareValue =CurrentEnemyDistanceToEnd;
                }
                 break;

                case 2: //close
                DistanceToEnemy = Vector3.Distance(TowerPosition,_EnemiesToCalculate[index].EnemyPosition);
                if(DistanceToEnemy<CompareValue)
                {
                    _EnemyToIndex[0]=index;
                    CompareValue =DistanceToEnemy;
                }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData EnemyToEvaluate)
        {
            float FinalDistance = Vector3.Distance(EnemyToEvaluate.EnemyPosition, _NodePositions[EnemyToEvaluate.NodeIndex]);

            for(int i=EnemyToEvaluate.NodeIndex; i< _NodeDistances.Length; i++)
            {
                FinalDistance +=_NodeDistances[i];
            }
            return FinalDistance;
        }
    }
}
