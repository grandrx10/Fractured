using UnityEngine;

namespace Cards.Environments
{
    public enum CardSubmitState
    {
        Invalid,
        Success,
        Failure
    }

    // We could probably replace this with some interfaces
    // Currently the expectation is that cards type check the CardEnv parameter to execute behavior
    public class CardEnv : MonoBehaviour
    {
        
    }
}