using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace QuizGame {

	public class GameLogic : MonoBehaviour {

		public int m_MinCountLetter = 5;
		public int m_NumberAttempts = 10;
		public int m_Points = 5;

		[Header("Панель у слова и префаб букв")]
		public Transform m_PanelWord;
		public GameObject m_PanelLetter;
		[Header("Панель клавиатуры")]
		public Transform m_PanelKeyboard;
		[Header("Текст с числом попыток и очков")]
		public Text m_CurrentAttempts;
		public Text m_CurrentPoints;

		[Header("Интерфейс и тексты побед")]
		public GameObject m_Interface;
		public GameObject m_Victory;
		public GameObject m_Defeat;
		public GameObject m_Passed;
		public AudioClip[] m_Sound;

		private List<GameObject> m_Letters = new List<GameObject> ();
		private Button[] m_ButtonsKeyboard;
		private string m_WordsWere;
		private string m_Word;

		private int m_RemainingLetters;
		private AudioSource m_Audio;


		void Awake ()
		{
			m_ButtonsKeyboard = m_PanelKeyboard.GetComponentsInChildren<Button> ();
			m_Audio = gameObject.AddComponent<AudioSource> ();

			m_CurrentAttempts.text = m_NumberAttempts.ToString ();
			m_CurrentPoints.text = "0";
			m_WordsWere = "";
			m_Word = "";
			m_RemainingLetters = 0;

			ContinuesGame ();
		}

		void ContinuesGame ()
		{
			//Берем слово
			m_Word = TextLogic ();

			//Запоминаем число букв
			if (m_Word.Length > 0) {
				m_RemainingLetters = m_Word.Length;
			} else {
				//Слова закончились, игра пройдена!
				m_Passed.SetActive(true);

				//Играем звук победы (Звуки в массиве: 0-Клик, 1-Победа, 2-Поражение)
				m_Audio.Stop ();
				m_Audio.clip = m_Sound [1];
				m_Audio.Play ();

				StartCoroutine (Background ());
				RestartGame ();
			}

			//Показываем вновь все кнопки
			foreach (Button bt in m_ButtonsKeyboard) {
				bt.gameObject.SetActive (true);
			}

			//Отчищаем список окон с буквами слова
			int num = m_Letters.Count;
			for (int i = 0; i < num; i++) {
				GameObject go = m_Letters [0];
				m_Letters.Remove (go);
				Destroy (go);
			}

			//Создаем новые окошки с буквами
			for (int i = 0; i < m_Word.Length; i++) {
				GameObject panelLetter = Instantiate (m_PanelLetter, Vector3.zero, Quaternion.identity);
				panelLetter.transform.SetParent (m_PanelWord);
				panelLetter.GetComponentInChildren<Text> ().text = "?";
				m_Letters.Add (panelLetter);
			}
		}

		private string TextLogic ()
		{
			string word = "";
			int start = -1;

			if (LoadWord.instance == null) {
				Debug.LogError ("Пожалуйста запускайте сцену: Scene_Start, а не игровую!");
			}

			//Пробегаем по буквам
			for (int i = 0; i < LoadWord.instance.m_WordText.Length; i++) {
				//Если встречаем пробел
				if (LoadWord.instance.m_WordText [i] == ' ') {

					//Запоминаем номер начала слова или смотрим слово
					if (start == -1) {
						start = i;
					} else {

						//Если количество символов больше минимума
						if (i - start + 1 >= m_MinCountLetter) {

							word = LoadWord.instance.m_WordText.Substring (start + 1, i - (start + 1));

							//Если такого слова еще не встречалось
							if (!m_WordsWere.Contains (word)) {
								m_WordsWere += " " + word;
								return word;
							}
						}

						//Если слово не подобрано продолжаем с тек. символа пробела
						start = i;
					}
				}
			}

			//Слова закончились!
			return "";
		}

		public void EnterWord (GameObject button)
		{
			//Берем букву кнопки и прячем ее
			string letter = button.GetComponentInChildren<Text> ().text;
			button.SetActive (false);

			m_Audio.Stop ();
			m_Audio.clip = m_Sound [0];
			m_Audio.Play ();

			//Если есть слово
			if (m_Word.Length > 0) {

				//Смотрим есть ли такая буква
				if (m_Word.Contains (letter)) {

					//Добавляем за каждую встречу в слове данной буквы очки
					for (int i = 0; i < m_Word.Length; i++) {
						if (m_Word [i] == letter [0]) {
							m_CurrentPoints.text = (int.Parse (m_CurrentPoints.text) + m_Points).ToString ();

							//И показываем в окошке угаданую букву
							m_Letters [i].GetComponentInChildren<Text> ().text = m_Word [i].ToString ();

							//Если все буквы угаданы, то берем следующее слово
							m_RemainingLetters -= 1;
							if (m_RemainingLetters <= 0) {
								m_Victory.SetActive (true);

								m_Audio.Stop ();
								m_Audio.clip = m_Sound [1];
								m_Audio.Play ();

								StartCoroutine (Background ());
								ContinuesGame ();
								//Завершаем функцию иначе он продолжит в новом слове искать текущую букву
								return;
							}
						}
					}
				} else {

					//Если нет уменьшаем число попыток
					m_CurrentAttempts.text = (int.Parse(m_CurrentAttempts.text) - 1).ToString();
					if (int.Parse(m_CurrentAttempts.text) <= 0) {
						m_Defeat.SetActive (true);

						m_Audio.Stop ();
						m_Audio.clip = m_Sound [2];
						m_Audio.Play ();

						StartCoroutine (Background ());
						RestartGame ();
					}
				}
			}
		}

		void RestartGame ()
		{
			//Обновляем значения
			m_CurrentAttempts.text = m_NumberAttempts.ToString ();
			m_CurrentPoints.text = "0";
			m_WordsWere = "";
			m_Word = "";

			ContinuesGame ();
		}

		private IEnumerator Background ()
		{
			m_Interface.SetActive (false);
			yield return new WaitForSeconds (2);
			m_Interface.SetActive (true);
			m_Victory.SetActive (false);
			m_Defeat.SetActive (false);
			m_Passed.SetActive (false);
		}
	}
}