using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [field: SerializeField] 
    private int maxHealth = 100;

    [field: SerializeField] 
    private Transform target;
    
    [field: SerializeField] 
    private float speed;

    private float _currentHealth;
    
    private Rigidbody _rigidbody;

    private Transform _transform;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = transform;

        target = FindObjectOfType<PlayerController>().transform;
    }

    private void FixedUpdate()
    {
        var pos = Vector3.MoveTowards
            (_transform.position, target.position, speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(pos);
        _transform.LookAt(target);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            UserInterfaceController.Instance.IncreaseKillCount();
            ObjectPooling.Instance.ReturnToPool(gameObject);
        }
    }

    private void OnEnable() 
        => _currentHealth = maxHealth;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Player")) 
            other.gameObject.GetComponent<PlayerController>().TakeDamage(50);
    }
}
