using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    [field: SerializeField, Tooltip("Sets how long bullet will exists")]
    protected float timeBeforeDestroying = .5f;

    [field: SerializeField, Tooltip("Damage of bullet")] 
    protected int damageOfShot;

    protected Transform _transform;
    
    private void Start()
    {
        _transform = transform;
    }
    
    private void OnEnable()
    {
        StartCoroutine(DestroyBulletAfterTimer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    
    /// <summary>
    /// Destroys bullet after timer goes down
    /// It stops when bullet hit target
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyBulletAfterTimer()
    {
        yield return new WaitForSeconds(timeBeforeDestroying);
        ObjectPooling.Instance.ReturnToPool(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyFollow>().TakeDamage(damageOfShot);
            ObjectPooling.Instance.ReturnToPool(gameObject);
        } 
    }
}
