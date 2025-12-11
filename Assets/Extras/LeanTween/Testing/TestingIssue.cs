using Extras.LeanTween.Framework;
using UnityEngine;

namespace Extras.LeanTween.Testing
{
	public class TestingIssue : MonoBehaviour {

		LTDescr lt,ff;
		int id,fid;

		void Start () {
			Framework.LeanTween.init();
		
			lt = Framework.LeanTween.move(gameObject,100*Vector3.one,2);
			id = lt.id;
			Framework.LeanTween.pause(id);

			ff = Framework.LeanTween.move(gameObject,Vector3.zero,2);
			fid = ff.id;
			Framework.LeanTween.pause(fid);
		}

		void Update () {
			if(Input.GetKeyDown(KeyCode.A))
			{
				// Debug.Log("id:"+id);
				Framework.LeanTween.resume(id);
			}
			if(Input.GetKeyDown(KeyCode.D))
			{
				Framework.LeanTween.resume(fid);
			}
		}
	}
}
