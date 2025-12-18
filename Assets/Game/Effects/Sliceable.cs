using UnityEngine;

namespace Cards.Card_Assets.RPS.Behaviors
{
    public class Sliceable : Slice, ICuttable
    {
        public void Cut(Vector3 cutNormal, Vector3 cutPosition)
        {
            ComputeSlice(cutNormal, cutPosition);
        }
    }
}