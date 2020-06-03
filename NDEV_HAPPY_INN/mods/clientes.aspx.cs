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
    public partial class clientes : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "id" };

        #endregion

        #region "ATRIBUTOS"

        private List<Cliente> Lista
        {
            get => (List<Cliente>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await ClienteMap.ReadAllAsync()).ToList();
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await ClienteMap.ReadAllAsync()).ToList();
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

                Cliente nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await ClienteMap.Create(nodo);
                }
                else
                {
                    string key = hfId.Value;

                    nodo = LlenarNodo(ClienteMap.Read(Guid.Parse(key)));

                    await ClienteMap.Update(nodo);
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

                string key = hfId.Value;

                await ClienteMap.Delete(await ClienteMap.ReadAsync(Guid.Parse(key)));

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
                string key = Convert.ToString(gridView.DataKeys[gridView.SelectedRow.RowIndex].Values[0]);

                Cliente registroSeleccionado = ClienteMap.Read(Guid.Parse(key));

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

        protected Cliente LlenarNodo(Cliente nodo = null)
        {
            if (nodo == null)
            {
                return new Cliente()
                {
                    nombre = txtNombre.Text,
                    ap_paterno = txtApPaterno.Text,
                    ap_materno = txtApMaterno.Text,
                    domicilio = txtDomicilio.Text,
                    fecha_nacimiento = DateTime.Parse(txtFechaNacimiento.Text),
                    tel_casa = txtTelCasa.Text,
                    tel_celular = txtTelCelular.Text,
                    referencia = txtReferencia.Text
                };
            }
            else
            {
                nodo.nombre = txtNombre.Text;
                nodo.ap_paterno = txtApPaterno.Text;
                nodo.ap_materno = txtApMaterno.Text;
                nodo.domicilio = txtDomicilio.Text;
                nodo.fecha_nacimiento = DateTime.Parse(txtFechaNacimiento.Text);
                nodo.tel_casa = txtTelCasa.Text;
                nodo.tel_celular = txtTelCelular.Text;
                nodo.referencia = txtReferencia.Text;

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

            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                ShowMsgError("Ingrese el nombre");
                return false;
            }

            if (string.IsNullOrEmpty(txtApPaterno.Text))
            {
                ShowMsgError("Ingrese el apellido paterno");
                return false;
            }

            if (string.IsNullOrEmpty(txtApMaterno.Text))
            {
                ShowMsgError("Ingrese el apellido materno");
                return false;
            }

            if (string.IsNullOrEmpty(txtFechaNacimiento.Text))
            {
                ShowMsgError("Ingrese la fecha de nacimiento");
                return false;
            }

            if (string.IsNullOrEmpty(txtDomicilio.Text))
            {
                ShowMsgError("Ingrese el domicilio");
                return false;
            }

            if (string.IsNullOrEmpty(txtTelCasa.Text))
            {
                ShowMsgError("Ingrese el teléfono de casa");
                return false;
            }

            if (string.IsNullOrEmpty(txtTelCelular.Text))
            {
                ShowMsgError("Ingrese el teléfono de celular");
                return false;
            }

            if (string.IsNullOrEmpty(txtReferencia.Text))
            {
                ShowMsgError("Ingrese la referencia");
                return false;
            }

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Cliente nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = Convert.ToString(nodo.id);

                // datos generales
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtApPaterno.Text = Convert.ToString(nodo.ap_paterno);
                txtApMaterno.Text = Convert.ToString(nodo.ap_materno);
                txtFechaNacimiento.Text = Convert.ToString(nodo.fecha_nacimiento.ToString("yyyy-MM-dd"));
                txtDomicilio.Text = Convert.ToString(nodo.domicilio);
                txtTelCasa.Text = Convert.ToString(nodo.tel_casa);
                txtTelCelular.Text = Convert.ToString(nodo.tel_celular);
                txtReferencia.Text = Convert.ToString(nodo.referencia);

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;

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
            txtApPaterno.Text = string.Empty;
            txtApMaterno.Text = string.Empty;
            txtFechaNacimiento.Text = string.Empty;
            txtDomicilio.Text = string.Empty;
            txtTelCasa.Text = string.Empty;
            txtTelCelular.Text = string.Empty;
            txtReferencia.Text = string.Empty;

            btnEliminar.Visible = false;
            lblTituloFormulario.Text = "Agregar nuevo";

            ShowMsgError(string.Empty, false);
            upFormulario.Update();
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