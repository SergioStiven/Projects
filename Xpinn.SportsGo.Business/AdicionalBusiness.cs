using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Business
{
    public class AdicionalBusiness
    {
        public async Task<List<Periodicidades>> ListarPeriodicidades()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdicionalRepository adicionalBusiness = new AdicionalRepository(context);
                List<Periodicidades> listaPeriodicidades = await adicionalBusiness.ListarPeriodicidades();

                return listaPeriodicidades;
            }
        }
    }
}
