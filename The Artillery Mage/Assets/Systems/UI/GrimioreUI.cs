using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimioreUI : MonoBehaviour
{
    [SerializeField] List<GameObject> pages;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;

    int _currentPage;

    void Start()
    {
        prevButton.onClick.AddListener(() => GoToPage(_currentPage - 1));
        nextButton.onClick.AddListener(() => GoToPage(_currentPage + 1));
        GoToPage(0);
    }

    void GoToPage(int index)
    {
        _currentPage = Mathf.Clamp(index, 0, pages.Count - 1);
        for (int i = 0; i < pages.Count; i++)
            pages[i].SetActive(i == _currentPage);
        prevButton.interactable = _currentPage > 0;
        nextButton.interactable = _currentPage < pages.Count - 1;
    }
}
