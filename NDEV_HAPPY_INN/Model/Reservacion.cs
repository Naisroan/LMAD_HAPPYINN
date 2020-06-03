using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Reservacion
    {
        [PartitionKey]
        public Guid id { get; set; }

		[ClusteringKey]
		public Guid id_cliente { get; set; }

		public string nombre_hotel { get; set; }

		public string tipo_habitacion { get; set; }

        public int cant_personas { get; set; }

        public decimal monto_anticipo { get; set; }

		public string medio_pago_anticipo { get; set; }

		public DateTime fecha_ini { get; set; }

		public DateTime fecha_fin { get; set; }

		public decimal monto_pago { get; set; }

		public string medio_pago { get; set; }

		public bool check_in { get; set; }

		public bool check_out { get; set; }

        public bool cancelada { get; set; }

        public List<string> servicios_extras { get; set; }

        public DateTime fecha_registro { get; set; }
    }

    public class ReservacionMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE id = ?";

        public static async Task Create(Reservacion nodo)
        {
            try
            {
                Habitacion_By_Hotel habitacion = Habitacion_By_HotelMap.Read(nodo.nombre_hotel, nodo.tipo_habitacion);
                IEnumerable<Reservacion> reservacionesPorHabitacion = ReservacionByFechaHabitacion(habitacion, nodo.fecha_ini, nodo.fecha_fin);

                if (reservacionesPorHabitacion.Count() >= habitacion.cantidad)
                {
                    throw new Exception($"No hay habitaciones disponibles del tipo {habitacion.tipo_habitacion} para el rango de fechas específicadas");
                }

                Reservacion ultimaResDeCliente = LastReservacionByCliente(nodo.id_cliente);
                
                // se verifica si ya tiene una reservación en curso
                if (ReservacionEnCurso(ultimaResDeCliente))
                {
                    // si es asi, se verifica que no se empalmen las fechas
                    if (EmpalmeReservaciones(ultimaResDeCliente, nodo))
                    {
                        throw new Exception($"El cliente ya tiene una reservación en curso en las fechas: " +
                            $"{ultimaResDeCliente.fecha_ini.ToLongDateString()} a {ultimaResDeCliente.fecha_fin.ToLongDateString()} " +
                            $"en el hotel Happy Inn - {ultimaResDeCliente.nombre_hotel}");
                    }
                }

                // Habitacion_By_HotelMap.HabitacionesDisponibles(habitacionHotel.tipo_habitacion, habitacionHotel.nombre) > 0;

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Reservacion nodo)
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

        public static async Task Delete(Reservacion nodo)
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

        public static Reservacion Read(Guid id)
        {
            try
            {
                return GetMapper().FirstOrDefault<Reservacion>(CQL_QUERY_WHERE_NAME, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Reservacion> ReadAsync(Guid id)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Reservacion>(CQL_QUERY_WHERE_NAME, id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Reservacion>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Reservacion>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Reservacion>> ReadAllByUsuarioAsync(string nick)
        {
            try
            {
                List<Hotel> hoteles = await HotelMap.ReadByUsuarioAsync(nick);

                return (await GetMapper().FetchAsync<Reservacion>()).Where(R => hoteles.Exists(H => H.nombre.Equals(R.nombre_hotel)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Reservacion> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Reservacion>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Reservacion> ReadAllByHotel(string id)
        {
            return ReadAll().Where(R => R.nombre_hotel.Equals(id));
        }

        public static IEnumerable<Reservacion> ReservacionByFechaHabitacion(Habitacion_By_Hotel habitacion, DateTime fechaInicial, DateTime fechaFinal)
        {
            return ReadAll().Where(R =>
                R.nombre_hotel.Equals(habitacion.nombre)
                && R.tipo_habitacion.Equals(habitacion.tipo_habitacion)
                && !R.cancelada
                && !R.check_out
                && (R.fecha_ini.Date >= fechaInicial.Date || R.fecha_fin.Date >= fechaInicial.Date)
                && (R.fecha_ini.Date <= fechaFinal.Date || R.fecha_fin.Date <= fechaFinal.Date));
        }

        public static Reservacion LastReservacionByCliente(Guid id)
        {
            return ReadAll().Where(R => R.id_cliente.Equals(id)).OrderByDescending(R => R.fecha_registro).FirstOrDefault();
        }

        public static bool ReservacionEnCurso(Reservacion reservacion) =>
            reservacion != null && !reservacion.cancelada && !reservacion.check_out;

        public static bool EmpalmeReservaciones(Reservacion res01, Reservacion res02)
            => (res01.fecha_ini.Date >= res02.fecha_ini.Date || res01.fecha_fin.Date >= res02.fecha_ini.Date)
                && (res01.fecha_ini.Date <= res02.fecha_fin.Date || res01.fecha_fin.Date <= res02.fecha_fin.Date);

        // esta es la función que valida todas las reservaciones y da el check inn si paso la fecha inicial de reservación y si el cliente no llego
        public static async Task EstablecerCheckInAutomatico(DateTime fechaInicio)
        {
            IEnumerable<Reservacion> reservaciones = (await ReadAllAsync()).Where(R => !R.cancelada && !R.check_in && !R.check_out);

            foreach (Reservacion reservacion in reservaciones)
            {
                if (fechaInicio.Date >= reservacion.fecha_ini.Date)
                {
                    reservacion.cancelada = true;
                    reservacion.servicios_extras = new List<string>();
                    reservacion.medio_pago = "";
                    reservacion.monto_pago = 0;

                    await Update(reservacion);
                }
            }
        }
    }
}