using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Usuario_By_Ciudad
    {
        [PartitionKey]
        public string nick { get; set; }

        public string ciudad { get; set; }
    }

    public class Usuario_By_CiudadMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nick = ?";

        public static async Task Create(Usuario_By_Ciudad nodo)
        {
            try
            {
                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Usuario_By_Ciudad nodo)
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

        public static async Task Delete(Usuario_By_Ciudad nodo)
        {
            try
            {
                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Usuario_By_Ciudad Read(string nombre)
        {
            try
            {
                return GetMapper().FirstOrDefault<Usuario_By_Ciudad>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Usuario_By_Ciudad> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Usuario_By_Ciudad>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Usuario_By_Ciudad>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Usuario_By_Ciudad>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Usuario_By_Ciudad>> ReadAllByUsuarioAsync(string nick)
        {
            try
            {
                return await GetMapper().FetchAsync<Usuario_By_Ciudad>(CQL_QUERY_WHERE_NAME, nick);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Usuario_By_Ciudad> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Usuario_By_Ciudad>();
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