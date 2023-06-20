using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bambamWS.Models;

namespace bambamWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnidadesController : ControllerBase
    {
        private readonly UsuarioContext _context;
        
        public UnidadesController(UsuarioContext context) {
            _context = context;
        }

        [HttpPost("registrarUnidad")]
        public async Task<ActionResult<RRRegUnidad>> registrarUnidad(Unidad uniX)
        {
            RRRegUnidad result = new RRRegUnidad();

            try
            {
                if (uniX.uniId == 0)
                {
                    _context.unidades.Add(uniX);
                    await _context.SaveChangesAsync();
                    result.ok = true;
                    result.msg = "Unidad de medida creada";
                    result.unidadId = uniX.uniId;
                }
                else
                {
                    Unidad uni = await _context.unidades.FindAsync(uniX.uniId);
                    uni.uniNombre = uniX.uniNombre;
                    await _context.SaveChangesAsync();
                    result.ok = true;
                    result.msg = "Unidad de medida actualizada";
                    result.unidadId = uniX.uniId;
                }
            }
            catch (Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

        [HttpGet("obtenerUnidades")]
        public async Task<ActionResult<RRObtUnidades>> obtenerUnidades()
        {
            RRObtUnidades result = new RRObtUnidades();

            try
            {
                var unis = await _context.unidades.Where(u => u.uniEstado == 1).ToListAsync();
                if (unis != null && unis.Count() > 0)
                {
                    result.unidades = unis;
                }
                result.ok = true;
                result.msg = "Unidades de medidas obtenidas";
            }
            catch (Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }
    }
}
