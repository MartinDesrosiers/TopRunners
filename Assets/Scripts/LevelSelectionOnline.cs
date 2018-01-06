using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionOnline : MonoBehaviour {
	[SerializeField] private LevelEntry levelEntryPrefab;
	[SerializeField] private Transform scrollViewContent;

	// UI elements for search
	[SerializeField] private InputField searchInputField;
	[SerializeField] private Button searchBtn;
	[SerializeField] private GameObject searchPopup;
	
	void Start () {
		// Make sure search popup is hidden at the start
		searchPopup.SetActive(false);
	}

	public void SortByLatest () {
		Debug.Log("SortByLatest");
		PopulateSelector(LevelManager.SortOption.Latest);
	}

	public void SortByOfficial () {
		Debug.Log("SortByOfficial. (Not yet implemented)");
	}

	public void FindById () {
		Debug.Log("FindById");
		ShowSearchPrompt(searchTerm => {
			PopulateSelector(LevelManager.SortOption.Id, searchTerm);
		});
	}

	public void FindByCreator () {
		Debug.Log("FindByCreator");
		ShowSearchPrompt(searchTerm => {
			PopulateSelector(LevelManager.SortOption.Creator, searchTerm);
		});
	}

	public void FindByName () {
		Debug.Log("FindByName");
		ShowSearchPrompt(searchTerm => {
			PopulateSelector(LevelManager.SortOption.Name, searchTerm);
		});
	}

	private void ShowSearchPrompt (System.Action<string> searchTerm) {
		searchPopup.SetActive(true);
		searchInputField.text = "";
		searchBtn.onClick.RemoveAllListeners();

		searchBtn.onClick.AddListener(() => {
			searchTerm(searchInputField.text);
			searchPopup.SetActive(false);
		});
	}

	private void PopulateSelector (LevelManager.SortOption sortOption) {
		PopulateSelector (sortOption, null);
	}

	private void PopulateSelector (LevelManager.SortOption sortOption, string searchTerm) {
		searchPopup.SetActive(false);

		LevelManager.Instance.FetchLevels(sortOption, searchTerm, levelInfos => {
			if (levelInfos != null) {
				if (levelInfos.Length == 0) {
					Debug.Log("No levels for search term " + searchTerm);
					return;
				}

				// Remove old level entries
				foreach (Transform child in scrollViewContent) {
					Destroy(child.gameObject);
				}

				foreach (LevelInfo levelInfo in levelInfos) {
					LevelEntry levelEntry = Instantiate(levelEntryPrefab);
					levelEntry.Init(levelInfo);
					levelEntry.transform.SetParent(scrollViewContent);
				}
			} else {
				Debug.LogError("Could not fetch levels.");
			}
		});
	}
}
