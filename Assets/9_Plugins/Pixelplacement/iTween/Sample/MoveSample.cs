using UnityEngine;

namespace _9_Plugins.Pixelplacement.iTween.Sample
{
	public class MoveSample : MonoBehaviour
	{	
		void Start(){
			global::iTween.MoveBy(gameObject, global::iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
		}
	}
}

