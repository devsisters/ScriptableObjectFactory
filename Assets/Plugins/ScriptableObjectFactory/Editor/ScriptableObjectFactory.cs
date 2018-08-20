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
        public static void CreateScriptableObjectFactoryWindow()
        {
            CreateScriptableObjectFactoryWindow(false);
        }

        public static void CreateScriptableObjectFactoryWindow(bool getAllAssemblies)
        {

            List<Type> allScriptableObjectTypes;

            try
            {
                var assemblies = getAllAssemblies ? GetAllAssemblies() : GetCSharpAssembly();

                allScriptableObjectTypes = new List<Type>(from assembly in assemblies
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(typeof(ScriptableObject))
                    select type);
            }
            catch (Exception e)
            {
                Debug.LogError("Error while opening scriptable object window: " + e.Message);
                allScriptableObjectTypes = new List<Type>();
            }

            ScriptableObjectWindow.Init(allScriptableObjectTypes.ToArray(), getAllAssemblies);
        }

        private static IEnumerable<Assembly> GetCSharpAssembly()
        {
            return new[] {Assembly.Load(new AssemblyName("Assembly-CSharp"))};
        }

        private static IEnumerable<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}