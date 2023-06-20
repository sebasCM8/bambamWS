using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bambamWS.Models;

namespace bambamWS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioContext _context;

        public UsuariosController(UsuarioContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet("obtenerRepartidores")]
        public async Task<ActionResult<RRObtRepartidores>> obtenerRepartidores()
        {
            RRObtRepartidores result = new RRObtRepartidores();
            try
            {
                var repartidores = await (from ur in _context.usurol
                                          join usu in _context.Usuarios
                                          on ur.urUsu equals usu.usuId
                                          where ur.urRol == BambamConstantes.REPARTIDOR && usu.usuEstado == 1
                                          select usu).ToListAsync();
                if (repartidores != null && repartidores.Count() > 0)
                {
                    result.repartidores = repartidores;
                }
                result.ok = true;
                result.msg = "Repartidores obtenidos";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }

            return result;
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(string id)
        {
          if (_context.Usuarios == null)
          {
              return NotFound();
          }
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(string id, Usuario usuario)
        {
            if (id != usuario.usuId)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
          if (_context.Usuarios == null)
          {
              return Problem("Entity set 'UsuarioContext.Usuarios'  is null.");
          }
            _context.Usuarios.Add(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UsuarioExists(usuario.usuId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUsuario", new { id = usuario.usuId }, usuario);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseResult>> Login(Usuario usr)
        {
            ResponseResult responseResult = new ResponseResult();
            Usuario? usu = await _context.Usuarios.Where(u => u.usuId == usr.usuId && u.usuEstado == 1).FirstOrDefaultAsync<Usuario>();
            if(usu == null)
            {
                responseResult.ok = false;
                responseResult.msg = "Datos incorrectos";
                return responseResult;
            }

            if(usu.usuPass != usr.usuPass)
            {
                responseResult.ok = false;
                responseResult.msg = "Datos incoorectos";
                return responseResult;
            }

            var permiso = await _context.usurol.
                Where(ur => ur.urUsu == usu.usuId && (ur.urRol == BambamConstantes.ADMINISTRADOR || ur.urRol == BambamConstantes.REPARTIDOR)).
                ToListAsync();
            if (permiso.Count() == 0 || permiso == null)
            {
                responseResult.ok = false;
                responseResult.msg = "Usuario no es trabajador";
                return responseResult;
            }

            responseResult.ok = true;
            responseResult.msg = "Login exitoso";
            return responseResult;
        }

        [HttpPost("verificarAdmin")]
        public async Task<ActionResult<ResponseResult>> verificarAdmin(Usuario usu)
        {
            ResponseResult result = new ResponseResult();
            var permiso = await _context.usurol.
                Where(ur => ur.urUsu == usu.usuId && ur.urRol == BambamConstantes.ADMINISTRADOR).ToListAsync();
            if(permiso.Count() == 0 || permiso == null)
            {
                result.ok = false;
                result.msg = "Usuario no es administrador";
                return result;
            }

            result.ok = true;
            result.msg = "Usuario es administrador";
            return result;

        }

        [HttpPost("verificarTrabajador")]
        public async Task<ActionResult<ResponseResult>> verificarTrabajador(Usuario usu)
        {
            ResponseResult result = new ResponseResult();
            var permiso = await _context.usurol.
                Where(ur => ur.urUsu == usu.usuId && (ur.urRol == BambamConstantes.ADMINISTRADOR || ur.urRol == BambamConstantes.REPARTIDOR)).
                ToListAsync();
            if (permiso.Count() == 0 || permiso == null)
            {
                result.ok = false;
                result.msg = "Usuario no es trabajador";
                return result;
            }

            result.ok = true;
            result.msg = "Usuario es trabajador";
            return result;

        }

        [HttpPost("registrarRepartidor")]
        public async Task<ActionResult<ResponseResult>> registrarRepartidor(Usuario usuX)
        {
            ResponseResult result = new ResponseResult();

            var existeUsu = await _context.Usuarios.Where(usu => usu.usuId == usuX.usuId).ToListAsync();
            if(existeUsu == null || existeUsu.Count() > 0)
            {
                result.ok = false;
                result.msg = "Usuario ya existe";
                return result;
            }

            Usurol rolRepartidor = new Usurol();
            rolRepartidor.urUsu = usuX.usuId;
            rolRepartidor.urRol = BambamConstantes.REPARTIDOR;
            _context.Usuarios.Add(usuX);
            _context.usurol.Add(rolRepartidor);
            try
            {
                await _context.SaveChangesAsync();
                result.ok = true;
                result.msg = "Repartidor creado correctamente";
            }
            catch (Exception e) { 
                result.ok = false;
                result.msg = e.Message;
            }


            return result;
        }

        [HttpPost("eliminarRepartidor")]
        public async Task<ActionResult<ResponseResult>> eliminarRepartidor(Usuario usuX)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                Usuario theUser = await _context.Usuarios.FindAsync(usuX.usuId);
                theUser.usuEstado = 0;
                await _context.SaveChangesAsync();
                result.ok = true;
                result.msg = "Repartidor eliminado";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }
            
            return result;
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(string id)
        {
            return (_context.Usuarios?.Any(e => e.usuId == id)).GetValueOrDefault();
        }
    }
}
