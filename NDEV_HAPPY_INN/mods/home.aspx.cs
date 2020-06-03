using NDEV_HAPPY_INN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Data;
using System.Text;

namespace NDEV_HAPPY_INN.mods
{
    public partial class home : HappyInnPage
    {
        #region "CONSTANTES"

        protected static readonly string[] DataKeyNames = { "nombre" };

        // ignorar esto, lo uso para establecer imagenes aleatorias
        protected static int LastSeed = 0;

        #endregion

        #region "ATRIBUTOS"

        // ignorar esto, son id de imagenes de una página de internet
        protected static List<string> Imagenes = new List<string>()
        {
            "1031",
            "1048",
            "1047",
            "1052",
            "1051",
            "1052",
            "1029",
            "1033"
        };

        private List<Hotel> Lista
        {
            get => (List<Hotel>)Session["Lista"];
            set => Session["Lista"] = value;
        }

        private List<Cliente> Clientes
        {
            get => (List<Cliente>)Session["Clientes"];
            set => Session["Clientes"] = value;
        }

        private List<Cliente> HabitacionesPorHotel
        {
            get => (List<Cliente>)Session["HabitacionesPorHotel"];
            set => Session["HabitacionesPorHotel"] = value;
        }

        private List<Reservacion> Reservaciones
        {
            get => (List<Reservacion>)Session["Reservaciones"];
            set => Session["Reservaciones"] = value;
        }

        private Factura FacturaPendiente
        {
            get => (Factura)Session["Factura"];
            set => Session["Factura"] = value;
        }

        #endregion

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var factura = FacturaPendiente;

                if (factura != null)
                {
                    Descarga descarga = new Descarga(factura.Nombre, ".txt", Encoding.Default.GetBytes(factura.Contenido));
                    descarga.DescargarArchivo(this, HttpContext.Current);
                }

                FacturaPendiente = null;

                if (UsuarioLogeado.is_admin.GetValueOrDefault(false))
                {
                    Lista = (await HotelMap.ReadAllAsync()).ToList();

                    Reservaciones = 
                        (await ReservacionMap.ReadAllAsync()).Where(R => !R.cancelada && (!R.check_out || !R.check_in)).ToList();
                }
                else
                {
                    Lista = (await HotelMap.ReadByUsuarioAsync(UsuarioLogeado.nick)).ToList();

                    Reservaciones = 
                        (await ReservacionMap.ReadAllByUsuarioAsync(UsuarioLogeado.nick)).Where(R => !R.cancelada && (!R.check_out || !R.check_in)).ToList();
                }

                Clientes = (await ClienteMap.ReadAllAsync()).ToList();
                ListaNodos = (await Servicio_ExtraMap.ReadAllAsync()).Select(S => S.nombre).ToList();

                Sys.FillRepeater(rpHoteles, Lista);
                Sys.FillRepeater(rpReservaciones, Reservaciones);
                Sys.FillDropDown(ddlCliente, Clientes, "id", "nombre");
                Sys.FillDropDown(ddlMetodoPago, App.MetodosPagos);
                Sys.FillDropDown(ddlMetodoPagoCheckOut, App.MetodosPagos);
            }
            else
            {
                AgregarNodosNuevos();
            }

