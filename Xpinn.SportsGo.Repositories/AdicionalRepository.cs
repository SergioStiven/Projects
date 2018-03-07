using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;

namespace Xpinn.SportsGo.Repositories
{
    public class AdicionalRepository
    {
        SportsGoEntities _context;

        public AdicionalRepository(SportsGoEntities context)
        {
            _context = context;
        }


        public async Task<List<Periodicidades>> ListarPeriodicidades()
        {
            List<Periodicidades> listaPeriodicidades = await _context.Periodicidades.AsNoTracking()
                                                                                    .ToListAsync();

            return listaPeriodicidades;
        }
    }
}
