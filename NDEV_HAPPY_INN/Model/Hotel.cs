using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Hotel
    {
        [PartitionKey]
        public string nombre { get; set; }

		public DateTime inicio_operaciones { get; set; }

		public string domicilio { get; set; }

		public string ciudad { get; set; }

		public bool zona_turistica { get; set; }

		public string usuario_registro { get; set; }

		public int num_pisos { get; set; }

		public List<string> caracteristicas { get; set; }
    }

    public class HotelMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nombre = ?";

        public static async Task Create(Hotel nodo)
        {
            try
            {
                if (await Existe(nodo.nombre))
                {
                    throw new Exception("El nombre del hotel ya existe");
                }

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Hotel nodo, List<Habitacion_By_Hotel> habitaciones = null)
        {
            try
            {
                await GetMapper().UpdateAsync(nodo);

                List<Habitacion_By_Hotel> habitacionesAnteriores = (await Habitacion_By_HotelMap.ReadByHotelAsync(nodo.nombre)).ToList();

                foreach (Habitacion_By_Hotel habitacionVieja in habitacionesAnteriores)
                {
                    await Habitacion_By_HotelMap.Delete(habitacionVieja);
                }

                foreach (Habitacion_By_Hotel habitacionNueva in habitaciones)
                {
                    await Habitacion_By_HotelMap.Create(habitacionNueva);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Delete(Hotel nodo)
        {
            try
            {
                List<Reservacion> hotelesReservaciones =
                    (await ReservacionMap.ReadAllAsync()).Where(R => R.nombre_hotel.Equals(nodo.nombre)).ToList();

                if (hotelesReservaciones.Count > 0)
                {
                    throw new Exception("Este hotel está siendo utilizado en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);

                List<Habitacion_By_Hotel> habitacionesAnteriores = (await Habitacion_By_HotelMap.ReadByHotelAsync(nodo.nombre)).ToList();

                foreach (Habitacion_By_Hotel habitacion in habitacionesAnteriores)
                {
                    await Habitacion_By_HotelMap.Delete(habitacion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Hotel Read(string nombre)
        {
            try
            {
                return GetMapper().FirstOrDefault<Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Hotel> ReadAsync(string nombre)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Hotel>(CQL_QUERY_WHERE_NAME, nombre);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Hotel>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Hotel>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Hotel>> ReadAllByCiudadAsync(string ciudad)
        {
            try
            {
                return (await GetMapper().FetchAsync<Hotel>()).Where(H => H.ciudad.Equals(ciudad));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Hotel> ReadAllByCiudad(string ciudad)
        {
            try
            {
                return GetMapper().Fetch<Hotel>().Where(H => H.ciudad.Equals(ciudad));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Hotel> ReadAllByPais(string pais)
        {
            try
            {
                List<Hotel> hotelesPorPais = new List<Hotel>();
                List<Ciudad> ciudadesPorPais = CiudadMap.ReadAll().Where(C => C.pais.Equals(pais)).Distinct().ToList();

                foreach (Ciudad ciudad in ciudadesPorPais)
                {
                    IEnumerable<Hotel> hotelesPorCiudad = ReadAllByCiudad(ciudad.nombre);

                    foreach (Hotel hotel in hotelesPorCiudad)
                    {
                        hotelesPorPais.Add(hotel);
                    }
                }

                return hotelesPorPais;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<Hotel> ReadAllByFechaAndPais(DateTime fecha, string pais)
        {
            List<Hotel> hoteles = new List<Hotel>();

            return hoteles;
        }

        public static async Task<List<Hotel>> ReadByUsuarioAsync(string nick)
        {
            try
            {
                List<Hotel> hotelesPorUsuario = new List<Hotel>();

                IEnumerable<Hotel> hoteles = await ReadAllAsync();
                IEnumerable<Usuario_By_Ciudad> ciudadesPorUsuario = await Usuario_By_CiudadMap.ReadAllByUsuarioAsync(nick);

                foreach (Usuario_By_Ciudad nodo in ciudadesPorUsuario)
                {
                    IEnumerable<Hotel> hotelesPorCiudad = await ReadAllByCiudadAsync(nodo.ciudad);

                    foreach (Hotel hotel in hotelesPorCiudad)
                    {
                        hotelesPorUsuario.Add(hotel);
                    }
                }

                return hotelesPorUsuario;
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

        public static int HabitacionesDisponibles(Hotel hotel, DateTime fecha)
        {
            int cant = 0;
            var habitaciones = Habitacion_By_HotelMap.ReadByHotel(hotel.nombre);

            foreach (var hab in habitaciones)
            {
                int cantReservaciones = ReservacionMap.ReservacionByFechaHabitacion(hab, fecha, fecha).Count();
                cant += hab.cantidad - cantReservaciones;
            }

            return cant;
        }

        public static int HabitacionesTotales(Hotel hotel)
        {
            return Habitacion_By_HotelMap.ReadByHotel(hotel.nombre).Select(H => H.cantidad).Sum();
        }

        public static float PorcentajeOcupacion(string nombre, DateTime fecha)
        {
            Hotel hotel = Read(nombre);

            int cantHabDisponibles = HabitacionesDisponibles(hotel, fecha);
            int cantHabTotales = HabitacionesTotales(hotel);
            int cantHabEnUso = cantHabTotales - cantHabDisponibles;

            return cantHabEnUso * 100.0f / cantHabTotales;
        }
    }
}