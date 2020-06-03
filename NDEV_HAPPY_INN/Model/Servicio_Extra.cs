using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Servicio_Extra
    {
        [PartitionKey]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public decimal precio { get; set; }
    }

    public class Servicio_ExtraMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nombre = ?";

        public static async Task Create(Servicio_Extra nodo)
        {
            try
            {
                if (await Existe(nodo.nombre))
                {
                    throw new Exception("El nombre del servicio extra ya existe");
                }

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Servicio_Extra nodo)
        {
            try
            {
                await GetMapper().UpdateAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Delete(Servicio_Extra nodo)
        {
            try
            {
                var reservacionesServiciosExtras =
                    (await ReservacionMap.ReadAllAsync()).Select(H => H.servicios_extras).ToList();

                var reservacionesServiciosExtrasByServicioExtra =
                    reservacionesServiciosExtras.Where(L => L.Exists(LS => LS.Equals(nodo.nombre))).ToList();

                if (reservacionesServiciosExtrasByServicioExtra.Count > 0)
                {
                    throw new Exception("Este servicio extra está siendo utilizado en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Servicio_Extra Read(string nombre)
        {
            try
            {
                return GetMapper().FirstOrDefault<Servicio_Extra>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Servicio_Extra> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Servicio_Extra>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Servicio_Extra>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Servicio_Extra>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<bool> Existe(string nombre)
        {
            try
            {
                return (await ReadAsync(nombre)) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}