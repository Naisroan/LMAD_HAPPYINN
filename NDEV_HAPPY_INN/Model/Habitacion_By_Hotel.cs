using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Habitacion_By_Hotel
    {
        [PartitionKey]
        public string nombre { get; set; }

        public string tipo_habitacion { get; set; }

        public int cantidad { get; set; }

        public bool disponible { get; set; }
    }

    public class Habitacion_By_HotelMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nombre = ?";
        private const string CQL_QUERY_WHERE_NAME_AND_TYPE = "WHERE nombre = ? AND tipo_habitacion = ?";

        public static async Task Create(Habitacion_By_Hotel nodo)
        {
            try
            {
                nodo.disponible = true;

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Habitacion_By_Hotel nodo)
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

        public static async Task Delete(Habitacion_By_Hotel nodo)
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

        public static Habitacion_By_Hotel Read(string nombreHotel, string tipoHabitacion)
        {
            try
            {
                return GetMapper().FirstOrDefault<Habitacion_By_Hotel>(CQL_QUERY_WHERE_NAME_AND_TYPE, nombreHotel, tipoHabitacion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Habitacion_By_Hotel> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Habitacion_By_Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Habitacion_By_Hotel>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Habitacion_By_Hotel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Habitacion_By_Hotel>> ReadByHotelAsync(string nombre)
        {
            try
            {
                return await GetMapper().FetchAsync<Habitacion_By_Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Habitacion_By_Hotel> ReadByHotel(string nombre)
        {
            try
            {
                return GetMapper().Fetch<Habitacion_By_Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Habitacion_By_Hotel> ReadAll(string nombre)
        {
            try
            {
                return GetMapper().Fetch<Habitacion_By_Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int HabitacionesDisponibles(Habitacion_By_Hotel habitacion, DateTime fecha)
        {
            return habitacion.cantidad - ReservacionMap.ReservacionByFechaHabitacion(habitacion, fecha, fecha).Count();

            //// obtenemos las reservaciones en curso
            //IEnumerable<Reservacion> reservacionesNoFinalizadas = 
            //    ReservacionMap.ReadAllByHotel(nombreHotel).Where(R => !R.check_out && !R.cancelada);

            //// obtenemos las reservaciones por el tipo de habitacion en base a las reservaciones en cursro
            //IEnumerable<Reservacion> reservacionesTipoHabitacion = 
            //    reservacionesNoFinalizadas.Where(R => R.tipo_habitacion.Equals(tipoHabitacion));

            //// cantidad de reservaciones por tipo de habitacion
            //int cant = reservacionesTipoHabitacion.Count();

            //// obtenemos la habitacion por hotel
            //Habitacion_By_Hotel habitacion = Read(nombreHotel, tipoHabitacion);

            //// calculamos
            //int cantHabitacionesDisp = habitacion.cantidad - cant;

            //return cantHabitacionesDisp >= 0 ? cantHabitacionesDisp : 0;
        }

        public static int HabitacionesTotales(string tipoHabitacion, string nombreHotel)
        {
            return Read(nombreHotel, tipoHabitacion).cantidad;
        }
    }
}