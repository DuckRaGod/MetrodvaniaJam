using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour{
    public SpriteRenderer sr;
    public Sprite Idle;
    public Sprite Walk;
    public Sprite Fall;
    public Sprite Jump;
    public Player player;

    public void FixedUpdate(){
        if(player.rb.velocity.y > 0.01f) sr.sprite = Jump;
        else if(player.rb.velocity.y < -0.01f) sr.sprite = Fall;
        else if(Mathf.Abs(player.rb.velocity.x) > 0.01f) sr.sprite = Walk;
        else sr.sprite = Idle;
    }
}
