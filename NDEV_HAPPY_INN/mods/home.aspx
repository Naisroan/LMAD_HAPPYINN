<%@ Page 
    Title="Happy Inn" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="home.aspx.cs" 
    Inherits="NDEV_HAPPY_INN.mods.home" 
    EnableEventValidation="false"
    Async="true" %>

<asp:Content ID="c_styles" ContentPlaceHolderID="cph_styles" runat="server">
    
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/bs/bootstrap-select.min.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/home/home.css") %>" />

</asp:Content>

<asp:Content ID="c_scripts" ContentPlaceHolderID="cph_scripts" runat="server">
    
    <script type="text/javascript" src="<%: ResolveUrl("~/Scripts/bs/bootstrap-select.min.js") %>"></script>
    <script type="text/javascript" src="<%: ResolveUrl("~/Content/tables/table.js") %>"></script>
    <script type="text/javascript" src="<%: ResolveUrl("~/Content/mods/home/home.js") %>"></script>

</asp:Content>

<asp:Content ID="c_body" ContentPlaceHolderID="cph_body" runat="server">
    
    <asp:UpdatePanel
        ID="upPage"
        runat="server"
        UpdateMode="Conditional"
    >
        <ContentTemplate>

            <% /*
                * ================================================================================
                * HERRAMIENTAS
                * ================================================================================
                */ %>

            <asp:UpdatePanel 
                ID="upToolbox" 
                runat="server" 
                UpdateMode="Conditional" 
            >
                <ContentTemplate>

                    <div class="d-block text-right mb-4">

                        <asp:LinkButton
                            ID="btnRecargar"
                            runat="server"
                            CssClass="btn btn-secondary btn-icon-split"
                            OnClick="btnRecargar_Click"
                        >
                            <span class="icon text-white-50">
                              <i class="fas fa-redo-alt"></i>
                            </span>
                            <span class="text">Recargar</span>
                        </asp:LinkButton>

                    </div>
                    
            <%--
                    <div class="d-flex mb-4">
                    
                        <div class="tipo-filtro d-inline-flex align-items-center mr-2">
                            <asp:DropDownList
                                ID="ddlTipoFiltro"
                                runat="server"
                                CssClass="custom-select mr-2"
                            >
                                <asp:ListItem Selected="True" Text="Filtros..." Value="0" />
                            </asp:DropDownList>
                        </div>
                    
                        <div class="input-group">
                            <asp:TextBox
                                ID="txtBusqueda"
                                runat="server"
                                CssClass="form-control bg-white border-0 small"
                                AutoCompleteType="None"
                                placeholder="Seleccione el tipo de filtro y realice una búsqueda"
                            />
                            <div class="input-group-append">
                                <asp:LinkButton
                                    ID="btnBuscar"
                                    runat="server"
                                    CssClass="btn btn-info"
                                >
                                    <i class="fas fa-search fa-sm"></i>
                                </asp:LinkButton>
                            </div>
                        </div>

                    </div>
            --%>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

            <% /*
                * ================================================================================
                * GRIDVIEW Y/O REPEATERS
                * ================================================================================
                */ %>

            <div class="row">

                <div class="col-12 col-lg-8">

                    <label class="mb-4">Crear reservación</label>

                    <asp:UpdatePanel 
                        ID="upDataRepeater" 
                        runat="server" 
                        UpdateMode="Conditional" 
                    >
                        <ContentTemplate>

                            <div class="card-columns hoteles">

                                <asp:Repeater
                                    ID="rpHoteles"
                                    runat="server"
                                    OnItemDataBound="rpHoteles_ItemDataBound"
                                >
                                    <ItemTemplate>

                                        <div class="card hotel">
                                            <%--<img 
                                                src="http://placeimg.com/400/200/arch"
                                                class="card-img-top" 
                                                alt="<%# Eval("nombre") %>"
                                            />--%>
                                            <asp:Image 
                                                ID="imgHotel"
                                                runat="server"
                                                CssClass="card-img-top"
                                            />
                                            <div class="card-body">
                                                <h5 class="card-title mb-1">
                                                    Happy Inn - <%# Eval("nombre") %>
                                                </h5>
                                                <small class="d-block text-left mb-3">
                                                    <%# Eval("ciudad") %>
                                                </small>
                                                <p class="card-text">
                                                    <%# Eval("domicilio") %>
                                                </p>
                                                <%--<div class="info-extra">
                                                    <h6>
                                                        Habitaciones disponibles 
                                                        <asp:Label
                                                            ID="lblHabitacionesDisponibles"
                                                            runat="server"
                                                            CssClass="badge"
                                                            Text="0"
                                                        />
                                                    </h6>
                                                </div>--%>
                                            </div>
                                            <div class="card-header">
                                                Características
                                            </div>
                                            <ul class="list-group list-group-flush">
                                                <asp:Repeater
                                                    ID="rpCaracteristicas"
                                                    runat="server"
                                                >
                                                    <ItemTemplate>
                                                        <li class="list-group-item">
                                                            <%# Convert.ToString(Container.DataItem) %>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                            <div class="card-body text-right">
                                                <asp:Button 
                                                    ID="btnReservacion"
                                                    runat="server"
                                                    CssClass="btn btn-primary"
                                                    CommandName="reservacion"
                                                    CommandArgument='<%# Eval("nombre") %>'
                                                    Text="Reservación nueva"
                                                    UseSubmitBehavior="false"
                                                    OnClick="btnReservacion_Click"
                                                />
                                            </div>
                                        </div>

                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>

                        </ContentTemplate>
                        <Triggers>
                        </Triggers>
                    </asp:UpdatePanel>

                </div>

                <div class="col-12 col-lg-4">

                    <label class="mb-4">Reservaciones pendientes en progreso</label>

                    <asp:UpdatePanel
                        ID="upReservaciones"
                        runat="server"
                        UpdateMode="Conditional"
                    >
                        <ContentTemplate>

                            <asp:Repeater
                                ID="rpReservaciones"
                                runat="server"
                                OnItemDataBound="rpReservaciones_ItemDataBound"
                            >
                                <ItemTemplate>

                                    <div 
                                        class="card shadow-sm mb-3 <%# Convert.ToBoolean(Eval("check_in")) ? "border-primary" : "border-light" %>"
                                    >
                                        <div class="card-header">
                                            Happy Inn - <%# Eval("nombre_hotel") %>
                                        </div>
                                        <div class="card-body">
                                            <h5 class="card-title">
                                                <asp:Literal
                                                    ID="litNombreCliente"
                                                    runat="server"
                                                    Text="Cliente"
                                                />
                                            </h5>
                                            <p class="card-text mb-3">
                                                Tipo de habitación <%# Eval("tipo_habitacion") %>
                                            </p>
                                            <p class="card-text mb-3">
                                                Anticipo de <strong>$ <%# Eval("monto_anticipo") %> MXN</strong>
                                            </p>
                                            <p class="card-text mb-1">
                                                <%# DateTime.Parse(Eval("fecha_ini").ToString()).ToLongDateString() %>
                                            </p>
                                            <p class="card-text mb-1">
                                                a
                                            </p>
                                            <p class="card-text mb-3">
                                                <%# DateTime.Parse(Eval("fecha_fin").ToString()).ToLongDateString() %>
                                            </p>
                                        </div>
                                        <div class="card-footer text-right">

                                            <asp:Panel
                                                ID="pnlGroupCancelarReservacion"
                                                runat="server"
                                                CssClass="input-group input-group-sm mb-3"
                                            >
                                                <asp:TextBox
                                                    ID="txtPasswordAdmin"
                                                    runat="server"
                                                    TextMode="Password"
                                                    CssClass="form-control"
                                                    placeholder="Clave de administrador"
                                                    aria-label="Clave de administrador"
                                                />
                                                <div class="input-group-append">
                                                    <asp:Button 
                                                        ID="btnCancelarReservacion"
                                                        runat="server"
                                                        Text="Cancelar"
                                                        CommandArgument='<%# Eval("id") %>'
                                                        CssClass="btn btn-outline-secondary btn-sm"
                                                        OnClientClick="return confirm('¿Está de acuerdo en cancelar la reservación?');"
                                                        OnClick="btnCancelarReservacion_Click"
                                                    />
                                                </div>
                                            </asp:Panel>

                                            <asp:Button 
                                                ID="btnCheckInn"
                                                runat="server"
                                                Text='<%# !Convert.ToBoolean(Eval("check_in")) ? "Check In" : "Check Out" %>'
                                                CommandName='<%# !Convert.ToBoolean(Eval("check_in")) ? "check_in" : "check_out" %>'
                                                CommandArgument='<%# Eval("id") %>'
                                                CssClass="btn btn-outline-primary btn-sm"
                                                OnClick="btnCheckInn_Click"
                                            />

                                        </div>
                                    </div>

                                </ItemTemplate>
                            </asp:Repeater>

                        </ContentTemplate>
                    </asp:UpdatePanel>

                </div>

            </div>

            <% /*
                * ================================================================================
                * FORMULARIO MODAL
                * ================================================================================
                */ %>

            <asp:UpdatePanel 
                ID="upFormulario" 
                runat="server" 
                UpdateMode="Conditional" 
                class="modal fade"
                data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-labelledby="label_form" aria-hidden="true"
            >
                <ContentTemplate>
                
                    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-xl">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="label_form">
                                    <asp:Label
                                        ID="lblTituloFormulario"
                                        runat="server"
                                        Text="Agregar"
                                    />
                                </h5>
                            </div>
                            <div class="modal-body">

                                <asp:HiddenField
                                    ID="hfId"
                                    runat="server"
                                    Value="-1" 
                                />

                                <div class="row">

                                    <div class="col-12 col-lg-5">
                                        <asp:UpdatePanel
                                            ID="upHabitaciones"
                                            runat="server"
                                            UpdateMode="Conditional"
                                        >
                                            <ContentTemplate>
                                                <label>Habitaciones</label>
                                                <div class="d-block mb-4">
                                                    <div class="list-group">
                                                        <asp:Repeater
                                                            ID="rpHabitacionesHotel"
                                                            runat="server"
                                                            OnItemDataBound="rpHabitacionesHotel_ItemDataBound"
                                                        >
                                                            <ItemTemplate>
                                                                <asp:Panel
                                                                    ID="pnlHabitacion"
                                                                    runat="server"
                                                                    CssClass="list-group-item list-group-item-action"
                                                                    GroupingText='<%# Eval("tipo_habitacion") %>'
                                                                >
                                                                    <div class="d-flex w-100 justify-content-between">
                                                                        <h5 class="mb-1">
                                                                            <asp:literal
                                                                                ID="litDescripcion"
                                                                                runat="server"
                                                                            />
                                                                        </h5>
                                                                        <small class="font-weight-bold">
                                                                            Costo/noche $
                                                                            <asp:literal
                                                                                ID="litPrecio"
                                                                                runat="server"
                                                                            />
                                                                        </small>
                                                                    </div>
                                                                    <div class="my-3">
                                                                        Capacidad de personas #
                                                                        <asp:literal
                                                                            ID="litCapacidad"
                                                                            runat="server"
                                                                        />
                                                                    </div>
                                                                    <div class="mb-1">
                                                                        <small>Características:</small>
                                                                    </div>
                                                                    <ul class="list-group list-group-horizontal-sm">
                                                                        <asp:Repeater
                                                                            ID="rpCaracteristicasHabitacion"
                                                                            runat="server"
                                                                        >
                                                                            <ItemTemplate>
                                                                                <li class="list-group-item">
                                                                                    <%# Container.DataItem %>
                                                                                </li>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </ul>
                                                                    <div class="mb-1">
                                                                        <small>Camas:</small>
                                                                    </div>
                                                                    <ul class="list-group list-group-horizontal-sm">
                                                                        <asp:Repeater
                                                                            ID="rpCamasHabitacion"
                                                                            runat="server"
                                                                        >
                                                                            <ItemTemplate>
                                                                                <li class="list-group-item">
                                                                                    <%# DataBinder.Eval((KeyValuePair<string, int>)Container.DataItem, "Key") %>
                                                                                </li>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </ul>
                                                                    <div class="text-right">
                                                                        <asp:Button 
                                                                            ID="btnSeleccionarHabitacion"
                                                                            runat="server"
                                                                            CssClass="btn btn-primary mb-1"
                                                                            CommandName="reservacion"
                                                                            CommandArgument='<%# Eval("tipo_habitacion") %>'
                                                                            Text="Seleccionar"
                                                                            UseSubmitBehavior="false"
                                                                            OnClick="btnSeleccionarHabitacion_Click"
                                                                        />
                                                                    </div>
                                                                </asp:Panel>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>

                                    <div class="col-12 col-lg-7">

                                        <div class="form-row">
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label>Cliente</label>
                                                <div class="d-block">
                                                    <asp:DropDownList
                                                        ID="ddlCliente"
                                                        runat="server"
                                                        CssClass="selectpicker"
                                                        data-live-search="true"
                                                    />
                                                </div>
                                            </div>
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label># de Personas</label>
                                                <asp:TextBox
                                                    ID="txtCantPersonas"
                                                    runat="server"
                                                    TextMode="Number"
                                                    CssClass="form-control"
                                                    Text="0"
                                                />
                                            </div>
                                        </div>
                                        
                                        <div class="form-row">
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label>$ Anticipo</label>
                                                <asp:TextBox
                                                    ID="txtAnticipo"
                                                    runat="server"
                                                    TextMode="Number"
                                                    CssClass="form-control"
                                                    Text="0.00"
                                                />
                                            </div>
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label>Método de pago</label>
                                                <asp:DropDownList
                                                    ID="ddlMetodoPago"
                                                    runat="server"
                                                    CssClass="custom-select"
                                                />
                                            </div>
                                        </div>
                                        
                                        <div class="form-row">
                                            <div class="col-12">
                                                <label>Fechas de reservación</label>
                                            </div>
                                            <div class="col mb-3">
                                                <asp:TextBox
                                                    ID="txtFechaInicio"
                                                    runat="server"
                                                    TextMode="Date"
                                                    CssClass="form-control"
                                                />
                                            </div>
                                            <div class="col-1 mb-3 text-center">
                                                <span> a </span>
                                            </div>
                                            <div class="col mb-3">
                                                <asp:TextBox
                                                    ID="txtFechaFin"
                                                    runat="server"
                                                    TextMode="Date"
                                                    CssClass="form-control"
                                                />
                                            </div>
                                        </div>

                                        <div class="form-row">
                                            <asp:UpdatePanel
                                                ID="upCheckBox"
                                                runat="server"
                                                UpdateMode="Conditional"
                                                class="col-12 mb-3">
                                                <ContentTemplate>
                                                    <label>¿El cliente ya entrará a la habitación?</label>
                                                    <div class="my-1">
                                                        <div class="custom-control custom-checkbox mr-sm-2">
                                                            <asp:CheckBox
                                                                ID="chkIn"
                                                                runat="server"
                                                                AutoPostBack="true"
                                                                ClientIDMode="Static"
                                                                CssClass="custom-control-input" 
                                                            />
                                                            <label class="custom-control-label" for="chkIn">
                                                                Sí
                                                            </label>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <asp:UpdatePanel
                                            ID="upMensajeFormulario"
                                            runat="server"
                                            UpdateMode="Conditional"
                                        >
                                            <ContentTemplate>
                                                <div class="card bg-danger text-white shadow">
                                                    <div class="card-body">
                                                        <span class="d-block font-weight-bolder">
                                                            !Alerta!
                                                        </span>
                                                        <div class="text-white small">
                                                            <asp:Label ID="lblMensajeError" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </div>

                                </div>

                            </div>

                            <div class="modal-footer">

                                <asp:UpdatePanel
                                    ID="upToolboxFormulario"
                                    runat="server"
                                    UpdateMode="Conditional"
                                >
                                    <ContentTemplate>

                                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                            <span class="text"> Cancelar y volver</span>
                                        </button>

                                        <asp:LinkButton
                                            ID="btnGuardar"
                                            runat="server"
                                            CssClass="btn btn-primary btn-icon-split"
                                            UseSubmitBehavior="false"
                                            OnClientClick="onButtonLoading(this, 'Guardando')"
                                            OnClick="btnGuardar_Click"
                                        >
                                            <span class="icon text-white-50">
                                                <i class="fas fa-calendar-plus fa-fw"></i>
                                            </span>
                                            <span class="text">Reservar</span>
                                        </asp:LinkButton>

                                    </ContentTemplate>

                                </asp:UpdatePanel>
                            </div>

                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

            <asp:UpdatePanel 
                ID="upFormularioCheckOut" 
                runat="server" 
                UpdateMode="Conditional" 
                class="modal fade"
                data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-labelledby="label_form_checkout" aria-hidden="true"
            >
                <ContentTemplate>
                
                    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="label_form_checkout">
                                    <asp:Label
                                        ID="lblTituloCheckOut"
                                        runat="server"
                                        Text="Finalizar reservación"
                                    />
                                </h5>
                            </div>
                            
                            <div class="modal-body">

                                <asp:HiddenField
                                    ID="hfIdCheckOut"
                                    runat="server"
                                    Value="-1" 
                                />

                                <div class="row">

                                    <div class="col-12 col-lg-7">

                                        <label>Servicios extras</label>
                                        

                                        <asp:UpdatePanel 
                                            ID="upNodosNuevos"
                                            runat="server"
                                            UpdateMode="Conditional"
                                            class="d-block mb-3"
                                        >
                                            <ContentTemplate>

                                                <asp:PlaceHolder ID="phNodosNuevos" runat="server" />

                                                <div class="form-row">
                                                    <div class="col-3 ml-auto">
                                                        <asp:Button 
                                                            ID="btnAgregarNodo"
                                                            runat="server"
                                                            Text="Agregar"
                                                            CssClass="btn btn-outline-secondary btn-block mb-1"
                                                            OnClick="btnAgregarNodo_Click"
                                                        />
                                                    </div>
                                                    <div class="col-12 text-right">
                                                        <small class="mb-3">
                                                            <asp:Label
                                                                ID="lblMensajeNodo"
                                                                runat="server"
                                                                Text=""
                                                                CssClass="text-danger m-0 p-0"
                                                            />
                                                        </small>
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </div>

                                    <div class="col-12 col-lg-5">

                                        <asp:HiddenField 
                                            ID="hfTotalOriginal"
                                            runat="server"
                                            Value="0"
                                        />
                                        
                                        <label>Resumen</label>

                                        <ul class="list-group mb-3">
                                            <li class="list-group-item">
                                                <asp:Label
                                                    ID="lblAnticipo"
                                                    runat="server"
                                                    CssClass=""
                                                    Text="$ 0.00"
                                                />
                                            </li>
                                            <li class="list-group-item">
                                                <asp:Label
                                                    ID="lblCantidadPersonas"
                                                    runat="server"
                                                    CssClass=""
                                                    Text="0"
                                                />
                                            </li>
                                            <li class="list-group-item">
                                                <asp:Label
                                                    ID="lblPrecioHabitacion"
                                                    runat="server"
                                                    CssClass=""
                                                    Text="0"
                                                />
                                            </li>
                                            <li class="list-group-item">
                                                <asp:Label
                                                    ID="lblDias"
                                                    runat="server"
                                                    CssClass=""
                                                    Text="0"
                                                />
                                            </li>
                                        </ul>
                                        
                                        <div class="form-row">
                                            <div class="col-12 col-lg-12 mb-3">
                                                <label>Método de pago</label>
                                                <asp:DropDownList
                                                    ID="ddlMetodoPagoCheckOut"
                                                    runat="server"
                                                    CssClass="custom-select"
                                                />
                                            </div>
                                        </div>
                                        
                                        <div class="form-row">
                                            <div class="col-12 mb-3 text-right">
                                                <span class="total d-block mb-3">
                                                    <strong>
                                                        $ 
                                                        <asp:Label
                                                            ID="txtTotal"
                                                            runat="server"
                                                            CssClass=""
                                                            Text="0.00"
                                                        />
                                                    </strong>
                                                </span>
                                                <small class="text-secondary">Total</small>
                                            </div>
                                        </div>

                                        <asp:UpdatePanel
                                            ID="upMensajeCheckOut"
                                            runat="server"
                                            UpdateMode="Conditional"
                                        >
                                            <ContentTemplate>
                                                <div class="card bg-danger text-white shadow">
                                                    <div class="card-body">
                                                        <span class="d-block font-weight-bolder">
                                                            !Alerta!
                                                        </span>
                                                        <div class="text-white small">
                                                            <asp:Label ID="lblMensajeCheckOut" runat="server"></asp:Label>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                    </div>

                                </div>

                            </div>

                            <div class="modal-footer">

                                <asp:UpdatePanel
                                    ID="upToolboxFormularioCheckOut"
                                    runat="server"
                                    UpdateMode="Conditional"
                                >
                                    <ContentTemplate>

                                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                                            <span class="text"> Cancelar y volver</span>
                                        </button>

                                        <asp:LinkButton
                                            ID="btnCheckOut"
                                            runat="server"
                                            CssClass="btn btn-primary btn-icon-split"
                                            UseSubmitBehavior="false"
                                            OnClientClick="onButtonLoading(this, 'Guardando')"
                                            OnClick="btnCheckOut_Click"
                                        >
                                            <span class="icon text-white-50">
                                                <i class="fas fa-calendar-check fa-fw"></i>
                                            </span>
                                            <span class="text">Check out</span>
                                        </asp:LinkButton>

                                    </ContentTemplate>

                                </asp:UpdatePanel>
                            </div>

                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
