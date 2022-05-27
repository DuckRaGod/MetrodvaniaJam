using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="player", menuName ="Player")]
public class PlayerStats : ScriptableObject{
    public int maxHealth;
    public int health;
}
