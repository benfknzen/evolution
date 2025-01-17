using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class InputUtils {

	private struct TouchInfo {
		public Touch touch;
		public bool overUI;
	}

	// Keep track of touches to be able to tell wether or not they were over
	// a UI element during the Ended TouchPhase
	// Maps from the touch fingerID to a TouchInfo struct
	private static Dictionary<int, TouchInfo> touches = new Dictionary<int, TouchInfo>();
	private static List<int> _keysToDelete = new List<int>();

	// This needs to be called exactly once per frame (before calling IsTouchOverUI)!
	public static void UpdateTouches() {

		#if !UNITY_IOS && !UNITY_ANDROID 
		return;
		#else 
		
		RemoveOutdatedTouches();
		if (Input.touchCount > 0) {
			for (int i = 0; i < Input.touchCount; i++) {
				var touch = Input.GetTouch(i);
				var overUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
				if (touch.phase == TouchPhase.Ended) {
					overUI |= touches[touch.fingerId].overUI;
				}
				touches[touch.fingerId] = new TouchInfo() {
					touch = touch,
					overUI = overUI
				};
			}
		} else {
			touches.Clear();
		}

		#endif
	}

	// Removes all touches that ended in the last frame from the "touches" cache
	private static void RemoveOutdatedTouches() {
		var allTouches = touches.Values;
		_keysToDelete.Clear();
		foreach (var touchInfo in allTouches) {
			if (touchInfo.touch.phase == TouchPhase.Ended) {
				_keysToDelete.Add(touchInfo.touch.fingerId);
			}
		}
		foreach (var key in _keysToDelete) {
			touches.Remove(key);
		}
	}

	public static bool IsTouchOverUI(int fingerId) {

		#if UNITY_IOS || UNITY_ANDROID 
		if (touches.ContainsKey(fingerId)) {
			return touches[fingerId].overUI;
		}

		#endif
		return false;
	}

    // /// <summary>
	// /// Returns true if the mouse is positioned over a UI element.
	// /// </summary>
	// public static bool IsPointerOverUIObject(){

	// 	PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
	// 	eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

	// 	List<RaycastResult> results = new List<RaycastResult>();
	// 	EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

	// 	return results.Count > 0;
	// }

	/// <summary>
	/// Returns true if the mouse/touch input ended on this frame.
	/// </summary>
	public static bool MouseUp() {
		return Input.GetMouseButtonUp(0) || 
			   (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
	}

	/// <summary>
	/// Returns true if the mouse/touch input began on an earlier
	/// frame and is still ongoing during this frame.
	/// </summary>
	public static bool MouseHeld() {
		return Input.GetMouseButton(0) ||
			   (Input.touchCount > 0 && (Input.GetTouch(0).phase != TouchPhase.Ended 
			   && Input.GetTouch(0).phase != TouchPhase.Ended));
	}
}