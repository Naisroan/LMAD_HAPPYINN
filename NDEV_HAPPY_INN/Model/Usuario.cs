using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.Model
{
    public class Usuario
    {
        public Guid id_usuario { get; set; }

        public string name { get; set; }

        public string password { get; set; }

        public bool is_admin { get; set; }
    }

    public class UsuarioMap : Connection
    {
        private const string CQL_FIRST_OR_DEFAULT = "WHERE name = ?";

        public Usuario Read(string name)
        {
            return GetMapper().FirstOrDefault<Usuario>(CQL_FIRST_OR_DEFAULT, name);
        }

        public async Task<Usuario> ReadAsync(string name)
        {
            return await GetMapper().FirstOrDefaultAsync<Usuario>(CQL_FIRST_OR_DEFAULT, name);
        }
    }
}