using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PS.Build.Tasks
{
    public class DomainAssemblyResolver : MarshalByRefObject,
                                           IDisposable
    {
        #region Static members

        private static string FindAtLocation(string queryAssemblyName, string location)
        {
            if (string.IsNullOrEmpty(location)) return null;
            return Directory.GetFiles(location, "*.dll")
                            .FirstOrDefault(r => string.Equals(Path.GetFileNameWithoutExtension(r),
                                                               queryAssemblyName,
                                                               StringComparison.InvariantCultureIgnoreCase));
        }

        #endregion

        private readonly string[] _assemblyReferences;
        private readonly string[] _additionalDirectories;

        #region Constructors

        public DomainAssemblyResolver(string[] additionalDirectories, string[] assemblyReferences)
        {
            if (additionalDirectories == null) throw new ArgumentNullException(nameof(additionalDirectories));
            if (assemblyReferences == null) throw new ArgumentNullException(nameof(assemblyReferences));
            _assemblyReferences = assemblyReferences;
            _additionalDirectories = additionalDirectories;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region Event handlers

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var queryAssemblyName = args.Name.Split(',').FirstOrDefault();
            var resolved = _assemblyReferences.FirstOrDefault(r => string.Equals(Path.GetFileNameWithoutExtension(r),
                                                                                 queryAssemblyName,
                                                                                 StringComparison.InvariantCultureIgnoreCase));
            foreach (var directory in _additionalDirectories)
            {
                if(resolved != null) break;
                resolved = FindAtLocation(queryAssemblyName, directory);
            }

            return string.IsNullOrWhiteSpace(resolved)
                ? null
                : Assembly.LoadFile(resolved);
        }

        #endregion
    }
}