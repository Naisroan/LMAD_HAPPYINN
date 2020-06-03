using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Caracteristica
    {
        [PartitionKey]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public string tipo { get; set; }
    }

    public class CaracteristicaMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nombre = ?";

        public static async Task Create(Caracteristica nodo)
        {
            try
            {
                if (await Existe(nodo.nombre))
                {
                    throw new Exception("El nombre de la caracteristica ya existe");
                }

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Caracteristica nodo)
        {
            try
            {
                Caracteristica caracteristicaAnterior = await ReadAsync(nodo.nombre);

                if (!caracteristicaAnterior.tipo.Equals(nodo.tipo))
                {
                    IEnumerable<List<string>> caracts = new List<List<string>>();

                    if (caracteristicaAnterior.tipo.Equals("Hotel"))
                    {
                        caracts =
                            (await HotelMap.ReadAllAsync()).Select(H => H.caracteristicas).ToList();
                    }

                    if (caracteristicaAnterior.tipo.Equals("Habitación"))
                    {
                        caracts =
                            (await Tipo_HabitacionMap.ReadAllAsync()).Select(TH => TH.caracteristicas).ToList();
                    }

                    if (caracteristicaAnterior.tipo.Equals("Ambas"))
                    {
                        caracts =
                            (await HotelMap.ReadAllAsync()).Select(H => H.caracteristicas).ToList();

                        caracts.Concat((await Tipo_HabitacionMap.ReadAllAsync()).Select(TH => TH.caracteristicas).ToList());
                    }

                    var caractsHotelesByCaracteristica =
                        caracts.Where(L => L.Exists(LS => LS.Equals(nodo.nombre))).ToList();

                    if (caractsHotelesByCaracteristica.Count > 0)
                    {
                        throw new Exception("El tipo de caracterísitca ya ha sido utilizada para definir algunos registros, no se puede cambiar");
                    }
                }

                await GetMapper().UpdateAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Delete(Caracteristica nodo)
        {
            try
            {
                var caractsHoteles = 
                    (await HotelMap.ReadAllAsync()).Select(H => H.caracteristicas).ToList();

                var caractsHotelesByCaracteristica =
                    caractsHoteles.Where(L => L.Exists(LS => LS.Equals(nodo.nombre))).ToList();

                var caractsTipoHab = 
                    (await Tipo_HabitacionMap.ReadAllAsync()).Select(TH => TH.caracteristicas).ToList();

                var caractsTipoHabByCaracteristica =
                    caractsTipoHab.Where(L => L.Exists(LS => LS.Equals(nodo.nombre))).ToList();

                if (caractsHotelesByCaracteristica.Count > 0 || caractsTipoHabByCaracteristica.Count > 0)
                {
                    throw new Exception("Está característica está siendo utilizada en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Caracteristica Read(string nombre)
        {
            try
            {
                return GetMapper().FirstOrDefault<Caracteristica>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Caracteristica> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Caracteristica>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Caracteristica>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Caracteristica>();
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