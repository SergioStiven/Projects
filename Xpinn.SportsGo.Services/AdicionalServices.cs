using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Services
{
    public class AdicionalServices : BaseService
    {
        public async Task<List<PeriodicidadesDTO>> ListarPeriodicidades()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<PeriodicidadesDTO> listaPeriodicidades = await client.PostAsync<List<PeriodicidadesDTO>>("Adicional/ListarPeriodicidades");

            return listaPeriodicidades;
        }
    }
}
