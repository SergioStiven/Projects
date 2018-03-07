using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class AdicionalController : ApiController
    {
        AdicionalBusiness _adicionalBusiness;

        public AdicionalController()
        {
            _adicionalBusiness = new AdicionalBusiness();
        }

        public async Task<IHttpActionResult> ListarPeriodicidades()
        {
            try
            {
                List<Periodicidades> listaPeriodicidades = await _adicionalBusiness.ListarPeriodicidades();

                return Ok(listaPeriodicidades);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
