using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeInOne.Games.Sumo
{

public enum PowerType
{
    Superpower,
    Projectiles,
    Smash
};

public class Powerup : MonoBehaviour
{
    public PowerType PowerType;
}

}
