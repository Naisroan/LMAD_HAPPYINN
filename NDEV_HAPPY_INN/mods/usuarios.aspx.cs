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
    public partial class usuarios : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "nick" };

        #endregion

        #region "ATRIBUTOS"

        private UsuarioMap Map => new UsuarioMap();

        private List<Usuario> Lista
        {
            get => (List<Usuario>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await Map.ReadAllAsync()).ToList();
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await Map.ReadAllAsync()).ToList();
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

                Usuario nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await Map.Create(nodo);
                }
                else
                {
                    nodo = LlenarNodo(Map.Read(hfId.Value));
                    await Map.Update(nodo);
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

                await Map.Delete(await Map.ReadAsync(hfId.Value));

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

                Usuario registroSeleccionado = Map.Read(dataKey);

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

        protected Usuario LlenarNodo(Usuario nodo = null)
        {
            if (nodo == null)
            {
                return new Usuario()
                {
                    nick = txtNick.Text,
                    password = txtPassword.Text,
                    nombre = txtNombre.Text,
                    ap_paterno = txtApPaterno.Text,
                    ap_materno = txtApMaterno.Text,
                    domicilio = txtDomicilio.Text,
                    fecha_nacimiento = DateTime.Parse(txtFechaNacimiento.Text),
                    is_admin = false,
                    tel_casa = txtTelCasa.Text,
                    tel_celular = txtTelCelular.Text
                };
            }
            else
            {
                nodo.nick = txtNick.Text;
                nodo.nombre = txtNombre.Text;
                nodo.ap_paterno = txtApPaterno.Text;
                nodo.ap_materno = txtApMaterno.Text;
                nodo.domicilio = txtDomicilio.Text;
                nodo.fecha_nacimiento = DateTime.Parse(txtFechaNacimiento.Text);
                nodo.tel_casa = txtTelCasa.Text;
                nodo.tel_celular = txtTelCelular.Text;

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
                if (string.IsNullOrEmpty(txtNick.Text))
                {
                    ShowMsgError("Ingrese el nick");
                    return false;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    ShowMsgError("Ingrese la contraseña");
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

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Usuario nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = Convert.ToString(nodo.nick);

                // datos generales
                txtNick.Text = Convert.ToString(nodo.nick);
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtApPaterno.Text = Convert.ToString(nodo.ap_paterno);
                txtApMaterno.Text = Convert.ToString(nodo.ap_materno);
                txtFechaNacimiento.Text = Convert.ToString(nodo.fecha_nacimiento.ToString("yyyy-MM-dd"));
                txtDomicilio.Text = Convert.ToString(nodo.domicilio);
                txtTelCasa.Text = Convert.ToString(nodo.tel_casa);
                txtTelCelular.Text = Convert.ToString(nodo.tel_celular);

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;
                txtNick.Enabled = false;
                txtPassword.Enabled = false;

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Editar " + nodo.nick;
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void LimpiarCampos()
        {
            hfId.Value = "-1";

            txtNick.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtApPaterno.Text = string.Empty;
            txtApMaterno.Text = string.Empty;
            txtFechaNacimiento.Text = string.Empty;
            txtDomicilio.Text = string.Empty;
            txtTelCasa.Text = string.Empty;
            txtTelCelular.Text = string.Empty;

            btnEliminar.Visible = false;
            txtNick.Enabled = true;
            txtPassword.Enabled = true;
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