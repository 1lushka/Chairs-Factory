using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public RectTransform menuPanel;
    public Vector3 startPos;
    public Vector3 endPos;
    public float animDuration = 0.5f;
    public float showDelay = 0.3f;

    public float hoverScaleMultiplier = 1.1f;
    public float hoverDuration = 0.2f;

    private void Start()
    {
        menuPanel.anchoredPosition3D = startPos;
        Invoke(nameof(ShowMenu), showDelay);

        Button[] buttons = menuPanel.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            AddHoverEffect(btn);
        }
    }

    private void AddHoverEffect(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        Vector3 baseScale = rect.localScale;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) =>
        {
            rect.DOScale(baseScale * hoverScaleMultiplier, hoverDuration).SetEase(Ease.OutBack);
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) =>
        {
            rect.DOScale(baseScale, hoverDuration).SetEase(Ease.OutBack);
        });
        trigger.triggers.Add(entryExit);
    }

    public void ShowMenu()
    {
        menuPanel.gameObject.SetActive(true);
        menuPanel.anchoredPosition3D = startPos;
        menuPanel.DOAnchorPos3D(endPos, animDuration).SetEase(Ease.OutBack);
    }

    public void HideMenu()
    {
        menuPanel.DOAnchorPos3D(startPos, animDuration).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                menuPanel.gameObject.SetActive(false);
            });
    }
}
