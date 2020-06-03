using NDEV_HAPPY_INN.Model;
using System;
using System.Web;
using System.Web.UI;

namespace NDEV_HAPPY_INN
{
    public partial class auth : HappyInnPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/mods/home.aspx", true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}