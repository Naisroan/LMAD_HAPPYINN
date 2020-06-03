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
    public partial class ciudades : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "pais", "estado", "nombre" };

        #endregion

        #region "ATRIBUTOS"

        private List<Ciudad> Lista
        {
            get => (List<Ciudad>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        #endregion

        #region "ATRIBUTOS NODOS"

        protected List<string> ListaNodos
        {
            get => (List<string>)Session["ListaNodos"];
            set => Session["ListaNodos"] = value;
        }

        private List<string> ListaNodosDeRegistro
        {
            get => (List<string>)Session["ListaNodosDeRegistro"];
            set => Session["ListaNodosDeRegistro"] = value;
        }

        private List<Panel> NodosNuevos
        {
            get => (List<Panel>)Session["NodosNuevos"];
            set => Session["NodosNuevos"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await CiudadMap.ReadAllAsync()).ToList();
                ListaNodos = UsuarioMap.ReadAll().Select(U => U.nick).ToList();
            }
            else
            {
                AgregarNodosNuevos();
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await CiudadMap.ReadAllAsync()).ToList();
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

                Ciudad nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await CiudadMap.Create(nodo);
                }
                else
                {
                    string[] keys = hfId.Value.Split(';');

                    UDT_Ciudad ciudadSeleccionada = new UDT_Ciudad()
                    {
                        pais = Convert.ToString(keys[0]),
                        estado = Convert.ToString(keys[1]),
                        nombre = Convert.ToString(keys[2])
                    };

                    nodo = LlenarNodo(CiudadMap.Read(ciudadSeleccionada));

                    await CiudadMap.Update(nodo, UsuariosAsignados(nodo));
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

                string[] keys = hfId.Value.Split(';');

                UDT_Ciudad ciudadSeleccionada = new UDT_Ciudad()
                {
                    pais = Convert.ToString(keys[0]),
                    estado = Convert.ToString(keys[1]),
                    nombre = Convert.ToString(keys[2])
                };

                await CiudadMap.Delete(await CiudadMap.ReadAsync(ciudadSeleccionada));

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
                UDT_Ciudad ciudadSeleccionada = new UDT_Ciudad()
                {
                    pais = Convert.ToString(gridView.DataKeys[gridView.SelectedRow.RowIndex].Values[0]),
                    estado = Convert.ToString(gridView.DataKeys[gridView.SelectedRow.RowIndex].Values[1]),
                    nombre = Convert.ToString(gridView.DataKeys[gridView.SelectedRow.RowIndex].Values[2])
                };

                Ciudad registroSeleccionado = CiudadMap.Read(ciudadSeleccionada);

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

        protected Ciudad LlenarNodo(Ciudad nodo = null)
        {
            if (nodo == null)
            {
                return new Ciudad()
                {
                    nombre = txtNombre.Text,
                    estado = txtEstado.Text,
                    pais = txtPais.Text
                };
            }
            else
            {
                nodo.nombre = txtNombre.Text;
                nodo.estado = txtEstado.Text;
                nodo.pais = txtPais.Text;

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

                string message = string.Empty;

                if (!ValidarNuevosNodos(ref message))
                {
                    ShowMsgError(message);
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

            if (string.IsNullOrEmpty(txtEstado.Text))
            {
                ShowMsgError("Ingrese el estado");
                return false;
            }

            if (string.IsNullOrEmpty(txtPais.Text))
            {
                ShowMsgError("Ingrese el país");
                return false;
            }

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Ciudad nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = $"{nodo.pais};{nodo.estado};{nodo.nombre}";

                // datos generales
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtEstado.Text = Convert.ToString(nodo.estado);
                txtPais.Text = Convert.ToString(nodo.pais);
                ListaNodosDeRegistro = Usuario_By_CiudadMap.ReadAll().Where(U => U.ciudad == nodo.nombre).Select(U => U.nick).ToList();

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;
                txtNombre.Enabled = false;
                txtEstado.Enabled = false;
                txtPais.Enabled = false;
                tabs.Visible = true;
                btnAgregarNodo.Enabled = ListaNodosDeRegistro.Count < ListaNodos.Count;

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Editar " + nodo.nombre;

                // llenamos el datarepeater
                Sys.FillRepeater(rpNodos, ListaNodosDeRegistro);
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
            txtEstado.Text = string.Empty;
            txtPais.Text = string.Empty;
            phNodosNuevos.Controls.Clear();

            if (ListaNodosDeRegistro != null)
            {
                ListaNodosDeRegistro.Clear();
            }

            if (NodosNuevos != null)
            {
                NodosNuevos.Clear();
            }

            btnEliminar.Visible = false;
            txtNombre.Enabled = true;
            txtEstado.Enabled = true;
            txtPais.Enabled = true;
            tabs.Visible = false;
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

        #region "BOTONES NODOS"

        protected void btnAgregarNodo_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            lblMensajeNodo.Text = string.Empty;

            if (!ValidarNuevosNodos(ref message))
            {
                lblMensajeNodo.Text = message;
                return;
            }

            if (NodosNuevos == null)
                NodosNuevos = new List<Panel>();

            NodosNuevos.Add(CrearNodo(NodosNuevos.Count));

            BloquearNodosAnteriores();
            AgregarNodosNuevos();

            btnAgregarNodo.Enabled = ListaNodosDeRegistro.Count + NodosNuevos.Count < ListaNodos.Count;

            upNodosNuevos.Update();
        }

        protected void btnRetirar_Click(object sender, EventArgs e)
        {
            Button btnRetirar = (Button)sender;

            if (string.IsNullOrEmpty(btnRetirar.CommandArgument))
                return;

            ListaNodosDeRegistro.Remove(btnRetirar.CommandArgument);
            Sys.FillRepeater(rpNodos, ListaNodosDeRegistro);

            if (NodosNuevos == null)
                NodosNuevos = new List<Panel>();

            btnAgregarNodo.Enabled = ListaNodosDeRegistro.Count + NodosNuevos.Count < ListaNodos.Count;

            upNodosAnteriores.Update();
            upNodosNuevos.Update();
        }

        #endregion

        #region "EVENTOS NODOS"

        protected void rpNodos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        #endregion

        #region "METODOS NODOS"

        private void AgregarNodosNuevos()
        {
            phNodosNuevos.Controls.Clear();

            if (NodosNuevos == null || NodosNuevos.Count <= 0)
                return;

            foreach (Panel pnlNodo in NodosNuevos)
            {
                phNodosNuevos.Controls.Add(pnlNodo);
            }
        }

        private Panel CrearNodo(int idx)
        {
            Panel pnlNodo = new Panel()
            {
                ID = $"pnlNodo_{idx.ToString()}",
                CssClass = "nodo"
            };

            Panel pnlRow = new Panel() { CssClass = "form-row" };
            Panel pnlColumn = new Panel() { CssClass = "col-12 mb-3" };

            DropDownList ddlUsuario = new DropDownList()
            {
                ID = $"{pnlNodo.ID}_ddlUsuario_{idx.ToString()}",
                CssClass = "custom-select"
            };

            Sys.FillDropDown(ddlUsuario, NodosSinDuplicados(ListaNodosDeRegistro));

            pnlColumn.Controls.Add(ddlUsuario);
            pnlRow.Controls.Add(pnlColumn);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private List<string> NodosSinDuplicados(List<string> nodosDeRegistro)
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(ListaNodos);

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = ListaNodos.Intersect(nodosDeRegistro).ToList();
            nodosRepetidos = nodosRepetidos.Union(ListaNodos.Intersect(nodosNuevos)).ToList();

            // se obtienen los nodos que quedan en base a las dos listas anteriores
            listaNodos.RemoveAll(LN => nodosRepetidos.Exists(NR => NR.Equals(LN)));

            return listaNodos;
        }

        private List<string> ObtenerValoresNodos()
        {
            List<string> nodosNuevos = new List<string>();
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlUsuario = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                nodosNuevos.Add(ddlUsuario.Text);
            }

            return nodosNuevos;
        }

        private bool ValidarNuevosNodos(ref string message)
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlUsuario = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                if (ddlUsuario == null)
                {
                    message = "No se encontro la información";
                    return false;
                }

                if (ddlUsuario.SelectedIndex <= 0)
                {
                    message = "Seleccione el usuario";
                    return false;
                }
            }

            return true;
        }

        private void BloquearNodosAnteriores()
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlUsuario = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                if (ddlUsuario == null)
                    continue;

                ddlUsuario.Enabled = false;
            }
        }

        private List<Usuario_By_Ciudad> UsuariosAsignados(Ciudad ciudad)
        {
            List<Usuario_By_Ciudad> usuariosAsignados = new List<Usuario_By_Ciudad>();

            foreach (string valorViejo in ListaNodosDeRegistro)
            {
                usuariosAsignados.Add(new Usuario_By_Ciudad()
                {
                    nick = valorViejo,
                    ciudad = ciudad.nombre
                });
            }

            List<string> valoresNuevos = ObtenerValoresNodos();

            foreach (string valorNuevo in valoresNuevos)
            {
                usuariosAsignados.Add(new Usuario_By_Ciudad()
                {
                    nick = valorNuevo,
                    ciudad = ciudad.nombre
                });
            }

            return usuariosAsignados;
        }

        #endregion
    }
}