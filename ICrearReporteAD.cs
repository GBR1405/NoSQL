using System;

public class ICrearReporteAD
{
	public interface ICrearReporteAD()
	{

        CrearReporteAD(IMongoDatabase database);

        Task<bool> AgregarReporteAsync(Reporte reporte);


    }
}
