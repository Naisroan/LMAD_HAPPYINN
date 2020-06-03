<%@ Page 
    Title="Historial de Reservaciones" 
    Language="C#" 
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="reservaciones.aspx.cs" 
    Inherits="NDEV_HAPPY_INN.mods.reservaciones" 
    EnableEventValidation="false"
    Async="true" %>

<asp:Content ID="c_styles" ContentPlaceHolderID="cph_styles" runat="server">
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/bs/bootstrap-select.min.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/tables/table.css") %>" />
    <link type="text/css" rel="stylesheet" href="<%: ResolveUrl("~/Content/mods/reservaciones/reservaciones.css") %>" />
</asp:Content>

<asp:Content ID="c_scripts" ContentPlaceHolderID="cph_scripts" runat="server">
    <script type="text/javascript" src="<%: ResolveUrl("~/Scripts/bs/bootstrap-select.min.js") %>"></script>
    <script type="text/javascript" src="<%: ResolveUrl("~/Content/tables/table.js") %>"></script>
    <script type="text/javascript" src="<%: ResolveUrl("~/Content/mods/reservaciones/reservaciones.js") %>"></script>
</asp:Content>

<asp:Content ID="c_body" ContentPlaceHolderID="cph_body" runat="server">
        
    <div class="introduction">
        <p class="mb-4">
            Reportes sobre el <%: Page.Title.ToLower() %>
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
                            CssClass="btn btn-secondary btn-icon-split"
                            OnClick="btnRecargar_Click"
                        >
                            <span class="icon text-white-50">
                              <i class="fas fa-redo-alt"></i>
                            </span>
                            <span class="text">Recargar</span>
                        </asp:LinkButton>

                    </div>
                    
                    <div class="d-flex mb-4">
                    
                        <%--<div class="tipo-filtro d-inline-flex align-items-center mr-2">
                            <asp:DropDownList
                                ID="ddlTipoFiltro"
                                runat="server"
                                CssClass="custom-select mr-2"
                            >
                                <asp:ListItem Selected="True" Text="Filtros..." Value="0" />
                                <asp:ListItem Text="Cliente" Value="cliente" />
                            </asp:DropDownList>
                        </div>--%>
                    
                        <div class="input-group">
                            <%--<asp:TextBox
                                ID="txtBusqueda"
                                runat="server"
                                CssClass="form-control bg-white border-0 small"
                                AutoCompleteType="None"
                                placeholder="Seleccione el tipo de filtro y realice una búsqueda"
                            />--%>
                            <asp:DropDownList
                                ID="ddlCliente"
                                runat="server"
                                CssClass="selectpicker"
                                data-live-search="true"
                            />

                            <div class="input-group-append">
                                <asp:LinkButton
                                    ID="btnBuscar"
                                    runat="server"
                                    CssClass="btn btn-info"
                                    OnClick="btnBuscar_Click"
                                >
                                    <i class="fas fa-search fa-sm"></i>
                                </asp:LinkButton>
                            </div>
                        </div>

                    </div>

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
                            OnRowDataBound="gvData_RowDataBound"
                        >
                            <Columns>
                                <asp:BoundField DataField="id" HeaderText="# de Reservación" Visible="false" />
                                <asp:BoundField DataField="nombre_hotel" HeaderText="Hotel" />
                                <asp:TemplateField HeaderText="Cliente">
                                    <ItemTemplate>
                                        <asp:Literal ID="litNombreCliente" runat="server" Text="" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Fechas de reservación">
                                    <ItemTemplate>
                                        <div>
                                            <strong>
                                                <%# DateTime.Parse(Convert.ToString(Eval("fecha_ini"))).ToLongDateString() %> 
                                            </strong>
                                        </div>
                                        -
                                        <div>
                                            <strong>
                                                <%# DateTime.Parse(Convert.ToString(Eval("fecha_fin"))).ToLongDateString() %>
                                            </strong>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Habitación">
                                    <ItemTemplate>
                                        <label><%# Eval("tipo_habitacion") %></label>
                                        <small class="text-muted d-block">
                                            $ <asp:Literal ID="litCostoHabitacion" runat="server" /> / noche
                                        </small>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="cant_personas" HeaderText="# Personas" />
                                <asp:TemplateField HeaderText="Servicios extras">
                                    <ItemTemplate>
                                        <ul class="list-group">
                                            <asp:Repeater
                                                ID="rpServiciosExtras"
                                                runat="server"
                                                OnItemDataBound="rpServiciosExtras_ItemDataBound"
                                            >
                                                <ItemTemplate>
                                                    <li class="list-group-item d-flex justify-content-between align-items-center">
                                                        <%# Container.DataItem %>
                                                        <span class="badge badge-primary badge-pill">
                                                            $ <asp:Literal ID="litPrecioServicioExtra" runat="server" />
                                                        </span>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <ItemTemplate>
                                        $ <%# Eval("monto_pago") %> 
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>