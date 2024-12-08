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
            // Crear el filtro para obtener los tipos de respuesta por usuarioId
            var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.IdOrganizacion, usuarioId);

            // Ejecutar la consulta en MongoDB, similar al ejemplo de reportes
            var tiposRespuesta = await _TipoDeRespuesta
                .Find(filter)
                .ToListAsync(); // Ejecuta la consulta y convierte a lista

            return tiposRespuesta;
        }

        //Arreglar
        public async Task<List<TipoDeRespuesta>> ObtenerTiposRespuestaPorOrganizacion(string organizacionId)
        {
            // Convertir el string organizacionId a Guid
            if (Guid.TryParse(organizacionId, out Guid organizacionGuid))
            {
                // Filtrar por el Guid obtenido
                var tiposRespuesta = await _TipoDeRespuesta
                    .Find(tr => tr.IdOrganizacion == organizacionGuid)  // Ahora se compara como Guid
                    .ToListAsync();

                return tiposRespuesta;
            }
            else
            {
                // Manejo de error si el id no es un Guid válido
                throw new ArgumentException("El ID de organización no es un formato Guid válido.");
            }
        }



        public async Task<bool> GuardarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                // Insertar el tipo de respuesta en la colección
                await _TipoDeRespuesta.InsertOneAsync(tipoDeRespuesta);
                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores (si es necesario)
                Console.WriteLine($"Error al guardar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EditarTipoDeRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                // Crear el filtro para encontrar el TipoDeRespuesta por su ID
                var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, tipoDeRespuesta.Id);

                // Crear la actualización con los nuevos valores
                var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Placa, tipoDeRespuesta.Placa)
                    .Set(tr => tr.TipoDeRespuestaGeneral, tipoDeRespuesta.TipoDeRespuestaGeneral)
                    .Set(tr => tr.CantidadMaxDePersonas, tipoDeRespuesta.CantidadMaxDePersonas);

                // Aplicar la actualización
                var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);

                // Verificar si la operación fue exitosa
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al editar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarTipoRespuestaAsync(TipoDeRespuesta tipoDeRespuesta)
        {
            try
            {
                // Crear el filtro para encontrar el TipoDeRespuesta por su ID
                var filter = Builders<TipoDeRespuesta>.Filter.Eq(tr => tr.Id, tipoDeRespuesta.Id);

                // Crear la actualización con el nuevo estado
                var update = Builders<TipoDeRespuesta>.Update
                    .Set(tr => tr.Estado, tipoDeRespuesta.Estado);

                // Aplicar la actualización
                var resultado = await _TipoDeRespuesta.UpdateOneAsync(filter, update);

                // Verificar si la operación fue exitosa
                return resultado.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // Manejo de errores
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
                // Crear el filtro para encontrar el TipoDeRespuesta por su ID
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
                // Verificar si la operación fue exitosa

            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al editar TipoDeRespuesta: {ex.Message}");
                return false;
            }
        }


    }
}
