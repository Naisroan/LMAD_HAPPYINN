using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Tipo_Habitacion
    {
        [PartitionKey]
        public string nombre { get; set; }

        public IDictionary<string, int> camas { get; set; }

        public string nivel { get; set; }

        public decimal costo_por_noche { get; set; }

        public int capacidad { get; set; }

        public List<string> caracteristicas { get; set; }
    }

    public class Tipo_HabitacionMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nombre = ?";

        public static async Task Create(Tipo_Habitacion nodo)
        {
            try
            {
                if (await Existe(nodo.nombre))
                {
                    throw new Exception("El tipo de habitacion ya existe");
                }

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Tipo_Habitacion nodo)
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

        public static async Task Delete(Tipo_Habitacion nodo)
        {
            try
            {
                List<Habitacion_By_Hotel> habitacionesHotel = 
                    (await Habitacion_By_HotelMap.ReadAllAsync()).Where(HH => HH.tipo_habitacion.Equals(nodo.nombre)).ToList();

                List<Reservacion> habitacionesReservaciones =
                    (await ReservacionMap.ReadAllAsync()).Where(R => R.tipo_habitacion.Equals(nodo.nombre)).ToList();

                if (habitacionesHotel.Count > 0 || habitacionesReservaciones.Count > 0)
                {
                    throw new Exception("Esta habitación está siendo utilizada en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Tipo_Habitacion Read(string nombre)
        {
            try
            {
                return GetMapper().FirstOrDefault<Tipo_Habitacion>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Tipo_Habitacion> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Tipo_Habitacion>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Tipo_Habitacion> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Tipo_Habitacion>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Tipo_Habitacion>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Tipo_Habitacion>();
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