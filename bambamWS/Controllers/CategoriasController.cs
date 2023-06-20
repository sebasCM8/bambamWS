using bambamWS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace bambamWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly UsuarioContext _context;
        public CategoriasController(UsuarioContext context)
        {
            _context=context;
        }

        [HttpPost("registrarCategoria")]
        public async Task<ActionResult<RRRegCategoria>> registrarCategoria(Categoria catX)
        {
            RRRegCategoria result = new RRRegCategoria();

            try
            {
                if(catX.catId == 0)
                {
                    _context.categorias.Add(catX);
                    await _context.SaveChangesAsync();
                    result.ok = true;
                    result.msg = "Categoria creada";
                    result.categoriaId = catX.catId;
                }
                else
                {
                    Categoria cat = await _context.categorias.FindAsync(catX.catId);
                    cat.catNombre = catX.catNombre;
                    await _context.SaveChangesAsync();
                    result.ok = true;
                    result.msg = "Categoria actualizada";
                    result.categoriaId = catX.catId;
                }
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

        [HttpGet("obtenerCategorias")]
        public async Task<ActionResult<RRObtCategorias>> obtenerCategorias()
        {
            RRObtCategorias result = new RRObtCategorias();

            try
            {
                var cats = await _context.categorias.Where(c => c.catEstado == 1).ToListAsync();
                if(cats != null && cats.Count() > 0)
                {
                    result.categorias = cats;
                }
                result.ok = true;
                result.msg = "Categorias obtenidas";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }
    }
}
