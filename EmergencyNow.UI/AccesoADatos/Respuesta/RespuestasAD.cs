using EmergencyNow.UI.Abstracciones.AccesoADatos.Respuesta;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Driver;

namespace EmergencyNow.UI.AccesoADatos.Respuesta
{
    public class RespuestasAD : IRespuestasAD
    {

        private readonly IMongoCollection<Respuestas> _respuestas;
        private readonly IMongoCollection<TipoDeRespuesta> _TipoDeRespuesta;
        private readonly IMongoCollection<ReporteDto> _reportes;

        public RespuestasAD(Contexto contexto)
        {
            _respuestas = contexto.Respuestas;
            _TipoDeRespuesta = contexto.TipoRespuesta;
            _reportes = contexto.Reportes;
        }

        public async Task<bool> AgregarReporteAsync(Respuestas Respuesta)
        {
            try
            {
                await _respuestas.InsertOneAsync(Respuesta);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Respuestas> ObtenerRespuestasEnProcesoPorUsuarioAsync(Guid usuarioId)
        {
            try
            {
                var filtro = Builders<Respuestas>.Filter.Eq(r => r.IdUsuario, usuarioId) &
                             Builders<Respuestas>.Filter.Eq(r => r.Estado, "En proceso");

                // Obtiene solo la primera respuesta que coincida con el filtro
                var respuestaEnProceso = await _respuestas.Find(filtro).FirstOrDefaultAsync();

                return respuestaEnProceso;
            }
            catch
            {
                return null; // Devuelve null si hay algún error
            }
        }

        public async Task<Respuestas> ObtenerRespuestaActivaPorUsuario(Guid idUsuario)
        {
            // Buscar una respuesta que esté vinculada al usuario y cuyo estado sea "Activo"
            var respuesta = await _respuestas
                .Find(r => r.IdUsuario == idUsuario && r.Estado == "Activo")
                .FirstOrDefaultAsync();

            // Si no se encuentra ninguna respuesta activa, retornar null
            if (respuesta == null)
            {
                return null;
            }

            // Mapear la respuesta al DTO para devolverla al controlador
            var respuestaDto = new Respuestas
            {
                Id = respuesta.Id,
                IdUsuario = respuesta.IdUsuario,
                IdIncidente = respuesta.IdIncidente,
                IdTipoRespuesta = respuesta.IdTipoRespuesta,
                Estado = respuesta.Estado,
                HoraDeRespuesta = respuesta.HoraDeRespuesta
                // Puedes añadir más propiedades aquí según el modelo de RespuestaDto
            };

            return respuestaDto;
        }

        public async Task<bool> ActualizarEstadoRespuestaAsync(Guid idUsuario, string nuevoEstado)
        {
            try
            {

                var tipoDeRespuesta = await _TipoDeRespuesta
                .Find(t => t.IdUsuario == idUsuario)
                .FirstOrDefaultAsync();

                if (tipoDeRespuesta == null)
                {
                    return false;
                }

                var respuesta = await _respuestas
                    .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id && r.Estado != "Finalizado")
                    .SortByDescending(r => r.HoraDeRespuesta) 
                    .FirstOrDefaultAsync();

                var filtro = Builders<Respuestas>.Filter.Eq(r => r.Id, respuesta.Id);
                var actualizacion = Builders<Respuestas>.Update.Set(r => r.Estado, nuevoEstado);
                var resultado = await _respuestas.UpdateOneAsync(filtro, actualizacion);

                if(nuevoEstado.ToLower() == "finalizado")
                {
                    var reporte = await _reportes.Find(r => r.Id == respuesta.IdIncidente).FirstOrDefaultAsync();
                    var filtroR = Builders<ReporteDto>.Filter.Eq(r => r.Id, reporte.Id);
                    var actualizacionReporte = Builders<ReporteDto>.Update.Set(r => r.Estado, nuevoEstado);
                    var ResultadoR = await _reportes.UpdateOneAsync(filtroR, actualizacionReporte);

                    var filtroTR = Builders<TipoDeRespuesta>.Filter.Eq(r => r.Id, tipoDeRespuesta.Id);
                    var actualizacionTipoDeRespuesta = Builders<TipoDeRespuesta>.Update.Set(r => r.Estado, "Activo");
                    var ResultadoTR = await _TipoDeRespuesta.UpdateOneAsync(filtroTR, actualizacionTipoDeRespuesta);
                }

                return resultado.ModifiedCount > 0;
            }
            catch
            {
                // Si ocurre un error, devuelve false
                return false;
            }
        }

        public async Task<List<ReporteDto>> ObtenerReportesPorUsuarioTipoRespuestaAsync(Guid idUsuarioTipoRespuesta)
        {
            try
            {
                // Paso 1: Obtener el TipoDeRespuesta asociado al usuario
                var tipoDeRespuesta = await _TipoDeRespuesta
                    .Find(t => t.IdUsuario == idUsuarioTipoRespuesta)
                    .FirstOrDefaultAsync();

                if (tipoDeRespuesta == null)
                {
                    return new List<ReporteDto>(); // Si no existe, devolver lista vacía
                }

                // Paso 2: Obtener las respuestas asociadas al TipoDeRespuesta
                var respuestas = await _respuestas
                    .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id)
                    .ToListAsync();

                if (respuestas.Count == 0)
                {
                    return new List<ReporteDto>(); // Si no hay respuestas, devolver lista vacía
                }

                // Paso 3: Obtener los IDs de los reportes asociados a las respuestas
                var reporteIds = respuestas.Select(r => r.IdIncidente).Distinct().ToList();

                // Paso 4: Obtener los reportes utilizando los IDs recopilados
                var reportes = await _reportes
                    .Find(r => reporteIds.Contains(r.Id))
                    .ToListAsync();

                return reportes;
            }
            catch
            {
                // Si ocurre algún error, devolver lista vacía
                return new List<ReporteDto>();
            }
        }


    }
}
