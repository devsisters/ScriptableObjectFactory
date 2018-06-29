using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectFactory
{
    public class ScriptableObjectFactory
    {
        [MenuItem("Assets/Create/ScriptableObject")]
        public static void CreateScriptableObject()
        {
            Type[] allScriptableObjects;

            try
            {
                var assembly = GetAssembly();
                allScriptableObjects = (from t in assembly.GetTypes()
                    where t.IsSubclassOf(typeof(ScriptableObject))
                    select t).ToArray();
            }
            catch (Exception)
            {
                allScriptableObjects = new Type[] { };
            }
             
            ScriptableObjectWindow.Init(allScriptableObjects);
        }

        private static Assembly GetAssembly()
        {
            return Assembly.Load(new AssemblyName("Assembly-CSharp"));
        }
    }
}