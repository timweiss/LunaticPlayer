using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaticPlayer.Classes;

namespace LunaticPlayer.GRadioAPI
{
    public interface IApiClient
    {
        /// <summary>
        /// Checks whether the GR API is reachable (required).
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckApiAccess();

        /// <summary>
        /// Loads the API data and deserializes it.
        /// </summary>
        /// <returns></returns>
        Task FetchRawApiData();

        /// <summary>
        /// Returns the currently playing song.
        /// </summary>
        /// <returns>Current song</returns>
        Song PlayingSong();
    }
}
