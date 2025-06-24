using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Prueba.Data;
using Prueba.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prueba.Services
{
    public class TrabajadorService
    {
        private readonly ApplicationDbContext _context;

        public TrabajadorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TrabajadorViewModel>> ObtenerTrabajadoresAsync()
        {
            try
            {
                // Log para depuración
                Console.WriteLine("Iniciando obtención de trabajadores");

                // Consulta directa con LINQ para evitar problemas con el SP
                var trabajadores = await (from t in _context.Trabajadores
                                          join d in _context.Departamento on t.IdDepartamento equals d.Id into deptJoin
                                          from dept in deptJoin.DefaultIfEmpty()
                                          join p in _context.Provincia on t.IdProvincia equals p.Id into provJoin
                                          from prov in provJoin.DefaultIfEmpty()
                                          join dist in _context.Distrito on t.IdDistrito equals dist.Id into distJoin
                                          from distrito in distJoin.DefaultIfEmpty()
                                          select new TrabajadorViewModel
                                          {
                                              Id = t.Id,
                                              TipoDocumento = t.TipoDocumento,
                                              NumeroDocumento = t.NumeroDocumento,
                                              Nombres = t.Nombres,
                                              Sexo = t.Sexo,
                                              IdDepartamento = t.IdDepartamento,
                                              NombreDepartamento = dept != null ? dept.NombreDepartamento : null,
                                              IdProvincia = t.IdProvincia,
                                              NombreProvincia = prov != null ? prov.NombreProvincia : null,
                                              IdDistrito = t.IdDistrito,
                                              NombreDistrito = distrito != null ? distrito.NombreDistrito : null
                                          }).ToListAsync();

                // Log para depuración
                Console.WriteLine($"Se obtuvieron {trabajadores.Count} trabajadores");
                foreach (var t in trabajadores)
                {
                    Console.WriteLine($"ID: {t.Id}, Nombre: {t.Nombres}, Depto: {t.NombreDepartamento}, Prov: {t.NombreProvincia}, Dist: {t.NombreDistrito}");
                }

                return trabajadores;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener trabajadores: {ex.Message}");
                return new List<TrabajadorViewModel>();
            }
        }

        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            return await _context.Departamento
                .OrderBy(d => d.NombreDepartamento)
                .Distinct() // Asegurarse de que no hay duplicados
                .ToListAsync();
        }

        public async Task<List<Provincia>> ObtenerProvinciasPorDepartamentoAsync(int departamentoId)
        {
            return await _context.Provincia
                .Where(p => p.IdDepartamento == departamentoId)
                .OrderBy(p => p.NombreProvincia)
                .ToListAsync();
        }

        public async Task<List<Distrito>> ObtenerDistritosPorProvinciaAsync(int provinciaId)
        {
            return await _context.Distrito
                .Where(d => d.IdProvincia == provinciaId)
                .OrderBy(d => d.NombreDistrito)
                .ToListAsync();
        }

        public async Task<bool> GuardarTrabajadorAsync(Trabajador trabajador)
        {
            if (trabajador == null)
                throw new ArgumentNullException(nameof(trabajador));

            try
            {
                // Verificamos si los IDs de ubicación seleccionados existen
                if (trabajador.IdDepartamento.HasValue)
                {
                    // Simplemente verificamos que exista
                    var departamento = await _context.Departamento.FindAsync(trabajador.IdDepartamento.Value);
                    if (departamento == null)
                    {
                        throw new Exception($"El departamento con ID {trabajador.IdDepartamento} no existe");
                    }
                }

                if (trabajador.IdProvincia.HasValue)
                {
                    var provincia = await _context.Provincia.FindAsync(trabajador.IdProvincia.Value);
                    if (provincia == null)
                    {
                        throw new Exception($"La provincia con ID {trabajador.IdProvincia} no existe");
                    }
                }

                if (trabajador.IdDistrito.HasValue)
                {
                    var distrito = await _context.Distrito.FindAsync(trabajador.IdDistrito.Value);
                    if (distrito == null)
                    {
                        throw new Exception($"El distrito con ID {trabajador.IdDistrito} no existe");
                    }
                }

                _context.Trabajadores.Add(trabajador);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Aquí podrías registrar el error en un log
                Console.WriteLine($"Error al guardar trabajador: {ex.Message}");
                throw;
            }
        }

        public async Task<Trabajador> ObtenerTrabajadorPorIdAsync(int id)
        {
            return await _context.Trabajadores.FindAsync(id);
        }

        public async Task<bool> ActualizarTrabajadorAsync(Trabajador trabajador)
        {
            try
            {
                _context.Trabajadores.Update(trabajador);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar trabajador: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarTrabajadorAsync(int id)
        {
            var trabajador = await _context.Trabajadores.FindAsync(id);
            if (trabajador == null)
                return false;

            try
            {
                _context.Trabajadores.Remove(trabajador);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar trabajador: {ex.Message}");
                throw;
            }
        }
    
        public async Task<TrabajadorViewModel> ObtenerTrabajadorViewModelPorIdAsync(int id)
        {
                // Si falla, construimos el modelo manualmente
                var trabajador = await _context.Trabajadores.FindAsync(id);
                if (trabajador == null) return null;

                // Obtener datos relacionados
                var departamento = trabajador.IdDepartamento.HasValue
                    ? await _context.Departamento.FindAsync(trabajador.IdDepartamento.Value)
                    : null;

                var provincia = trabajador.IdProvincia.HasValue
                    ? await _context.Provincia.FindAsync(trabajador.IdProvincia.Value)
                    : null;

                var distrito = trabajador.IdDistrito.HasValue
                    ? await _context.Distrito.FindAsync(trabajador.IdDistrito.Value)
                    : null;

                return new TrabajadorViewModel
                {
                    Id = trabajador.Id,
                    TipoDocumento = trabajador.TipoDocumento,
                    NumeroDocumento = trabajador.NumeroDocumento,
                    Nombres = trabajador.Nombres,
                    Sexo = trabajador.Sexo,
                    IdDepartamento = trabajador.IdDepartamento,
                    NombreDepartamento = departamento?.NombreDepartamento,
                    IdProvincia = trabajador.IdProvincia,
                    NombreProvincia = provincia?.NombreProvincia,
                    IdDistrito = trabajador.IdDistrito,
                    NombreDistrito = distrito?.NombreDistrito
                };
            }
        }

    }