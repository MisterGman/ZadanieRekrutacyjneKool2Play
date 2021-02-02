using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserInterfaceController : MonoBehaviour
{
    public static UserInterfaceController Instance;
    
    [field: SerializeField] 
    private TextMeshProUGUI health;
    
    [field: SerializeField] 
    private TextMeshProUGUI killCount;

    private int _currKillCount;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != null) 
            Destroy(gameObject);
    }

    public void SetHealthText(int currHp) 
        => health.text = currHp.ToString();  
    
    public void IncreaseKillCount()
    {
        _currKillCount++;
        killCount.text = _currKillCount.ToString();
    }
}
