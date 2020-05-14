using NDEV_HAPPY_INN.Model;
using System;
using System.Web;
using System.Web.Security;

namespace NDEV_HAPPY_INN
{
    public partial class login : HappyInnPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Response.Redirect(UrlDefault);
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
                
                Usuario usuario = await new UsuarioMap().ReadAsync(txtUsuario.Text);

                if (usuario == null || !usuario.password.Equals(txtPassword.Text))
                {
                    Msg.Show(this, Msg.CaptionEspera, "El usuario y/o contraseña no coinciden", Msg.Tipo.Warning);
                    return;
                }

                if (chkMantenerSesion.Checked)
                {
                    // aqui hacemos la cookie, pero no me acuerdo como hacerla asi que la dejo para after
                }

                // establecemos la variable global del UsuarioLogeado [obtiene el usuario guardado en Session]
                UsuarioLogeado = usuario;

                FormsAuthentication.RedirectFromLoginPage(usuario.nick, chkMantenerSesion.Checked);
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

        #endregion
    }
}