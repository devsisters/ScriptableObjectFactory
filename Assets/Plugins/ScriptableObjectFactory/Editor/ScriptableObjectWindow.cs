using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace ScriptableObjectFactory
{
    internal class EndNameEdit : EndNameEditAction
    {
        #region implemented abstract members of EndNameEditAction

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId),
                AssetDatabase.GenerateUniqueAssetPath(pathName));
        }

        #endregion
    }

    public class ScriptableObjectWindow : EditorWindow
    {
        private const float LabelWidth = 180f;

        private bool _initialFocusSet;

        private string _searchFilter = string.Empty;

        private string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                if (value != _searchFilter)
                    _searchResults = FindMatchingNames(value);
                _searchFilter = value;
            }
        }

        private string[] _searchResults = { };
        private int _selectedIndex;
        private static string[] _scriptableObjectNames;

        private static Type[] _types;

        private static Type[] Types
        {
            get { return _types; }
            set
            {
                _types = value;
                _scriptableObjectNames = _types.Select(t => t.FullName).ToArray();
            }
        }

        public static void Init(Type[] scriptableObjects)
        {
            Types = scriptableObjects;

            var window = GetWindow<ScriptableObjectWindow>(true, "Create a new ScriptableObject", true);
            window.ShowPopup();
        }

        private void Awake()
        {
            _searchResults = FindMatchingNames(string.Empty);
        }

        public void OnGUI()
        {
            DrawSearch();
            DrawSelectionPopup();
            DrawCreateButton();
        }

        private void DrawSearch()
        {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search:", GUILayout.Width(LabelWidth));

            GUI.SetNextControlName("SearchField");
            SearchFilter = EditorGUILayout.TextField(SearchFilter);
            GUILayout.EndHorizontal();

            if (!_initialFocusSet)
            {
                _initialFocusSet = true;
                EditorGUI.FocusTextInControl("SearchField");
            }
        }

        private void DrawSelectionPopup()
        {
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select ScriptableObject Class:", GUILayout.Width(LabelWidth));
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _searchResults);
            GUILayout.EndHorizontal();
        }

        private void DrawCreateButton()
        {
            GUILayout.Space(6);
            if (GUILayout.Button("Create"))
            {
                var realIndex = Array.FindIndex(_scriptableObjectNames, n => n == _searchResults[_selectedIndex]);
                var asset = CreateInstance(_types[realIndex]);
                var fileName = _scriptableObjectNames[realIndex].Split('.').Last();

                ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                    asset.GetInstanceID(),
                    CreateInstance<EndNameEdit>(),
                    string.Format("{0}.asset", fileName),
                    AssetPreview.GetMiniThumbnail(asset),
                    null);

                Close();
            }
        }

        private string[] FindMatchingNames(string filter)
        {
            var selectedName = _searchResults.Length == 0 
                ? string.Empty
                : _searchResults[_selectedIndex];

            var matchingNames =  string.IsNullOrEmpty(filter)
                ? _scriptableObjectNames
                : _scriptableObjectNames.Where(scriptableObjectname => IsMatch(scriptableObjectname, filter)).ToArray();

            var newIndex = Array.FindIndex(matchingNames, matchingName => matchingName == selectedName);
            _selectedIndex = newIndex < 0 
                ? 0 
                : newIndex;

            return matchingNames;
        }

        private static bool IsMatch(string text, string searchTerm)
        {
            try
            {
                var splitSearchTerms = searchTerm.Split(null);
                return splitSearchTerms.All(term =>
                {
                    var regex = new Regex(term, RegexOptions.IgnoreCase);
                    return regex.Match(text).Success;
                });
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}