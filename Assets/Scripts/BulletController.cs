using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : WeaponParent
{
    [field: SerializeField, Tooltip("Speed of bullet")] 
    private float speed = 10f;

    private void Update()
    {
        _transform.Translate(_transform.forward * (speed * Time.deltaTime), Space.World);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        
        if (other.CompareTag("Obstacle"))             
            ObjectPooling.Instance.ReturnToPool(gameObject);
    }
}
