#region Namespaces
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

/// <summary> Fixes button highlighting </summary>
[RequireComponent(typeof(Selectable))]
public class HighlightFixUI : MonoBehaviour, IPointerEnterHandler, IDeselectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current.alreadySelecting) { return; }
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }

    public void OnDeselect(BaseEventData eventData) => this.GetComponent<Selectable>().OnPointerExit(null);
}

#region Credits
/// Script created by Tyler Nichols
#endregion