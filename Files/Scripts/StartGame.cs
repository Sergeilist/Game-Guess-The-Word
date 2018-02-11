using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace QuizGame {

	public class StartGame : MonoBehaviour {

		private IEnumerator Start ()
		{
			//Ждем когда все загрузится
			while (!LoadWord.instance.GetIsReady ())
			{
				yield return null;
			}

			SceneManager.LoadScene ("Scene_Game");
		}
	}
}