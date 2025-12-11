using UnityEngine;

namespace Extras.LeanTween.Testing
{
	public class Testing248 : MonoBehaviour {

		public GameObject dude1;

		// Use this for initialization
		void Start () {
			//dude1.LeanMoveX(10f, 1f);
			int id = Framework.LeanTween.moveX(dude1, 1f, 3f).id;
			Debug.Log("id:" + id);
			if (Framework.LeanTween.isTweening(id))
				Debug.Log("I am tweening!");
		}
	}
}
