using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Cliente
    {
        [PartitionKey]
        public Guid id { get; set; }

        public string nombre { get; set; }

        public string ap_paterno { get; set; }

        public string ap_materno { get; set; }

        public DateTime fecha_nacimiento { get; set; }

        public string domicilio { get; set; }

        public string tel_casa { get; set; }

        public string tel_celular { get; set; }

        public string referencia { get; set; }
    }

    public class ClienteMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE id = ?";

        public static async Task Create(Cliente nodo)
        {
            try
            {
                nodo.id = Guid.NewGuid();

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Cliente nodo)
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

        public static async Task Delete(Cliente nodo)
        {
            try
            {
                var reservacionesCliente =
                    (await ReservacionMap.ReadAllAsync()).Where(R => R.id_cliente.Equals(nodo.id)).ToList();

                if (reservacionesCliente.Count > 0)
                {
                    throw new Exception("Este cliente está siendo utilizado en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Cliente Read(Guid id)
        {
            try
            {
                return GetMapper().FirstOrDefault<Cliente>(CQL_QUERY_WHERE_NAME, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Cliente> ReadAsync(Guid id)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Cliente>(CQL_QUERY_WHERE_NAME, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Cliente>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Cliente>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Cliente> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Cliente>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<bool> Existe(Guid id)
        {
            try
            {
                return (await ReadAsync(id)) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string NombreCompleto(Cliente cliente)
        {
            return $"{cliente.nombre} {cliente.ap_paterno} {cliente.ap_materno}".TrimEnd();
        }
    }
}