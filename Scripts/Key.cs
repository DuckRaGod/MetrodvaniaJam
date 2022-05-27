using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour,IInteractable{
    public GameObject door;

    public void Interact(){
        if(door == null) return;
        Destroy(door);
        Destroy(gameObject);
    }
}
