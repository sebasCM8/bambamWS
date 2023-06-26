using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bambamWS.Models;
using Microsoft.Identity.Client;

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

        [HttpPost("registrarCliente")]
        public async Task<ActionResult<ResponseResult>> registrarCliente(Usuario usuX)
        {
            ResponseResult result = new ResponseResult();
            try {
                var usuExiste = await _context.Usuarios.FindAsync(usuX.usuId);
                if(usuExiste != null)
                {
                    result.ok = false;
                    result.msg = "Usuario ya existe";
                    return result;
                }
                _context.Usuarios.Add(usuX);
                await _context.SaveChangesAsync();
                result.ok = true;
                result.msg = "Usuario cliente registrado";
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }
            return result;
        }

        [HttpPost("loginCliente")]
        public async Task<ActionResult<ResponseResult>> loginCliente(Usuario usuX)
        {
            ResponseResult result = new ResponseResult();
            try
            {
                var usu = await _context.Usuarios.Where(u => u.usuId == usuX.usuId && u.usuEstado == 1).FirstOrDefaultAsync();    
                if(usu == null)
                {
                    result.ok = false;
                    result.msg = "Datos incorrectoss";
                    return result;
                }
                if(usu.usuPass != usuX.usuPass)
                {
                    result.ok = false;
                    result.msg = "Datos incorrectos";
                    return result;
                }
                result.ok = true;
                result.msg = "Login exitoso";
                return result;
            }catch(Exception e)
            {
                result.ok = false;
                result.msg = e.Message;
            }
            return result;
        }

        [HttpGet("usuapitest")]
        public async Task<ActionResult<RRObtRepartidores>> usuapitest()
        {
            RRObtRepartidores result = new RRObtRepartidores();
            result.ok = true;
            result.msg = "PRUEBA EXITOSA";
            result.repartidores = new List<Usuario>();
            string[] nombres = { "Pablo", "Jeremy", "Ana", "John", "Alicia", "Michelle", "Frank", "Jan", "Islam" };
            int i = 0;
            foreach(string nombre in nombres) 
            {
                i++;
                Usuario nueUsu = new Usuario();
                nueUsu.usuNombre = nombre;
                nueUsu.usuId = "usuId" + i.ToString();
                nueUsu.usuPass = "123";
                nueUsu.usuApellido = "Dern Maia";
                nueUsu.usuCelular = "77982182";
                nueUsu.usuCI = "77921929";
                nueUsu.usuEstado = 1;
                result.repartidores.Add(nueUsu);
            }
            return result;
        }

    }
}
