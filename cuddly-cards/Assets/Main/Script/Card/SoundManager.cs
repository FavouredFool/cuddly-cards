using FMODUnity;
using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum CardSound { PULL, PLACE };

    [SerializeField]
    private EventReference _pullCardEvent;

    [SerializeField]
    private EventReference _placeCardEvent;

    public EventReference PullCardEvent => _pullCardEvent;

    public EventReference PlaceCardEvent => _placeCardEvent;


    public void PlayCardSound(CardSound sound)
    {
        FMODUnity.RuntimeManager.PlayOneShot(GetEventFromCardSound(sound));
    }

    public EventReference GetEventFromCardSound(CardSound sound)
    {
        switch (sound)
        {
            case CardSound.PULL:
                return PullCardEvent;
            case CardSound.PLACE:
                return PlaceCardEvent;
            default:
                return PullCardEvent;
        }
    }
}
