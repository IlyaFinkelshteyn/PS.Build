using System;
using System.ComponentModel;
using PS.Build.Services;

namespace DefinitionLibrary.Assembly
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    [Designer("PS.Build.Adaptation")]
    public sealed class AllAttribute : Attribute
    {
        #region Members

        void PostBuild(IServiceProvider provider)
        {
            var logger = (ILogger)provider.GetService(typeof(ILogger));
            logger.Info("--- PostBuild:" + GetType().Name);
        }

        void PreBuild(IServiceProvider provider)
        {
            var logger = (ILogger)provider.GetService(typeof(ILogger));
            logger.Info("--- PreBuild:" + GetType().Name);
        }

        #endregion
    }
}