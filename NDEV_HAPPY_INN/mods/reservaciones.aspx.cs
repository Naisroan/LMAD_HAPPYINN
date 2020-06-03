using NDEV_HAPPY_INN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.mods
{
    public partial class reservaciones : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "id" };

        #endregion

        #region "ATRIBUTOS"

        private List<Reservacion> Lista
        {
            get => (List<Reservacion>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        private List<Cliente> Clientes
        {
            get => (List<Cliente>)Session["Clientes"];
            set => Session["Clientes"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await ReservacionMap.ReadAllAsync()).OrderByDescending(R => R.fecha_registro).ToList();
                Clientes = (await ClienteMap.ReadAllAsync()).OrderBy(C => ClienteMap.NombreCompleto(C)).ToList();

                Sys.FillDropDown(ddlCliente, Clientes, "id", "nombre", "Seleccione un cliente para ver sus reservaciones...");
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await ReservacionMap.ReadAllAsync()).OrderByDescending(R => R.fecha_registro).ToList();
                Sys.FillGridView(gvData, Lista);
                upGridView.Update();
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            if(ddlCliente.SelectedIndex <= 0)
            {
                Msg.Show(this, Msg.CaptionEspera, "Seleccione un cliente antes de buscar", Msg.Tipo.Warning);
                return;
            }

            Guid idCliente = Guid.Parse(ddlCliente.SelectedValue);

            Lista = 
                ReservacionMap.ReadAll().Where(R => R.id_cliente.Equals(idCliente)).OrderByDescending(R => R.fecha_registro).ToList();

            Sys.FillGridView(gvData, Lista);
            upGridView.Update();
        }

        #endregion

        #region "EVENTOS"

        protected void gridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gridView = (GridView)sender;

            try
            {
                gridView.PageIndex = e.NewPageIndex;
                gridView.DataBind();
                upGridView.Update();
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType.Equals(DataControlRowType.DataRow))
            {
                Reservacion reservacion = (Reservacion)e.Row.DataItem;
                
                Literal litNombreCliente = (Literal)e.Row.Cells[1].FindControl("litNombreCliente");
                Literal litCostoHabitacion = (Literal)e.Row.Cells[2].FindControl("litCostoHabitacion");
                Repeater rpServiciosExtras = (Repeater)e.Row.Cells[5].FindControl("rpServiciosExtras");

                litNombreCliente.Text = ClienteMap.NombreCompleto(ClienteMap.Read(reservacion.id_cliente));
                litCostoHabitacion.Text = Convert.ToString(Tipo_HabitacionMap.Read(reservacion.tipo_habitacion).costo_por_noche);
                Sys.FillRepeater(rpServiciosExtras, reservacion.servicios_extras);

                if (reservacion.cancelada)
                {
                    e.Row.Cells[1].CssClass = "cancelada";
                }
                else
                {
                    if (!reservacion.check_in)
                    {
                        e.Row.Cells[1].CssClass = "checkinpendiente";
                    }
                    else
                    {
                        if (!reservacion.check_out)
                        {
                            e.Row.Cells[1].CssClass = "checkoutpendiente";
                        }
                    }
                }
            }
        }

        protected void rpServiciosExtras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            string strServicioExtra = Convert.ToString(e.Item.DataItem);
            Servicio_Extra servicioExtra = Servicio_ExtraMap.Read(strServicioExtra);

            Literal litPrecioServicioExtra = (Literal)e.Item.FindControl("litPrecioServicioExtra");

            litPrecioServicioExtra.Text = Convert.ToString(servicioExtra.precio);
        }

        protected void ddlCantidadElementos_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dropDownList = (DropDownList)sender;

            try
            {
                if (dropDownList.SelectedItem != null)
                {
                    int tamEspecificado = int.Parse(dropDownList.SelectedItem.Value);
                    gvData.PageSize = tamEspecificado.Equals(0) ? Lista.Count : tamEspecificado;

                    Sys.FillGridView(gvData, Lista);
                    upGridView.Update();
                }
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void ddlFiltros_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dropDownList = (DropDownList)sender;

            try
            {
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        #endregion

        #region "METODOS"

        protected bool ValidarCampos()
        {

            return true;
        }

        #endregion
    }
}