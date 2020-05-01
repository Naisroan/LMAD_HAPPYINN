using System;
using System.Web;

namespace NDEV_HAPPY_INN
{
    public partial class auth : HappyInnPage
    {
        protected override void Page_PreInit(object sender, EventArgs e)
        {
            try
            {
                // base.Page_PreInit(sender, e);

                if (UsuarioLogeado == null)
                {
                    Sys.SignOut(HttpContext.Current);
                    return;
                }

                if (UsuarioLogeado.is_admin)
                {
                    Response.Redirect("~/mods/home.aspx");
                }
                else
                {
                    Response.Redirect("~/mods/home.aspx");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}