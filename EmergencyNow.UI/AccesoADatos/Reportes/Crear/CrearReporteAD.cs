using EmergencyNow.UI.Models.Interfaces.Reportes;
using EmergencyNow.UI;
using MongoDB.Driver;
using EmergencyNow.UI.Abstracciones.AccesoADatos.Reportes.Crear;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using MongoDB.Bson;

namespace EmergencyNow.UI.AccesoADatos.Reportes.Crear
{
    public class CrearReporteAD : ICrearReporte
    {
        private readonly IMongoCollection<ReporteDto> _reportes;
        private readonly IMongoCollection<Organizaciones> _Organizaciones;
        private readonly IMongoCollection<Respuestas> _Respuestas;
        private readonly IMongoCollection<TipoDeRespuesta> _TipoDeRespuesta;

        public CrearReporteAD(Contexto contexto)
        {
            _reportes = contexto.Reportes;
            _Organizaciones = contexto.Organizaciones;
            _Respuestas = contexto.Respuestas;
            _TipoDeRespuesta = contexto.TipoRespuesta;

        }

        public async Task<bool> AgregarReporteAsync(ReporteDto reporte)
        {
            try
            {
                await _reportes.InsertOneAsync(reporte);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ReporteDto>> ObtenerReportesPorUsuarioAsync(string idUsuario)
        {
            var filter = Builders<ReporteDto>.Filter.Eq(r => r.IdUsuario, idUsuario);
            return await _reportes
                .Find(filter)
                .SortByDescending(r => r.FechaReporte) 
                .ToListAsync();
        }

        public async Task<ReporteDto> ObtenerReportePorIdAsync(string id)
        {
            var filtro = Builders<ReporteDto>.Filter.Eq(r => r.Id, id);
            return await _reportes.Find(filtro).FirstOrDefaultAsync();
        }

        public List<ReporteDto> ObtenerReportesActivos()
        {
            var filtro = Builders<ReporteDto>.Filter.Eq(nameof(ReporteDto.Estado), "Activo");
            return _reportes.Find(filtro).ToList();
        }

        public async Task<bool> ActualizarEstadoAsync(string id, int tipo)
        {
            try
            {
                string nuevoEstado = tipo switch
                {
                    1 => "En proceso",  
                    2 => "Finalizado",   
                    _ => throw new ArgumentException("Tipo de estado no válido") 
                };

                var filtro = Builders<ReporteDto>.Filter.Eq(r => r.Id, id);

                var actualizacion = Builders<ReporteDto>.Update.Set(r => r.Estado, nuevoEstado);

                var resultado = await _reportes.UpdateOneAsync(filtro, actualizacion);

                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ModeloCompleto> ObtenerReporteCompletoAsync(string id)
        {
            var objectId = ObjectId.Parse(id); 

            var reporte = await _reportes.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (reporte == null)
            {
                return null;
            }

            var respuesta = await _Respuestas.Find(r => r.IdIncidente == reporte.Id).FirstOrDefaultAsync();
            if (respuesta == null)
            {
                return null;
            }

            var organizacion = await _Organizaciones.Find(o => o.UsuarioId == respuesta.IdUsuario).FirstOrDefaultAsync();
            if (organizacion == null)
            {
                return null;
            }

            var tipoDeRespuesta = await _TipoDeRespuesta.Find(t => t.Id == respuesta.IdTipoRespuesta).FirstOrDefaultAsync();
            if (tipoDeRespuesta == null)
            {
                return null;
            }

            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  
                Estado = reporte.Estado,      
                Placa = tipoDeRespuesta.Placa,
                TipoDeRespuestaGeneral = tipoDeRespuesta.TipoDeRespuestaGeneral,
                CantidadMaxDePersonas = tipoDeRespuesta.CantidadMaxDePersonas,
                Nombre = organizacion.Nombre,
                EstadoR = respuesta.Estado,
                HoraDeRespuesta = respuesta.HoraDeRespuesta
            };


            return modeloCompleto;
        }

        public async Task<List<ReporteDto>> ObtenerReportesPorUsuario(string idUsuario)
        {
            if (!Guid.TryParse(idUsuario, out var guidUsuario))
            {
                throw new ArgumentException("El ID de usuario proporcionado no es válido.");
            }
            var respuestasUsuario = await _Respuestas
                .Find(respuesta => respuesta.IdUsuario == guidUsuario)
                .ToListAsync();

            if (!respuestasUsuario.Any())
            {
                return new List<ReporteDto>(); 
            }

            var listaReportes = new List<ReporteDto>();

            foreach (var respuesta in respuestasUsuario)
            {
                var reporte = await _reportes
                    .Find(r => r.Id == respuesta.IdIncidente)
                    .FirstOrDefaultAsync();

                if (reporte != null)
                {
                    var tipoDeRespuesta = await _TipoDeRespuesta
                        .Find(tipo => tipo.Id == respuesta.IdTipoRespuesta)
                        .FirstOrDefaultAsync();

                    var reporteDto = new ReporteDto
                    {
                        Id = reporte.Id,
                        TipoIncidente = reporte.TipoIncidente,
                        Descripcion = reporte.Descripcion,
                        Gravedad = reporte.Gravedad,
                        Ubicacion = reporte.Ubicacion,
                        UbicacionString = tipoDeRespuesta?.Placa ?? "Sin placa", 
                        Estado = reporte.Estado,
                        FechaReporte = reporte.FechaReporte,
                        IdUsuario = reporte.IdUsuario
                    };
                    listaReportes.Add(reporteDto);
                }
            }

            return listaReportes;
        }

        public async Task<ModeloCompleto> ReporteActivoAsync(Guid idUsuario)
        {
            var tipoDeRespuesta = await _TipoDeRespuesta
                .Find(t => t.IdUsuario == idUsuario)
                .FirstOrDefaultAsync();

            if (tipoDeRespuesta == null)
            {
                return null;
            }

            var respuesta = await _Respuestas
                .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id && r.Estado != "Finalizado")
                .SortByDescending(r => r.HoraDeRespuesta) 
                .FirstOrDefaultAsync();

            if (respuesta == null)
            {
                return null;
            }

            var reporte = await _reportes.Find(r => r.Id == respuesta.IdIncidente).FirstOrDefaultAsync();
            if (reporte == null)
            {
                return null;
            }

            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  
                Estado = reporte.Estado,        
                Placa = tipoDeRespuesta.Placa,  
                TipoDeRespuestaGeneral = "null", 
                CantidadMaxDePersonas = 0, 
                Nombre = "no aplica",   
                EstadoR = respuesta.Estado,     
                HoraDeRespuesta = respuesta.HoraDeRespuesta
            };

            return modeloCompleto;
        }

        public async Task<ModeloCompleto> ObtenerReporteCompletoPorTipoRespuestaAsync(string tipoRespuestaId)
        {
            var objectId = ObjectId.Parse(tipoRespuestaId); 

            var tipoDeRespuesta = await _TipoDeRespuesta.Find(t => t.Id == tipoRespuestaId).FirstOrDefaultAsync();
            if (tipoDeRespuesta == null)
            {
                
                return null;
            }

            
            var respuesta = await _Respuestas
                .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id && r.Estado != "Finalizado")
                .SortByDescending(r => r.HoraDeRespuesta) 
                .FirstOrDefaultAsync();
            if (respuesta == null)
            {
                return null;
            }

           
            var reporte = await _reportes.Find(r => r.Id == respuesta.IdIncidente).FirstOrDefaultAsync();
            if (reporte == null)
            {
                return null;
            }

            var organizacion = await _Organizaciones.Find(o => o.UsuarioId == respuesta.IdUsuario).FirstOrDefaultAsync();
            if (organizacion == null)
            {
                return null;
            }

            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  
                Estado = reporte.Estado,     
                Placa = tipoDeRespuesta.Placa,
                TipoDeRespuestaGeneral = tipoDeRespuesta.TipoDeRespuestaGeneral,
                CantidadMaxDePersonas = tipoDeRespuesta.CantidadMaxDePersonas,
                Nombre = organizacion.Nombre,
                EstadoR = respuesta.Estado,
                HoraDeRespuesta = respuesta.HoraDeRespuesta
            };

            return modeloCompleto;
        }




    }
}
