<%@ Page 
    Title="Ciudades" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="ciudades.aspx.cs" 
    Inherits="NDEV_HAPPY_INN.mods.cats.ciudades" 
    EnableEventValidation="false"
    Async="true" %>

<asp:Content ID="c_styles" ContentPlaceHolderID="cph_styles" runat="server">
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/cats/ciudades/ciudades.css") %>" />
</asp:Content>

<asp:Content ID="c_scripts" ContentPlaceHolderID="cph_scripts" runat="server">
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.js") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/cats/ciudades/ciudades.js") %>" />
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
                    
                    
                    <% /*
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
                        */ %>

                </ContentTemplate>
                <Triggers>
                </Triggers>
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
                                <asp:BoundField DataField="pais" HeaderText="Pais" />
                                <asp:BoundField DataField="estado" HeaderText="Estado" />
                                <asp:BoundField DataField="nombre" HeaderText="Ciudad" />
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
                                            Información general
                                        </a>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <a class="nav-link" id="nodos-tab" data-toggle="tab" href="#nodos" aria-controls="nodos" role="tab" aria-selected="false">
                                            Usuarios
                                        </a>
                                    </li>
                                </ul>

                                <div id="tab_content" class="tab-content">
                                    <div id="default" class="tab-pane fade show active" aria-labelledby="default-tab">
                                        
                                        <div class="form-group">
                                            <label>Nombre</label>
                                            <asp:TextBox
                                                ID="txtNombre"
                                                runat="server"
                                                CssClass="form-control"
                                                placeholder="Ingrese el nombre de la ciudad" />
                                        </div>
                                
                                        <div class="form-group">
                                            <label>Estado</label>
                                            <asp:TextBox
                                                ID="txtEstado"
                                                runat="server"
                                                CssClass="form-control"
                                                placeholder="Ingrese el nombre del estado" />
                                        </div>
                                
                                        <div class="form-group">
                                            <label>País</label>
                                            <asp:TextBox
                                                ID="txtPais"
                                                runat="server"
                                                CssClass="form-control"
                                                placeholder="Ingrese el nombre del país" />
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
                                                                <div class="col-9 mb-3">
                                                                    <asp:TextBox
                                                                        ID="txtUsuario"
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