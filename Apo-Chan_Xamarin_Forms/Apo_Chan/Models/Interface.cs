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

}
