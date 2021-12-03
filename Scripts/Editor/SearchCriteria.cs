using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SoSearcher
{
    public class IntCriteria : SearchCriteria
    {
        public int Value;
    
        public override bool CompareTo(SerializedProperty serializedProperty)
        {
            if (serializedProperty.type != "int") return false;
            return Value == serializedProperty.intValue;
        }
    
        public override void OnGUI()
        {
            Value = EditorGUILayout.IntField("Value", Value);
        }
    }

    [Serializable]
    public class GameObjectCriteria : SearchCriteria
    {
        [SerializeField]
        public GameObject gameObject;
    
        public override bool CompareTo(SerializedProperty serializedProperty)
        {
            if (serializedProperty.type != "PPtr<$GameObject>") return false;
            return gameObject == serializedProperty.objectReferenceValue;
        }
    
        public override void OnGUI()
        {
            gameObject = EditorGUILayout.ObjectField("GameObject", gameObject, typeof(GameObject), false) as GameObject;
        }
    }

    [Serializable]
    public class SearchCriteria
    {
        public virtual bool CompareTo(SerializedProperty serializedProperty)
        {
            return true;
        }

        public virtual void OnGUI()
        {
        
        }
    }
}