<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="NDEV_HAPPY_INN.login" Async="true" EnableEventValidation="false" %>

<!DOCTYPE html>

<html lang="es-mx">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Login - Happy Inn</title>
    <link href="/favicon.ico" rel="shortcut icon" type="image/x-icon" />
    <webopt:bundlereference runat="server" path="~/Content/css" />
    <link href="../Content/login/login.css" rel="stylesheet" />
</head>
<body>
    <form id="frm_login" runat="server" class="form-signin">

        <asp:ScriptManager LoadScriptsBeforeUI="true" runat="server" EnableCdn="true">
            <Scripts>
                <asp:ScriptReference Name="MsAjaxBundle" />
                <asp:ScriptReference Path="~/Scripts/jquery/jquery-3.4.1.min.js" />
                <asp:ScriptReference Name="WebForms.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebForms.js" />
                <asp:ScriptReference Name="WebUIValidation.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebUIValidation.js" />
                <asp:ScriptReference Name="MenuStandards.js" Assembly="System.Web" Path="~/Scripts/WebForms/MenuStandards.js" />
                <asp:ScriptReference Name="GridView.js" Assembly="System.Web" Path="~/Scripts/WebForms/GridView.js" />
                <asp:ScriptReference Name="DetailsView.js" Assembly="System.Web" Path="~/Scripts/WebForms/DetailsView.js" />
                <asp:ScriptReference Name="TreeView.js" Assembly="System.Web" Path="~/Scripts/WebForms/TreeView.js" />
                <asp:ScriptReference Name="WebParts.js" Assembly="System.Web" Path="~/Scripts/WebForms/WebParts.js" />
                <asp:ScriptReference Name="Focus.js" Assembly="System.Web" Path="~/Scripts/WebForms/Focus.js" />
                <asp:ScriptReference Name="WebFormsBundle" />
            </Scripts>
        </asp:ScriptManager>

        <asp:UpdatePanel
            ID="upLogin"
            runat="server"
        >
            <ContentTemplate>
                <div class="content text-center">
                    <%--<img class="mb-4" src="/docs/4.4/assets/brand/bootstrap-solid.svg" alt="" width="72" height="72">--%>

                    <asp:Image
                        ID="imgLogo"
                        runat="server"
                        CssClass="mb-4"
                        Width="162"
                        ImageUrl="~/Content/images/logos/emp_logo_b.png"
                    />

                    <h1 class="h3 mb-3 text-light font-weight-normal">
                        Iniciar Sesión
                    </h1>

                    <label for='<%: txtUsuario.ClientID %>' class="sr-only">Usuario</label>
                    <asp:TextBox
                        ID="txtUsuario"
                        runat="server"
                        CssClass="form-control"
                        placeholder="Usuario"
                        required="" 
                        autofocus=""
                    ></asp:TextBox>

                    <label for='<%: txtPassword.ClientID %>' class="sr-only">Contraseña</label>
                    <asp:TextBox
                        ID="txtPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password"
                        placeholder="Contraseña"
                        required="" 
                    ></asp:TextBox>

                    <asp:UpdatePanel
                        ID="upCheckBox"
                        runat="server"
                        UpdateMode="Conditional"
                        class="form-group">
                        <ContentTemplate>
                            <div class="my-1">
                                <div class="custom-control custom-checkbox mr-sm-2">
                                    <asp:CheckBox
                                        ID="chkMantenerSesion"
                                        runat="server"
                                        AutoPostBack="true"
                                        ClientIDMode="Static"
                                        CssClass="custom-control-input" 
                                    />
                                    <label class="custom-control-label text-light" for="chkMantenerSesion">
                                        Mantener Sesión
                                    </label>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <asp:Button
                        ID="btnIniciarSesion"
                        runat="server"
                        CssClass="btn btn-primary btn-block"
                        Text="Entrar"
                        Enabled="false"
                        OnClick="btnIniciarSesion_Click"
                        OnClientClick="onButtonLoading(this, 'Entrando')"
                    />

                    <p class="mt-5 mb-3 text-muted">© Happy Inn 2020</p>
                </div>
            </ContentTemplate>
            <Triggers>
            </Triggers>

        </asp:UpdatePanel>

        <asp:PlaceHolder runat="server">
            <%: Scripts.Render("~/bundles/bootstrap") %>
        </asp:PlaceHolder>

        <script src="../Content/login/login.js"></script>

    </form>
</body>
</html>
