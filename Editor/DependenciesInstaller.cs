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
            { "com.egorlin.disolated", "https://github.com/egorlin/com.egorlin.disolated.git#1.1.1" },
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

                var installedVersions = new Dictionary<string, System.Version>();
                if (listRequest.Result != null)
                {
                    foreach (var p in listRequest.Result)
                    {
                        if (System.Version.TryParse(p.version, out var v))
                            installedVersions[p.name] = v;
                    }
                }

                var toInstall = new List<string>();
                foreach (var (name, url) in Packages)
                {
                    var desiredVersion = ParseVersionFromUrl(url);

                    if (!installedVersions.TryGetValue(name, out var installedVersion))
                    {
                        toInstall.Add(url);
                    }
                    else if (desiredVersion != null && desiredVersion > installedVersion)
                    {
                        toInstall.Add(url);
                    }
                }

                if (toInstall.Count > 0)
                    Client.AddAndRemove(toInstall.ToArray());
            }
        }

        private static System.Version ParseVersionFromUrl(string url)
        {
            var hash = url.LastIndexOf('#');
            
            if (hash < 0)
            {
                return null;
            }
            
            var tag = url.Substring(hash + 1);
            return System.Version.TryParse(tag, out var v) ? v : null;
        }
    }
}