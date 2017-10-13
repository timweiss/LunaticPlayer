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
        Task<bool> CheckApiAccess();
        Task FetchRawApiData();
        Song PlayingSong();
    }
}
