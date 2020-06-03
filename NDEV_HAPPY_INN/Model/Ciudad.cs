using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NDEV_HAPPY_INN.Model
{
    public class Ciudad
    {
        [PartitionKey]
        public string pais { get; set; }

        [ClusteringKey]
        public string estado { get; set; }

        [ClusteringKey]
        public string nombre { get; set; }
    }

    public class CiudadMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE pais = ? AND estado = ? AND nombre = ?";

        public static async Task Create(Ciudad nodo)
        {
            try
            {
                UDT_Ciudad ciudad = new UDT_Ciudad()
                {
                    nombre = nodo.nombre,
                    estado = nodo.estado,
                    pais = nodo.pais
                };

                if (await Existe(ciudad))
                {
                    throw new Exception("Esta ciudad ya existe");
                }

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Ciudad nodo, List<Usuario_By_Ciudad> usuarioAsignados = null)
        {
            try
            {
                if (usuarioAsignados == null)
                    return;

                List<Usuario_By_Ciudad> usuariosAsignadosAnteriores = (await Usuario_By_CiudadMap.ReadAllAsync()).ToList();
                usuariosAsignadosAnteriores.Where(U => U.ciudad == nodo.nombre);

                foreach (Usuario_By_Ciudad usuarioAsignadoViejo in usuariosAsignadosAnteriores)
                {
                    await Usuario_By_CiudadMap.Delete(usuarioAsignadoViejo);
                }

                foreach (Usuario_By_Ciudad usuarioAsignado in usuarioAsignados)
                {
                    await Usuario_By_CiudadMap.Create(usuarioAsignado);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Delete(Ciudad nodo, List<Usuario_By_Ciudad> usuarioAsignados = null)
        {
            try
            {
                List<Hotel> hotelesByCiudad = (await HotelMap.ReadAllByCiudadAsync(nodo.nombre)).ToList();

                if (hotelesByCiudad.Count > 0)
                {
                    throw new Exception("Está ciudad está siendo utilizada en otros registros, no se puede eliminar");
                }

                // se quita la validacion para verificar si habia usuarios asignados a la ciudad, pero
                // considero que es posible retirar el permiso de realizar reservaciones de un usuario

                //IEnumerable<Usuario_By_Ciudad> usuariosByCiudad = 
                //    (await Usuario_By_CiudadMap.ReadAllAsync()).Where(UC => UC.ciudad.Equals(nodo.nombre));

                //if (usuariosByCiudad.Count() > 0)
                //{
                //    throw new Exception("Está ciudad está siendo utilizada en otros registros, no se puede eliminar");
                //}

                await GetMapper().DeleteAsync(nodo);

                if (usuarioAsignados == null)
                    return;

                foreach (Usuario_By_Ciudad usuarioAsignado in usuarioAsignados)
                {
                    await Usuario_By_CiudadMap.Delete(usuarioAsignado);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Ciudad Read(UDT_Ciudad ciudad)
        {
            try
            {
                return GetMapper().FirstOrDefault<Ciudad>(CQL_QUERY_WHERE_NAME, ciudad.pais, ciudad.estado, ciudad.nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Ciudad> ReadAsync(UDT_Ciudad ciudad)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Ciudad>(CQL_QUERY_WHERE_NAME, ciudad.pais, ciudad.estado, ciudad.nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Ciudad> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Ciudad>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Ciudad>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Ciudad>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<bool> Existe(UDT_Ciudad ciudad)
        {
            try
            {
                return (await ReadAsync(ciudad)) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}