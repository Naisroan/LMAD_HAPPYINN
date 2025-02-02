﻿using Cassandra.Mapping.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Usuario
    {
        [PartitionKey]
        public string nick { get; set; }

        public string password { get; set; }

        public bool? is_admin { get; set; }

        public int? nomina { get; set; }

        public string nombre { get; set; }

        public string ap_paterno { get; set; }

        public string ap_materno { get; set; }

        public DateTime fecha_nacimiento { get; set; }

        public string domicilio { get; set; }

        public string tel_casa { get; set; }

        public string tel_celular { get; set; }
    }

    public class UsuarioMap : Connection
    {
        private const string CQL_QUERY_WHERE_NAME = "WHERE nick = ?";

        public static async Task Create(Usuario nodo)
        {
            try
            {
                if (await Existe(nodo.nick))
                {
                    throw new Exception("El nick de usuario ya existe");
                }

                nodo.nomina = (await MaxNomina()) + 1;

                await GetMapper().InsertAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task Update(Usuario nodo)
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

        public static async Task Delete(Usuario nodo)
        {
            try
            {
                List<Reservacion> reservacionesUsuario = 
                    (await ReservacionMap.ReadAllByUsuarioAsync(nodo.nick)).ToList();

                if (reservacionesUsuario.Count > 0)
                {
                    throw new Exception("El usuario está siendo utilizado en otros registros, no se puede eliminar");
                }

                await GetMapper().DeleteAsync(nodo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Usuario Read(string nick)
        {
            try
            {
                return GetMapper().FirstOrDefault<Usuario>(CQL_QUERY_WHERE_NAME, nick);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<Usuario> ReadAsync(string nick)
        {
            try
            {
                return await GetMapper().FirstOrDefaultAsync<Usuario>(CQL_QUERY_WHERE_NAME, nick);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<IEnumerable<Usuario>> ReadAllAsync()
        {
            try
            {
                return await GetMapper().FetchAsync<Usuario>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<Usuario> ReadAll()
        {
            try
            {
                return GetMapper().Fetch<Usuario>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<int> MaxNomina()
        {
            try
            {
                List<Usuario> usuarios = (await ReadAllAsync()).ToList();

                if (usuarios.Count <= 0)
                    return 1;

                return usuarios.Max(U => U.nomina).GetValueOrDefault(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<bool> Existe(string nick)
        {
            try
            {
                return (await ReadAsync(nick)) != null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ObtenerClaveAdmin()
        {
            return Read("admin").password;
        }
    }
}