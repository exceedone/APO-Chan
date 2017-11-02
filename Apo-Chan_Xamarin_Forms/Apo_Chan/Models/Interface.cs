using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apo_Chan.Models
{
    /// <summary>
    /// Authentication Interface
    /// </summary>
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync(Constants.EProviderType providerType);
        Task<bool> SignOutAsync();
    }

    /// <summary>
    /// app packagename, versionname, versioncode
    /// http://itblogdsi.blog.fc2.com/blog-entry-79.html
    /// </summary>
    public interface IAssemblyService
    {
        /// <summary>
        /// get application packege name
        /// </summary>
        /// <returns></returns>
        string GetPackageName();
        /// <summary>
        /// application version name. ex. 1.0.1
        /// </summary>
        /// <returns></returns>
        string GetVersionName();
        int GetVersionCode();
    }

    /// <summary>
    /// Session Data Repository
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// Set Data
        /// </summary>
        void SetValue<T>(T value) where T : class;

        /// <summary>
        /// Set Data
        /// </summary>
        void SetValue<T>(string key, T value) where T : class;

        /// <summary>
        /// Get Data
        /// </summary>
        T GetValue<T>() where T : class;

        /// <summary>
        /// Get Data
        /// </summary>
        T GetValue<T>(string key) where T : class;

        /// <summary>
        /// Initilize
        /// </summary>
        bool Initilize();

        /// <summary>
        /// Load Session Data
        /// </summary>
        Task<bool> LoadAsync();

        /// <summary>
        /// Save Session Data
        /// </summary>
        Task<bool> SaveAsync();
    }
}
