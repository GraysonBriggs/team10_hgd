﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow {

	bool mapInScene;
	public static Transform platforms, startLocation, endFlag, outOfBounds, powerUps;
	List<GameObject> placablePlatforms;
	List<GameObject> placablePowerUps;
	List<GameObject> placableWeapons;
	GameObject mousedOver;

	static bool platSnap;
	static float platSnapSize;
	static GameObject draggedObj;

	//Texture saw; //TODO test

	public static bool meme;

	//Map references
	GameObject mapObj;
	MapInfo mapInfo;

	//Map Info
	string mapName;

	//Add this window to the Window menu
	[MenuItem ("Window/MapEditor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow (typeof(MapEditor));
	}

	void OnFocus() {
		// Remove delegate listener if it has previously
		// been assigned.
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		SceneView.onSceneGUIDelegate += OnSceneGUI;

		if (!mapInScene) {
			if (mapObj = GameObject.FindObjectOfType<MapInfo> ().gameObject) {
				readInfo ();
				mapInScene = true;
			}
		}

		if (placablePlatforms == null) {
			placablePlatforms = new List<GameObject> ();
			placablePowerUps = new List<GameObject> ();
			placableWeapons = new List<GameObject> ();
			Object[] placables = Resources.LoadAll ("Platforms/");
			foreach (Object obj in placables) {
				if (((GameObject)obj).GetComponent<PlatformController> () != null)
					placablePlatforms.Add ((GameObject)obj);
			}
			placables = Resources.LoadAll ("PowerUps/");
			foreach (Object obj in placables) {
				if (((GameObject)obj).GetComponent<PickUpPower> () != null)
					placablePowerUps.Add ((GameObject)obj);
				if (((GameObject)obj).GetComponent<PickUpWeapon> () != null)
					placableWeapons.Add ((GameObject)obj);
			}
		}
	}

	void OnDestroy() {
		// When the window is destroyed, remove the delegate
		// so that it will no longer do any drawing.
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	void Awake() {
		mapInScene = false;
		meme = false;
		mapName = "Map";
		platSnap = true;
		platSnapSize = 1.0f;
	}

	private static void OnSceneGUI(SceneView sceneview)
	{
		Handles.BeginGUI ();

		//if (GUILayout.Button("Press Me"))
		//	Debug.Log("Got it to work.");

		if (meme)
			GUI.Label (new Rect (Screen.width - 250, Screen.height - 250, 250, 250), Resources.Load ("meme") as Texture);


		if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) {
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // show a drag-add icon on the mouse cursor

			if (draggedObj == null)
				draggedObj = (GameObject)Object.Instantiate (DragAndDrop.objectReferences [0]);

			Vector3 mousePos = Event.current.mousePosition;
			mousePos.y = sceneview.camera.pixelHeight - mousePos.y;
			mousePos = sceneview.camera.ScreenToWorldPoint (mousePos);
			mousePos.z = 0;
			//Snap platform to grid
			if (draggedObj.GetComponent<PlatformController> () && platSnap) {
				mousePos.x = Mathf.Round (mousePos.x / platSnapSize) * platSnapSize;
				mousePos.y = Mathf.Round (mousePos.y / platSnapSize) * platSnapSize;
			}
			draggedObj.transform.position = mousePos;

			if (Event.current.type == EventType.DragPerform) {
				DragAndDrop.AcceptDrag ();
				Selection.activeGameObject = draggedObj;

				//Move the placed object into the correct location in the map hierarchy
				if (draggedObj.GetComponent<PlatformController> ()) {
					draggedObj.transform.SetParent (platforms);
				} else if (draggedObj.GetComponent<PickUpPower> () || draggedObj.GetComponent<PickUpWeapon> ()) {
					draggedObj.transform.SetParent (powerUps);
				}

				draggedObj = null;
			}

			Event.current.Use ();
		}

		GameObject selected = Selection.activeGameObject;

		if(selected) {
			if (selected.GetComponent<PlatformController> () && platSnap) {
				switch(Tools.current) {
				case Tool.Move:
					Vector3 mousePos = Event.current.mousePosition;
					mousePos.y = sceneview.camera.pixelHeight - mousePos.y;
					mousePos = sceneview.camera.ScreenToWorldPoint (mousePos);
					mousePos.z = 0;
					//Snap platform to grid
					mousePos.x = Mathf.Round (mousePos.x / platSnapSize) * platSnapSize;
					mousePos.y = Mathf.Round (mousePos.y / platSnapSize) * platSnapSize;
					selected.transform.position = mousePos;
					break;
				case Tool.Scale:
					Vector3 scale = selected.transform.localScale;
					scale.x = Mathf.Round (scale.x / platSnapSize) * platSnapSize;
					scale.y = Mathf.Round (scale.y / platSnapSize) * platSnapSize;
					selected.transform.localScale = scale;
					break;
				}
			}
		}
		Handles.EndGUI();
	}

	void OnGUI() {
		
		GUILayout.Label ("Map Settings", EditorStyles.boldLabel);
		mapName = EditorGUILayout.TextField ("Map Name", mapName);

		if (!mapInScene || mapInfo == null) {

			if (GUILayout.Button ("Generate Map \"" + mapName + "\"")) {
				mapInScene = createMap ();
			}
		}
		else {

			mapInfo.timeToFinish = EditorGUILayout.FloatField ("Time to Finish (sec)", mapInfo.timeToFinish);
			mapInfo.mapMoney = EditorGUILayout.IntField ("Map Money", mapInfo.mapMoney);

			mousedOver = null;

			EditorGUILayout.Separator ();
			GUILayout.Label ("Platforms", EditorStyles.boldLabel);


			foreach (GameObject go in placablePlatforms) {
				Texture goTex;
				if (goTex = go.GetComponent<MeshRenderer>().sharedMaterial.mainTexture) {
					GUIContent content = new GUIContent (go.name + " Platform", goTex);
					GUILayout.Box (content);

					//Check if this box was moused over
					if (GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition)) {
						mousedOver = go;
					}
				}
			}

			platSnap = EditorGUILayout.BeginToggleGroup ("Snap to Grid", platSnap);
			platSnapSize = EditorGUILayout.FloatField ("Snap Grid Size", platSnapSize);
			if (platSnapSize <= 0)
				platSnapSize = 0.03125f;
			EditorGUILayout.EndToggleGroup ();

			EditorGUILayout.Separator ();
			GUILayout.Label ("PowerUps", EditorStyles.boldLabel);

			//EditorGUILayout.BeginHorizontal ();
			foreach (GameObject go in placablePowerUps) {
				Texture goTex;
				if (goTex = go.GetComponent<SpriteRenderer> ().sprite.texture) {
					GUIContent content = new GUIContent (go.name, goTex);
					GUILayout.Box (content);

					//Check if this box was moused over
					if (GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition)) {
						mousedOver = go;
					}
				}
			}
			//EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			GUILayout.Label ("Weapons", EditorStyles.boldLabel);

			//EditorGUILayout.BeginHorizontal ();
			foreach (GameObject go in placableWeapons) {
				Texture goTex;
				if (goTex = go.GetComponent<PickUpWeapon> ()
						.attachedWeaponPrefab.GetComponent<SpriteRenderer> ().sprite.texture) {
					GUIContent content = new GUIContent (go.name, goTex);
					GUILayout.Box (content);

					//Check if this box was moused over
					if (GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition)) {
						mousedOver = go;
					}
				}
			}
			//EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Separator ();
			if (meme = EditorGUILayout.Toggle ("Meme", meme)) {
				SceneView.RepaintAll ();
			}

			handleDrag ();
		}
	}

	void handleDrag() {
		if (mousedOver) {
			switch (Event.current.type) {
			case EventType.MouseDrag:
				DragAndDrop.PrepareStartDrag ();
				Object[] or = { mousedOver };
				DragAndDrop.objectReferences = or;
				DragAndDrop.StartDrag ("BOOP");

				Event.current.Use ();
				break;
			}
		}
	}

	//Returns whether the map was created or not
	bool createMap() {
		//Check if this map name exists already
		if (Resources.Load ("Maps/" + mapName) as GameObject) {
			bool result = EditorUtility.DisplayDialog ("Map Already Exists", "A map with this name already exists," +
				" would you like to load it?", "OK", "Never Mind");
			if (!result)
				return false;
			else {
				//TODO
				//Load existing map
				return false;
			}
		}

		//Generate an empty map using the template
		GameObject template = Resources.Load ("Maps/Map_Template") as GameObject;

		mapObj = Instantiate (template);
		mapObj.name = mapName;

		platforms = mapObj.transform.Find ("Platforms");
		startLocation = mapObj.transform.Find ("StartLocation");
		endFlag = mapObj.transform.Find ("EndFlag");
		outOfBounds = mapObj.transform.Find ("OutOfBounds");
		powerUps = mapObj.transform.Find ("PowerUps");
		mapInfo = mapObj.GetComponent<MapInfo> ();

		return true;
	}

	//Pull MapInfo from mapObj
	void readInfo() {
		mapName = mapObj.name;
		platforms = mapObj.transform.Find ("Platforms");
		startLocation = mapObj.transform.Find ("StartLocation");
		endFlag = mapObj.transform.Find ("EndFlag");
		outOfBounds = mapObj.transform.Find ("OutOfBounds");
		powerUps = mapObj.transform.Find ("PowerUps");
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}

	//Push MapInfo to mapObj
	void WriteInfo() {
		mapObj.name = mapName;
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}
}
