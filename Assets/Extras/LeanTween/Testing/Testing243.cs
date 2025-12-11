using UnityEngine;

namespace Extras.LeanTween.Testing
{
	public class Testing243 : MonoBehaviour {
		public GameObject cube1;
		public Light lightInScene;

		public RectTransform imageRectTransform;

		// Use this for initialization
//	void Start () {
//		cube1.transform.localPosition = new Vector3(0, 10, -10);
//		LeanTween.move(cube1, new Vector3(0, 0, -10), 1f).setEaseInOutQuart().setOnUpdate( ( Vector3 val )=>{
//			Debug.Log("val:"+val);	
//		}).setOnComplete(() => {
//			Debug.Log("cube1 end pos:"+cube1.transform.position);
//		});
//	}

		void Start () {
//		LeanTween.alpha (imageRectTransform, 0, 0.3f).setLoopPingPong (-1);

			// LeanTween.move (cube1, new Vector3(10f,10f,10f), 10f).setLoopPingPong (-1).setPassed(5f);
//		LeanTween.moveLocal(cube1, cube1.transform.localPosition+new Vector3(0f,1f,0f),1f).setEaseShake();

			Framework.LeanTween.sequence().append(Framework.LeanTween.scale(cube1, Vector3.one * 3f, .1f)).append(Framework.LeanTween.scale(cube1, Vector3.one*5.6f, .2f));
			Framework.LeanTween.sequence().append(Framework.LeanTween.value (lightInScene.gameObject, (float f) =>	lightInScene.range = f, 8, 12, .1f)).append (Framework.LeanTween.value (lightInScene.gameObject, (float f) =>	lightInScene.range = f, 12, 8, .2f));
		} 
	
		// Update is called once per frame
		void Update () {
	
		}
	}
}
