using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunController : WeaponParent
{
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyFollow>().TakeDamage(damageOfShot);
        } 
    }
}
