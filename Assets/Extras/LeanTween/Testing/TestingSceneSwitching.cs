using Extras.LeanTween.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Extras.LeanTween.Testing
{
	public class TestingSceneSwitching : MonoBehaviour {

		public GameObject cube;

		private static int sceneIter = 0;

		private int tweenCompleteCnt;

		// Use this for initialization
		void Start () {
			LeanTest.expected = 6;
		
			// Start a couple of tweens and make sure they complete
			tweenCompleteCnt = 0;

			Framework.LeanTween.scale(cube, new Vector3(3f,3f,3f), 0.1f).setDelay(0.1f).setOnComplete( ()=>{
				tweenCompleteCnt++;
			});

			Framework.LeanTween.move(cube, new Vector3(3f,3f,3f), 0.1f).setOnComplete( ()=>{
				tweenCompleteCnt++;
			});

			Framework.LeanTween.delayedCall(cube, 0.1f, ()=>{
				tweenCompleteCnt++;
			});

			// Schedule a couple of tweens, make sure some only half complete than switch scenes

			Framework.LeanTween.delayedCall(cube, 1f, ()=>{
				Framework.LeanTween.scale(cube, new Vector3(3f,3f,3f), 1f).setDelay(0.1f).setOnComplete( ()=>{

				});

				Framework.LeanTween.move(cube, new Vector3(3f,3f,3f), 1f).setOnComplete( ()=>{

				});
			});

			// Load next scene
			Framework.LeanTween.delayedCall(cube, 0.5f, ()=>{
				LeanTest.expect( tweenCompleteCnt==3, "Scheduled tweens completed:"+sceneIter);
				if(sceneIter<5){
					sceneIter++;
					SceneManager.LoadScene(0);
				}
			});
		}
	
	
	}
}
