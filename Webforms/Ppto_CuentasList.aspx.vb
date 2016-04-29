Imports System.Data
Imports System.Collections.Generic
Imports ingNovar.Utilidades2
Imports Especiales

Partial Class Ppto_CuentasList
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim objper_perfil As New SeguridadUsuarios.Permisos_PerfilBrl
        Dim objper_usuario As New SeguridadUsuarios.Permisos_UsuarioBrl
        '
        ' Ingresa Primero aca
        ' Validamos la seguridad
        '
        ' Perfil
        Dim objusuario As SeguridadUsuarios.UsuariosBrl
        objusuario = SeguridadUsuarios.UsuariosBrl.CargarPorID(CType(Session("IdUsuario"), Integer))
        If objusuario Is Nothing Then
            Response.Redirect(strPaginaError)
            Exit Sub
        End If
        ' Pagina

        Dim objpagina As SeguridadUsuarios.PaginasBrl
        objpagina = SeguridadUsuarios.PaginasBrl.CargarPorURL(Request.FilePath)
        If objpagina Is Nothing Then
            Response.Redirect(strPaginaError)
            Exit Sub
        End If

        ' Permisos por Perfil

        objper_perfil = SeguridadUsuarios.Usuarios.EstadoPerPagina(objusuario.Id_Perfil, objpagina.ID)
        objper_usuario = SeguridadUsuarios.Usuarios.EstadoPerUsuario(objusuario.ID, objpagina.ID)

        If objper_perfil Is Nothing And objper_usuario Is Nothing Then
            Response.Redirect(strPaginaError)
            Exit Sub
        End If


        ' En alguno de los dos hay permisos
        ' Hay datos en la pagina de perfiles, se inicia la validacion de datos
        If objper_perfil IsNot Nothing Then
            If objper_perfil.Pver = False Or objper_perfil.Pconsultar = False Then
                Response.Redirect(strPaginaError)
                Exit Sub
            End If

            ' asignando los permisos
            btn_recargar.Enabled = objper_perfil.Pcrear
            chb_Editar.Checked = objper_perfil.Peditar
            chb_Eliminar.Checked = objper_perfil.Peliminar
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToCsvButton = objper_perfil.Pexportarcsv
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToExcelButton = objper_perfil.Pexportarexcel
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToPdfButton = objper_perfil.Pexportarpdf
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToWordButton = objper_perfil.Pexportarword
        End If

        If objper_usuario IsNot Nothing Then
            If objper_usuario.Pver = False Or objper_usuario.Pconsultar = False Then
                Response.Redirect(strPaginaError)
                Exit Sub
            End If

            ' asignando los permisos
            btn_recargar.Enabled = objper_usuario.Pcrear
            chb_Editar.Checked = objper_usuario.Peditar
            chb_Eliminar.Checked = objper_usuario.Peliminar

            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToCsvButton = objper_usuario.Pexportarcsv
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToExcelButton = objper_usuario.Pexportarexcel
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToPdfButton = objper_usuario.Pexportarpdf
            Rg_Listado.MasterTableView.CommandItemSettings.ShowExportToWordButton = objper_usuario.Pexportarword
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.IsPostBack Then Exit Sub
        If Session("IdUsuario") Is Nothing Then
            Session.RemoveAll()
            Response.Redirect("../login.aspx")
            Exit Sub
        End If
        If Session("Id_Proyecto") Is Nothing Then
            Session.RemoveAll()
            Response.Redirect("../login.aspx")
            Exit Sub
        End If

        Dim lblidUsuario As Label
        lblidUsuario = Master.FindControl("lblidusuario")
        lblidUsuario.Text = Session("IdUsuario")

        Dim LblNombreUsuario As Label
        LblNombreUsuario = Master.FindControl("LblNombreUsuario")
        LblNombreUsuario.Text = Session("NombreUsuario")

        Dim Lbl_regional As Label
        Lbl_regional = Master.FindControl("Lbl_regional")
        Lbl_regional.Text = Session("NombreRegional")

        Dim Lbl_usuario As Label
        Lbl_usuario = Master.FindControl("Lbl_usuario")
        Lbl_usuario.Text = Session("LoginUsuario")

        Dim Lbl_perfil As Label
        Lbl_perfil = Master.FindControl("Lbl_perfil")
        Lbl_perfil.Text = Session("NombrePerfil")

        Dim ListCuentas As List(Of Ppto_CuentasBrl) = Session("ListCuentas")

        'si no hay una variable de sesión con la lista.
        If ListCuentas Is Nothing Or Request.QueryString.Item("Refrescar") = 1 Then
            ListCuentas = Ppto_CuentasBrl.CargarTodos(Session("Id_Proyecto"))
            Session("ListCuentas") = ListCuentas
        End If

        Rg_Listado.DataSource = Session("ListCuentas")
        Rg_Listado.DataBind()

    End Sub

    Protected Sub Rg_Listado_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles Rg_Listado.NeedDataSource
        Rg_Listado.DataSource = Session("ListCuentas")
    End Sub

    Protected Sub btn_home_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_home.Click
        Response.Redirect("../webforms/principal.aspx")
    End Sub

    Protected Sub btn_cerrar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_cerrar.Click
        Response.Redirect("../webforms/principal.aspx")
    End Sub

    Protected Sub btn_buscar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_buscar.Click
        If pnl_buscar.Visible Then
            pnl_buscar.Visible = False
            btn_buscar.ImageUrl = "~/Images/Zoom In.jpg"
        Else
            pnl_buscar.Visible = True
            btn_buscar.ImageUrl = "~/Images/Zoom Out.jpg"
        End If
    End Sub

    Protected Sub btn_actualizar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_actualizar.Click
        Response.Redirect("../webforms/Ppto_CuentasList.aspx?Refrescar=1")
    End Sub

    Protected Sub btn_recargar_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles btn_recargar.Click
        Dim listniveles As List(Of SubTablasBrl) = SubTablasBrl.CargarPorNivelesPorProyecto(Session("Id_Proyecto"))
        Dim objcuenta As New Ppto_CuentasBrl
        Dim ListCuentas As List(Of Ppto_CuentasBrl)
        For Each fila As SubTablasBrl In listniveles
            ListCuentas = Ppto_CuentasBrl.CargarPorId_Nivel(fila.ID)
            If ListCuentas.Count = 0 Then
                ' crear registro
                objcuenta = New Ppto_CuentasBrl
                objcuenta.Id_Nivel = fila.ID
                objcuenta.Descripcion = fila.Descripcion_PadreHijo
                objcuenta.Visible = True
                objcuenta.Guardar()
            End If
        Next
        Response.Redirect("Ppto_CuentasList.aspx?Refrescar=1")
    End Sub

    Public Sub subCommandItem(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        Select Case e.CommandName
            Case "EliminarCuenta"
                subEliminar(sender, e)
            Case "GrabarCuenta"
                subGrabar(sender, e)
        End Select
    End Sub

    Public Sub subEliminar(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        Dim lblId As Label
        lblId = e.Item.FindControl("lblId")
        Dim objdetalle As Ppto_CuentasBrl = Ppto_CuentasBrl.CargarPorID(lblId.Text)
        Try
            objdetalle.Eliminar()
            Response.Redirect("Ppto_CuentasList.aspx?Refrescar=1")
        Catch ex As Exception
        End Try
    End Sub

    Public Sub subGrabar(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs)
        Dim lblId As Label
        lblId = e.Item.FindControl("lblId")
        Dim txt_cuenta As TextBox
        txt_cuenta = e.Item.FindControl("txt_cuenta")
        Dim chk_visible As CheckBox
        chk_visible = e.Item.FindControl("chk_visible")
        If txt_cuenta.Text.Trim <> "" Then
            Dim objdetalle As Ppto_CuentasBrl = Ppto_CuentasBrl.CargarPorID(lblId.Text)
            Try
                objdetalle.Cuenta = txt_cuenta.Text
                objdetalle.Visible = chk_visible.Checked
                objdetalle.Guardar()
                Response.Redirect("Ppto_CuentasList.aspx?Refrescar=1")
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub Rg_Listado_ItemDataBound(ByVal sender As Object, ByVal e As Telerik.Web.UI.GridItemEventArgs) Handles Rg_Listado.ItemDataBound
        If e.Item.ItemType = Telerik.Web.UI.GridItemType.AlternatingItem Or e.Item.ItemType = Telerik.Web.UI.GridItemType.Item Then
            Dim btn_aceptar, btn_eliminar As ImageButton
            btn_aceptar = e.Item.FindControl("btn_aceptar")
            btn_eliminar = e.Item.FindControl("btn_eliminar")
            btn_aceptar.Enabled = chb_Editar.Checked
            btn_eliminar.Enabled = chb_Eliminar.Checked
        End If
    End Sub
End Class
