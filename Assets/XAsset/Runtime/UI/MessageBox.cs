﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class MessageBox : IEnumerator
{
	public bool isOk { get; private set; }

	private bool _visible = true;

	#region IEnumerator implementation

	public bool MoveNext ()
	{
		return _visible;
	}

	public void Reset ()
	{
	}

	public object Current {
		get {
			return null;
		}
	}

	#endregion

	private GameObject gameObject { get; set; }

	private Text _title;
	private Text _content;
	private Text _textOk;
	private Text _textNo;

	private static GameObject _prefab = Resources.Load<GameObject> ("MessageBox"); 
	private static  List<MessageBox> _showed = new List<MessageBox> ();
	private static  List<MessageBox> _hidden = new List<MessageBox> ();

	public static void Dispose ()
	{
		foreach (var item in _hidden) {
			item.Destroy ();
		} 
		_hidden.Clear ();
		_hidden = null;

		foreach (var item in _showed) {
			item.Destroy ();
		}
		_showed.Clear ();
		_showed = null; 
        
		_prefab = null;  
	}

	public static MessageBox Show (string title, string content, string ok = "确定", string no = "取消")
	{
		if (_hidden.Count > 0) {
			var mb = _hidden [0];
			mb.Init (title, content, ok, no);
			mb.gameObject.SetActive (true);
			_hidden.RemoveAt (0);
			return mb;
		} else {
			return new MessageBox (title, content, ok, no);
		}
	}

	public void Destroy ()
	{
		_title = null;
		_textOk = null;
		_textNo = null;
		_content = null;
		Object.DestroyImmediate (gameObject);
		gameObject = null;
	}

	private MessageBox (string title, string content, string ok, string no)
	{
		gameObject = Object.Instantiate (_prefab);
		gameObject.name = title;

		_title = GetComponent<Text> ("Title");
		_content = GetComponent<Text> ("Content/Text");
		_textOk = GetComponent<Text> ("Buttons/Ok/Text");
		_textNo = GetComponent<Text> ("Buttons/No/Text");

		var ok1 = GetComponent<Button> ("Buttons/Ok");
		var no1 = GetComponent<Button> ("Buttons/No");
		ok1.onClick.AddListener (OnClickOk);
		no1.onClick.AddListener (OnClickNo);

		Init (title, content, ok, no);
	}

	private void Init (string title, string content, string ok, string no)
	{
		_title.text = title;
		_content.text = content;
		_textOk.text = ok;
		_textNo.text = no; 
		_showed.Add (this);
		_visible = true;
		isOk = false;
	}

	public enum EventId
	{
		Ok,
		No,
	}

	public Action<EventId> onComplete { get; set; }

	private T GetComponent<T> (string path) where T : Component
	{
		var trans = gameObject.transform.Find (path);
		return trans.GetComponent<T> ();
	}

	private void OnClickNo ()
	{
		HandleEvent (EventId.No);
	}

	private void OnClickOk ()
	{
		HandleEvent (EventId.Ok);
	}

	private void HandleEvent (EventId id)
	{
		switch (id) {
		case EventId.Ok:
			break;
		case EventId.No:
			break;
		default:
			throw new ArgumentOutOfRangeException ("id", id, null);
		}

		gameObject.SetActive (false);
		_hidden.Add (this);
		_showed.Remove (this);
		_visible = false;

		isOk = id == EventId.Ok; 

		if (onComplete != null) {
			onComplete (id);
			onComplete = null;
		} 
	}
}