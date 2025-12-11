using UnityEngine;

namespace Extras.LeanTween.Testing
{
	public class TestingDescr : MonoBehaviour {

		private int tweenId;

		public GameObject go;

		// start a tween
		public void startTween(){
			tweenId = Framework.LeanTween.moveX(go, 10f, 1f).id;
			Debug.Log("tweenId:" + tweenId);
		}

		// check tween descr
		public void checkTweenDescr(){
			var descr = Framework.LeanTween.descr(tweenId);
			Debug.Log("descr:" + descr);
			Debug.Log("isTweening:"+Framework.LeanTween.isTweening(tweenId));
		}
	}
}
