using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool open;
    private float countDown;

    private void Start() {
        open = false;
        countDown = 0;
    }

    private void Update() {
        if(countDown > 0) countDown -= Time.deltaTime;
    }

    public void OpenClose() {

        if(countDown > 0) return;

        if(open) animator.Play("DoorClose");
        else animator.Play("DoorOpen");

        open = !open;

        countDown = 2.0f;

    }
}
