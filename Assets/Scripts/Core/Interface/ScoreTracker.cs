using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private Text target;

    void Awake() => Score.OnModified += OnScoreModified;

    private void OnScoreModified(int modification)
    {
        if (Score.Value < 10) target.text = $"0{Score.Value}";
        else target.text = Score.Value.ToString();
    }
}