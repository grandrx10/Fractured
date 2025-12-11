using UnityEngine;

namespace Extras.LeanTween.Testing
{
    public class TestingOptionalCancel : MonoBehaviour {

        public GameObject cube1;

        // Use this for initialization
        void Start () {
            Framework.LeanTween.init(1);
            // Fire up a bunch with onUpdates
            Framework.LeanTween.moveX(cube1, 10f, 1f).setOnUpdate((float val) =>
            {
                Debug.Log("on update.... val:"+val+" cube1.x:"+cube1.transform.position.x);
            });

        }

        private bool alternate = true;

        private void Update()
        {
            if(Input.GetMouseButtonDown(0)){
                Framework.LeanTween.moveX(cube1, alternate ? -10f : 10f, 1f).setOnUpdate((float val) =>
                {
                    Debug.Log("2 on update.... val:" + val + " cube1.x:" + cube1.transform.position.x);
                });
                alternate = !alternate;
            }
        }
    }
}
