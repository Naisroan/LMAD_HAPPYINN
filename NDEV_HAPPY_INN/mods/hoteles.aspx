<%@ Page 
    Title="Hoteles" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="hoteles.aspx.cs" 
    Inherits="NDEV_HAPPY_INN.mods.hoteles" 
    EnableEventValidation="false"
    Async="true" %>

<asp:Content ID="c_styles" ContentPlaceHolderID="cph_styles" runat="server">
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/hoteles/hoteles.css") %>" />
</asp:Content>

<asp:Content ID="c_scripts" ContentPlaceHolderID="cph_scripts" runat="server">
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.js") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/hoteles/hoteles.js") %>" />
</asp:Content>


<asp:Content ID="c_body" ContentPlaceHolderID="cph_body" runat="server">
        
    <div class="introduction">
        <p class="mb-4">
            Crea, lee, actualiza y borra <%: Page.Title.ToLower() %>
        </p>
    </div>
    
    <asp:Panel
        ID="pnlLoad"
        runat="server"
        Visible="false"
        CssClass="text-center"
    >
        <i class="fas fa-3x fa-circle-notch fa-spin"></i>
    </asp:Panel>
    
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

                        <div class="cantidad-registros d-inline-flex align-items-center mr-2">
                            <span class="text-secondary mr-2">
                                Mostrar
                            </span>
                            <asp:DropDownList
                                ID="ddlCantidadRegistros"
                                runat="server"
                                AutoPostBack="true"
                                CssClass="custom-select mr-2"
                                OnSelectedIndexChanged="ddlCantidadElementos_SelectedIndexChanged"
                            >
                                <asp:ListItem Enabled="true" Text="1" Value="1" />
                                <asp:ListItem Enabled="true" Text="3" Value="3" />
                                <asp:ListItem Selected="True" Enabled="true" Text="6" Value="6" />
                                <asp:ListItem Enabled="true" Text="9" Value="9" />
                                <asp:ListItem Enabled="true" Text="15" Value="15" />
                                <asp:ListItem Enabled="true" Text="30" Value="30" />
                                <asp:ListItem Enabled="true" Text="Todos" Value="0" />
                            </asp:DropDownList>
                            <span class="text-secondary">
                                registro(s)
                            </span>
                        </div>

                        <asp:LinkButton
                            ID="btnRecargar"
                            runat="server"
                            CssClass="btn btn-secondary btn-icon-split mr-2"
                            OnClick="btnRecargar_Click"
                        >
                            <span class="icon text-white-50">
                              <i class="fas fa-redo-alt"></i>
                            </span>
                            <span class="text">Recargar</span>
                        </asp:LinkButton>

                        <asp:LinkButton
                            ID="btnAgregar"
                            runat="server"
                            CssClass="btn btn-primary btn-icon-split"
                            OnClick="btnAgregar_Click"
                        >
                            <span class="icon text-white-50">
                              <i class="fas fa-plus"></i>
                            </span>
                            <span class="text">Agregar</span>
                        </asp:LinkButton>

                    </div>

                    <div class="d-flex mb-4">
                    
                        <div class="tipo-filtro d-inline-flex align-items-center mr-2">
                            <asp:DropDownList
                                ID="ddlTipoFiltro"
                                runat="server"
                                AutoPostBack="true"
                                CssClass="custom-select mr-2"
                                OnSelectedIndexChanged="ddlTipoFiltro_SelectedIndexChanged"
                            >
                                <asp:ListItem Selected="True" Text="Filtros..." Value="0" />
                                <asp:ListItem Text="País y fecha" Value="pais_fecha" />
                            </asp:DropDownList>
                        </div>
                    
                        <asp:TextBox
                            ID="txtBusqueda"
                            runat="server"
                            CssClass="form-control bg-white border-0 small mr-2"
                            AutoCompleteType="None"
                            placeholder="Seleccione el tipo de filtro y realice una búsqueda"
                        />
                        <asp:DropDownList
                            ID="ddlPaisFiltro"
                            runat="server"
                            CssClass="custom-select mr-2"
                            Visible="false"
                        />
                        <asp:TextBox
                            ID="txtFecha"
                            runat="server"
                            CssClass="form-control bg-white small mr-2"
                            AutoCompleteType="None"
                            TextMode="Date"
                            placeholder="Fecha que desea ver el reporte"
                            Visible="false"
                        />
                        <asp:LinkButton
                            ID="btnBuscar"
                            runat="server"
                            CssClass="btn btn-info"
                            OnClick="btnBuscar_Click"
                        >
                            <i class="fas fa-search fa-sm"></i>
                        </asp:LinkButton>

                    </div>

                </ContentTemplate>

            </asp:UpdatePanel>

            <asp:UpdatePanel 
                ID="upToolboxOrdenamiento" 
                runat="server" 
                UpdateMode="Conditional" 
            >
                <ContentTemplate>

                    <div class="d-block text-right mb-4">

                        <div class="cantidad-registros d-inline-flex align-items-center mr-2">
                            <span class="text-secondary mr-2">
                                Ordenar
                            </span>
                            <asp:DropDownList
                                ID="ddlTipoOrden"
                                runat="server"
                                AutoPostBack="true"
                                CssClass="custom-select mr-2"
                            >
                                <asp:ListItem Selected="True" Text="Orden..." Value="0" />
                                <asp:ListItem Text="Porcentaje" Value="porcentaje" />
                                <asp:ListItem Text="Hotel" Value="hotel" />
                                <asp:ListItem Text="Ciudad" Value="ciudad" />
                            </asp:DropDownList>
                        </div>

                        <asp:LinkButton
                            ID="btnOrdenarPor"
                            runat="server"
                            CssClass="btn btn-secondary btn-icon-split"
                            OnClick="btnOrdenarPor_Click"
                        >
                            <span class="icon text-white-50">
                              <i class="fas fa-filter"></i>
                            </span>
                            <span class="text">Aplicar</span>
                        </asp:LinkButton>

                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>

            <% /*
                * ================================================================================
                * GRIDVIEW
                * ================================================================================
                */ %>

            <asp:UpdatePanel 
                ID="upGridView" 
                runat="server" 
                UpdateMode="Conditional" 
            >
                <ContentTemplate>

                    <div class="d-block mb-4">
                        <asp:GridView
                            ID="gvData"
                            runat="server"
                            AutoGenerateColumns="false"
                            AllowPaging="true"
                            DataKeyNames="<%# DataKeyNames %>"
                            BorderStyle="None"
                            GridLines="None"
                            CssClass="table bg-white shadow-sm"
                            HeaderStyle-CssClass="bg-white text-uppercase"
                            EmptyDataText="No existen registros o no hay coincidencias con los filtros especificados"
                            PageSize="6"
                            PagerSettings-FirstPageText="Inicio"
                            PagerSettings-LastPageText="Fin"
                            PagerSettings-Mode="NumericFirstLast"
                            PagerStyle-HorizontalAlign="Right"
                            PagerStyle-CssClass="table-pagging"
                            OnPageIndexChanging="gridView_PageIndexChanging"
                            OnRowDataBound="gridView_RowDataBound"
                            OnSelectedIndexChanged="gridView_SelectedIndexChanged"
                        >
                            <Columns>
                                <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                                <asp:BoundField DataField="ciudad" HeaderText="Ciudad" SortExpression="" />
                                <asp:TemplateField HeaderText="% de ocupación">
                                    <ItemTemplate>
                                        <div class="progress">
                                            <asp:Panel
                                                ID="pnlPorcentaje"
                                                runat="server"
                                                CssClass="progress-bar"
                                                aria-valuemin="0" 
                                                aria-valuemax="100"
                                            >
                                                <asp:Literal ID="litPorcentajeUso" runat="server" Text="25" />%
                                            </asp:Panel>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Habitaciones">
                                    <ItemTemplate>
                                        <ul class="list-group">
                                            <asp:Repeater
                                                ID="rpHabitaciones"
                                                runat="server"
                                                OnItemDataBound="rpHabitaciones_ItemDataBound"
                                            >
                                                <ItemTemplate>
                                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                                        <%# Eval("tipo_habitacion") %>
                                                    <span class="badge badge-primary badge-pill">
                                                        <asp:Literal ID="litHabitacionesDisponibles" runat="server" /> 
                                                        / 
                                                        <asp:Literal ID="litHabitacionesTotales" runat="server" />
                                                    </span>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

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
                
                    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
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

                                <ul id="tabs" runat="server" class="nav nav-tabs mb-3" role="tablist">
                                    <li class="nav-item" role="presentation">
                                        <a class="nav-link active" id="default-tab" data-toggle="tab" href="#default" aria-controls="default" role="tab" aria-selected="true">
                                            Información
                                        </a>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <a class="nav-link" id="nodos-tab" data-toggle="tab" href="#nodos" aria-controls="nodos" role="tab" aria-selected="false">
                                            Habitaciones
                                        </a>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <a class="nav-link" id="nodos2-tab" data-toggle="tab" href="#nodos2" aria-controls="nodos2" role="tab" aria-selected="false">
                                            Caracteristicas
                                        </a>
                                    </li>
                                </ul>

                                <div id="tab_content" class="tab-content">

                                    <div id="default" class="tab-pane fade show active" aria-labelledby="default-tab">

                                        <div class="form-row">
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label>Nombre</label>
                                                <asp:TextBox
                                                    ID="txtNombre"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    placeholder="Ingrese el nombre" />
                                            </div>
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label>Inicio de operaciones</label>
                                                <asp:TextBox
                                                    ID="txtFechaIniOperaciones"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    TextMode="Date"
                                                    placeholder="Ingrese la fecha" />
                                            </div>
                                        </div>

                                        <div class="form-row">
                                            <div class="col-12 col-lg-6 mb-3">
                                                <label># de pisos</label>
                                                <asp:TextBox
                                                    ID="txtCantPisos"
                                                    runat="server"
                                                    CssClass="form-control"
                                                    TextMode="Number"
                                                    Text="1"
                                                    placeholder="Ingrese # de pisos" />
                                            </div>
                                            <asp:UpdatePanel
                                                ID="upCheckBox"
                                                runat="server"
                                                UpdateMode="Conditional"
                                                class="col-12 col-lg-6 mb-3">
                                                <ContentTemplate>
                                                    <label>¿En zona turistica?</label>
                                                    <div class="my-1">
                                                        <div class="custom-control custom-checkbox mr-sm-2">
                                                            <asp:CheckBox
                                                                ID="chkZonaTuristica"
                                                                runat="server"
                                                                AutoPostBack="true"
                                                                ClientIDMode="Static"
                                                                CssClass="custom-control-input" 
                                                            />
                                                            <label class="custom-control-label" for="chkZonaTuristica">
                                                                Sí
                                                            </label>
                                                        </div>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>

                                        <asp:UpdatePanel
                                            ID="upCiudades"
                                            runat="server"
                                            UpdateMode="Conditional"
                                        >
                                            <ContentTemplate>

                                                <div class="form-row">
                                                    <div class="col-12 col-lg-4 mb-3">
                                                        <label>País</label>
                                                        <asp:DropDownList
                                                            ID="ddlPais"
                                                            runat="server"
                                                            AutoPostBack="true"
                                                            CssClass="custom-select"
                                                            OnSelectedIndexChanged="ddlPais_SelectedIndexChanged"
                                                        />
                                                    </div>
                                                    <div class="col-12 col-lg-4 mb-3">
                                                        <label>Estado</label>
                                                        <asp:DropDownList
                                                            ID="ddlEstado"
                                                            runat="server"
                                                            AutoPostBack="true"
                                                            CssClass="custom-select"
                                                            OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged"
                                                        />
                                                    </div>
                                                    <div class="col-12 col-lg-4 mb-3">
                                                        <label>Ciudad</label>
                                                        <asp:DropDownList
                                                            ID="ddlCiudad"
                                                            runat="server"
                                                            CssClass="custom-select"
                                                        />
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                        <div class="form-group">
                                            <label>Domicilio</label>
                                            <asp:TextBox
                                                ID="txtDomicilio"
                                                runat="server"
                                                CssClass="form-control"
                                                Rows="3"
                                                TextMode="MultiLine"
                                                placeholder="Ingrese el domicilio" 
                                            />
                                        </div>

                                    </div>
                                    <div id="nodos" class="tab-pane fade" aria-labelledby="nodos-tab">

                                        <asp:UpdatePanel 
                                            ID="upNodosAnteriores"
                                            runat="server"
                                            UpdateMode="Conditional"
                                            class="rpNodos"
                                        >
                                            <ContentTemplate>
                                                <asp:Repeater
                                                    ID="rpNodos"
                                                    runat="server"
                                                    OnItemDataBound="rpNodos_ItemDataBound"
                                                >
                                                    <ItemTemplate>
                                                        <div class="rpNodos_nodo">
                                                            <div class="form-row">
                                                                <div class="col-5 mb-3">
                                                                    <asp:DropDownList
                                                                        ID="ddlTipoHabitacion"
                                                                        runat="server"
                                                                        CssClass="custom-select"
                                                                        Enabled="false"
                                                                    />
                                                                </div>
                                                                <div class="col-4 mb-3">
                                                                    <asp:TextBox
                                                                        ID="txtCantidad"
                                                                        runat="server"
				                                                        Text='<%# Convert.ToString(Eval("cantidad")) %>'
				                                                        CssClass="form-control"
				                                                        TextMode="Number"
				                                                        Enabled="false"
                                                                    />
                                                                </div>
                                                                <div class="col-3 mb-3">
                                                                    <asp:Button 
                                                                        ID="btnRetirar"
                                                                        runat="server"
                                                                        UseSubmitBehavior="false"
                                                                        Text="Retirar"
                                                                        OnClick="btnRetirar_Click"
                                                                        CommandName="tipo_habitacion"
                                                                        CommandArgument='<%# Convert.ToString(Eval("tipo_habitacion")) %>'
                                                                        CssClass="btn btn-outline-secondary btn-block"
                                                                    />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

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
                                                            CommandName="tipo_habitacion"
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
                                    <div id="nodos2" class="tab-pane fade" aria-labelledby="nodos2-tab">

                                        <asp:UpdatePanel 
                                            ID="upNodosAnteriores2"
                                            runat="server"
                                            UpdateMode="Conditional"
                                            class="rpNodos"
                                        >
                                            <ContentTemplate>
                                                <asp:Repeater
                                                    ID="rpNodos2"
                                                    runat="server"
                                                >
                                                    <ItemTemplate>
                                                        <div class="rpNodos_nodo">
                                                            <div class="form-row">
                                                                <div class="col-9 mb-3">
                                                                    <asp:TextBox
                                                                        ID="txtCaracteristica"
                                                                        runat="server"
                                                                        CssClass="form-control"
                                                                        Enabled="false"
                                                                        Text='<%# Convert.ToString(Container.DataItem) %>'
                                                                    />
                                                                </div>
                                                                <div class="col-3 mb-3">
                                                                    <asp:Button 
                                                                        ID="btnRetirar"
                                                                        runat="server"
                                                                        UseSubmitBehavior="false"
                                                                        Text="Retirar"
                                                                        OnClick="btnRetirar_Click"
                                                                        CommandName="caracteristica"
                                                                        CommandArgument='<%# Convert.ToString(Container.DataItem) %>'
                                                                        CssClass="btn btn-outline-secondary btn-block"
                                                                    />
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>

                                        <asp:UpdatePanel 
                                            ID="upNodosNuevos2"
                                            runat="server"
                                            UpdateMode="Conditional"
                                            class="d-block mb-3"
                                        >
                                            <ContentTemplate>

                                                <asp:PlaceHolder ID="phNodosNuevos2" runat="server" />

                                                <div class="form-row">
                                                    <div class="col-3 ml-auto">
                                                        <asp:Button 
                                                            ID="btnAgregarNodo2"
                                                            runat="server"
                                                            Text="Agregar"
                                                            CssClass="btn btn-outline-secondary btn-block mb-1"
                                                            CommandName="caracteristica"
                                                            OnClick="btnAgregarNodo_Click"
                                                        />
                                                    </div>
                                                    <div class="col-12 text-right">
                                                        <small class="mb-3">
                                                            <asp:Label
                                                                ID="lblMensajeNodo2"
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

                                </div>

                                <asp:UpdatePanel
                                    ID="upMensajeFormulario"
                                    runat="server"
                                    UpdateMode="Conditional"
                                    Visible="false"
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
                                            ID="btnEliminar"
                                            runat="server"
                                            CssClass="btn btn-danger btn-icon-split"
                                            Visible="false"
                                            UseSubmitBehavior="false"
                                            OnClientClick="return confirm('¿Está de acuerdo en eliminar el registro seleccionado?');"
                                            OnClick="btnEliminar_Click"
                                        >
                                            <span class="icon text-white-50">
                                                <i class="fas fa-trash"></i>
                                            </span>
                                            <span class="text">Eliminar</span>
                                        </asp:LinkButton>

                                        <asp:LinkButton
                                            ID="btnGuardar"
                                            runat="server"
                                            CssClass="btn btn-primary btn-icon-split"
                                            UseSubmitBehavior="false"
                                            OnClientClick="onButtonLoading(this, 'Guardando')"
                                            OnClick="btnGuardar_Click"
                                        >
                                            <span class="icon text-white-50">
                                                <i class="fas fa-save"></i>
                                            </span>
                                            <span class="text">Guardar</span>
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