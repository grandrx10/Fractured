using System;
using Characters.Interactables;
using Game;
using UnityEngine;

namespace Utils
{
    public class SwapSpeaker : MonoBehaviour
    {
        public string eventName;
        public string newConvoName;
        public Speaker target;

        private void Start()
        {
            if (GlobalState.instance.HasEvent(eventName)) target.conversationName = newConvoName;
        }
    }
}