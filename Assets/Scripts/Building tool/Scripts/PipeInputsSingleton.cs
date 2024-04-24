using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeInputsSingleton : MonoBehaviour
{
    public static PipeInputsSingleton  pipeInputs { get; private set; } 

    [SerializeField] private GameObject lineObj;
    [SerializeField] private Material lineObjMatFuel;
    [SerializeField] private Material lineObjMatCoolant;
    [SerializeField] private LayerMask raycastMask;
    private List<GameObject> inputsList;

    void Awake(){
        pipeInputs = this;

        inputsList = new List<GameObject>();

        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None)){
            if(obj.layer == LayerMask.NameToLayer("Snapping Point Pipe Input")) {
                inputsList.Add(obj);
            }
        }
    }

    public void AddInput(GameObject toBeAdded){

        inputsList.Add(toBeAdded);

    }

    public void DisplayInputSnaps(){

        foreach (GameObject pipeInput in inputsList){

            if((pipeInput.transform.childCount <= 0) && pipeInput.GetComponent<Collider>().enabled) {

                Physics.Raycast(pipeInput.transform.position, pipeInput.transform.forward, out RaycastHit hit, 20, raycastMask);
                
                GameObject obj = Instantiate(lineObj, pipeInput.transform.position, pipeInput.transform.rotation, pipeInput.transform);
                obj.transform.rotation *= Quaternion.Euler(-90, 0, 0);
                obj.transform.localScale = new Vector3( obj.transform.localScale.x, hit.distance, obj.transform.localScale.z);
                if( pipeInput.GetComponent<FluidInput>().GetInputsTo().GetFluidType() == FluidContainer.FluidType.fuel ) obj.GetComponent<Renderer>().material = lineObjMatFuel;
                else if( pipeInput.GetComponent<FluidInput>().GetInputsTo().GetFluidType() == FluidContainer.FluidType.coolant ) obj.GetComponent<Renderer>().material = lineObjMatCoolant;
            }

        }

    }
    public void DestroyDisplayInputSnaps(){

        foreach (GameObject pipeInput in inputsList){

            if(pipeInput.transform.childCount > 0) if( pipeInput.transform.GetChild(0).gameObject != null ) Destroy(pipeInput.transform.GetChild(0).gameObject);

        }

    }
}
