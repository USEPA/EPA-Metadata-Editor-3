Public NotInheritable Class AboutBox

    Private Sub AboutBox_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Text = String.Format("About {0}", My.Settings.CommonEditorAbbreviation)
        ' Initialize all of the text displayed on the About Box.
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersion.Text = String.Format("Full Version {0}", My.Application.Info.Version.ToString)
        Me.LabelCopyright.Text = "Developed for U.S. Environmental Protection Agency by " + My.Application.Info.CompanyName
    End Sub

    ''' <summary>
    ''' Open Coeur d'Alene Tribe website in a browser window.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub pbCDA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbCDA.Click
        System.Diagnostics.Process.Start("http://www.cdatribe-nsn.gov")
    End Sub

    ''' <summary>
    ''' Open EPA website in a browser window.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub pbEPA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbEPA.Click
        System.Diagnostics.Process.Start("http://www.epa.gov")
    End Sub

    ''' <summary>
    ''' Open EPA OEI website in a browser window.
    ''' </summary>
    ''' <param name="sender">Event sender. Not used.</param>
    ''' <param name="e">Event arguments. Not used.</param>
    ''' <remarks></remarks>
    Private Sub pbOEI_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbOEI.Click
        System.Diagnostics.Process.Start("http://www.epa.gov/oei/")
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub
End Class
