using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    // Static method to fetch camera only once per scene. More efficient than calling Camera.main repeatedly.
    private static Camera _camera;
    public static Camera Camera
    {
        get
        {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }


    // Cache WaitForSeconds for coroutines to improve efficiency. Each WaitForSeconds is stored in the dictionary.
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }


    // Takes pointer data, sends a raycast and if anything is returned, we know the pointer is over the UI.
    // Use this to create a bool that tells us whether the pointer is over your UI.
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }


    // Spawn particle effect on canvas or spawn 3d object on canvas using this.
    // Returns a Vector 2. Set object location to output of this function to put it on the canvas.
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }


    // Deletes all child objects of transform input.
    // Iterates over the children of the transform object and destroys them in series.
    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
}
