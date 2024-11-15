using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoneDescriptionHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image imgComponent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        imgComponent.enabled = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imgComponent.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        var imgObject = transform.Find("Desc");
        imgComponent = imgObject.GetComponent<Image>();

        ////imgObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
        //var imgPosition = /*imgObject.*/GetComponent<RectTransform>();
        //imgPosition.position = GetComponent<RectTransform>().position + Vector3.down;
        //Debug.Log($"Position: {imgPosition.position}\n"+
        //    $"localPosition: {imgPosition.localPosition}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
