using NDEV_HAPPY_INN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NDEV_HAPPY_INN
{
    public partial class SiteMaster : MasterPage
    {
        public static Usuario UsuarioLogeado
        {
            get
            {
                HttpContext cntx = HttpContext.Current;

                if (!cntx.User.Identity.IsAuthenticated)
                    return null;

                var userSession = cntx.Session["UsuarioActual"];

                if (userSession == null)
                    cntx.Session["UsuarioActual"] = new UsuarioMap().Read(cntx.User.Identity.Name);

                return (Usuario)cntx.Session["UsuarioActual"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
            }
        }

        #region "BOTONES"

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Sys.SignOut(HttpContext.Current);
        }

        #endregion
    }
}