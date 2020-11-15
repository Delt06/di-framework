using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Demo.Scripts.UI
{
	public sealed class RestartSceneButton : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			var activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(activeSceneBuildIndex);
		}
	}
}