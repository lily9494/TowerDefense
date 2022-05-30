using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask PlacementCheckMask;
    [SerializeField] private LayerMask PlacementCollidMask;
    [SerializeField] private Camera PlayerCamera;
    // Start is called before the first frame update
    private GameObject currentPlacingTower;
    

    // Update is called once per frame
    void Update()
    {
        if(currentPlacingTower != null)
        {
            Ray camray=PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit HitInfo;
            if(Physics.Raycast(camray,out HitInfo,100f,PlacementCollidMask)){
                currentPlacingTower.transform.position=HitInfo.point;
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                Destroy(currentPlacingTower);
                currentPlacingTower=null;
                return;
            }
            if(Input.GetMouseButtonDown(0) && HitInfo.collider.gameObject !=null)  
            {
                if(!HitInfo.collider.gameObject.CompareTag("CanTPlace"))
                {
                    BoxCollider TowerCollider= currentPlacingTower.gameObject.GetComponent<BoxCollider>();
                   TowerCollider.isTrigger=true;
                    Vector3 BoxCenter = currentPlacingTower.gameObject.transform.position +TowerCollider.center;
                    Vector3 HalfExtents = TowerCollider.size/2;
                    if(!Physics.CheckBox(BoxCenter,HalfExtents,Quaternion.identity,PlacementCheckMask,QueryTriggerInteraction.Ignore))
                   {
                       GameLoopManager.TowersInGame.Add(currentPlacingTower.GetComponent<TowerBehaviour>());

                       TowerCollider.isTrigger=false;
                       currentPlacingTower=null;
                   } 
                }
              
            }
        }
    }
    public void setTowerPlace(GameObject tower){
currentPlacingTower= Instantiate(tower,Vector3.zero,Quaternion.identity);
    }
}
