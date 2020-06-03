using System;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NDEV_HAPPY_INN.Model;

namespace NDEV_HAPPY_INN
{
    public class HappyInnPage : Page
    {
        #region "CONSTANTES"

        public const string NAME_SESSION_USUARIO_LOGEADO = "usuario_logeado";

        #endregion

        #region "ATRIBUTOS"

        protected static readonly string UrlLogin = $"~{FormsAuthentication.LoginUrl}";
        protected static readonly string UrlDefault = $"~{FormsAuthentication.DefaultUrl}";

        private const string UrlHome = "~/mods/home.aspx";
        private const string UrlError403 = "~/errors/unauth_access.html";
        private const string UrlError404 = "~/errors/page_not_found.html";
        private const string UrlError500 = "~/errors/ups.html";

        protected string NamePantalla => Path.GetFileName(Request.Path);

        public static Usuario UsuarioLogeado
        {
            get
            {
                HttpContext cntx = HttpContext.Current;

                if (!cntx.User.Identity.IsAuthenticated)
                    return null;

                var nickUsuario = cntx.User.Identity.Name;

                if (nickUsuario == null)
                {
                    Sys.SignOut(cntx, true);
                    return null;
                }

                var usuarioLogeado = cntx.Session[NAME_SESSION_USUARIO_LOGEADO];

                if (usuarioLogeado == null)
                {
                    usuarioLogeado = UsuarioMap.Read(nickUsuario);

                    if (usuarioLogeado == null)
                    {
                        Sys.SignOut(cntx, true);
                        return null;
                    }

                    cntx.Session[NAME_SESSION_USUARIO_LOGEADO] = (Usuario)usuarioLogeado;
                }

                return (Usuario)usuarioLogeado;
            }

            set => HttpContext.Current.Session[NAME_SESSION_USUARIO_LOGEADO] = value;
        }

        #endregion

        #region "EVENTOS"

        protected virtual void Page_PreInit(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    // NO ESTA AUTENTICADO
                    if (!HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        // VERIFICA QUE LA URL ACTUAL NO SEA LA DE LOGIN
                        // (ESTO EVITA QUE SE CICLE)
                        if (!AppRelativeVirtualPath.Equals(UrlLogin))
                        {
                            // COMO NO ES LA PAGINA DE LOGIN Y NO ESTA AUTENTICADO
                            // REDIRECCIONA A LOGIN PARA QUE INICIE SESION
                            FormsAuthentication.RedirectToLoginPage();
                        }
                    }
                    // AUTENTICADO
                    else
                    {
                        // VERIFICA QUE NO SEA LA URL DE LOGIN  O DE AUTH O DEFAULT
                        if (!AppRelativeVirtualPath.Equals(UrlLogin) &&
                            !AppRelativeVirtualPath.Equals(UrlDefault) &&
                            !AppRelativeVirtualPath.Equals(UrlHome))
                        {
                            string NameFilePage = Path.GetFileName(Request.Path);
                        }

                        // cancela las reservaciones si el cliente no ha llegado para su fecha inicial de reservación
                        ReservacionMap.EstablecerCheckInAutomatico(DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region "METODOS"

        public void HideLoad(UpdatePanel upPage, Panel pnlLoad, bool hide = true)
        {
            upPage.Visible = hide;
            pnlLoad.Visible = !hide;
        }

        #endregion
    }
}