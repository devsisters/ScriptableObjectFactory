using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectFactory
{
    public static class ScriptableObjectFactory
    {
        [MenuItem("Assets/Create/ScriptableObject")]
        public static void CreateScriptableObject() {

            List<Type> allScriptableObjects;

            try {
                var assemblies = GetAssemblies();

                allScriptableObjects= new List<Type>(from assembly in assemblies
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(typeof(ScriptableObject))
                    select type);
            }
            catch (Exception e) {
                Debug.LogError("Error while opening scriptable object window: "+e.Message);
                allScriptableObjects = new List<Type>();
            }

            ScriptableObjectWindow.Init(allScriptableObjects.ToArray());
        }

        private static IEnumerable<Assembly> GetAssemblies() {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}