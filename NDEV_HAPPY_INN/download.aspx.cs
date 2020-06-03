using System;
using System.Web.UI;

namespace NDEV_HAPPY_INN
{
    public partial class download : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var value = Session["descarga"];

                if (value != null)
                    if (value.GetType().Equals(typeof(Descarga)))
                        ((Descarga)value).ObtenerArchivo(this.Context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}