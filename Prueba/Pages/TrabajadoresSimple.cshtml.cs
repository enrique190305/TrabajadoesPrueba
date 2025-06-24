using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Prueba.Models;
using Prueba.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Linq;

namespace Prueba.Pages
{
    public class TrabajadoresSimpleModel : PageModel
    {
        private readonly TrabajadorService _trabajadorService;
        private readonly ILogger<TrabajadoresSimpleModel> _logger;

        public TrabajadoresSimpleModel(TrabajadorService trabajadorService, ILogger<TrabajadoresSimpleModel> logger)
        {
            _trabajadorService = trabajadorService;
            _logger = logger;
        }

        public List<TrabajadorViewModel> Trabajadores { get; set; } = new List<TrabajadorViewModel>();
        public List<TrabajadorViewModel> TrabajadoresFiltrados { get; set; } = new List<TrabajadorViewModel>();
        public List<Departamento> Departamentos { get; set; } = new List<Departamento>();

        [BindProperty(SupportsGet = true)]
        public string FiltroSexo { get; set; }

        [BindProperty]
        public Trabajador NuevoTrabajador { get; set; } = new Trabajador();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Cargando página TrabajadoresSimple");
                Trabajadores = await _trabajadorService.ObtenerTrabajadoresAsync();
                Departamentos = await _trabajadorService.ObtenerDepartamentosAsync();
                
                if (!string.IsNullOrEmpty(FiltroSexo))
                {
                    TrabajadoresFiltrados = Trabajadores.Where(t => t.Sexo == FiltroSexo).ToList();
                }
                else
                {
                    TrabajadoresFiltrados = Trabajadores;
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar TrabajadoresSimple");
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnGetProvinciasAsync(int departamentoId)
        {
            try
            {
                var provincias = await _trabajadorService.ObtenerProvinciasPorDepartamentoAsync(departamentoId);
                return new JsonResult(provincias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener provincias");
                return new JsonResult(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetDistritosAsync(int provinciaId)
        {
            try
            {
                var distritos = await _trabajadorService.ObtenerDistritosPorProvinciaAsync(provinciaId);
                return new JsonResult(distritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener distritos");
                return new JsonResult(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostAgregarTrabajadorAsync([FromBody] Trabajador trabajador)
        {
            try
            {
                if (trabajador == null)
                {
                    _logger.LogWarning("Datos del trabajador nulos");
                    return new JsonResult(new { success = false, message = "No se recibieron datos" });
                }

                _logger.LogInformation("Recibidos datos: {Datos}", JsonSerializer.Serialize(trabajador));

                await _trabajadorService.GuardarTrabajadorAsync(trabajador);
                
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar trabajador");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnGetObtenerTrabajadorAsync(int id)
        {
            try
            {
                var trabajador = await _trabajadorService.ObtenerTrabajadorViewModelPorIdAsync(id);
                if (trabajador == null)
                {
                    return new JsonResult(new { success = false, message = "Trabajador no encontrado" });
                }
                return new JsonResult(new { success = true, data = trabajador });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el trabajador");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostActualizarTrabajadorAsync([FromBody] Trabajador trabajador)
        {
            try
            {
                if (trabajador == null)
                {
                    _logger.LogWarning("Datos del trabajador nulos");
                    return new JsonResult(new { success = false, message = "No se recibieron datos" });
                }

                _logger.LogInformation("Recibidos datos para actualizar: {Datos}", JsonSerializer.Serialize(trabajador));

                var resultado = await _trabajadorService.ActualizarTrabajadorAsync(trabajador);
                return new JsonResult(new { success = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar trabajador");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostEliminarTrabajadorAsync([FromBody] int id)
        {
            try
            {
                _logger.LogInformation("Eliminando trabajador con ID: {Id}", id);
                var resultado = await _trabajadorService.EliminarTrabajadorAsync(id);
                return new JsonResult(new { success = resultado });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar trabajador");
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}