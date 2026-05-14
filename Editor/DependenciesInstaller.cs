using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;

namespace EgorLin.Storage.Editor
{
    [InitializeOnLoad]
    public static class DependenciesInstaller
    {
        private static readonly Dictionary<string, string> Packages = new()
        {
            { "com.egorlin.disolated", "https://github.com/egorlin/com.egorlin.disolated.git#1.1.0" },
        };

        static DependenciesInstaller()
        {
            var listRequest = Client.List(offlineMode: false, includeIndirectDependencies: true);
            EditorApplication.update += WaitForList;
            
            return;

            void WaitForList()
            {
                if (!listRequest.IsCompleted)
                {
                    return;
                }
                
                EditorApplication.update -= WaitForList;

                var installed = new HashSet<string>();
                if (listRequest.Result != null)
                {
                    foreach (var p in listRequest.Result)
                    {
                        installed.Add(p.name);
                    }
                }

                var toInstall = new List<string>();
                foreach (var (name, url) in Packages)
                {
                    if (!installed.Contains(name))
                    {
                        toInstall.Add(url);
                    }
                }

                if (toInstall.Count > 0)
                {
                    Client.AddAndRemove(toInstall.ToArray());
                }
            }
        }
    }
}