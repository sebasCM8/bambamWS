using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bambamWS.Models;

namespace bambamWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly UsuarioContext _context;
        public ProductosController(UsuarioContext context)
        {
            _context=context;
        }

        [HttpGet("obtenerProductos")]
        public async Task<ActionResult<RRObtProductos>> obtenerProductos()
        {
            RRObtProductos result = new RRObtProductos();
            result.productos = new List<Producto>();
            try
            {
                var productos = await (from p in _context.productos
                                       join c in _context.categorias on p.proCat equals c.catId
                                       join u in _context.unidades on p.proUni equals u.uniId
                                       where p.proEstado == 1
                                       select new { p, u.uniNombre, c.catNombre }).ToListAsync();

                foreach (var item in productos)
                {
                    Producto pp = item.p;
                    pp.proUniNombre = item.uniNombre;
                    pp.proCatNombre = item.catNombre;
                    result.productos.Add(pp);
                }

                result.ok = true;
                result.msg = "Productos obtenidos";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

        [HttpPost("registrarProducto")]
        public async Task<ActionResult<ResponseResult>> registrarProducto(Producto proX)
        {
            ResponseResult result = new ResponseResult();

            try
            {
                _context.productos.Add(proX);
                await _context.SaveChangesAsync();

                result.ok = true;
                result.msg = "Producto registrado";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

        [HttpGet("obtenerCatUni")]
        public async Task<ActionResult<RRObtCatsUnis>> obtenerCatsUnis()
        {
            RRObtCatsUnis result = new RRObtCatsUnis();

            try
            {
                var cats = await _context.categorias.Where(c => c.catEstado == 1).ToListAsync();
                if (cats != null && cats.Count() > 0)
                {
                    result.categorias = cats;
                }

                var unis = await _context.unidades.Where(u => u.uniEstado == 1).ToListAsync();
                if (unis != null && unis.Count() > 0)
                {
                    result.unidades = unis;
                }

                result.ok = true;
                result.msg  = "Parametros de producto obtenidos";
            }
            catch (Exception e){
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

    }
}
