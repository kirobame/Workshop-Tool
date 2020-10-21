using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTracker : MonoBehaviour
{
    [SerializeField] private Text target;

    void Awake() => Enemy.OnCountChanged += OnCountChanged;
    
    private void OnCountChanged() => target.text = Enemy.Count.ToString();
}