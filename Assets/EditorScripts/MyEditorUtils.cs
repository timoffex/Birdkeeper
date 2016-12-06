using UnityEngine;
using UnityEditor;
using System;


public static class MyEditorUtils {
	public static void CheckForDragDrop<T> (Rect area, bool allowMultiples, Action<T> process) {
		Event evt = Event.current;

		switch (evt.type) {
		case EventType.DragUpdated:
		case EventType.DragPerform:
			if (area.Contains (evt.mousePosition)) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform) {
					DragAndDrop.AcceptDrag ();

					foreach (object draggedObj in DragAndDrop.objectReferences) {
						if (draggedObj is T) {
							process ((T)draggedObj);

							if (!allowMultiples)
								break;
						} else {
							string errorText = string.Format ("Please enter an object of type {0}", typeof(T).Name);
							Debug.Log (errorText);
							EditorWindow.focusedWindow.ShowNotification (new GUIContent (errorText));
						}
					}
				}
			}

			break;
		}
	}


	public static void DrawSprite (Rect area, Sprite spr) {
		if (spr == null)
			GUI.Box (area, (Texture2D)null);
		else if (spr.rect.width == spr.texture.width)
			GUI.DrawTexture (area, spr.texture, ScaleMode.ScaleToFit);
		else {
			float width = spr.texture.width;
			float height = spr.texture.height;

			Rect sourceRect = spr.textureRect;
			Rect normalizedSourceRect = new Rect (sourceRect.x / width, sourceRect.y / height,
				sourceRect.width / width, sourceRect.height / height);

			float aspectRatio = sourceRect.width / sourceRect.height;

			Rect areaCorrectAspect;
			if (aspectRatio >= 1) {
				var center = area.center;
				var newSize = new Vector2 (area.width, area.height / aspectRatio);
				areaCorrectAspect = new Rect ();
				areaCorrectAspect.size = newSize;
				areaCorrectAspect.center = center;
			} else {
				var center = area.center;
				var newSize = new Vector2 (area.width * aspectRatio, area.height);
				areaCorrectAspect = new Rect ();
				areaCorrectAspect.size = newSize;
				areaCorrectAspect.center = center;
			}
			

			Graphics.DrawTexture (areaCorrectAspect, spr.texture, normalizedSourceRect,
				(int)spr.border [0], (int)spr.border [1], (int)spr.border [2], (int)spr.border [3]);
		}
	}


	public static bool TextField (Rect area, string text, out string newText) {
		EditorGUI.BeginChangeCheck ();
		newText = GUI.TextField (area, text);
		return EditorGUI.EndChangeCheck ();
	}
}
