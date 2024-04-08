using UnityEngine;

namespace _9_Plugins.Pixelplacement.iTween.Sample
{
	public class RotateSample : MonoBehaviour
	{	
		void Start(){
			global::iTween.RotateBy(gameObject, global::iTween.Hash("x", .25, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
		}
	}
}

