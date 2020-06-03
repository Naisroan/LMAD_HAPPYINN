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
    public partial class habitaciones : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "nombre" };

        #endregion

        #region "ATRIBUTOS"

        private List<Tipo_Habitacion> Lista
        {
            get => (List<Tipo_Habitacion>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        #endregion

        #region "ATRIBUTOS NODOS CAMAS"

        protected static List<string> ListaNodos = new List<string>
        {
            "Individual",
            "Matrimonial",
            "Queen Size",
            "King Size"
        };

        private IDictionary<string, int> ListaNodosDeRegistro
        {
            get => (IDictionary<string, int>)Session["ListaNodosDeRegistro"];
            set => Session["ListaNodosDeRegistro"] = value;
        }

        private List<Panel> NodosNuevos
        {
            get => (List<Panel>)Session["NodosNuevos"];
            set => Session["NodosNuevos"] = value;
        }

        #endregion

        #region "ATRIBUTOS NODOS CARACTERISTICAS"

        protected List<string> ListaNodos2
        {
            get => (List<string>)Session["ListaNodos2"];
            set => Session["ListaNodos2"] = value;
        }

        private List<string> ListaNodosDeRegistro2
        {
            get => (List<string>)Session["ListaNodosDeRegistro2"];
            set => Session["ListaNodosDeRegistro2"] = value;
        }

        private List<Panel> NodosNuevos2
        {
            get => (List<Panel>)Session["NodosNuevos2"];
            set => Session["NodosNuevos2"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await Tipo_HabitacionMap.ReadAllAsync()).ToList();

                ListaNodos2 = (await CaracteristicaMap.ReadAllAsync())
                    .Where(C => C.tipo.Equals("Habitación") || C.tipo.Equals("Ambas")).Select(C => C.nombre).ToList();

                ListaNodosDeRegistro = null;
                ListaNodosDeRegistro2 = null;
                NodosNuevos = null;
                NodosNuevos2 = null;

                tabs.Visible = false;
            }
            else
            {
                AgregarNodosNuevos();
                AgregarNodosNuevos2();
            }

            Sys.FillGridView(gvData, Lista);
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await Tipo_HabitacionMap.ReadAllAsync()).ToList();
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

                Tipo_Habitacion nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await Tipo_HabitacionMap.Create(nodo);
                }
                else
                {
                    nodo = LlenarNodo(Tipo_HabitacionMap.Read(hfId.Value));
                    await Tipo_HabitacionMap.Update(nodo);
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

                await Tipo_HabitacionMap.Delete(Tipo_HabitacionMap.Read(hfId.Value));

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

                Tipo_Habitacion registroSeleccionado = Tipo_HabitacionMap.Read(dataKey);

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

        protected Tipo_Habitacion LlenarNodo(Tipo_Habitacion nodo = null)
        {
            if (nodo == null)
            {
                return new Tipo_Habitacion()
                {
                    nombre = txtNombre.Text,
                    costo_por_noche = Convert.ToDecimal(txtPrecio.Text),
                    nivel = ddlNivel.Text,
                    capacidad = Convert.ToInt32(txtCantidad.Text)
                };
            }
            else
            {
                nodo.nombre = txtNombre.Text;
                nodo.nivel = ddlNivel.Text;
                nodo.costo_por_noche = Convert.ToDecimal(txtPrecio.Text);
                nodo.capacidad = Convert.ToInt32(txtCantidad.Text);
                nodo.camas = TiposDeCamasSeleccionados();
                nodo.caracteristicas = CaracteristicasSeleccionadas();

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

                if (!ValidarNuevosNodos2(ref message))
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

            if (ddlNivel.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el nivel de habitación");
                return false;
            }

            if (!Sys.EsNumero(txtPrecio.Text, true))
            {
                ShowMsgError("El precio debe ser númerico");
                return false;
            }
            else
            {
                if (Convert.ToDecimal(txtPrecio.Text) <= 0)
                {
                    ShowMsgError("El precio debe ser mayor que 0");
                    return false;
                }
            }

            if (!Sys.EsNumero(txtCantidad.Text))
            {
                ShowMsgError("La cantidad debe ser númerica");
                return false;
            }
            else
            {
                if (Convert.ToInt32(txtCantidad.Text) <= 0)
                {
                    ShowMsgError("La capacidad de personas debe ser mayor que 0");
                    return false;
                }
            }

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Tipo_Habitacion nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = Convert.ToString(nodo.nombre);

                // datos generales
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtCantidad.Text = Convert.ToString(nodo.capacidad);
                txtPrecio.Text = Convert.ToString(nodo.costo_por_noche);
                ddlNivel.SelectedValue = Convert.ToString(nodo.nivel);
                ListaNodosDeRegistro = nodo.camas;
                ListaNodosDeRegistro2 = nodo.caracteristicas;

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;
                txtNombre.Enabled = false;
                tabs.Visible = true;
                btnAgregarNodo.Enabled = ListaNodosDeRegistro.Count < ListaNodos.Count;
                btnAgregarNodo2.Enabled = ListaNodosDeRegistro2.Count < ListaNodos2.Count;

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Editar " + nodo.nombre;

                // llenamos el datarepeater
                Sys.FillRepeater(rpNodos, ListaNodosDeRegistro);
                Sys.FillRepeater(rpNodos2, ListaNodosDeRegistro2);
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
            txtCantidad.Text = "0";
            txtPrecio.Text = "0.00";
            ddlNivel.SelectedIndex = 0;
            phNodosNuevos.Controls.Clear();
            phNodosNuevos2.Controls.Clear();

            var lista = ListaNodosDeRegistro;

            if (lista != null)
            {
                ListaNodosDeRegistro.Clear();
            }

            if (NodosNuevos != null)
            {
                NodosNuevos.Clear();
            }

            if (ListaNodosDeRegistro2 != null)
            {
                ListaNodosDeRegistro2.Clear();
            }

            if (NodosNuevos2 != null)
            {
                NodosNuevos2.Clear();
            }

            btnEliminar.Visible = false;
            txtNombre.Enabled = true;
            tabs.Visible = false;
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

        #region "BOTONES NODOS"

        protected void btnAgregarNodo_Click(object sender, EventArgs e)
        {
            Button btnAgregarNodo = (Button)sender;

            if (string.IsNullOrEmpty(btnAgregarNodo.CommandName))
                return;

            string message = string.Empty;
            lblMensajeNodo.Text = string.Empty;

            if (btnAgregarNodo.CommandName.Equals("cama"))
            {
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
            else if(btnAgregarNodo.CommandName.Equals("caracteristica"))
            {
                if (!ValidarNuevosNodos2(ref message))
                {
                    lblMensajeNodo2.Text = message;
                    return;
                }

                if (NodosNuevos2 == null)
                    NodosNuevos2 = new List<Panel>();

                NodosNuevos2.Add(CrearNodo2(NodosNuevos2.Count));

                BloquearNodosAnteriores2();
                AgregarNodosNuevos2();

                btnAgregarNodo2.Enabled = ListaNodosDeRegistro2.Count + NodosNuevos2.Count < ListaNodos2.Count;

                upNodosNuevos2.Update();
            }
        }

        protected void btnRetirar_Click(object sender, EventArgs e)
        {
            Button btnRetirar = (Button)sender;

            if (string.IsNullOrEmpty(btnRetirar.CommandName))
                return;

            if (string.IsNullOrEmpty(btnRetirar.CommandArgument))
                return;

            if (btnRetirar.CommandName.Equals("cama"))
            {
                ListaNodosDeRegistro.Remove(btnRetirar.CommandArgument);
                Sys.FillRepeater(rpNodos, ListaNodosDeRegistro);

                if (NodosNuevos == null)
                    NodosNuevos = new List<Panel>();

                btnAgregarNodo.Enabled = ListaNodosDeRegistro.Count + NodosNuevos.Count < ListaNodos.Count;

                upNodosAnteriores.Update();
                upNodosNuevos.Update();
            }
            else if (btnRetirar.CommandName.Equals("caracteristica"))
            {
                ListaNodosDeRegistro2.Remove(btnRetirar.CommandArgument);
                Sys.FillRepeater(rpNodos2, ListaNodosDeRegistro2);

                if (NodosNuevos2 == null)
                    NodosNuevos2 = new List<Panel>();

                btnAgregarNodo2.Enabled = ListaNodosDeRegistro2.Count + NodosNuevos2.Count < ListaNodos2.Count;

                upNodosAnteriores2.Update();
                upNodosNuevos2.Update();
            }
        }

        #endregion

        #region "EVENTOS NODOS CAMAS"

        protected void rpNodos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                DropDownList ddlTipoCama = (DropDownList)e.Item.FindControl("ddlTipoCama");
                Sys.FillDropDown(ddlTipoCama, ListaNodos);

                string tipoCama = Convert.ToString(((KeyValuePair<string, int>)e.Item.DataItem).Key);

                tipoCama = string.IsNullOrEmpty(tipoCama) ? "-1" : tipoCama;
                ddlTipoCama.SelectedValue = tipoCama;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "METODOS NODOS CAMAS"

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
                ID = $"pnlNodo1_{idx.ToString()}",
                CssClass = "nodo1"
            };

            Panel pnlRow = new Panel() { CssClass = "form-row" };
            Panel pnlColumn1 = new Panel() { CssClass = "col-5 mb-3" };
            Panel pnlColumn2 = new Panel() { CssClass = "col-4 mb-3" };

            DropDownList ddlTipoCama = new DropDownList()
            {
                ID = $"{pnlNodo.ID}_ddlTipoCama_{idx.ToString()}",
                CssClass = "custom-select"
            };

            TextBox txtCantidadCamas = new TextBox()
            {
                ID = $"{pnlNodo.ID}_txtCantidadCamas_{idx.ToString()}",
                CssClass = "form-control",
                TextMode = TextBoxMode.Number,
                Text = "0"
            };

            Sys.FillDropDown(ddlTipoCama, NodosSinDuplicados(ListaNodosDeRegistro));

            pnlColumn1.Controls.Add(ddlTipoCama);
            pnlColumn2.Controls.Add(txtCantidadCamas);
            pnlRow.Controls.Add(pnlColumn1);
            pnlRow.Controls.Add(pnlColumn2);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private List<string> NodosSinDuplicados(IDictionary<string, int> nodosDeRegistro)
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(ListaNodos);

            // nodos ya creados
            List<string> nodos = nodosDeRegistro.Select(C => C.Key).ToList();

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos().Select(N => N.Key).ToList();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = ListaNodos.Intersect(nodos).ToList();
            nodosRepetidos = nodosRepetidos.Union(ListaNodos.Intersect(nodosNuevos)).ToList();

            // se obtienen los nodos que quedan en base a las dos listas anteriores
            listaNodos.RemoveAll(LN => nodosRepetidos.Exists(NR => NR.Equals(LN)));

            return listaNodos;
        }

        private IDictionary<string, int> ObtenerValoresNodos()
        {
            IDictionary<string, int> nodosNuevos = new Dictionary<string, int>();
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlTipoCama = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();
                TextBox txtCantidadCamas = pnlNuevoNodo.Controls[0].Controls[1].Controls.OfType<TextBox>().FirstOrDefault();

                nodosNuevos.Add(ddlTipoCama.Text, Convert.ToInt32(txtCantidadCamas.Text));
            }

            return nodosNuevos;
        }

        private bool ValidarNuevosNodos(ref string message)
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlTipoCama = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();
                TextBox txtCantidadCamas = pnlNuevoNodo.Controls[0].Controls[1].Controls.OfType<TextBox>().FirstOrDefault();

                if (ddlTipoCama == null || txtCantidadCamas == null)
                {
                    message = "No se encontro la información";
                    return false;
                }

                if (ddlTipoCama.SelectedIndex <= 0)
                {
                    message = "Seleccione el tipo de cama";
                    return false;
                }

                if (!Sys.EsNumero(txtCantidadCamas.Text))
                {
                    message = "La cantidad debe ser númerica";
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(txtCantidadCamas.Text) <= 0)
                    {
                        message = "La cantidad debe ser mayor a 0";
                        return false;
                    }
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

        private IDictionary<string, int> TiposDeCamasSeleccionados()
        {
            IDictionary<string, int> tiposCamas = new Dictionary<string, int>();

            foreach (KeyValuePair<string, int> valorViejo in ListaNodosDeRegistro)
            {
                tiposCamas.Add(valorViejo.Key, valorViejo.Value);
            }

            IDictionary<string, int> valoresNuevos = ObtenerValoresNodos();

            foreach (KeyValuePair<string, int> valorNuevo in valoresNuevos)
            {
                tiposCamas.Add(valorNuevo.Key, valorNuevo.Value);
            }

            return tiposCamas;
        }

        #endregion

        #region "METODOS NODOS CARACTERISTICAS"

        private void AgregarNodosNuevos2()
        {
            phNodosNuevos2.Controls.Clear();

            if (NodosNuevos2 == null || NodosNuevos2.Count <= 0)
                return;

            foreach (Panel pnlNodo in NodosNuevos2)
            {
                phNodosNuevos2.Controls.Add(pnlNodo);
            }
        }

        private Panel CrearNodo2(int idx)
        {
            Panel pnlNodo = new Panel()
            {
                ID = $"pnlNodo2_{idx.ToString()}",
                CssClass = "nodo"
            };

            Panel pnlRow = new Panel() { CssClass = "form-row" };
            Panel pnlColumn = new Panel() { CssClass = "col-12 mb-3" };

            DropDownList ddlCaracteristica = new DropDownList()
            {
                ID = $"{pnlNodo.ID}_ddlCaracteristica_{idx.ToString()}",
                CssClass = "custom-select"
            };

            Sys.FillDropDown(ddlCaracteristica, NodosSinDuplicados2(ListaNodosDeRegistro2));

            pnlColumn.Controls.Add(ddlCaracteristica);
            pnlRow.Controls.Add(pnlColumn);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private List<string> NodosSinDuplicados2(List<string> nodosDeRegistro)
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(ListaNodos2);

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos2();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = ListaNodos2.Intersect(nodosDeRegistro).ToList();
            nodosRepetidos = nodosRepetidos.Union(ListaNodos2.Intersect(nodosNuevos)).ToList();

            // se obtienen los nodos que quedan en base a las dos listas anteriores
            listaNodos.RemoveAll(LN => nodosRepetidos.Exists(NR => NR.Equals(LN)));

            return listaNodos;
        }

        private List<string> ObtenerValoresNodos2()
        {
            List<string> nodosNuevos = new List<string>();
            List<Panel> panelesNodosNuevos = phNodosNuevos2.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlCaracteristica = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                nodosNuevos.Add(ddlCaracteristica.Text);
            }

            return nodosNuevos;
        }

        private bool ValidarNuevosNodos2(ref string message)
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos2.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlCaracteristica = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                if (ddlCaracteristica == null)
                {
                    message = "No se encontro la información";
                    return false;
                }

                if (ddlCaracteristica.SelectedIndex <= 0)
                {
                    message = "Seleccione la característica";
                    return false;
                }
            }

            return true;
        }

        private void BloquearNodosAnteriores2()
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos2.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlCaracteristica = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                if (ddlCaracteristica == null)
                    continue;

                ddlCaracteristica.Enabled = false;
            }
        }

        private List<string> CaracteristicasSeleccionadas()
        {
            List<string> caracteristicasSeleccionadas = new List<string>();

            foreach (string valorViejo in ListaNodosDeRegistro2)
            {
                caracteristicasSeleccionadas.Add(valorViejo);
            }

            List<string> valoresNuevos = ObtenerValoresNodos2();

            foreach (string valorNuevo in valoresNuevos)
            {
                caracteristicasSeleccionadas.Add(valorNuevo);
            }

            return caracteristicasSeleccionadas;
        }

        #endregion
    }
}