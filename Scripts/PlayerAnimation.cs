using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour{
    public SpriteRenderer sr;
    Player player;
    Rigidbody2D rb;
    public AnimationSet[] anim;
    AnimationSet animCur;
    int frame;
    public float speed;
    float time;

    const int IDLE = 0;
    const int WALK = 1;
    const int JUMP = 2;
    const int FALL = 3;
    const int ROLL = 4;
    const int WALLSLIDE = 5;
    const int DASH = 6;
    const int IDLEROLL = 7;

    void Awake(){
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animCur = anim[0];
    }

    void FixedUpdate(){
        StateMachine();
        PlayAnimation();
    }

    void StateMachine(){
        if(player.isWallSlide && rb.velocity.y < .1f) {
            ChangeAnimation(WALLSLIDE);
            return;
        }
        else if(player.isDash){
            ChangeAnimation(DASH);
            return;
        }
        else if(rb.velocity.y < -.1f){
            ChangeAnimation(FALL);
            return;
        }
        else if(player.isRoll && rb.velocity.x != 0){
            ChangeAnimation(ROLL);
            return;
        }
        else if(player.isRoll && rb.velocity.x == 0){
            ChangeAnimation(IDLEROLL);
            return;
        }


        if(rb.velocity.y == 0){
            if(Mathf.Abs(rb.velocity.x) > .1f && Mathf.Abs(Input.GetAxisRaw("Horizontal")) > .1f) {
                ChangeAnimation(WALK);
                return;
            }
        }
        else if(rb.velocity.y > .1f){
            ChangeAnimation(JUMP);
            return;
        }
        ChangeAnimation(IDLE);
    }

    void ChangeAnimation(int index){
        if(animCur == anim[index]) return;
        animCur = anim[index];
        frame = 0;
        time = 0;
    }

    void PlayAnimation(){
        if(time > Time.time) return;
        time = Time.time + speed;
        sr.sprite = animCur.sprite[frame];
        frame++;
        if(frame >= animCur.sprite.Length) frame = 0;
    }
}