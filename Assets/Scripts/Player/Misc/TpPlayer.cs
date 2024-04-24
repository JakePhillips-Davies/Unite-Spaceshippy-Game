using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpPlayer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Transform destination;
    private void Start() {
        player = FindAnyObjectByType<PlayerMovement>().gameObject;
    }
    public void Tp(){
        player.transform.position = destination.position;
    }
}
