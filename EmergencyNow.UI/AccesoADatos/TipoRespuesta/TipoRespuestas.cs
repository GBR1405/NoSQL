using EmergencyNow.UI.Abstracciones.AccesoADatos.TipoRespuesta;
using EmergencyNow.UI.Models.Interfaces.Modelos;
using EmergencyNow.UI.Models.Interfaces.Reportes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EmergencyNow.UI.AccesoADatos.TipoRespuesta
{
    public class TipoRespuestas : ITipoRespuestaAD
    {

        private readonly IMongoCollection<TipoDeRespuesta> _TipoDeRespuesta;

        public TipoRespuestas(Contexto contexto)
        {
            _TipoDeRespuesta = contexto.TipoRespuesta;
        }

        public async Task<List<TipoDeRespuesta>> ObtenerTiposRespuestaPorUsuario(Guid usuarioId)
        {
            var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.IdOrganizacion, usuarioId);

            var tiposRespuesta = await _TipoDeRespuesta
                .Find(filter)
                .ToListAsync(); 

            return tiposRespuesta;
        }

        //Arreglar
        public async Task<List<TipoDeRespuesta>> ObtenerTiposRespuestaPorOrganizacion(string organizacionId)
        {
            if (Guid.TryParse(organizacionId, out Guid organizacionGuid))
            {
                var tiposRespuesta = await _TipoDeRespuesta
                    .Find(tr => tr.IdOrganizacion == organizacionGuid)  
                    .ToListAsync();

                return tiposRespuesta;
            }
            else
            {
                throw new ArgumentException("El ID de organización no es un formato Guid válido.");
            }
        }



        public async Task<bool> GuardarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                await _TipoDeRespuesta.InsertOneAsync(tipoDeRespuesta);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EditarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, tipoDeRespuesta.Id);

                var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Placa, tipoDeRespuesta.Placa)
                    .Set(tr => tr.TipoDeRespuestaGeneral, tipoDeRespuesta.TipoDeRespuestaGeneral)
                    .Set(tr => tr.CantidadMaxDePersonas, tipoDeRespuesta.CantidadMaxDePersonas);

                var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);

                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarTipoRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, tipoDeRespuesta.Id);

                var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Estado, tipoDeRespuesta.Estado);

                var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);

                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }


        public async Task<TipoDeRespuesta> ObtenerTipoRespuestaPorIdAsync(string id)
        {
            var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, id);
            return await _TipoDeRespuesta.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> CambiarOcupacion(string id, int tipo)
        {
            try
            {
                var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, id);

                if (tipo == 1)
                {
                    var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Estado, "Ocupado");

                    var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);
                    return resultado.ModifiedCount > 0;

                } else if (tipo == 2)
                {
                    var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Estado, "Activo");

                    var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);
                    return resultado.ModifiedCount > 0;
                }

                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }


    }
}
