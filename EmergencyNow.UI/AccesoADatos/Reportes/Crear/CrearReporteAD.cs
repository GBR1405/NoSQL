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
            // Obtener todos los reportes hechos por un usuario específico
            var filter = Builders<ReporteDto>.Filter.Eq(r => r.IdUsuario, idUsuario);
            return await _reportes
                .Find(filter)
                .SortByDescending(r => r.FechaReporte) // Ordenar por fecha
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
                // Determinar el nuevo estado basado en el tipo
                string nuevoEstado = tipo switch
                {
                    1 => "En proceso",  // Si tipo es 1, el estado es "En proceso"
                    2 => "Finalizado",   // Si tipo es 2, el estado es "Finalizado"
                    _ => throw new ArgumentException("Tipo de estado no válido") // Lanzar excepción si el tipo no es válido
                };

                // Crear el filtro para buscar el reporte por ID
                var filtro = Builders<ReporteDto>.Filter.Eq(r => r.Id, id);

                // Crear la actualización del estado
                var actualizacion = Builders<ReporteDto>.Update.Set(r => r.Estado, nuevoEstado);

                // Ejecutar la actualización
                var resultado = await _reportes.UpdateOneAsync(filtro, actualizacion);

                // Verificar si se actualizó al menos un documento
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Puedes registrar la excepción si lo deseas o manejarla de alguna manera
                return false;
            }
        }

        public async Task<ModeloCompleto> ObtenerReporteCompletoAsync(string id)
        {
            var objectId = ObjectId.Parse(id); // Convierte el ID del reporte

            // Paso 1: Obtener el reporte
            var reporte = await _reportes.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (reporte == null)
            {
                // Si no se encuentra el reporte, devolver null o lanzar una excepción
                return null;
            }

            // Paso 2: Obtener la respuesta asociada al reporte
            var respuesta = await _Respuestas.Find(r => r.IdIncidente == reporte.Id).FirstOrDefaultAsync();
            if (respuesta == null)
            {
                // Si no se encuentra la respuesta, devolver null o manejarlo según sea necesario
                return null;
            }

            // Paso 3: Obtener la organización relacionada con la respuesta
            // Aquí, asumimos que 'IdUsuario' es un Guid y lo buscamos en la colección 'Organizaciones'
            var organizacion = await _Organizaciones.Find(o => o.UsuarioId == respuesta.IdUsuario).FirstOrDefaultAsync();
            if (organizacion == null)
            {
                // Si no se encuentra la organización, devolver null o manejarlo
                return null;
            }

            // Paso 4: Obtener el tipo de respuesta asociado a la respuesta
            var tipoDeRespuesta = await _TipoDeRespuesta.Find(t => t.Id == respuesta.IdTipoRespuesta).FirstOrDefaultAsync();
            if (tipoDeRespuesta == null)
            {
                // Si no se encuentra el tipo de respuesta, devolver null o manejarlo
                return null;
            }

            // Paso 5: Crear el objeto ModeloCompleto
            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  // Accede a las coordenadas si existen
                Estado = reporte.Estado,      // Accede al estado actual si existe
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

            // 1. Obtener todas las respuestas relacionadas con el usuario
            var respuestasUsuario = await _Respuestas
                .Find(respuesta => respuesta.IdUsuario == guidUsuario)
                .ToListAsync();

            if (!respuestasUsuario.Any())
            {
                return new List<ReporteDto>(); // Retorna lista vacía si no hay respuestas asociadas
            }

            // Lista para almacenar los reportes resultantes
            var listaReportes = new List<ReporteDto>();

            // 2. Procesar cada respuesta encontrada
            foreach (var respuesta in respuestasUsuario)
            {
                // Obtener el reporte relacionado
                var reporte = await _reportes
                    .Find(r => r.Id == respuesta.IdIncidente)
                    .FirstOrDefaultAsync();

                if (reporte != null)
                {
                    // Obtener información del tipo de respuesta
                    var tipoDeRespuesta = await _TipoDeRespuesta
                        .Find(tipo => tipo.Id == respuesta.IdTipoRespuesta)
                        .FirstOrDefaultAsync();

                    // Crear el objeto ReporteDto con los datos recopilados
                    var reporteDto = new ReporteDto
                    {
                        Id = reporte.Id,
                        TipoIncidente = reporte.TipoIncidente,
                        Descripcion = reporte.Descripcion,
                        Gravedad = reporte.Gravedad,
                        Ubicacion = reporte.Ubicacion,
                        UbicacionString = tipoDeRespuesta?.Placa ?? "Sin placa", // Placa del tipo de respuesta
                        Estado = reporte.Estado,
                        FechaReporte = reporte.FechaReporte,
                        IdUsuario = reporte.IdUsuario
                    };

                    // Añadir el reporte a la lista
                    listaReportes.Add(reporteDto);
                }
            }

            // Devolver la lista completa de ReporteDto
            return listaReportes;
        }

        public async Task<ModeloCompleto> ReporteActivoAsync(Guid idUsuario)
        {
            // Paso 1: Buscar el TipoDeRespuesta asociado al usuario
            var tipoDeRespuesta = await _TipoDeRespuesta
                .Find(t => t.IdUsuario == idUsuario)
                .FirstOrDefaultAsync();

            if (tipoDeRespuesta == null)
            {
                // Si no hay un TipoDeRespuesta asociado, devolvemos null
                return null;
            }

            // Paso 2: Buscar la primera respuesta activa asociada al TipoDeRespuesta
            var respuesta = await _Respuestas
                .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id && r.Estado != "Finalizado")
                .SortByDescending(r => r.HoraDeRespuesta) // Ordenamos por fecha para obtener la última
                .FirstOrDefaultAsync();

            if (respuesta == null)
            {
                // Si no se encuentra una respuesta activa, devolvemos null
                return null;
            }

            // Paso 3: Obtener el reporte asociado a la respuesta
            var reporte = await _reportes.Find(r => r.Id == respuesta.IdIncidente).FirstOrDefaultAsync();
            if (reporte == null)
            {
                // Si no se encuentra el reporte, devolvemos null
                return null;
            }

            // Paso 5: Crear el objeto ModeloCompleto
            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  // Coordenadas geográficas
                Estado = reporte.Estado,        // Estado actual del reporte
                Placa = tipoDeRespuesta.Placa,  // Placa del vehículo en el TipoDeRespuesta
                TipoDeRespuestaGeneral = "null", // Tipo de respuesta (ej: médica, policial, etc.)
                CantidadMaxDePersonas = 0, // Máxima capacidad del equipo
                Nombre = "no aplica",   // Nombre de la organización
                EstadoR = respuesta.Estado,     // Estado de la respuesta
                HoraDeRespuesta = respuesta.HoraDeRespuesta
            };

            return modeloCompleto;
        }

        public async Task<ModeloCompleto> ObtenerReporteCompletoPorTipoRespuestaAsync(string tipoRespuestaId)
        {
            var objectId = ObjectId.Parse(tipoRespuestaId); // Convierte el ID del TipoRespuesta

            // Paso 1: Obtener el tipo de respuesta
            var tipoDeRespuesta = await _TipoDeRespuesta.Find(t => t.Id == tipoRespuestaId).FirstOrDefaultAsync();
            if (tipoDeRespuesta == null)
            {
                // Si no se encuentra el tipo de respuesta, devolver null o lanzar una excepción
                return null;
            }

            // Paso 2: Obtener la respuesta asociada al tipo de respuesta
            var respuesta = await _Respuestas
                .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id && r.Estado != "Finalizado")
                .SortByDescending(r => r.HoraDeRespuesta) // Ordenamos por fecha para obtener la última
                .FirstOrDefaultAsync();
            if (respuesta == null)
            {
                // Si no se encuentra la respuesta, devolver null o manejarlo según sea necesario
                return null;
            }

            // Paso 3: Obtener el reporte asociado a la respuesta
            var reporte = await _reportes.Find(r => r.Id == respuesta.IdIncidente).FirstOrDefaultAsync();
            if (reporte == null)
            {
                // Si no se encuentra el reporte, devolver null o manejarlo
                return null;
            }

            // Paso 4: Obtener la organización relacionada con la respuesta
            var organizacion = await _Organizaciones.Find(o => o.UsuarioId == respuesta.IdUsuario).FirstOrDefaultAsync();
            if (organizacion == null)
            {
                // Si no se encuentra la organización, devolver null o manejarlo
                return null;
            }

            // Paso 5: Crear el objeto ModeloCompleto
            var modeloCompleto = new ModeloCompleto
            {
                Id = reporte.Id,
                TipoIncidente = reporte.TipoIncidente,
                Descripcion = reporte.Descripcion,
                Gravedad = reporte.Gravedad,
                Ubicacion = reporte.Ubicacion,  // Accede a las coordenadas si existen
                Estado = reporte.Estado,      // Accede al estado actual si existe
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
