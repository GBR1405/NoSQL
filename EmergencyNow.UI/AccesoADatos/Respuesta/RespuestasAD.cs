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

                var respuestaEnProceso = await _respuestas.Find(filtro).FirstOrDefaultAsync();

                return respuestaEnProceso;
            }
            catch
            {
                return null; 
            }
        }

        public async Task<Respuestas> ObtenerRespuestaActivaPorUsuario(Guid idUsuario)
        {
            var respuesta = await _respuestas
                .Find(r => r.IdUsuario == idUsuario && r.Estado == "Activo")
                .FirstOrDefaultAsync();

            if (respuesta == null)
            {
                return null;
            }

            var respuestaDto = new Respuestas
            {
                Id = respuesta.Id,
                IdUsuario = respuesta.IdUsuario,
                IdIncidente = respuesta.IdIncidente,
                IdTipoRespuesta = respuesta.IdTipoRespuesta,
                Estado = respuesta.Estado,
                HoraDeRespuesta = respuesta.HoraDeRespuesta
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
                return false;
            }
        }

        public async Task<List<ReporteDto>> ObtenerReportesPorUsuarioTipoRespuestaAsync(Guid idUsuarioTipoRespuesta)
        {
            try
            {
                var tipoDeRespuesta = await _TipoDeRespuesta
                    .Find(t => t.IdUsuario == idUsuarioTipoRespuesta)
                    .FirstOrDefaultAsync();

                if (tipoDeRespuesta == null)
                {
                    return new List<ReporteDto>(); 
                }

                var respuestas = await _respuestas
                    .Find(r => r.IdTipoRespuesta == tipoDeRespuesta.Id)
                    .ToListAsync();

                if (respuestas.Count == 0)
                {
                    return new List<ReporteDto>(); 
                }

                var reporteIds = respuestas.Select(r => r.IdIncidente).Distinct().ToList();

                var reportes = await _reportes
                    .Find(r => reporteIds.Contains(r.Id))
                    .ToListAsync();

                return reportes;
            }
            catch
            {
                return new List<ReporteDto>();
            }
        }


    }
}
