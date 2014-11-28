using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace OmnisharpForUnity
{
    internal class BaseUtility
    {
        internal static string projectPath
        {
            get
            {
                return  Directory.GetParent(Application.dataPath).FullName;
            }
        }

        internal static string projectName
        {
            get
            {
                return Path.GetFileName(projectPath);
            }
        }

        internal static string[] scriptAssemblies
        {
            get
            {
                return Directory.GetFiles("Library/ScriptAssemblies", "*.dll").Select(path => Path.GetFileName(path)).ToArray();
            }
        }


        internal static string unityAssembliesDirectoryPath
        {
            get
            {
                return Path.Combine(projectPath, "Library/UnityAssemblies");
            }
        }

        internal static string[] loadedAssemblyPaths
        {
            get
            {
                return AssemblyHelper.GetNamesOfAssembliesLoadedInCurrentDomain();
            }
        }
    }
}