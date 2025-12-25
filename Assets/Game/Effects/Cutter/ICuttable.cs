using UnityEngine;

namespace Cards.Card_Assets.RPS.Behaviors
{
    public interface ICuttable
    {
        public void Cut(Vector3 cutNormal, Vector3 cutPosition);
    }
}