using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Token roomHandlerToken;
    [SerializeField] private Token inputHandlerToken;

    [Space, SerializeField] private AudioEffect soundEffect;
    [SerializeField] private AudioMixerSnapshot musicOn, musicOff;

    private bool state;
    
    public void Toggle()
    {
        var inputHandler = Repository.GetFirst<InputHandler>(inputHandlerToken);
        
        if (state == false)
        {
            target.SetActive(true);
            Time.timeScale = 0f;

            inputHandler.SetActiveMap("Standard", false);
            soundEffect.Play(0);
            
            musicOff.TransitionTo(1f);
            state = true;
        }
        else
        {
            soundEffect.Play(1);
            TurnOff(inputHandler);
        }
    }

    public void Restart()
    {
        Enemy.SetCount(0);
        Score.ModifyBy(-Score.Value);
        
        var inputHandler = Repository.GetFirst<InputHandler>(inputHandlerToken);
        TurnOff(inputHandler);
        
        soundEffect.Play(2);

        var roomHandler = Repository.GetFirst<RoomHandler>(roomHandlerToken);
        roomHandler.ActivateSpecific(0);
    }

    private void TurnOff(InputHandler inputHandler)
    {
        target.SetActive(false);
        Time.timeScale = 1f;

        inputHandler.SetActiveMap("Standard", true);

        musicOn.TransitionTo(1f);
        state = false;
    }
}