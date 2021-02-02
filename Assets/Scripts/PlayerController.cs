using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    #region Variables

    [field: SerializeField, Tooltip("Spawn points for bullets")] 
    private Transform bulletHolder;

    [field: SerializeField, Tooltip("Bullet prefab")] 
    private GameObject bulletPrefab;
    
    [field: SerializeField, Tooltip("Cone prefab")] 
    private GameObject shotgunConePrefab;
    
    [field: SerializeField, Tooltip("Delay between shooting")] 
    private float cooldownBetweenShots = .5f;
    
    [field: SerializeField, Tooltip("Delay between shooting shotgun")] 
    private float cooldownShotgun = 2f;

    [field: SerializeField, Tooltip("Speed of movement")]
    private float speedMovement = 5f;

    [field: SerializeField, Tooltip("Max health")]
    private int maxHealth;
    
    /// <summary>
    /// Player inputs
    /// </summary>
    private PlayerActions _actions;

    /// <summary>
    /// Determines if player can shoot
    /// </summary>
    private bool _canShoot = true;    
    
    /// <summary>
    /// Determines if player can use shotgun
    /// </summary>
    private bool _canShotgun = true;

    /// <summary>
    /// Main camera
    /// </summary>
    private Camera _playerCamera;
    
    /// <summary>
    /// Plane for ray
    /// </summary>
    private Plane _plane;

    /// <summary>
    /// On button hold shooting loop
    /// </summary>
    private IEnumerator _nonStopShooting;
    
    /// <summary>
    /// Direction and speed of player's movement
    /// </summary>
    private Vector3 _moveVelocity;

    private Transform _transform;

    private Rigidbody _rigidbody;

    public int CurrentHp { get; private set; }

    #endregion
    
    private void Awake()
    {
        _transform = transform;
        _rigidbody = GetComponent<Rigidbody>();
        _actions = new PlayerActions();
        _playerCamera = Camera.main;
        _plane = new Plane(Vector3.up, Vector3.zero);
        _nonStopShooting = NonStopShooting();

        CurrentHp = maxHealth;
        
        UserInterfaceController.Instance.SetHealthText(CurrentHp);

        InitControls();
    }

    private void OnEnable()
    {
        _actions.Enable();
    }

    private void OnDisable()
    {
        _actions.Disable();
    }

    private void InitControls()
    {
        _actions.PlayerMovement.Shoot.started += ShootPressHandler;
        _actions.PlayerMovement.Shoot.canceled += ShootCancelHandler;

        _actions.PlayerMovement.ShotgunShoot.performed += ShotgunShootHandler;
    }
    

    private void OnDestroy()
    {
        _actions.PlayerMovement.Shoot.started -= ShootPressHandler;
        _actions.PlayerMovement.Shoot.canceled -= ShootCancelHandler;
        
        _actions.PlayerMovement.ShotgunShoot.performed -= ShotgunShootHandler;
    }

    private void Update()
    {
        SetMovementVelocity();
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _moveVelocity;
    }

    #region InputHandlers

    private void RotatePlayer()
    {
        var aimPosition = _actions.PlayerMovement.Aiming.ReadValue<Vector2>();

        var ray = _playerCamera.ScreenPointToRay(aimPosition);

        if (!_plane.Raycast(ray, out var distance)) return;
        
        var target = ray.GetPoint(distance);
        var direction = target - _transform.position;
        var rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            
        _transform.rotation = Quaternion.Euler(0, rotation, 0);
    }
    
    private void SetMovementVelocity()
    {
        var inputs = _actions.PlayerMovement.HorizontalMovement.ReadValue<Vector2>();
        _moveVelocity = new Vector3(inputs.x, 0, inputs.y) * speedMovement;
    }

    /// <summary>
    /// Spawns bullet in front of player
    /// </summary>
    /// <param name="obj"></param>
    private void Shoot(InputAction.CallbackContext obj)
    {
        if(!_canShoot) return;

        var bulletObject = ObjectPooling.Instance.GetObjectFromPool(bulletPrefab);
        bulletObject.transform.position = bulletHolder.position;
        bulletObject.transform.rotation = bulletHolder.rotation;

        StartCoroutine(CooldownBetweenShooting());
    }

    /// <summary>
    /// Start shooting loop
    /// </summary>
    /// <param name="obj"></param>
    private void ShootPressHandler(InputAction.CallbackContext obj) 
        => StartShooting(true);

    /// <summary>
    /// Stop shooting loop
    /// </summary>
    /// <param name="obj"></param>
    private void ShootCancelHandler(InputAction.CallbackContext obj) 
        => StartShooting(false);

    private void StartShooting(bool isShoot)
    {
        if (isShoot)
            StartCoroutine(_nonStopShooting);
        else
            StopCoroutine(_nonStopShooting);
    }

    private void ShotgunShootHandler(InputAction.CallbackContext obj)
    {
        if(!_canShotgun) return;
        
        var bulletObject = ObjectPooling.Instance.GetObjectFromPool(shotgunConePrefab);
        bulletObject.transform.position = bulletHolder.position;
        bulletObject.transform.rotation = bulletHolder.rotation;

        StartCoroutine(CooldownBetweenShotgun());
    }

    #endregion

    #region Cooldowns

    /// <summary>
    /// Auto shooting loop
    /// </summary>
    /// <returns></returns>
    private IEnumerator NonStopShooting()
    {
        while (true)
        {
            Shoot(default);
            yield return new WaitForSeconds(cooldownBetweenShots);
        }
    }
    
    /// <summary>
    /// Cooldown between shots for manual shooting
    /// </summary>
    /// <returns></returns>
    private IEnumerator CooldownBetweenShooting()
    {
        _canShoot = false;
    
        yield return new WaitForSeconds(cooldownBetweenShots);
    
        _canShoot = true;
    }
    
    /// <summary>
    /// Cooldown between shotgun shots for manual shooting
    /// </summary>
    /// <returns></returns>
    private IEnumerator CooldownBetweenShotgun()
    {
        _canShotgun = false;
    
        yield return new WaitForSeconds(cooldownShotgun);
    
        _canShotgun = true;
    }

    #endregion

    #region Health

    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;
        
        UserInterfaceController.Instance.SetHealthText(CurrentHp);

        if(CurrentHp <= 0)
            SceneManager.LoadScene("MainScene");    
    }

    #endregion
}