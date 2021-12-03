using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SoSearcher
{
    public class SOSearcherWindow : EditorWindow
    {
        protected virtual string ToolTip => "This window does stuff-";
    
        protected UnityEditor.Editor Editor;
        protected Vector2 scrollPos;

        private ScriptableObject _selectedScriptableObject;

        private List<SearchCriteria> _searchCriteria = new List<SearchCriteria>();
        private List<ScriptableObject> _selection;

        [MenuItem("Window/SOSearcher")]
        static void Init()
        {
            SOSearcherWindow window;
            window = (SOSearcherWindow)GetWindow(typeof(SOSearcherWindow));
            window.Show();
        }

        private void OnEnable()
        {
            if (_selection == null)
            {
                ResetSelection();
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal("box");
            {
                if (GUILayout.Button("Create GO search criteria"))
                {
                    _searchCriteria.Add(new GameObjectCriteria());
                }
                if (GUILayout.Button("Create int search criteria"))
                {
                    _searchCriteria.Add(new IntCriteria());
                }
            }
            EditorGUILayout.EndHorizontal();

            for (var i = _searchCriteria.Count - 1; i >= 0; i--)
            {
                SearchCriteria searchCriteria = _searchCriteria[i];
                GUILayout.BeginHorizontal("Box");
                {
                    searchCriteria.OnGUI();
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        _searchCriteria.Remove(searchCriteria);
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Reset selection"))
                {
                    ResetSelection();
                }
        
                if (GUILayout.Button("Search"))
                {
                    Search();
                }
            }
            GUILayout.EndHorizontal();
        
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.ExpandHeight(true)))
            {
                if (!Editor)
                {
                    Editor = UnityEditor.Editor.CreateEditor(this);
                }

                scrollPos = scrollView.scrollPosition;
            
                EditorGUILayout.HelpBox(ToolTip, MessageType.Info);
            
                Editor.OnInspectorGUI();
            
                GUILayout.BeginVertical("Box");
                {
                    DrawBody();
                }
                GUILayout.EndVertical();
            }
        }

        private void DrawBody()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical("box", GUILayout.Width(150));
                {
                    foreach (var scriptableObject in _selection)
                    {
                        GUILayout.BeginVertical("box");
                        {
                            if (GUILayout.Button(scriptableObject.name))
                            {
                                _selectedScriptableObject = scriptableObject;
                            }
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                {
                    if (_selectedScriptableObject != null)
                    {
                        GUILayout.Label(_selectedScriptableObject.name);
                        var editor = Editor.CreateEditor(_selectedScriptableObject);
                        editor.OnInspectorGUI();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void ResetSelection()
        {
            _selection = AssetDatabase.FindAssets("t: scriptableobject")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableObject>)
                .ToList();
        }
    
        private void Search()
        {
            for (var index = _selection.Count - 1; index >= 0; index--)
            {
                ScriptableObject scriptableObject = _selection[index];
                SerializedObject serializedObject = new SerializedObject(scriptableObject);

                var match = _searchCriteria.TrueForAll(criteria =>
                {
                    bool found = false;
                    using (SerializedProperty iterator = serializedObject.GetIterator())
                    {
                        iterator.Next(true);
                        while (iterator.Next(true))
                        {
                            if (criteria.CompareTo(iterator))
                            {
                                found = true;
                                break;
                            }
                        }
                    }

                    return found;
                });
            
                if(!match)
                    _selection.Remove(scriptableObject);
            }
        }
    
        public void Reload()
        {
            
        }
    }
}