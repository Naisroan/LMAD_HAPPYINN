using NDEV_HAPPY_INN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;

namespace NDEV_HAPPY_INN.mods.cats
{
    public partial class caracteristicas : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "nombre" };

        #endregion

        #region "ATRIBUTOS"

        private List<Caracteristica> Lista
        {
            get => (List<Caracteristica>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await CaracteristicaMap.ReadAllAsync()).ToList();
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await CaracteristicaMap.ReadAllAsync()).ToList();
                Sys.FillGridView(gvData, Lista);
                upGridView.Update();
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiarCampos();
                Sys.ShowModal(this, upFormulario);
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        protected async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                bool nuevo = !string.IsNullOrEmpty(hfId.Value) && hfId.Value.Equals("-1");

                if (!ValidarCampos(nuevo))
                    return;

                Caracteristica nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await CaracteristicaMap.Create(nodo);
                }
                else
                {
                    nodo = LlenarNodo(CaracteristicaMap.Read(hfId.Value));
                    await CaracteristicaMap.Update(nodo);
                }

                Msg.Show(this, Msg.CaptionFinalizado, "Se ha guardado el registro",
                    Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                ShowMsgError("Ha ocurrido un error: " + ex.Message);
            }
        }

        protected async void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(hfId.Value))
                {
                    ShowMsgError("Hubo un problema al obtener el valor del registro");
                    return;
                }

                await CaracteristicaMap.Delete(await CaracteristicaMap.ReadAsync(hfId.Value));

                Msg.Show(this, Msg.CaptionFinalizado, "Se ha eliminado el registro",
                    Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                ShowMsgError("Ha ocurrido un error: " + ex.Message);
            }
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

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridView gridView = (GridView)sender;

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(gridView, "select$" + e.Row.RowIndex.ToString(), true));
                }
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void gridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView gridView = (GridView)sender;

            if (gridView.SelectedRow.RowIndex < 0)
                return;

            try
            {
                string dataKey = Convert.ToString(gridView.DataKeys[gridView.SelectedRow.RowIndex].Value);

                if (string.IsNullOrEmpty(dataKey))
                    return;

                Caracteristica registroSeleccionado = CaracteristicaMap.Read(dataKey);

                if (registroSeleccionado == null)
                    return;

                LimpiarCampos();
                LlenarCampos(registroSeleccionado);
                Sys.ShowModal(this, upFormulario);
            }
            catch (Exception ex)
            {
                Msg.Show(upGridView, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
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

        protected Caracteristica LlenarNodo(Caracteristica nodo = null)
        {
            if (nodo == null)
            {
                return new Caracteristica()
                {
                    nombre = txtNombre.Text,
                    descripcion = txtDescripcion.Text,
                    tipo = ddlTipo.Text
                };
            }
            else
            {
                nodo.nombre = txtNombre.Text;
                nodo.descripcion = txtDescripcion.Text;
                nodo.tipo = ddlTipo.Text;

                return nodo;
            }
        }

        protected bool ValidarCampos(bool esNuevo = false)
        {
            if (!esNuevo)
            {
                if (string.IsNullOrEmpty(hfId.Value))
                {
                    ShowMsgError("Hubo un problema al obtener el valor del registro");
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(txtNombre.Text))
                {
                    ShowMsgError("Ingrese el nombre");
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                ShowMsgError("Ingrese la descripción");
                return false;
            }

            if (ddlTipo.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el tipo de característica");
                return false;
            }

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Caracteristica nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = Convert.ToString(nodo.nombre);

                // datos generales
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtDescripcion.Text = Convert.ToString(nodo.descripcion);
                ddlTipo.SelectedValue = Convert.ToString(nodo.tipo);

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;
                txtNombre.Enabled = false;

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Editar " + nodo.nombre;
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void LimpiarCampos()
        {
            hfId.Value = "-1";

            txtNombre.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;

            btnEliminar.Visible = false;
            txtNombre.Enabled = true;
            lblTituloFormulario.Text = "Agregar nuevo";

            ShowMsgError(string.Empty, false);
        }

        private void ShowMsgError(string errorMessage, bool show = true)
        {
            upMensajeFormulario.Visible = show;
            lblMensajeError.Text = errorMessage;

            if (!show)
            {
                lblMensajeError.Text = string.Empty;
            }

            upFormulario.Update();
        }

        #endregion
    }
}