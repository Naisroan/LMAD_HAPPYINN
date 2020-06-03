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
    public partial class hoteles : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "nombre" };

        #endregion

        #region "ATRIBUTOS"

        private List<Hotel> Lista
        {
            get => (List<Hotel>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        private List<Hotel> ListaFiltros
        {
            get => (List<Hotel>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        private string FiltroAplicado
        {
            get => (string)Session["FiltrosAplicados"];
            set => Session["FiltrosAplicados"] = value;
        }

        // aqui vendría cargandose todos los paises, estados y ciudades
        private List<Ciudad> Ciudades
        {
            get => (List<Ciudad>)Session["Ciudades"];
            set => Session["Ciudades"] = value;
        }

        private List<string> Paises => Ciudades.Select(C => C.pais).Distinct().ToList();

        private List<string> EstadosByPais(string pais)
            => Ciudades.Where(C => C.pais.Equals(pais)).Select(C => C.estado).Distinct().ToList();

        private List<string> MunicipiosByEstado(string estado)
            => Ciudades.Where(C => C.estado.Equals(estado)).Select(C => C.nombre).Distinct().ToList();

        #endregion

        #region "ATRIBUTOS HABITACIONES"

        // catalogo de habitaciones
        private List<string> TiposHabitacion
        {
            get => (List<string>)Session["TiposHabitacion"];
            set => Session["TiposHabitacion"] = value;
        }

        // habitaciones de un registro seleccionado
        private List<Habitacion_By_Hotel> Habitaciones
        {
            get => (List<Habitacion_By_Hotel>)Session["Habitacion_By_Hotel"];
            set => Session["Habitacion_By_Hotel"] = value;
        }

        // habitaciones nuevas agregadas del registro selecionado
        private List<Panel> HabitacionesNuevas
        {
            get => (List<Panel>)Session["HabitacionesNuevas"];
            set => Session["HabitacionesNuevas"] = value;
        }

        #endregion

        #region "ATRIBUTOS CARACTERISTICAS"

        // catalogo de caracteristicas
        private List<string> ListaCaracteristicas
        {
            get => (List<string>)Session["ListaCaracteristicas"];
            set => Session["ListaCaracteristicas"] = value;
        }

        // caracteristicas de un registro seleccionado
        private List<string> Caracteristicas
        {
            get => (List<string>)Session["Caracteristicas"];
            set => Session["Caracteristicas"] = value;
        }

        // caracteristicas nuevas agregadas del registro selecionado
        private List<Panel> CaracteristicasNuevas
        {
            get => (List<Panel>)Session["CaracteristicasNuevas"];
            set => Session["CaracteristicasNuevas"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Lista = (await HotelMap.ReadAllAsync()).ToList();
                ListaFiltros = new List<Hotel>(Lista);
                Ciudades = (await CiudadMap.ReadAllAsync()).ToList();
                TiposHabitacion = (await Tipo_HabitacionMap.ReadAllAsync()).Select(TH => TH.nombre).ToList();

                ListaCaracteristicas = (await CaracteristicaMap.ReadAllAsync())
                    .Where(C => C.tipo.Equals("Hotel") || C.tipo.Equals("Ambas")).Select(C => C.nombre).ToList();

                Sys.FillDropDown(ddlPais, Paises);
                Sys.FillDropDown(ddlPaisFiltro, Paises);
                Sys.FillDropDown(ddlEstado, null);
                Sys.FillDropDown(ddlCiudad, null);

                FiltroAplicado = string.Empty;
                MostrarFiltros("");

                tabs.Visible = false;
            }
            else
            {
                AgregarNodosNuevos();
                AgregarNodosNuevos2();
            }

            Sys.FillGridView(gvData, ListaFiltros);

            //if (!string.IsNullOrEmpty(FiltroAplicado))
            //{
            //    Sys.FillGridView(gvData, ListaFiltros);
            //}
            //else
            //{
            //    Sys.FillGridView(gvData, Lista);
            //}
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                Lista = (await HotelMap.ReadAllAsync()).ToList();
                ListaFiltros = new List<Hotel>(Lista);
                FiltroAplicado = string.Empty;

                MostrarFiltros("");

                Sys.FillGridView(gvData, ListaFiltros);

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

                Hotel nodo = null;

                if (nuevo)
                {
                    nodo = LlenarNodo();
                    await HotelMap.Create(nodo);
                }
                else
                {
                    nodo = LlenarNodo(HotelMap.Read(hfId.Value));
                    await HotelMap.Update(nodo, TiposDeHabitacionSeleccionados(nodo));
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

                await HotelMap.Delete(await HotelMap.ReadAsync(hfId.Value));

                Msg.Show(this, Msg.CaptionFinalizado, "Se ha eliminado el registro",
                    Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                ShowMsgError("Ha ocurrido un error: " + ex.Message);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string message = string.Empty;

            if (!ValidarFiltros(ddlTipoFiltro.SelectedValue, ref message))
            {
                Msg.Show(this, Msg.CaptionEspera, message, Msg.Tipo.Warning);
                return;
            }

            AplicarFiltros(ddlTipoFiltro.SelectedValue);
        }

        protected void btnOrdenarPor_Click(object sender, EventArgs e)
        {
            if (ddlTipoOrden.SelectedIndex <= 0)
            {
                Msg.Show(this, Msg.CaptionEspera, "Seleccione el orden", Msg.Tipo.Warning);
                return;
            }

            switch (ddlTipoOrden.SelectedValue)
            {
                case "porcentaje":
                    {
                        List<HotelResponse> hotelesResponse = new List<HotelResponse>();

                        foreach (GridViewRow row in gvData.Rows)
                        {
                            Hotel hotel = HotelMap.Read(Convert.ToString(gvData.DataKeys[row.RowIndex].Value));
                            Literal litPorcentajeUso = (Literal)row.Cells[2].FindControl("litPorcentajeUso");

                            float porcentaje = float.Parse(litPorcentajeUso.Text);

                            hotelesResponse.Add(HotelResponse.Parse(hotel, porcentaje));
                        }

                        hotelesResponse = hotelesResponse.OrderByDescending(H => H.porcentaje).ToList();

                        Sys.FillGridView(gvData, hotelesResponse);
                        upGridView.Update();

                        break;
                    }

                case "hotel":
                    {
                        ListaFiltros = ListaFiltros.OrderBy(H => H.nombre).ToList();

                        Sys.FillGridView(gvData, ListaFiltros);
                        upGridView.Update();

                        break;
                    }

                case "ciudad":
                    {
                        ListaFiltros = ListaFiltros.OrderBy(H => H.ciudad).ToList();

                        Sys.FillGridView(gvData, ListaFiltros);
                        upGridView.Update();

                        break;
                    }

                default:
                    {
                        break;
                    }
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

                    string nombreHotel = Convert.ToString(gridView.DataKeys[e.Row.RowIndex].Value);

                    Panel pnlPorcentaje = (Panel)e.Row.Cells[2].FindControl("pnlPorcentaje");
                    Literal litPorcentajeUso = (Literal)e.Row.Cells[2].FindControl("litPorcentajeUso");
                    Repeater rpHabitaciones = (Repeater)e.Row.Cells[3].FindControl("rpHabitaciones");

                    string strPorcentajeOcupacion = string.Empty;

                    if (!string.IsNullOrEmpty(FiltroAplicado))
                    {
                        switch(FiltroAplicado)
                        {
                            case "pais_fecha":
                                {
                                    DateTime fecha = DateTime.Parse(txtFecha.Text);

                                    strPorcentajeOcupacion = Convert.ToString(HotelMap.PorcentajeOcupacion(nombreHotel, fecha));
                                    break;
                                }
                        }
                    }
                    else
                    {
                        strPorcentajeOcupacion = Convert.ToString(HotelMap.PorcentajeOcupacion(nombreHotel, DateTime.Now));
                    }

                    pnlPorcentaje.Style.Add("width", $"{strPorcentajeOcupacion}%");
                    pnlPorcentaje.Attributes.Add("aria-valuenow", $"{strPorcentajeOcupacion}");
                    litPorcentajeUso.Text = $"{strPorcentajeOcupacion}";

                    Sys.FillRepeater(rpHabitaciones, Habitacion_By_HotelMap.ReadByHotel(nombreHotel).ToList());
                }
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void rpHabitaciones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Habitacion_By_Hotel habitacion = (Habitacion_By_Hotel)e.Item.DataItem;

            Literal litHabitacionesTotales = (Literal)e.Item.FindControl("litHabitacionesTotales");
            Literal litHabitacionesDisponibles = (Literal)e.Item.FindControl("litHabitacionesDisponibles");

            int habitacionesDisponibles = Habitacion_By_HotelMap.HabitacionesDisponibles(habitacion, DateTime.Now);

            if (!string.IsNullOrEmpty(FiltroAplicado))
            {
                if (FiltroAplicado.Equals("pais_fecha"))
                {
                    DateTime fecha = DateTime.Parse(txtFecha.Text);

                    habitacionesDisponibles = Habitacion_By_HotelMap.HabitacionesDisponibles(habitacion, fecha);
                }
            }

            litHabitacionesTotales.Text = Convert.ToString(habitacion.cantidad);
            litHabitacionesDisponibles.Text = Convert.ToString(habitacionesDisponibles);
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

                Hotel registroSeleccionado = HotelMap.Read(dataKey);

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

        protected void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlPais.SelectedIndex <= 0)
            {
                Sys.FillDropDown(ddlEstado, null);
                return;
            }

            string strPais = ddlPais.SelectedValue;

            Sys.FillDropDown(ddlEstado, EstadosByPais(strPais));
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEstado.SelectedIndex <= 0)
            {
                Sys.FillDropDown(ddlCiudad, null);
                return;
            }

            string strEstado = ddlEstado.SelectedValue;

            Sys.FillDropDown(ddlCiudad, MunicipiosByEstado(strEstado));
        }

        protected void ddlTipoFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlTipoFiltro.SelectedIndex <= 0)
            {
                return;
            }

            string strTipoFiltro = ddlTipoFiltro.SelectedValue;

            MostrarFiltros(strTipoFiltro);
        }

        #endregion

        #region "METODOS"

        protected Hotel LlenarNodo(Hotel nodo = null)
        {
            if (nodo == null)
            {
                return new Hotel()
                {
                    nombre = txtNombre.Text,
                    inicio_operaciones = Convert.ToDateTime(txtFechaIniOperaciones.Text),
                    ciudad = ddlCiudad.SelectedValue,
                    domicilio = txtDomicilio.Text,
                    num_pisos = Convert.ToInt32(txtCantPisos.Text),
                    usuario_registro = UsuarioLogeado.nick,
                    zona_turistica = chkZonaTuristica.Checked
                };
            }
            else
            {
                nodo.nombre = txtNombre.Text;
                nodo.inicio_operaciones = Convert.ToDateTime(txtFechaIniOperaciones.Text);
                nodo.ciudad = ddlCiudad.SelectedValue;
                nodo.domicilio = txtDomicilio.Text;
                nodo.num_pisos = Convert.ToInt32(txtCantPisos.Text);
                nodo.usuario_registro = UsuarioLogeado.nick;
                nodo.zona_turistica = chkZonaTuristica.Checked;
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

            if (string.IsNullOrEmpty(txtFechaIniOperaciones.Text))
            {
                ShowMsgError("Ingrese el incio de operaciones");
                return false;
            }

            if (Convert.ToInt32(txtCantPisos.Text) <= 0)
            {
                ShowMsgError("La cantidad de pisos debe ser mayor que 0");
                return false;
            }

            if (ddlPais.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el país donde se ubica");
                return false;
            }

            if (ddlEstado.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el estado donde se ubica");
                return false;
            }

            if (ddlCiudad.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione la ciudad donde se ubica");
                return false;
            }

            if (string.IsNullOrEmpty(txtDomicilio.Text))
            {
                ShowMsgError("Ingrese el domicilio");
                return false;
            }

            ShowMsgError(string.Empty, false);

            return true;
        }

        protected void LlenarCampos(Hotel nodo)
        {
            try
            {
                // llenamos el input oculto con el id del registro
                hfId.Value = Convert.ToString(nodo.nombre);

                // datos generales
                txtNombre.Text = Convert.ToString(nodo.nombre);
                txtFechaIniOperaciones.Text = Convert.ToString(nodo.inicio_operaciones.ToString("yyyy-MM-dd"));
                txtCantPisos.Text = Convert.ToString(nodo.num_pisos);
                chkZonaTuristica.Checked = nodo.zona_turistica;
                txtDomicilio.Text = Convert.ToString(nodo.domicilio);
                Habitaciones = Habitacion_By_HotelMap.ReadAll(nodo.nombre).ToList();
                Caracteristicas = nodo.caracteristicas;

                Ciudad ciudad = Ciudades.Where(C => C.nombre.Equals(nodo.ciudad)).FirstOrDefault();

                Sys.FillDropDown(ddlEstado, EstadosByPais(ciudad.pais));
                Sys.FillDropDown(ddlCiudad, MunicipiosByEstado(ciudad.estado));

                ddlPais.SelectedValue = Convert.ToString(ciudad.pais);
                ddlEstado.SelectedValue = Convert.ToString(ciudad.estado);
                ddlCiudad.SelectedValue = Convert.ToString(ciudad.nombre);

                // habilitamos el boton de eliminar
                btnEliminar.Visible = true;
                txtNombre.Enabled = false;
                tabs.Visible = true;
                btnAgregarNodo.Enabled = Habitaciones.Count < TiposHabitacion.Count;
                btnAgregarNodo2.Enabled = Caracteristicas.Count < ListaCaracteristicas.Count;

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Editar " + nodo.nombre;

                // llenamos el datarepeater
                Sys.FillRepeater(rpNodos, Habitaciones);
                Sys.FillRepeater(rpNodos2, Caracteristicas);
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
            txtFechaIniOperaciones.Text = string.Empty;
            txtCantPisos.Text = "1";
            chkZonaTuristica.Checked = false;
            ddlCiudad.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
            ddlPais.SelectedIndex = 0;
            txtDomicilio.Text = string.Empty;
            phNodosNuevos.Controls.Clear();
            phNodosNuevos2.Controls.Clear();

            if (Habitaciones != null)
            {
                Habitaciones.Clear();
            }

            if (HabitacionesNuevas != null)
            {
                HabitacionesNuevas.Clear();
            }

            if (Caracteristicas != null)
            {
                Caracteristicas.Clear();
            }

            if (CaracteristicasNuevas != null)
            {
                CaracteristicasNuevas.Clear();
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

        private void MostrarFiltros(string tipoFiltro)
        {
            // limpiamos y escondemos
            txtBusqueda.Text = string.Empty;
            txtBusqueda.Visible = false;

            txtFecha.Text = string.Empty;
            txtFecha.Visible = false;

            switch(tipoFiltro)
            {
                case "pais_fecha":
                    {
                        ddlPaisFiltro.Visible = true;
                        txtFecha.Visible = true;
                        break;
                    }

                default:
                    {
                        txtBusqueda.Visible = true;
                        break;
                    }
            }

            upToolbox.Update();
        }

        private bool ValidarFiltros(string tipoFiltro, ref string message)
        {
            switch (tipoFiltro)
            {
                case "pais_fecha":
                    {
                        if (ddlPaisFiltro.SelectedIndex <= 0)
                        {
                            message = "Seleccione un país";
                            return false;
                        }

                        if (!Sys.EsFecha(txtFecha.Text))
                        {
                            message = "Seleccione una fecha valida";
                            return false;
                        }

                        return true;
                    }

                default:
                    {
                        message = "Seleccione un filtro";
                        return false;
                    }
            }
        }

        private void AplicarFiltros(string tipoFiltro)
        {
            gvData.DataSource = null;
            FiltroAplicado = tipoFiltro;

            switch (tipoFiltro)
            {
                case "pais_fecha":
                    {
                        ListaFiltros = HotelMap.ReadAllByPais(ddlPaisFiltro.SelectedValue);
                        Sys.FillGridView(gvData, ListaFiltros);
                        break;
                    }

                default:
                    {
                        Sys.FillGridView(gvData, Lista);
                        break;
                    }
            }

            upToolbox.Update();
            upGridView.Update();
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

            if (btnAgregarNodo.CommandName.Equals("tipo_habitacion"))
            {
                if (!ValidarNuevosNodos(ref message))
                {
                    lblMensajeNodo.Text = message;
                    return;
                }

                if (HabitacionesNuevas == null)
                    HabitacionesNuevas = new List<Panel>();

                HabitacionesNuevas.Add(CrearNodo(HabitacionesNuevas.Count));

                BloquearNodosAnteriores();
                AgregarNodosNuevos();

                btnAgregarNodo.Enabled = Habitaciones.Count + HabitacionesNuevas.Count < TiposHabitacion.Count;

                upNodosNuevos.Update();
            }
            else if (btnAgregarNodo.CommandName.Equals("caracteristica"))
            {
                if (!ValidarNuevosNodos2(ref message))
                {
                    lblMensajeNodo2.Text = message;
                    return;
                }

                if (CaracteristicasNuevas == null)
                    CaracteristicasNuevas = new List<Panel>();

                CaracteristicasNuevas.Add(CrearNodo2(CaracteristicasNuevas.Count));

                BloquearNodosAnteriores2();
                AgregarNodosNuevos2();

                btnAgregarNodo2.Enabled = Caracteristicas.Count + CaracteristicasNuevas.Count < ListaCaracteristicas.Count;

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

            if (btnRetirar.CommandName.Equals("tipo_habitacion"))
            {
                Habitaciones.Remove(Habitaciones.FirstOrDefault(H => H.tipo_habitacion.Equals(btnRetirar.CommandArgument)));
                Sys.FillRepeater(rpNodos, Habitaciones);

                if (HabitacionesNuevas == null)
                    HabitacionesNuevas = new List<Panel>();

                btnAgregarNodo.Enabled = Habitaciones.Count + HabitacionesNuevas.Count < TiposHabitacion.Count;

                upNodosAnteriores.Update();
                upNodosNuevos.Update();
            }
            else if (btnRetirar.CommandName.Equals("caracteristica"))
            {
                Caracteristicas.Remove(btnRetirar.CommandArgument);
                Sys.FillRepeater(rpNodos2, Caracteristicas);

                if (CaracteristicasNuevas == null)
                    CaracteristicasNuevas = new List<Panel>();

                btnAgregarNodo2.Enabled = Caracteristicas.Count + CaracteristicasNuevas.Count < ListaCaracteristicas.Count;

                upNodosAnteriores2.Update();
                upNodosNuevos2.Update();
            }
        }

        #endregion

        #region "EVENTOS Y METODOS HABITACIONES"

        protected void rpNodos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                DropDownList ddlTipoHabitacion = (DropDownList)e.Item.FindControl("ddlTipoHabitacion");
                Sys.FillDropDown(ddlTipoHabitacion, TiposHabitacion);

                string tipo = Convert.ToString(((Habitacion_By_Hotel)e.Item.DataItem).tipo_habitacion);

                tipo = string.IsNullOrEmpty(tipo) ? "-1" : tipo;
                ddlTipoHabitacion.SelectedValue = tipo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AgregarNodosNuevos()
        {
            phNodosNuevos.Controls.Clear();

            if (HabitacionesNuevas == null || HabitacionesNuevas.Count <= 0)
                return;

            foreach (Panel pnlNodo in HabitacionesNuevas)
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

            DropDownList ddlTipoHabitacion = new DropDownList()
            {
                ID = $"{pnlNodo.ID}_ddlTipoHabitacion_{idx.ToString()}",
                CssClass = "custom-select"
            };

            TextBox txtCantidad = new TextBox()
            {
                ID = $"{pnlNodo.ID}_txtCantidad_{idx.ToString()}",
                CssClass = "form-control",
                TextMode = TextBoxMode.Number,
                Text = "0"
            };

            Sys.FillDropDown(ddlTipoHabitacion, NodosSinDuplicados(Habitaciones.Select(H => H.tipo_habitacion).ToList()));

            pnlColumn1.Controls.Add(ddlTipoHabitacion);
            pnlColumn2.Controls.Add(txtCantidad);
            pnlRow.Controls.Add(pnlColumn1);
            pnlRow.Controls.Add(pnlColumn2);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private List<string> NodosSinDuplicados(List<string> nodosDeRegistro)
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(TiposHabitacion);

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos().Select(N => N.Key).ToList();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = TiposHabitacion.Intersect(nodosDeRegistro).ToList();
            nodosRepetidos = nodosRepetidos.Union(TiposHabitacion.Intersect(nodosNuevos)).ToList();

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
                DropDownList ddlTipoHabitacion = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();
                TextBox txtCantidad = pnlNuevoNodo.Controls[0].Controls[1].Controls.OfType<TextBox>().FirstOrDefault();

                nodosNuevos.Add(ddlTipoHabitacion.Text, Convert.ToInt32(txtCantidad.Text));
            }

            return nodosNuevos;
        }

        private bool ValidarNuevosNodos(ref string message)
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlTipoHabitacion = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();
                TextBox txtCantidad = pnlNuevoNodo.Controls[0].Controls[1].Controls.OfType<TextBox>().FirstOrDefault();

                if (ddlTipoHabitacion == null || ddlTipoHabitacion == null)
                {
                    message = "No se encontro la información";
                    return false;
                }

                if (ddlTipoHabitacion.SelectedIndex <= 0)
                {
                    message = "Seleccione el tipo de habitación";
                    return false;
                }

                if (!Sys.EsNumero(txtCantidad.Text))
                {
                    message = "La cantidad debe ser númerica";
                    return false;
                }
                else
                {
                    if (Convert.ToInt32(txtCantidad.Text) <= 0)
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
                DropDownList ddlTipoHabitacion = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();
                TextBox txtCantidad = pnlNuevoNodo.Controls[0].Controls[1].Controls.OfType<TextBox>().FirstOrDefault();

                if (ddlTipoHabitacion == null || txtCantidad == null)
                    continue;

                ddlTipoHabitacion.Enabled = false;
                txtCantidad.Enabled = false;
            }
        }

        private List<Habitacion_By_Hotel> TiposDeHabitacionSeleccionados(Hotel hotel)
        {
            List<Habitacion_By_Hotel> tiposHabitacion = new List<Habitacion_By_Hotel>();

            foreach (Habitacion_By_Hotel valorViejo in Habitaciones)
            {
                tiposHabitacion.Add(valorViejo);
            }

            IDictionary<string, int> valoresNuevos = ObtenerValoresNodos();

            foreach (KeyValuePair<string, int> valorNuevo in valoresNuevos)
            {
                tiposHabitacion.Add(new Habitacion_By_Hotel()
                {
                    nombre = hotel.nombre,
                    tipo_habitacion = valorNuevo.Key,
                    cantidad = Convert.ToInt32(valorNuevo.Value)
                });
            }

            return tiposHabitacion;
        }

        #endregion

        #region "EVENTOS Y METODOS CARACTERISTICAS"

        private void AgregarNodosNuevos2()
        {
            phNodosNuevos2.Controls.Clear();

            if (CaracteristicasNuevas == null || CaracteristicasNuevas.Count <= 0)
                return;

            foreach (Panel pnlNodo in CaracteristicasNuevas)
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

            Sys.FillDropDown(ddlCaracteristica, NodosSinDuplicados2(Caracteristicas));

            pnlColumn.Controls.Add(ddlCaracteristica);
            pnlRow.Controls.Add(pnlColumn);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private List<string> NodosSinDuplicados2(List<string> nodosDeRegistro)
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(ListaCaracteristicas);

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos2();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = ListaCaracteristicas.Intersect(nodosDeRegistro).ToList();
            nodosRepetidos = nodosRepetidos.Union(ListaCaracteristicas.Intersect(nodosNuevos)).ToList();

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

            foreach (string valorViejo in Caracteristicas)
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

    public class HotelResponse
    {
        public string nombre { get; set; }

        public string ciudad { get; set; }

        public float porcentaje { get; set; }

        public static HotelResponse Parse(Hotel hotel, float porcentaje)
        {
            return new HotelResponse()
            {
                nombre = hotel.nombre,
                ciudad = hotel.ciudad,
                porcentaje = porcentaje
            };
        }
    }
}