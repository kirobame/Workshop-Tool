using UnityEngine;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Token roomHandlerToken;
    [SerializeField] private Token inputHandlerToken;

    private bool state;
    
    public void Toggle()
    {
        var inputHandler = Repository.GetFirst<InputHandler>(inputHandlerToken);
        
        if (state == false)
        {
            target.SetActive(true);
            Time.timeScale = 0f;

            inputHandler.SetActiveMap("Standard", false);
            
            state = true;
        }
        else
        {
            target.SetActive(false);
            Time.timeScale = 1f;

            inputHandler.SetActiveMap("Standard", true);
            
            state = false;
        }
    }

    public void Restart()
    {
        Enemy.SetCount(0);
        Score.ModifyBy(-Score.Value);
        
        var inputHandler = Repository.GetFirst<InputHandler>(inputHandlerToken);
        inputHandler.SetActiveMap("Standard", true);
        
        target.SetActive(false);
        Time.timeScale = 1f;
        
        var roomHandler = Repository.GetFirst<RoomHandler>(roomHandlerToken);
        roomHandler.ActivateSpecific(0);
    }
}