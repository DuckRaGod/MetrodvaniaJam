using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,IInteractable{
    public Player player;
    public float damage;

    public void Interact(){
        player.Hit((int)damage);
    }
}