            LastSeed = 0;
        }

        #region "BOTONES"

        protected async void btnRecargar_Click(object sender, EventArgs e)
        {
            try
            {
                if (UsuarioLogeado.is_admin.GetValueOrDefault(false))
                {
                    Lista = (await HotelMap.ReadAllAsync()).ToList();

                    Reservaciones =
                        (await ReservacionMap.ReadAllAsync()).Where(R => !R.cancelada && (!R.check_out || !R.check_in)).ToList();
                }
                else
                {
                    Lista = (await HotelMap.ReadByUsuarioAsync(UsuarioLogeado.nick)).ToList();

                    Reservaciones =
                        (await ReservacionMap.ReadAllByUsuarioAsync(UsuarioLogeado.nick)).Where(R => !R.cancelada && (!R.check_out || !R.check_in)).ToList();
                }

                Sys.FillRepeater(rpHoteles, Lista);
                Sys.FillRepeater(rpReservaciones, Reservaciones);

                upDataRepeater.Update();
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        protected void btnReservacion_Click(object sender, EventArgs e)
        {
            try
            {
                Button btnReservacion = (Button)sender;
                Hotel hotel = HotelMap.Read(btnReservacion.CommandArgument);

                LimpiarCampos();
                LlenarCampos(hotel);

                Sys.ShowModal(this, upFormulario);
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionInfo, $"{Msg.CaptionError} {ex.Message}", Msg.Tipo.Error);
            }
        }

        protected void btnSeleccionarHabitacion_Click(object sender, EventArgs e)
        {
            Button btnSeleccionarHabitacion = (Button)sender;
            IEnumerable<RepeaterItem> rpItems = rpHabitacionesHotel.Controls.OfType<RepeaterItem>();

            foreach (RepeaterItem rpItem in rpItems)
            {
                IEnumerable<Panel> pnlHabitaciones = rpItem.Controls.OfType<Panel>();

                foreach (Panel pnlHabitacion in pnlHabitaciones)
                {
                    pnlHabitacion.CssClass = pnlHabitacion.CssClass.Replace(" active", "");

                    if (pnlHabitacion.GroupingText.Equals(btnSeleccionarHabitacion.CommandArgument))
                    {
                        if (!pnlHabitacion.CssClass.Contains("active"))
                        {
                            pnlHabitacion.CssClass += " active";
                        }
                    }
                }
            }
        }

        protected async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                Reservacion nodo = LlenarNodo();
                await ReservacionMap.Create(nodo);

                Msg.Show(this, Msg.CaptionFinalizado, "Se ha reservado la habitación",
                    Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
            }
            catch (Exception ex)
            {
                ShowMsgError("Ha ocurrido un error: " + ex.Message);
            }
        }

        protected async void btnCheckInn_Click(object sender, EventArgs e)
        {
            Button btnCheckInn = (Button)sender;
            Guid idReservacion = Guid.Parse(btnCheckInn.CommandArgument);

            Reservacion reservacion = ReservacionMap.Read(idReservacion);

            if (btnCheckInn.CommandName.Equals("check_in"))
            {
                reservacion.check_in = true;
                reservacion.servicios_extras = new List<string>();
                reservacion.medio_pago = "";
                reservacion.monto_pago = 0;

                await ReservacionMap.Update(reservacion);

                Msg.Show(this, Msg.CaptionFinalizado, "Se ha marcado que el cliente a entrado a su habitación",
                    Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
            }
            else if (btnCheckInn.CommandName.Equals("check_out"))
            {
                hfIdCheckOut.Value = "-1";
                hfTotalOriginal.Value = "0";
                lblTituloCheckOut.Text = "Finalizar reservación";
                txtTotal.Text = "0.00";
                ddlMetodoPagoCheckOut.SelectedIndex = 0;
                lblAnticipo.Text = $"$ 0.00 de anticipo";
                lblCantidadPersonas.Text = "0 personas en habitación";
                lblPrecioHabitacion.Text = $"$ 0.00 / noche de habitación";
                lblDias.Text = $"0 noches reservadas";

                if (NodosNuevos != null)
                    NodosNuevos.Clear();

                lblMensajeNodo.Text = string.Empty;
                phNodosNuevos.Controls.Clear();

                Tipo_Habitacion habitacion = Tipo_HabitacionMap.Read(reservacion.tipo_habitacion);

                double cantDias = (reservacion.fecha_fin - reservacion.fecha_ini).TotalDays;
                decimal total = Convert.ToDecimal(cantDias) * habitacion.costo_por_noche * reservacion.cant_personas;
                total -= reservacion.monto_anticipo;

                hfIdCheckOut.Value = Convert.ToString(reservacion.id);
                txtTotal.Text = Convert.ToString(total);
                hfTotalOriginal.Value = txtTotal.Text;

                lblAnticipo.Text = $"$ {Convert.ToString(reservacion.monto_anticipo)} de anticipo";
                lblCantidadPersonas.Text = $"Habitación para {Convert.ToString(reservacion.cant_personas)} personas";
                lblPrecioHabitacion.Text = $"$ {Convert.ToString(habitacion.costo_por_noche)} / noche (costo de habitación)";
                lblDias.Text = $"{cantDias} noches reservadas";

                ShowMsgErrorCheckOut(string.Empty, false);
                Sys.ShowModal(this, upFormularioCheckOut);

                upFormularioCheckOut.Update();
            }
        }

        protected async void btnCheckOut_Click(object sender, EventArgs e)
        {
            if (ddlMetodoPagoCheckOut.SelectedIndex <= 0)
            {
                ShowMsgErrorCheckOut("Seleccione el método de pago");
                return;
            }

            Reservacion reservacion = ReservacionMap.Read(Guid.Parse(hfIdCheckOut.Value));
            List<Servicio_Extra> serviciosExtra = ServiciosExtras();

            reservacion.monto_pago = Convert.ToDecimal(txtTotal.Text);
            reservacion.medio_pago = ddlMetodoPagoCheckOut.Text;
            reservacion.servicios_extras = serviciosExtra.Select(SE => SE.nombre).ToList();
            reservacion.check_out = true;

            await ReservacionMap.Update(reservacion);

            string nombreFactura = $"HappyInn_Factura_{Convert.ToString(reservacion.id)}";

            FacturaPendiente = new Factura()
            {
                Nombre = nombreFactura,
                Contenido = CrearFactura(nombreFactura, reservacion, serviciosExtra)
            };

            Msg.Show(this, Msg.CaptionFinalizado, "Reservación finalizada, se ha liberado la habitación",
                Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
        }

        protected async void btnCancelarReservacion_Click(object sender, EventArgs e)
        {
            Button btnCancelarReservacion = (Button)sender;
            TextBox txtPasswordAdmin = (TextBox)btnCancelarReservacion.Parent.FindControl("txtPasswordAdmin");

            if (string.IsNullOrEmpty(txtPasswordAdmin.Text))
            {
                Msg.Show(this, Msg.CaptionEspera, "Ingrese la clave del administrador", Msg.Tipo.Warning);
                return;
            }

            if (!txtPasswordAdmin.Text.Equals(UsuarioMap.ObtenerClaveAdmin()))
            {
                Msg.Show(this, Msg.CaptionError, "Clave incorrecta, intentelo de nuevo", Msg.Tipo.Error);
                return;
            }

            Guid idReservacion = Guid.Parse(btnCancelarReservacion.CommandArgument);
            Reservacion reservacion = ReservacionMap.Read(idReservacion);

            reservacion.cancelada = true;
            reservacion.servicios_extras = new List<string>();
            reservacion.medio_pago = "";
            reservacion.monto_pago = 0;

            await ReservacionMap.Update(reservacion);

            Msg.Show(this, Msg.CaptionFinalizado, "Se ha cancelado la reservación y quedó libre la habitación",
                Msg.Tipo.Success, Msg.BotonFinalizado, HttpContext.Current.Request.Url.AbsolutePath);
        }

        #endregion

        #region "EVENTOS"

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

        protected void rpHoteles_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                Hotel hotel = (Hotel)e.Item.DataItem;

                Image imgHotel = (Image)e.Item.FindControl("imgHotel");
                Repeater rpCaracteristicas = (Repeater)e.Item.FindControl("rpCaracteristicas");
                //Label lblHabitacionesDisponibles = (Label)e.Item.FindControl("lblHabitacionesDisponibles");
                Button btnReservacion = (Button)e.Item.FindControl("btnReservacion");

                string randomImage = GetRandomImage();
                //int cantHabDisponibles = HotelMap.HabitacionesDisponibles(hotel);

                imgHotel.ImageUrl = randomImage;

                //lblHabitacionesDisponibles.Text = Convert.ToString(cantHabDisponibles);
                //lblHabitacionesDisponibles.CssClass += cantHabDisponibles > 0 ? " badge-info" : " badge-danger";
                //btnReservacion.Enabled = cantHabDisponibles > 0;

                Sys.FillRepeater(rpCaracteristicas, hotel.caracteristicas);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void rpHabitacionesHotel_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Habitacion_By_Hotel habitacionHotel = (Habitacion_By_Hotel)e.Item.DataItem;
            Tipo_Habitacion habitacion = Tipo_HabitacionMap.Read(habitacionHotel.tipo_habitacion);

            Repeater rpCaracteristicas = (Repeater)e.Item.FindControl("rpCaracteristicasHabitacion");
            Repeater rpCamas = (Repeater)e.Item.FindControl("rpCamasHabitacion");
            Literal litPrecio = (Literal)e.Item.FindControl("litPrecio");
            Literal litCapacidad = (Literal)e.Item.FindControl("litCapacidad");
            Literal litDescripcion = (Literal)e.Item.FindControl("litDescripcion");
            // Button btnSeleccionarHabitacion = (Button)e.Item.FindControl("btnSeleccionarHabitacion");

            litPrecio.Text = Convert.ToString(habitacion.costo_por_noche);
            litCapacidad.Text = Convert.ToString(habitacion.capacidad);
            litDescripcion.Text = habitacion.nivel;

            Sys.FillRepeater(rpCaracteristicas, habitacion.caracteristicas);
            Sys.FillRepeater(rpCamas, habitacion.camas);
        }

        protected void rpReservaciones_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Reservacion reservacion = (Reservacion)e.Item.DataItem;

            Literal litNombreCliente = (Literal)e.Item.FindControl("litNombreCliente");
            Panel pnlGroupCancelarReservacion = (Panel)e.Item.FindControl("pnlGroupCancelarReservacion");

            litNombreCliente.Text = ClienteMap.NombreCompleto(ClienteMap.Read(reservacion.id_cliente));
            pnlGroupCancelarReservacion.Visible = !reservacion.check_in;
        }

        #endregion

        #region "METODOS"

        protected Reservacion LlenarNodo(Reservacion nodo = null)
        {
            if (nodo == null)
            {
                return new Reservacion()
                {
                    id = Guid.NewGuid(),
                    id_cliente = Guid.Parse(ddlCliente.SelectedValue),
                    nombre_hotel = hfId.Value,
                    cant_personas = Convert.ToInt32(txtCantPersonas.Text),
                    check_in = chkIn.Checked,
                    check_out = false,
                    cancelada = false,
                    fecha_ini = DateTime.Parse(txtFechaInicio.Text),
                    fecha_fin = DateTime.Parse(txtFechaFin.Text),
                    medio_pago_anticipo = ddlMetodoPago.Text,
                    monto_anticipo = Convert.ToDecimal(txtAnticipo.Text),
                    fecha_registro = DateTime.Now,
                    tipo_habitacion = ObtenerTipoHabitacionSeleccionda()
                };
            }
            else
            {
                return nodo;
            }
        }

        protected bool ValidarCampos()
        {
            if (string.IsNullOrEmpty(hfId.Value))
            {
                ShowMsgError("Hubo un problema al obtener el hotel");
                return false;
            }

            string tipoHabitacionSeleccionada = ObtenerTipoHabitacionSeleccionda();

            if (string.IsNullOrEmpty(tipoHabitacionSeleccionada))
            {
                ShowMsgError("Seleccione la habitación");
                return false;
            }

            if (!Sys.EsNumero(txtCantPersonas.Text))
            {
                ShowMsgError("La cantidad de personas debe ser númerico");
                return false;
            }
            else
            {
                int cantidadPresonas = Convert.ToInt32(txtCantPersonas.Text);

                if (cantidadPresonas <= 0)
                {
                    ShowMsgError("La cantidad de personas tiene que ser mayor a 0");
                    return false;
                }
                else
                {
                    Tipo_Habitacion habitacion = Tipo_HabitacionMap.Read(tipoHabitacionSeleccionada);

                    if (cantidadPresonas > habitacion.capacidad)
                    {
                        ShowMsgError("La cantidad de personas excede la capacidad de la habitación");
                        return false;
                    }
                }
            }

            if (ddlCliente.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el cliente");
                return false;
            }

            if (ddlMetodoPago.SelectedIndex <= 0)
            {
                ShowMsgError("Seleccione el método de pago");
                return false;
            }

            if (!Sys.EsNumero(txtAnticipo.Text, true))
            {
                ShowMsgError("El anticipo tiene que ser númerico");
                return false;
            }
            else
            {
                if (Convert.ToDecimal(txtAnticipo.Text) <= 0)
                {
                    ShowMsgError("El anticipo tiene que ser mayor a 0");
                    return false;
                }
            }

            if (!Sys.EsFecha(txtFechaInicio.Text))
            {
                ShowMsgError("Ingrese la fecha inicial de reservación correctamente");
                return false;
            }

            if (!Sys.EsFecha(txtFechaFin.Text))
            {
                ShowMsgError("Ingrese la fecha final de reservación correctamente");
                return false;
            }

            DateTime fechaInicial = DateTime.Parse(txtFechaInicio.Text);
            DateTime fechaFinal = DateTime.Parse(txtFechaFin.Text);

            if (fechaInicial.Date < DateTime.Now.Date)
            {
                ShowMsgError("La fecha inicial debe ser mayor o igual a la fecha actual");
                return false;
            }

            if (fechaFinal.Date <= fechaInicial)
            {
                ShowMsgError("La fecha final debe ser mayor a la fecha inicial");
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
                hfId.Value = nodo.nombre;

                // grid de habitaciones
                List<Habitacion_By_Hotel> habitaciones = Habitacion_By_HotelMap.ReadByHotel(nodo.nombre).ToList();
                Sys.FillRepeater(rpHabitacionesHotel, habitaciones);

                // establecemos el titulo del formulario
                lblTituloFormulario.Text = "Nueva reservación en Happy Inn - " + nodo.nombre;
            }
            catch (Exception ex)
            {
                Msg.Show(this, Msg.CaptionError, ex.Message, Msg.Tipo.Error);
            }
        }

        protected void LimpiarCampos()
        {
            hfId.Value = "-1";

            ddlCliente.SelectedIndex = 0;
            txtAnticipo.Text = "0.00";
            txtFechaFin.Text = string.Empty;
            txtFechaInicio.Text = string.Empty;
            txtCantPersonas.Text = "0";
            ddlMetodoPago.SelectedIndex = 0;

            if (HabitacionesPorHotel != null)
            {
                HabitacionesPorHotel.Clear();
            }

            lblTituloFormulario.Text = "Nueva reservación";

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

        private void ShowMsgErrorCheckOut(string errorMessage, bool show = true)
        {
            upMensajeCheckOut.Visible = show;
            lblMensajeCheckOut.Text = errorMessage;

            if (!show)
            {
                lblMensajeCheckOut.Text = string.Empty;
            }

            upFormularioCheckOut.Update();
        }

        protected string GetRandomImage()
        {
            int seed = new Random(LastSeed).Next();
            int idxRandom = new Random(seed + new Random().Next()).Next(0, Imagenes.Count - 1);
            string imageRandom = Imagenes.ElementAt(idxRandom);

            LastSeed = seed;
            LastSeed += 5;

            return $"https://picsum.photos/id/{imageRandom}/400/200?random=1";
        }

        protected string ObtenerTipoHabitacionSeleccionda()
        {
            string tipoHabitacion = string.Empty;

            IEnumerable<RepeaterItem> rpItems = rpHabitacionesHotel.Controls.OfType<RepeaterItem>();

            foreach (RepeaterItem rpItem in rpItems)
            {
                IEnumerable<Panel> pnlHabitaciones = rpItem.Controls.OfType<Panel>();

                foreach (Panel pnlHabitacion in pnlHabitaciones)
                {
                    if (pnlHabitacion.CssClass.Contains("active"))
                    {
                        tipoHabitacion = pnlHabitacion.GroupingText;
                        break;
                    }
                }
            }

            return tipoHabitacion;
        }

        protected string CrearFactura(string nombreFactura, Reservacion reservacion, List<Servicio_Extra> servicioExtras)
        {
            string factura = string.Empty;

            Cliente cliente = ClienteMap.Read(reservacion.id_cliente);
            Tipo_Habitacion habitacion = Tipo_HabitacionMap.Read(reservacion.tipo_habitacion);

            factura += $"{nombreFactura},,";
            factura += $"Cliente:\t\t\t{ClienteMap.NombreCompleto(cliente)},,";
            factura += $"Hotel:\t\t\t\t{reservacion.nombre_hotel},,";
            factura += $"Habitación:\t\t\t{reservacion.tipo_habitacion},";
            factura += $"\t\t\t\t$ {Convert.ToString(habitacion.costo_por_noche)} c/noche,,";
            factura += $"# de personas:\t\t\t{Convert.ToString(reservacion.cant_personas)},";
            factura += $"\t\t\t\t$ {Convert.ToString(habitacion.costo_por_noche * reservacion.cant_personas)} en total,,";
            factura += $"# de noches:\t\t\t{Convert.ToString((reservacion.fecha_fin - reservacion.fecha_ini).TotalDays)},";
            factura += $"\t\t\t\t$ {Convert.ToString((int)(reservacion.fecha_fin - reservacion.fecha_ini).TotalDays * habitacion.costo_por_noche * reservacion.cant_personas)} en total,,";
            factura += $",";
            factura += $"Los servicios extra:";
            factura += $",,";

            factura += $"------------------------------------------------------------,,";

            foreach (Servicio_Extra servicio in servicioExtras)
            {
                factura += $"{servicio.nombre}\t\t\t$ {Convert.ToString(servicio.precio)},";
            }

            factura += $",\t\t\t\t$ {Convert.ToString(servicioExtras.Select(S => S.precio).Sum())} en total,";

            factura += $",------------------------------------------------------------,,";

            factura += $",";
            factura += $"Anticipo de: \t\t\t$ {reservacion.monto_anticipo},";
            factura += $"Metodo: \t\t\t{reservacion.medio_pago_anticipo},,";
            factura += $"Total de: \t\t\t$ {reservacion.monto_pago},";
            factura += $"Metodo: \t\t\t{reservacion.medio_pago},";

            factura = factura.Replace(",", Environment.NewLine);

            return factura;
        }

        #endregion

        #region "ATRIBUTOS NODOS SERVICIOS EXTRAS"

        protected List<string> ListaNodos
        {
            get => (List<string>)Session["ListaNodos"];
            set => Session["ListaNodos"] = value;
        }

        private List<Panel> NodosNuevos
        {
            get => (List<Panel>)Session["NodosNuevos"];
            set => Session["NodosNuevos"] = value;
        }

        #endregion

        #region "BOTONES NODOS SERVICIOS EXTRAS"

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

            btnAgregarNodo.Enabled = NodosNuevos.Count < ListaNodos.Count;

            if (NodosNuevos.Count > 1)
            {
                hfTotalOriginal.Value = txtTotal.Text;
                upFormularioCheckOut.Update();
            }

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

        #region "METODOS NODOS SERVICIOS EXTRAS"

        private void AgregarNodosNuevos()
        {
            phNodosNuevos.Controls.Clear();

            if (NodosNuevos == null || NodosNuevos.Count <= 0)
                return;

            foreach (Panel pnlNodo in NodosNuevos)
            {
                DropDownList ddlServicioExtra = pnlNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                ddlServicioExtra.SelectedIndexChanged += ddlServicioExtra_SelectedIndexChanged;

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

            DropDownList ddlServicioExtra = new DropDownList()
            {
                ID = $"{pnlNodo.ID}_ddlServicioExtra_{idx.ToString()}",
                CssClass = "custom-select",
                AutoPostBack = true
            };

            ddlServicioExtra.SelectedIndexChanged += ddlServicioExtra_SelectedIndexChanged;

            Sys.FillDropDown(ddlServicioExtra, NodosSinDuplicados());

            pnlColumn.Controls.Add(ddlServicioExtra);
            pnlRow.Controls.Add(pnlColumn);
            pnlNodo.Controls.Add(pnlRow);

            return pnlNodo;
        }

        private void ddlServicioExtra_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlServicioExtra = (DropDownList)sender;

            decimal total = Convert.ToDecimal(hfTotalOriginal.Value);

            if (ddlServicioExtra.SelectedIndex <= 0)
            {
                txtTotal.Text = hfTotalOriginal.Value;
                upFormularioCheckOut.Update();

                return;
            }

            Servicio_Extra servicio = Servicio_ExtraMap.Read(ddlServicioExtra.Text);

            if (servicio == null)
            {
                txtTotal.Text = hfTotalOriginal.Value;
                upFormularioCheckOut.Update();

                return;
            }

            total += servicio.precio;
            txtTotal.Text = Convert.ToString(total);

            upFormularioCheckOut.Update();
        }

        private List<string> NodosSinDuplicados()
        {
            // tipos de nodo
            List<string> listaNodos = new List<string>(ListaNodos);

            // obtenemos nodos agregados
            List<string> nodosNuevos = ObtenerValoresNodos();

            // obtebenos nodos repetidos
            List<string> nodosRepetidos = ListaNodos.Intersect(nodosNuevos).ToList();

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
                DropDownList ddlServicioExtra = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                nodosNuevos.Add(ddlServicioExtra.Text);
            }

            return nodosNuevos;
        }

        private bool ValidarNuevosNodos(ref string message)
        {
            List<Panel> panelesNodosNuevos = phNodosNuevos.Controls.OfType<Panel>().ToList();

            foreach (Panel pnlNuevoNodo in panelesNodosNuevos)
            {
                DropDownList ddlServicioExtra = pnlNuevoNodo.Controls[0].Controls[0].Controls.OfType<DropDownList>().FirstOrDefault();

                if (ddlServicioExtra == null)
                {
                    message = "No se encontro la información";
                    return false;
                }

                if (ddlServicioExtra.SelectedIndex <= 0)
                {
                    message = "Seleccione el servicio antes de agregar otro";
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

        private List<Servicio_Extra> ServiciosExtras()
        {
            List<Servicio_Extra> servicio_extras = new List<Servicio_Extra>();
            List<string> valoresNuevos = ObtenerValoresNodos();

            foreach (string valorNuevo in valoresNuevos)
            {
                Servicio_Extra servicio = Servicio_ExtraMap.Read(valorNuevo);

                if (servicio == null)
                    continue;

                servicio_extras.Add(servicio);
            }

            return servicio_extras;
        }

        #endregion
    }

    public class Factura
    {
        public string Nombre { get; set; }

        public string Contenido { get; set; }
    }
}