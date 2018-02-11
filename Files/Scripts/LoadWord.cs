using UnityEngine;
using System.IO;
using System.Collections;

namespace QuizGame {

	public class LoadWord : MonoBehaviour {

		public static LoadWord instance;

		[HideInInspector]
		public string m_WordText;

		private string m_FilePath;
		private bool m_IsReady = false;


		void Awake ()
		{
			//Задаем статичную ссылку
			if (instance == null) {
				instance = this;
			} else if (instance != this) {
				Destroy (gameObject);
			}

			//Не разрушаем объект при загрузке новых сцен
			DontDestroyOnLoad (gameObject);

			//Создаем путь к файлу
			m_FilePath = Path.Combine (Application.streamingAssetsPath, "TextWord.txt");
			//Загружаем настройки из файла
			StartCoroutine (LoadFile ());
		}

		IEnumerator LoadFile ()
		{
			//Если файл есть
			if (File.Exists (m_FilePath)) {

				//Если путь URL (Андройд)
				string dataWord = "";
				if (m_FilePath.Contains ("://")) {
					WWW www = new WWW (m_FilePath);
					yield return www;
					if (string.IsNullOrEmpty (www.error)) {
						dataWord = www.text;
					}
				} else {
					dataWord = File.ReadAllText (m_FilePath);
				}

				//Заполняем загруженными данными, только буквы и пробелы
				m_WordText = " ";
				for (int i = 0; i < dataWord.Length; i++) {
					if (char.IsLetter (dataWord [i]) || dataWord [i] == ' ') {
						m_WordText += dataWord [i];
					}
				}
				m_WordText += " ";

				//Делаем все буквы заглавными
				m_WordText = m_WordText.ToUpper ();

				Debug.Log ("Данные загружены!");
			} else {
				Debug.LogError ("Не удается найти файл!");
			}

			m_IsReady = true;
		}

		public bool GetIsReady()
		{
			//Проверяем готовы ли данные
			return m_IsReady;
		}
	}
}