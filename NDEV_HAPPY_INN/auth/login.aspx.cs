using NDEV_HAPPY_INN.Model;
using System;
using System.Web;
using System.Web.Security;

namespace NDEV_HAPPY_INN
{
    public partial class login : HappyInnPage
    {
        #region "CONSTANTES"

        public const string NAME_COOKIE_RECORDAR_USUARIO = "recordar_usuario";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string message = string.Empty;

                if (!TestConnectionCassandra(ref message))
                {
                    Msg.Show(upLogin, "Conexión a Cassandra", message, Msg.Tipo.Warning);

                    return;
                }
                else
                {
                    btnIniciarSesion.Enabled = true;
                }

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Response.Redirect(UrlDefault);
                }

                Usuario usuarioCookie = null;

                if (UsuarioRecordadoDeCookie(ref usuarioCookie))
                {
                    UsuarioLogeado = usuarioCookie;
                    FormsAuthentication.RedirectFromLoginPage(usuarioCookie.nick, chkMantenerSesion.Checked);
                }
            }
        }

        #region "BOTONES"

        protected async void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                string validationMessage = string.Empty;

                if (!CamposCorrectos(ref validationMessage))
                {
                    Msg.Show(this, Msg.CaptionEspera, validationMessage, Msg.Tipo.Warning, Msg.BotonEspera);
                }
                
                Usuario usuario = await UsuarioMap.ReadAsync(txtUsuario.Text);

                if (usuario == null || !usuario.password.Equals(txtPassword.Text))
                {
                    Msg.Show(this, Msg.CaptionEspera, "El usuario y/o contraseña no coinciden", Msg.Tipo.Warning);
                    return;
                }

                if (chkMantenerSesion.Checked)
                {
                    Response.Cookies.Add(new HttpCookie(NAME_COOKIE_RECORDAR_USUARIO, usuario.nick)
                    {
                        Expires = DateTime.Now.AddYears(1)
                    });
                }

                // establecemos la variable global del UsuarioLogeado [obtiene el usuario guardado en Session]
                UsuarioLogeado = usuario;

                FormsAuthentication.RedirectFromLoginPage(usuario.nick, chkMantenerSesion.Checked);
                Response.Redirect(UrlDefault);
            }
            catch (Exception ex)
            {
                Msg.Show(upLogin, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        #endregion

        #region "METODOS"

        protected bool CamposCorrectos(ref string message)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                message = "Ingrese el usuario";
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                message = "Ingrese la contraseña";
                return false;
            }

            return true;
        }

        protected bool TestConnectionCassandra(ref string message)
        {
            try
            {
                Connection connection = new Connection();

                return true;
            }
            catch (ConnectionCassandraException ex)
            {
                message = $"No se ha podido conectar a cassandra: {ex.Message}".Replace("'", "\"");
                return false;
            }
        }

        protected bool UsuarioRecordadoDeCookie(ref Usuario usuario)
        {
            var cookieUsuario = Request.Cookies.Get(NAME_COOKIE_RECORDAR_USUARIO);

            if (cookieUsuario == null)
                return false;

            if (string.IsNullOrEmpty(cookieUsuario.Value))
                return false;

            Usuario usuarioObtenido = UsuarioMap.Read(cookieUsuario.Value);

            if (usuarioObtenido == null)
                return false;

            usuario = usuarioObtenido;

            return true;
        }

        #endregion
    }
}