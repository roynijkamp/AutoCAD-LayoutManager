Imports System.Drawing.Design
Imports System.Drawing.Drawing2D
Imports System.ComponentModel.Design
Imports System.ComponentModel
Imports System.Windows.Forms.Design
'Version 1.0 January 2009

<ToolboxItem(True), ToolboxBitmap(GetType(DDContainer), "DDContainer.DDContainer.bmp")> _
<Designer(GetType(DDContainerDesigner))> _
<DefaultEvent("DropDown")> _
Public Class DDContainer
    Inherits ContainerControl

    Private blnIsResizeOK As Boolean = False
    Private rectTextBox As Rectangle = New Rectangle(0, 0, 107, 20)
    Private rectDropDownButton As Rectangle = New Rectangle(0, 0, 20, 20)
    Private rectPushPin As Rectangle = New Rectangle(0, 0, 20, 20)
    Private PushPinRotate As Short = 90
    Private TSDropDown As New ToolStripDropDown
    Private TSHost As ToolStripControlHost
    Private GripBox As Rectangle

    Public Event DropDown(ByVal sender As Object, ByVal IsOpen As Boolean)
    Public Event TextBoxChanged(ByVal sender As Object)

#Region "Initialize"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.DoubleBuffer, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor, True)

        AddHandler TSDropDown.Closing, AddressOf TSDropDown_Closing
        AddHandler TSDropDown.Opening, AddressOf TSDropDown_Opening

        Me.Width = 300
        Me.Height = 21
        blnIsResizeOK = False
        UpdateTextArea()
    End Sub

    Private Sub DDContainer_HandleCreated(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles Me.HandleCreated

        'I put this here because there is no Load Event
        If Not Me.DesignMode Then

            Me.CloseDesignDropDown()
            Me.Region = Nothing

            blnIsResizeOK = True

            If _DropControl IsNot Nothing Then

                TSHost = New ToolStripControlHost(_DropControl)
                TSHost.Margin = Padding.Empty
                TSHost.Padding = Padding.Empty
                TSHost.AutoSize = False
                TSHost.Size = _DropControl.Size

                TSDropDown.AutoSize = False
                TSDropDown.Size = TSHost.Size
                TSDropDown.Items.Add(TSHost)
                TSDropDown.BackColor = _DDBackColor
                TSDropDown.DropShadowEnabled = _DDShadow
                Me.Controls.Remove(_DropControl)

            End If
        End If

        ResizeMe()
    End Sub

#End Region 'Initialize

#Region "Hide Properties"

    <Browsable(False)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property BackgroundImage() As Boolean
        Get
            Return False 'always false 
        End Get
        Set(ByVal value As Boolean) 'empty 
        End Set
    End Property

    <Browsable(False)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property BackgroundImageLayout() As Boolean
        Get
            Return False 'always false 
        End Get
        Set(ByVal value As Boolean) 'empty 
        End Set
    End Property

    <Browsable(False)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property BorderStyle() As Boolean
        Get
            Return False 'always false 
        End Get
        Set(ByVal value As Boolean) 'empty 
        End Set
    End Property

    <Browsable(False)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property AutoScroll() As Boolean
        Get
            Return False 'always false 
        End Get
        Set(ByVal value As Boolean) 'empty 
        End Set
    End Property

    <Browsable(False)> _
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Shadows Property AutoSize() As Boolean
        Get
            Return False 'always false 
        End Get
        Set(ByVal value As Boolean) 'empty 
        End Set
    End Property

#End Region 'Hide Properties

#Region "Control Properties"

#Region "General"

    Private _DropControl As Control = Nothing

    <Editor(GetType(DropControlsEditor), GetType(UITypeEditor))> _
    <Category("Appearance DropDown")> _
    <Description("Get or Set Control to show in dropdown")> _
    Public Property DropControl() As Control
        Get
            Return _DropControl
        End Get
        Set(ByVal value As Control)
            _DropControl = Nothing
            If value IsNot Nothing Then
                _DropControl = value
                SizeToDropControl()
                AddHandler _DropControl.Resize, AddressOf DropControl_Resize
                AddHandler _DropControl.Move, AddressOf DropControl_Move
            End If
        End Set
    End Property

    Private Sub DropControl_Move(ByVal sender As Object, ByVal e As System.EventArgs)
        SizeToDropControl()
    End Sub

    Private Sub DropControl_Resize(ByVal sender As Object, ByVal e As System.EventArgs)
        SizeToDropControl()
    End Sub

    Private _IsOpen As Boolean = False
    <Category("Appearance DropDown")> _
    <Description("Get or Set if the Panel is in the Open State")> _
    <DefaultValue(False)> _
    Public Property IsOpen() As Boolean
        Get
            Return _IsOpen
        End Get
        Set(ByVal value As Boolean)
            blnIsResizeOK = False
            _IsOpen = value

            If value Then
                If DesignMode Then
                    Me.BringToFront()
                    Me.Width = Math.Max(Me.PanelSize.Width, Me.HeaderWidth)
                    Me.Height = Me.PanelSize.Height + _HeaderHeight + 2
                    UpdateRegion()
                End If
            Else
                If DesignMode Then Me.Width = Me.HeaderWidth
                Me.Height = _HeaderHeight + 1
                Me.Region = Nothing
            End If

            Me.Invalidate()
            blnIsResizeOK = True
        End Set
    End Property

    Private _HeaderHeight As Integer = 20
    <Browsable(False)> _
    <Category("Appearance DropDown")> _
    <Description("Get or Set the width of the Text Window")> _
    <DefaultValue(150)> _
    Public Property HeaderHeight() As Integer
        Get
            Return _HeaderHeight
        End Get
        Set(ByVal value As Integer)
            _HeaderHeight = value
            If Not _IsOpen Then
                blnIsResizeOK = False
                Me.Height = value + 1
                blnIsResizeOK = True
            End If
            UpdateTextArea()
            ResizeMe()
            Me.Invalidate()
        End Set
    End Property

    Private _HeaderWidth As Integer = 200
    <Category("Appearance DropDown")> _
    <Description("Get or Set the width of the Text Window")> _
    <DefaultValue(200)> _
    Public Property HeaderWidth() As Integer
        Get
            Return _HeaderWidth
        End Get
        Set(ByVal value As Integer)
            _HeaderWidth = value
            If Not _IsOpen Then
                blnIsResizeOK = False
                Me.Width = Me.HeaderWidth
                blnIsResizeOK = True
            End If
            UpdateTextArea()
            ResizeMe()
            Me.Invalidate()
        End Set
    End Property

#End Region 'General

#Region "Panel"

    Private _PanelSize As Size = New Size(150, 150)
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Size of the DropDown Panel")> _
    <DefaultValue(GetType(Size), "150, 150")> _
    Public Property PanelSize() As Size
        Get
            Return _PanelSize
        End Get
        Set(ByVal value As Size)
            If _DropControl Is Nothing Then
                _PanelSize = value
            Else
                _PanelSize = Size.Add(_DropControl.Size, Me._DDPadding.Size)
            End If
            If DesignMode And _IsOpen Then
                Me.Width = Me._PanelSize.Width
                Me.Height = Me._PanelSize.Height + _HeaderHeight + 2
                UpdateRegion()
            End If
            Me.Invalidate()
        End Set
    End Property

    Private _DDAlignment As StringAlignment = StringAlignment.Near
    <Category("Appearance DropDown"), _
   Description("Get or Set the horizontal position of the RunTime Dropdown"), _
   DefaultValue(StringAlignment.Near)> _
    Public Property DDAlignment() As StringAlignment
        Get
            Return _DDAlignment
        End Get
        Set(ByVal Value As StringAlignment)
            _DDAlignment = Value
        End Set
    End Property

    Private _DDPadding As New Padding
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Right and Bottom Margin from the controls")> _
    <DefaultValue(GetType(Padding), "0,0,0,0")> _
    Public Property DDPadding() As Padding
        Get
            Return _DDPadding
        End Get
        Set(ByVal value As Padding)
            _DDPadding = value
            SizeToDropControl()
        End Set
    End Property

    Private _DDBackColor As Color = Color.WhiteSmoke
    <Category("Appearance DropDown")> _
    <Description("Get or Set the DropDown BackColor")> _
    <DefaultValue(GetType(Color), "WhiteSmoke")> _
    Public Property DDBackColor() As Color
        Get
            Return _DDBackColor
        End Get
        Set(ByVal value As Color)
            _DDBackColor = value
            TSDropDown.BackColor = _DDBackColor
            Me.Invalidate()
        End Set
    End Property

    Private _DDShadow As Boolean = True
    <Category("Appearance DropDown")> _
    <Description("Get or Set if the RunTime DropDown has a Shadow")> _
    <DefaultValue(True)> _
    Public Property DDShadow() As Boolean
        Get
            Return _DDShadow
        End Get
        Set(ByVal value As Boolean)
            _DDShadow = value
            TSDropDown.DropShadowEnabled = value
        End Set
    End Property

    Private _DDOpacity As Double = 1
    <Category("Appearance DropDown")> _
    <Description("Get or Set The DropDown Opacity (number between 0 and 1)")> _
    <DefaultValue(True)> _
    Public Property DDOpacity() As Double
        Get
            Return _DDOpacity
        End Get
        Set(ByVal value As Double)
            If value > 1 Then value = 1
            If value < 0 Then value = 0
            _DDOpacity = value
            TSDropDown.Opacity = value
        End Set
    End Property

#End Region 'Panel

#Region "Text"

    Private _Text As String = ""
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Text to appear in the Text Box")> _
    <Browsable(True)> _
       Overrides Property Text() As String
        Get
            Return _Text
        End Get
        Set(ByVal value As String)
            _Text = value
            Me.Invalidate(rectTextBox)
            RaiseEvent TextBoxChanged(Me)
        End Set
    End Property

    Private _TextShadow As Boolean = False
    <Category("Appearance DropDown")> _
    <Description("Get or Set if the Text should be Shadowed")> _
    <DefaultValue(False)> _
       Public Property TextShadow() As Boolean
        Get
            Return _TextShadow
        End Get
        Set(ByVal value As Boolean)
            _TextShadow = value
            Me.Invalidate(rectTextBox)
        End Set
    End Property

    Private _TextShadowColor As Color = Color.Gray
    <Category("Appearance DropDown")> _
    <Description("Get or Set if the Color of the Shadowed Text")> _
    <DefaultValue(GetType(Color), "Gray")> _
       Public Property TextShadowColor() As Color
        Get
            Return _TextShadowColor
        End Get
        Set(ByVal value As Color)
            _TextShadowColor = value
            Me.Invalidate(rectTextBox)
        End Set
    End Property

    Private _TextBoxCornerRadius As Integer = 5
    <Category("Appearance DropDown"), _
   Description("Get or Set the Corner Radius of the Text Box"), _
   DefaultValue(5)> _
    Public Property TextBoxCornerRadius() As Integer
        Get
            Return _TextBoxCornerRadius
        End Get
        Set(ByVal Value As Integer)
            If Value < 0 Then
                _TextBoxCornerRadius = 0
            Else
                _TextBoxCornerRadius = Value
            End If
            Me.Invalidate()
        End Set
    End Property

    Enum eGradientType
        Solid
        BackwardDiagonal
        ForwardDiagonal
        Horizontal
        Vertical
    End Enum
    Private _TextBoxGradientType As eGradientType = eGradientType.Solid
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Gradient type to fill the Text Box with")> _
    <DefaultValue(eGradientType.Solid)> _
    Public Property TextBoxGradientType() As eGradientType
        Get
            Return _TextBoxGradientType
        End Get
        Set(ByVal value As eGradientType)
            _TextBoxGradientType = value
            Me.Invalidate()
        End Set
    End Property

    Private _TextBoxBackColorA As Color = Color.White
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Primary Color of the Text Box")> _
    <DefaultValue(GetType(Color), "White")> _
    Public Property TextBoxBackColorA() As Color
        Get
            Return _TextBoxBackColorA
        End Get
        Set(ByVal value As Color)
            _TextBoxBackColorA = value
            Me.Invalidate()
        End Set
    End Property

    Private _TextBoxBackColorB As Color = Color.White
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Secondary Color of the Text Box")> _
    <DefaultValue(GetType(Color), "White")> _
    Public Property TextBoxBackColorB() As Color
        Get
            Return _TextBoxBackColorB
        End Get
        Set(ByVal value As Color)
            _TextBoxBackColorB = value
            Me.Invalidate()
        End Set
    End Property

    Private _TextBoxBorderColor As Color = Color.Gray
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Border Color of the Text Box")> _
    <DefaultValue(GetType(Color), "Gray")> _
    Public Property TextBoxBorderColor() As Color
        Get
            Return _TextBoxBorderColor
        End Get
        Set(ByVal value As Color)
            _TextBoxBorderColor = value
            Me.Invalidate()
        End Set
    End Property

    Private _TextAlignment As StringAlignment = StringAlignment.Center
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Alignment of the Text Box")> _
    <DefaultValue(StringAlignment.Center)> _
    Public Property TextAlignment() As StringAlignment
        Get
            Return _TextAlignment
        End Get
        Set(ByVal value As StringAlignment)
            _TextAlignment = value
            Me.Invalidate()
        End Set
    End Property
#End Region 'Text

#Region "Button"

    Enum eButtonShape
        Square
        Circle
    End Enum

    Private _ButtonShape As eButtonShape = eButtonShape.Circle
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Shape of the DropDown Button")> _
    <DefaultValue(eButtonShape.Circle)> _
    Public Property ButtonShape() As eButtonShape
        Get
            Return _ButtonShape
        End Get
        Set(ByVal value As eButtonShape)
            _ButtonShape = value
            Me.Invalidate()
        End Set
    End Property

    Private _ButtonForeColor As Color = Color.DimGray
    <Category("Appearance DropDown")> _
    <Description("Get or Set the color of the Arrow on the DropDown Button")> _
    <DefaultValue(GetType(Color), "DimGray")> _
    Public Property ButtonForeColor() As Color
        Get
            Return _ButtonForeColor
        End Get
        Set(ByVal value As Color)
            _ButtonForeColor = value
            Me.Invalidate()
        End Set
    End Property

    Private _ButtonBackColor As Color = Color.LightSteelBlue
    <Category("Appearance DropDown")> _
    <Description("Get or Set the base color of the DropDown Button")> _
    <DefaultValue(GetType(Color), "LightSteelBlue")> _
    Public Property ButtonBackColor() As Color
        Get
            Return _ButtonBackColor
        End Get
        Set(ByVal value As Color)
            _ButtonBackColor = value
            Me.Invalidate()
        End Set
    End Property

    Private _ButtonHighlight As Color = Color.White
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Highlight color of the DropDown Button")> _
    <DefaultValue(GetType(Color), "White")> _
    Public Property ButtonHighlight() As Color
        Get
            Return _ButtonHighlight
        End Get
        Set(ByVal value As Color)
            _ButtonHighlight = value
            Me.Invalidate()
        End Set
    End Property

    Private _ButtonBorder As Color = Color.Navy
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Border Color of the DropDown Button")> _
    <DefaultValue(GetType(Color), "Navy")> _
    Public Property ButtonBorder() As Color
        Get
            Return _ButtonBorder
        End Get
        Set(ByVal value As Color)
            _ButtonBorder = value
            Me.Invalidate()
        End Set
    End Property

#End Region 'Button

#Region "PushPin"
    Private _Pin As Color = Color.Gray
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Push Pin pin Color")> _
    <DefaultValue(GetType(Color), "Gray")> _
    Public Property Pin() As Color
        Get
            Return _Pin
        End Get
        Set(ByVal value As Color)
            _Pin = value
            Me.Invalidate(rectPushPin)
        End Set
    End Property

    Private _PinBody As Color = Color.CornflowerBlue
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Push Pin Body Color")> _
    <DefaultValue(GetType(Color), "CornflowerBlue")> _
    Public Property PinBody() As Color
        Get
            Return _PinBody
        End Get
        Set(ByVal value As Color)
            _PinBody = value
            Me.Invalidate(rectPushPin)
        End Set
    End Property

    Private _PinHighlight As Color = Color.White
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Push Pin Body Highlight Color")> _
    <DefaultValue(GetType(Color), "White")> _
    Public Property PinHighlight() As Color
        Get
            Return _PinHighlight
        End Get
        Set(ByVal value As Color)
            _PinHighlight = value
            Me.Invalidate(rectPushPin)
        End Set
    End Property

    Private _PinOutline As Color = Color.Navy
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Push Pin Body Outline Color")> _
    <DefaultValue(GetType(Color), "Navy")> _
    Public Property PinOutline() As Color
        Get
            Return _PinOutline
        End Get
        Set(ByVal value As Color)
            _PinOutline = value
            Me.Invalidate(rectPushPin)
        End Set
    End Property

    Private _ShowPushPin As Boolean = False
    <Category("Appearance DropDown")> _
    <Description("Get or Set If the Push Pin is Visible")> _
    <DefaultValue(False)> _
    Public Property ShowPushPin() As Boolean
        Get
            Return _ShowPushPin
        End Get
        Set(ByVal value As Boolean)
            _ShowPushPin = value
            UpdateTextArea()
            Me.Invalidate()
        End Set
    End Property

    Enum ePushPinState
        Up = 0
        Down = -1
    End Enum
    Private _PushPinState As ePushPinState = ePushPinState.Up
    <Category("Appearance DropDown")> _
    <Description("Get or Set If the Push Pin is Up or Down")> _
    <DefaultValue(ePushPinState.Up)> _
    Public Property PushPinState() As ePushPinState
        Get
            Return _PushPinState
        End Get
        Set(ByVal value As ePushPinState)
            _PushPinState = value
            Me.Invalidate()
        End Set
    End Property

#End Region 'PushPin

#Region "Graphic"

    Private _GraphicBorderColor As Color = Color.Gray
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Border Color around the Graphic")> _
    <DefaultValue(GetType(Color), "Gray")> _
    Public Property GraphicBorderColor() As Color
        Get
            Return _GraphicBorderColor
        End Get
        Set(ByVal value As Color)
            _GraphicBorderColor = value
            Me.Invalidate()
        End Set
    End Property

    Private _GraphicImage As Bitmap = Nothing
    <Category("Appearance DropDown")> _
    <Description("Get or Set the Image next to the Text Box")> _
    Public Property GraphicImage() As Bitmap
        Get
            Return _GraphicImage
        End Get
        Set(ByVal value As Bitmap)
            _GraphicImage = value
            UpdateTextArea()
            Me.Invalidate(New Rectangle(0, 0, _HeaderWidth, _HeaderHeight + 1))
        End Set
    End Property

    Private _GraphicWidth As Integer = 30
    <Category("Appearance DropDown")> _
    <Description("Get or Set the width of the Graphic Image width")> _
    <DefaultValue(30)> _
    Public Property GraphicWidth() As Integer
        Get
            Return _GraphicWidth
        End Get
        Set(ByVal value As Integer)
            _GraphicWidth = value
            UpdateTextArea()
            Me.Invalidate(New Rectangle(0, 0, _HeaderWidth, _HeaderHeight + 1))

        End Set
    End Property

    Private _GraphicAutoWidth As Boolean = True
    <Category("Appearance DropDown")> _
    <Description("Get or Set to Automatically Size the Width from the Image Aspect Ratio")> _
    <DefaultValue(True)> _
    Public Property GraphicAutoWidth() As Boolean
        Get
            Return _GraphicAutoWidth
        End Get
        Set(ByVal value As Boolean)
            _GraphicAutoWidth = value
            UpdateTextArea()
            Me.Invalidate(New Rectangle(0, 0, _HeaderWidth, _HeaderHeight + 1))
        End Set
    End Property

#End Region 'Graphic

#End Region 'Control Properties

#Region "ToolStripDropDown"

    Private Sub TSDropDown_Opening(ByVal sender As Object, ByVal e As CancelEventArgs)
        RaiseEvent DropDown(Me, True)
    End Sub

    Private Sub TSDropDown_Closing(ByVal sender As Object, _
      ByVal e As ToolStripDropDownClosingEventArgs)
        Try
            If (Not GetButtonPath.IsVisible(PointToClient(Control.MousePosition)) _
                Or (e.CloseReason = ToolStripDropDownCloseReason.Keyboard)) _
                And Not CBool(_PushPinState) Then
                IsOpen = False
            End If

            e.Cancel = CBool(_PushPinState)

            If Not e.Cancel Then
                Me.Invalidate()
                RaiseEvent DropDown(Me, False)

            End If
        Catch ex As Exception

        End Try
    End Sub


#End Region 'ToolStripDropDown

#Region "Mouse Events"
    Private ButtonHighlightAdjust As Integer = 4
    Private MouseDownButton As Boolean = False

    Private Sub DDContainer_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If e.Button = System.Windows.Forms.MouseButtons.Left Then
            If e.Y < _HeaderHeight Then
                If GetButtonPath.IsVisible(e.Location) Then
                    If PushPinState = ePushPinState.Down Then Exit Sub
                    MouseDownButton = True
                    ButtonHighlightAdjust = 16
                    Me.Invalidate(rectDropDownButton)

                    'RunTime Only
                    If Not DesignMode Then
                        If IsOpen Then
                            If Not CBool(_PushPinState) Then
                                CloseDropDown()
                            End If
                        Else
                            OpenDropDown()
                        End If
                    End If
                ElseIf ShowPushPin And Not DesignMode Then
                    If rectPushPin.Contains(e.Location) Then
                        PushPinState = Not PushPinState
                        PushPinTimer.Start()

                        If CBool(_PushPinState) Then
                            OpenDropDown()
                        Else
                            CloseDropDown()
                        End If
                        Me.Invalidate(rectPushPin)

                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DDContainer_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If MouseDownButton Then
            If DesignMode Then 'DesignTime Only
                IsOpen = Not IsOpen
                'Redraw the Selection Rectangle
                Dim selectservice As ISelectionService = _
                    CType(GetService(GetType(ISelectionService)), ISelectionService)
                Dim selection As New ArrayList
                selection.Clear()
                selectservice.SetSelectedComponents(selection, SelectionTypes.Replace)
                selection.Add(Me)
                selectservice.SetSelectedComponents(selection, SelectionTypes.Add)
            End If
            MouseDownButton = False
        End If
        ButtonHighlightAdjust = 4
        Me.Invalidate(rectDropDownButton)
    End Sub

#End Region 'Mouse Events

#Region "Painting"

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        'Draw the Graphic if available and resize
        If _GraphicImage IsNot Nothing Then
            Dim GW As Integer = CInt(IIf(_GraphicAutoWidth, _
                 _HeaderHeight * (_GraphicImage.Width / _GraphicImage.Height) _
                 , _GraphicWidth))
            e.Graphics.DrawImage(_GraphicImage, 0, 0, GW, _HeaderHeight)
            e.Graphics.DrawRectangle(New Pen(_GraphicBorderColor), _
                                     0, 0, GW, _HeaderHeight - 1)
        End If

        'Draw the Text Box
        If rectTextBox.Width > _TextBoxCornerRadius * 2 Then DrawTextBox(e.Graphics)

        'Draw the Drop Down Button
        DrawDropDownButton(e.Graphics)

        'Draw the Push Pin
        If _ShowPushPin Then DrawPushPin(e.Graphics)

        'Adjust any miss placed control positioned on the Header
        For Each c As Control In Me.Controls
            If c.Location.Y < _HeaderHeight + 1 Then
                If CStr(c.Tag) <> "IgnoreMe" Then
                    c.Location = New Point(c.Location.X, _HeaderHeight + 1)
                End If
            End If
        Next
    End Sub

    Sub DrawTextBox(ByRef g As Graphics)
        If Me._TextBoxGradientType = eGradientType.Solid Then
            g.FillPath(New SolidBrush(_TextBoxBackColorA), _
                GetRectPath(rectTextBox, TextBoxCornerRadius))
        Else
            Using lgbr As LinearGradientBrush = New LinearGradientBrush _
              (rectTextBox, _TextBoxBackColorA, _TextBoxBackColorB, _
              CType([Enum].Parse(GetType(LinearGradientMode), _
              _TextBoxGradientType.ToString), LinearGradientMode))

                g.FillPath(lgbr, GetRectPath(rectTextBox, TextBoxCornerRadius))
            End Using
        End If

        g.DrawPath(New Pen(_TextBoxBorderColor), GetRectPath(rectTextBox, TextBoxCornerRadius))

        'Draw the Text if Available
        If _Text <> "" Then

            Using sf As StringFormat = New StringFormat
                sf.Alignment = _TextAlignment
                sf.LineAlignment = StringAlignment.Center
                sf.FormatFlags = StringFormatFlags.NoWrap
                sf.Trimming = StringTrimming.EllipsisCharacter
                If _TextShadow Then
                    Dim Shadow As Rectangle = rectTextBox
                    Shadow.Offset(1, 1)
                    g.DrawString(_Text, Me.Font, _
                        New SolidBrush(_TextShadowColor), Shadow, sf)
                End If
                g.DrawString(_Text, Me.Font, _
                    New SolidBrush(Me.ForeColor), rectTextBox, sf)
            End Using

        End If
    End Sub

    Sub DrawDropDownButton(ByRef g As Graphics)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Using pn As Pen = New Pen(_ButtonForeColor, 2)
            pn.StartCap = LineCap.Round
            pn.EndCap = LineCap.Round
            Dim gp As New GraphicsPath
            Dim gpButton As GraphicsPath = GetButtonPath()
            If IsOpen Then
                gp.AddLine(rectDropDownButton.X + 5, _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.59)), _
                           CInt(rectDropDownButton.X + (rectDropDownButton.Width / 2)), _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.39)))
                gp.AddLine(CInt(rectDropDownButton.X + (rectDropDownButton.Width / 2)), _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.39)), _
                           rectDropDownButton.X + rectDropDownButton.Width - 5, _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.59)))
            Else
                gp.AddLine(rectDropDownButton.X + 5, _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.39)), _
                           CInt(rectDropDownButton.X + (rectDropDownButton.Width / 2)), _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.59)))
                gp.AddLine(CInt(rectDropDownButton.X + (rectDropDownButton.Width / 2)), _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.59)), _
                           rectDropDownButton.X + rectDropDownButton.Width - 5, _
                           CInt(rectDropDownButton.Y + (rectDropDownButton.Height * 0.39)))
            End If
            Using pgbr As PathGradientBrush = New PathGradientBrush(gpButton)
                pgbr.CenterColor = _ButtonHighlight
                pgbr.CenterPoint = New PointF(rectDropDownButton.X + ButtonHighlightAdjust, _
                                              rectDropDownButton.Y + ButtonHighlightAdjust)
                pgbr.SurroundColors = New Color() {_ButtonBackColor}
                g.FillPath(pgbr, gpButton)
            End Using
            g.DrawPath(pn, gp)
            g.DrawPath(New Pen(_ButtonBorder), gpButton)

            gpButton.Dispose()
            gp.Dispose()
        End Using
    End Sub

    Function GetButtonPath() As GraphicsPath
        Dim gp As New GraphicsPath
        If ButtonShape = eButtonShape.Circle Then
            gp.AddEllipse(rectDropDownButton)
        Else
            gp.AddRectangle(rectDropDownButton)
        End If
        Return gp
    End Function

    Sub DrawPushPin(ByRef g As Graphics)
        g.SmoothingMode = SmoothingMode.AntiAlias
        Dim gp As New GraphicsPath
        Dim mx As New Matrix
        mx.RotateAt(PushPinRotate, New Point(rectPushPin.X + 10, 9))
        g.Transform = mx

        gp.FillMode = FillMode.Winding
        gp.AddEllipse(rectPushPin.X + 6, 0, 8, 4)
        gp.AddEllipse(rectPushPin.X + 3, 7, 14, 6)
        gp.AddRectangle(New Rectangle(rectPushPin.X + 7, 3, 6, 8))
        g.FillPath(New LinearGradientBrush _
            (New Rectangle(rectPushPin.X + 2, 0, 14, 18), _
            _PinHighlight, _PinBody, _
             LinearGradientMode.Horizontal), gp)

        Using pn As Pen = New Pen(_Pin, 3)
            pn.EndCap = LineCap.Triangle
            g.DrawLine(pn, rectPushPin.X + 10, 13, rectPushPin.X + 10, 18)
        End Using

        gp.Reset()
        gp.AddEllipse(rectPushPin.X + 6, 0, 8, 4)
        gp.AddArc(rectPushPin.X + 3, 7, 14, 6, 326, 246)
        gp.StartFigure()
        gp.AddLine(rectPushPin.X + 7, 3, rectPushPin.X + 7, 9)
        gp.StartFigure()
        gp.AddLine(rectPushPin.X + 13, 3, rectPushPin.X + 13, 9)
        gp.StartFigure()
        gp.AddArc(rectPushPin.X + 7, 7, 6, 3, 0, 150)
        g.DrawPath(New Pen(_PinOutline, 1), gp)

        gp.Dispose()
        mx.Dispose()
    End Sub

    Private Sub PushPinTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PushPinTimer.Tick
        PushPinRotate += CShort(CShort(IIf(CBool(_PushPinState), -1, 1)) * 5)
        Me.Invalidate(rectPushPin)

        If _PushPinState = ePushPinState.Down Then
            If PushPinRotate <= 0 Then PushPinTimer.Stop()
        Else
            If PushPinRotate >= 90 Then PushPinTimer.Stop()
        End If
    End Sub

    Public Function GetRectPath(ByVal BaseRect As RectangleF, ByVal CornerRadius As Integer) As GraphicsPath
        Dim ArcRect As RectangleF
        Dim MyPath As New GraphicsPath()
        If CornerRadius = 0 Then
            MyPath.AddRectangle(BaseRect)
        Else
            With MyPath
                ArcRect = New RectangleF(BaseRect.Location, _
                    New SizeF(CornerRadius * 2, CornerRadius * 2))
                ' top left arc
                .AddArc(ArcRect, 180, 90)

                ' top right arc
                ArcRect.X = BaseRect.Right - (CornerRadius * 2)
                .AddArc(ArcRect, 270, 90)

                ' bottom right arc
                ArcRect.Y = BaseRect.Bottom - (CornerRadius * 2)
                .AddArc(ArcRect, 0, 90)

                ' bottom left arc
                ArcRect.X = BaseRect.Left
                .AddArc(ArcRect, 90, 90)

                .CloseFigure()
            End With
        End If

        Return MyPath
    End Function

#End Region 'Painting

#Region "Methods"

    Private Sub UpdateTextArea()
        Dim vbit As Integer = 0
        Dim ShowPin As Integer = 0

        If _GraphicImage IsNot Nothing Then
            vbit = CInt(IIf(_GraphicAutoWidth, _HeaderHeight * (_GraphicImage.Width / _GraphicImage.Height), _GraphicWidth)) + 3
        End If

        If _ShowPushPin Then
            rectPushPin.X = _HeaderWidth - rectPushPin.Width
            rectDropDownButton.X = rectPushPin.X - 22
            ShowPin = rectPushPin.Width
        Else
            rectDropDownButton.X = _HeaderWidth - 22
        End If
        rectTextBox = New Rectangle(vbit, 0, _HeaderWidth - rectDropDownButton.Width - ShowPin - vbit - 5, _HeaderHeight)

    End Sub

    Private Sub UpdateRegion()
        Me.Region = Nothing
        Dim rgn As Region = New Region(New Rectangle(0, 0, Me.HeaderWidth, _HeaderHeight + 1))
        rgn.Union(New RectangleF(0, _HeaderHeight + 2, Me.PanelSize.Width, Me.PanelSize.Height))
        Me.Region = rgn.Clone
        rgn.Dispose()
    End Sub

#Region "Dropdown - RunTime"
    Private HorzPos As Short
    Private VertPos As Short

    Public Sub OpenDropDown()
        If TSHost IsNot Nothing Then
            VertPos = CShort(Me.rectTextBox.Bottom + 2)
            Dim TSDDD As ToolStripDropDownDirection = ToolStripDropDownDirection.BelowRight
            Select Case _DDAlignment
                Case StringAlignment.Far
                    HorzPos = CShort(Me.Width - Me.TSHost.Width)
                Case StringAlignment.Near
                    HorzPos = 0
                Case StringAlignment.Center
                    HorzPos = CShort((Me.Width - Me.TSHost.Width) / 2)
            End Select
            Try
                If Me.Location.Y + Me.ParentForm.Location.Y + VertPos + Me.TSHost.Height > Screen.FromControl(Me).WorkingArea.Bottom - 35 Then
                    VertPos = CShort(Me.rectTextBox.Top - 2)
                    TSDDD = ToolStripDropDownDirection.AboveRight
                End If
            Catch ex As Exception

            End Try
            Me.TSDropDown.Show(Me, New Point(HorzPos, VertPos), TSDDD)
            IsOpen = True
        End If
    End Sub

    Public Sub CloseDropDown()
        If PushPinState = ePushPinState.Up Then
            Me.TSDropDown.Hide()
            IsOpen = False
        End If
    End Sub

    Public Sub ForceCloseDropDown()
        Me.TSDropDown.Hide()
        IsOpen = False
        If PushPinState = ePushPinState.Down Then
            PushPinState = ePushPinState.Up
            PushPinTimer.Start()
        End If
    End Sub

#End Region 'Dropdown - RunTime

#Region "Dropdown - DesignTime"

    Private Sub CloseDesignDropDown()
        IsOpen = False
    End Sub

    Private Sub OpenDesignDropDown()
        IsOpen = True
    End Sub

#End Region 'Dropdown - DesignTime

    Private Sub ResizeMe()
        blnIsResizeOK = False
        If _IsOpen Then
            If DesignMode Then
                If Me.Width < Me.HeaderWidth Then Me.Width = Me.HeaderWidth
                If Me.Height < _HeaderHeight Then Me.Height = _HeaderHeight
                UpdateRegion()
            End If
        Else
            Dim PushPin As Integer = CInt(IIf(_ShowPushPin, _HeaderHeight, 0))
            If Me.Width < 21 + PushPin Then Me.Width = 21 + PushPin
            If Me.HeaderWidth <> Me.Width Then Me.HeaderWidth = Me.Width
            Me.Region = Nothing
        End If
        blnIsResizeOK = True
    End Sub

    Private Sub SizeToDropControl()
        If _DropControl IsNot Nothing Then
            Me.PanelSize = Size.Add(_DropControl.Size, Me._DDPadding.Size)

            If DesignMode Then
                _DropControl.Location = New Point(Me._DDPadding.Left, _HeaderHeight + 2 + Me._DDPadding.Top)
            Else
                TSDropDown.Size = Size.Add(Me.PanelSize, New Size(2, 2))
                _DropControl.Location = New Point(Me._DDPadding.Left + 1, Me._DDPadding.Top + 1)
            End If
        End If
    End Sub
#End Region 'Methods

#Region "Control Events"

    Private Sub DDContainer_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'Block resizing when the parent form minimizes 
        Try
            If Not DesignMode Then
                ForceCloseDropDown()
            End If

            If Me.FindForm.WindowState <> FormWindowState.Minimized And blnIsResizeOK Then
                ResizeMe()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DDContainer_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDoubleClick
        If Not _IsOpen AndAlso Me.rectTextBox.Contains(e.Location) Then OpenDropDown()
    End Sub

    Private Sub DDContainer_ControlRemoved(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs) Handles Me.ControlRemoved
        If DesignMode And e.Control Is _DropControl Then
            RemoveHandler _DropControl.Resize, AddressOf DropControl_Resize
            RemoveHandler _DropControl.Move, AddressOf DropControl_Move
            _DropControl = Nothing
        End If
    End Sub

#End Region 'Control Events

End Class

#Region "DDContainer SmartTag"

#Region "DDContainerDesigner"

Public Class DDContainerDesigner

    Inherits ScrollableControlDesigner

    Protected Overrides Function GetHitTest(ByVal point As System.Drawing.Point) As Boolean
        Dim DDC As DDContainer = CType(Component, DDContainer)
        point = DDC.PointToClient(point)
        Return DDC.GetButtonPath.IsVisible(point)
    End Function

    Public Overrides ReadOnly Property SelectionRules() _
      As System.Windows.Forms.Design.SelectionRules
        Get
            Dim DDC As DDContainer = CType(Component, DDContainer)
            If DDC.IsOpen Then
                Return MyBase.SelectionRules
            Else
                Return SelectionRules.LeftSizeable _
                       Or SelectionRules.RightSizeable _
                       Or System.Windows.Forms.Design.SelectionRules.Visible _
                       Or System.Windows.Forms.Design.SelectionRules.Moveable
            End If
        End Get
    End Property

    Protected Overrides Sub OnPaintAdornments _
      (ByVal pe As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaintAdornments(pe)
        Dim DDC As DDContainer = CType(Component, DDContainer)
        If DDC.IsOpen Then
            Dim rect As Rectangle = New Rectangle(0, DDC.HeaderHeight + 2, _
                DDC.PanelSize.Width - 1, DDC.PanelSize.Height - 1)
            Using g As Graphics = pe.Graphics
                g.FillRectangle(New SolidBrush(DDC.DDBackColor), rect)
                Using pn As Pen = New Pen(Color.Gray, 1)
                    pn.DashStyle = DashStyle.Dash
                    g.DrawRectangle(pn, rect)
                End Using
            End Using
        End If
    End Sub

#Region "ActionLists"

    Private _Lists As DesignerActionListCollection

    Public Overrides ReadOnly Property ActionLists() As System.ComponentModel.Design.DesignerActionListCollection
        Get
            If _Lists Is Nothing Then
                _Lists = New DesignerActionListCollection
                _Lists.Add(New DDPActionList(Me.Component))
            End If
            Return _Lists
        End Get
    End Property

#End Region 'ActionLists

End Class

#End Region 'DDContainerDesigner

#Region "DDPActionList"

Public Class DDPActionList
    Inherits DesignerActionList

    Private _DDCSelector As DDContainer
    Private _DesignerService As DesignerActionUIService = Nothing

    Public Sub New(ByVal component As IComponent)
        MyBase.New(component)

        ' Save a reference to the control we are designing.
        _DDCSelector = DirectCast(component, DDContainer)

        ' Save a reference to the DesignerActionUIService
        _DesignerService = _
            CType(GetService(GetType(DesignerActionUIService)), _
            DesignerActionUIService)

        'Makes the Smart Tags open automatically 
        Me.AutoShow = True
    End Sub

#Region "Smart Tag Items"

#Region "Properties"

#Region "General"

    <Editor(GetType(DropControlsEditor), GetType(UITypeEditor))> _
    Public Property DropControl() As Control
        Get
            Return _DDCSelector.DropControl
        End Get
        Set(ByVal value As Control)
            SetControlProperty("DropControl", value)
        End Set
    End Property

#End Region 'General

#Region "TextBox"

    Public Property TextBoxGradientType() As DDContainer.eGradientType
        Get
            Return _DDCSelector.TextBoxGradientType
        End Get
        Set(ByVal value As DDContainer.eGradientType)
            SetControlProperty("TextBoxGradientType", value)
            _DesignerService.HideUI(_DDCSelector)
            _DesignerService.ShowUI(_DDCSelector)
        End Set
    End Property

    Public Property TextBoxBackColorA() As Color
        Get
            Return _DDCSelector.TextBoxBackColorA
        End Get
        Set(ByVal value As Color)
            SetControlProperty("TextBoxBackColorA", value)
        End Set
    End Property

    Public Property TextBoxBackColorB() As Color
        Get
            Return _DDCSelector.TextBoxBackColorB
        End Get
        Set(ByVal value As Color)
            SetControlProperty("TextBoxBackColorB", value)
        End Set
    End Property

    Public Property TextBoxBorderColor() As Color
        Get
            Return _DDCSelector.TextBoxBorderColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("TextBoxBorderColor", value)
        End Set
    End Property

    Public Property TextBoxCornerRadius() As Integer
        Get
            Return _DDCSelector.TextBoxCornerRadius
        End Get
        Set(ByVal value As Integer)
            SetControlProperty("TextBoxCornerRadius", value)
        End Set
    End Property

    Public Property TextAlignment() As StringAlignment
        Get
            Return _DDCSelector.TextAlignment
        End Get
        Set(ByVal value As StringAlignment)
            SetControlProperty("TextAlignment", value)
        End Set
    End Property

    Public Property ForeColor() As Color
        Get
            Return _DDCSelector.ForeColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("ForeColor", value)
        End Set
    End Property

    Public Property TextShadow() As Boolean
        Get
            Return _DDCSelector.TextShadow
        End Get
        Set(ByVal value As Boolean)
            SetControlProperty("TextShadow", value)
            _DesignerService.HideUI(_DDCSelector)
            _DesignerService.ShowUI(_DDCSelector)
        End Set
    End Property

    Public Property TextShadowColor() As Color
        Get
            Return _DDCSelector.TextShadowColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("TextShadowColor", value)
        End Set
    End Property

#End Region 'TextBox

#Region "PushPin"

    Public Property ShowPushPin() As Boolean
        Get
            Return _DDCSelector.ShowPushPin
        End Get
        Set(ByVal value As Boolean)
            SetControlProperty("ShowPushPin", value)
            _DesignerService.HideUI(_DDCSelector)
            _DesignerService.ShowUI(_DDCSelector)
        End Set
    End Property

    Public Property PinBody() As Color
        Get
            Return _DDCSelector.PinBody
        End Get
        Set(ByVal value As Color)
            SetControlProperty("PinBody", value)
        End Set
    End Property

    Public Property PinHighlight() As Color
        Get
            Return _DDCSelector.PinHighlight
        End Get
        Set(ByVal value As Color)
            SetControlProperty("PinHighlight", value)
        End Set
    End Property

    Public Property PinOutline() As Color
        Get
            Return _DDCSelector.PinOutline
        End Get
        Set(ByVal value As Color)
            SetControlProperty("PinOutline", value)
        End Set
    End Property

    Public Property Pin() As Color
        Get
            Return _DDCSelector.Pin
        End Get
        Set(ByVal value As Color)
            SetControlProperty("Pin", value)
        End Set
    End Property

#End Region 'PushPin

#Region "Drop Down Button"

    Public Property ButtonShape() As DDContainer.eButtonShape
        Get
            Return _DDCSelector.ButtonShape
        End Get
        Set(ByVal value As DDContainer.eButtonShape)
            SetControlProperty("ButtonShape", value)
        End Set
    End Property

    Public Property ButtonForeColor() As Color
        Get
            Return _DDCSelector.ButtonForeColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("ButtonForeColor", value)
        End Set
    End Property

    Public Property ButtonBackColor() As Color
        Get
            Return _DDCSelector.ButtonBackColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("ButtonBackColor", value)
        End Set
    End Property

    Public Property ButtonHighlight() As Color
        Get
            Return _DDCSelector.ButtonHighlight
        End Get
        Set(ByVal value As Color)
            SetControlProperty("ButtonHighlight", value)
        End Set
    End Property

    Public Property ButtonBorder() As Color
        Get
            Return _DDCSelector.ButtonBorder
        End Get
        Set(ByVal value As Color)
            SetControlProperty("ButtonBorder", value)
        End Set
    End Property

#End Region 'Drop Down Button

#Region "Graphic"

    Public Property GraphicBorderColor() As Color
        Get
            Return _DDCSelector.GraphicBorderColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("GraphicBorderColor", value)
        End Set
    End Property

    Public Property GraphicWidth() As Integer
        Get
            Return _DDCSelector.GraphicWidth
        End Get
        Set(ByVal value As Integer)
            SetControlProperty("GraphicWidth", value)
        End Set
    End Property

    Public Property GraphicAutoWidth() As Boolean
        Get
            Return _DDCSelector.GraphicAutoWidth
        End Get
        Set(ByVal value As Boolean)
            SetControlProperty("GraphicAutoWidth", value)
            _DesignerService.Refresh(_DDCSelector)
        End Set
    End Property

#End Region 'Graphic

#Region "DropDown"

    Public Property DDBackColor() As Color
        Get
            Return _DDCSelector.DDBackColor
        End Get
        Set(ByVal value As Color)
            SetControlProperty("DDBackColor", value)
        End Set
    End Property

    Public Property DDAlignment() As StringAlignment
        Get
            Return _DDCSelector.DDAlignment
        End Get
        Set(ByVal value As StringAlignment)
            SetControlProperty("DDAlignment", value)
        End Set
    End Property

    Public Property DDShadow() As Boolean
        Get
            Return _DDCSelector.DDShadow
        End Get
        Set(ByVal value As Boolean)
            SetControlProperty("DDShadow", value)
        End Set
    End Property

#End Region 'DropDown

    'For SmartTag
    Public ReadOnly Property CurrDDC() As DDContainer
        Get
            Return _DDCSelector
        End Get
    End Property

#End Region 'Properties

#Region "Methods"

    Public Sub OpenClosePanel()
        _DDCSelector.IsOpen = Not _DDCSelector.IsOpen
        SetControlProperty("IsOpen", _DDCSelector.IsOpen)
        'forces the control and Designer to refresh
        _DesignerService.Refresh(_DDCSelector)
        _DDCSelector.Refresh()
    End Sub

    ' Set a control property. This method makes Undo/Redo
    ' work properly and marks the form as modified in the IDE.
    Private Sub SetControlProperty(ByVal property_name As String, ByVal value As Object)
        TypeDescriptor.GetProperties(_DDCSelector) _
            (property_name).SetValue(_DDCSelector, value)
    End Sub

#End Region 'Methods

#Region "Action Items"

    ' Return the smart tag action items.
    Public Overrides Function GetSortedActionItems() As System.ComponentModel.Design.DesignerActionItemCollection
        Dim items As New DesignerActionItemCollection()

        items.Add( _
            New DesignerActionMethodItem( _
                  Me, _
                "OpenClosePanel", _
                CStr(IIf(_DDCSelector.IsOpen, "Close Panel", "Open Panel")), _
                "", _
                "Open or Close Panel", _
                True))
        items.Add( _
            New DesignerActionPropertyItem( _
                "DropControl", _
                "Drop Control", _
                "", _
                "Choose the control for the dropdown"))

        'Drop Down Button
        items.Add(New DesignerActionHeaderItem("Drop Down Button"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ButtonShape", _
                "Button Shape", _
                "Drop Down Button", _
                "Shape of Drop Down Button"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ButtonForeColor", _
                "Arrow Color", _
                "Drop Down Button", _
                "Color for the Arrow on the Button"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ButtonBackColor", _
                "Button Color", _
                "Drop Down Button", _
                "Color for the Button"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ButtonHighlight", _
                "HighLight Color", _
                "Drop Down Button", _
                "Color for the Button HighLight"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ButtonBorder", _
                "Border Color", _
                "Drop Down Button", _
                "Color for the Button's Border"))

        'PushPin
        items.Add(New DesignerActionHeaderItem("PushPin"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ShowPushPin", _
                "Show the Push Pin", _
                "PushPin", _
                "Show or Not Show the Push Pin"))
        If _DDCSelector.ShowPushPin Then
            items.Add( _
                New DesignerActionPropertyItem( _
                    "PinBody", _
                    "PushPin Body", _
                    "PushPin", _
                    "Color for the main Push Pin body"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "PinHighlight", _
                    "PushPin Highlight", _
                    "PushPin", _
                    "Highlight Color for the main Push Pin body"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "PinOutline", _
                    "PushPin Outline", _
                    "PushPin", _
                    "Outline Color for the main Push Pin body"))
            items.Add( _
                New DesignerActionPropertyItem( _
                    "Pin", _
                    "Pin color", _
                    "PushPin", _
                    "Color for the pointy part of the Push Pin"))
        End If

        'Text Area
        items.Add(New DesignerActionHeaderItem("Text Area"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextAlignment", _
                "Alignment of Text", _
                "Text Area", _
                "The alignment of the Text in the Text Box"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "ForeColor", _
                "Color of Text", _
                "Text Area", _
                "The color to Text in the Text Box"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextBoxGradientType", _
                "Gradient Type", _
                "Text Area", _
                "The type of gradient to use on the Fill"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextBoxBackColorA", _
                "Fill Color A", _
                "Text Area", _
                "The color to fill Text Box with"))
        If _DDCSelector.TextBoxGradientType <> DDContainer.eGradientType.Solid Then
            items.Add( _
                New DesignerActionPropertyItem( _
                    "TextBoxBackColorB", _
                    "Fill Color B", _
                    "Text Area", _
                    "The second color to fill Text Box with"))
        End If
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextBoxBorderColor", _
                "Border Color", _
                "Text Area", _
                "The color of the border around the Text Box"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextBoxCornerRadius", _
                "TextBox Corner Radius", _
                "Text Area", _
                "The amount to curve the corners of the border around the Text Box"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "TextShadow", _
                "Text Shadow", _
                "Text Area", _
                "Show a Text Shadow"))
        If _DDCSelector.TextShadow Then
            items.Add( _
                New DesignerActionPropertyItem( _
                    "TextShadowColor", _
                    "Shadow Color", _
                    "Text Area", _
                    "Color of the Text Shadow"))
        End If

        'DropDown
        items.Add(New DesignerActionHeaderItem("DropDown"))

        items.Add( _
            New DesignerActionPropertyItem( _
                "DDBackColor", _
                "DropDown BackColor", _
                "DropDown", _
                "The DropDown BackColor that shows when the padding is used"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "DDAlignment", _
                "DropDown Alignment", _
                "DropDown", _
                "The RunTime DropDown opens Near, Center, or Far"))
        items.Add( _
            New DesignerActionPropertyItem( _
                "DDShadow", _
                "DropDown Shadow", _
                "DropDown", _
                "Use the RunTime DropDown Shadow effect"))

        'Graphic
        items.Add(New DesignerActionHeaderItem("Graphic"))

        items.Add( _
            New DesignerActionPropertyItem( _
                "GraphicAutoWidth", _
                "Auto Size Width", _
                "Graphic", _
                "The Automatically Size the Width from the Image Aspect Ratio"))
        If Not _DDCSelector.GraphicAutoWidth Then
            items.Add( _
                New DesignerActionPropertyItem( _
                    "GraphicWidth", _
                    "Width of Image", _
                    "Graphic", _
                    "The Width of the Image"))
        End If

        items.Add( _
            New DesignerActionPropertyItem( _
                "GraphicBorderColor", _
                "Border Color", _
                "Graphic", _
                "The color of the border around the Graphic"))

        'Add Text Item 
        items.Add( _
            New DesignerActionTextItem( _
                Space(28) & "Gonzo Diver", _
                " "))
        Return items
    End Function

#End Region 'Action Items

#End Region ' Smart Tag Items

End Class

#End Region 'DDPActionList

#End Region 'DDContainer SmartTag

#Region "DropControlsEditor"

Public Class DropControlsEditor
    Inherits UITypeEditor

    ' Indicate that we display a dropdown.
    Public Overrides Function GetEditStyle(ByVal context As System.ComponentModel.ITypeDescriptorContext) As System.Drawing.Design.UITypeEditorEditStyle
        Return UITypeEditorEditStyle.DropDown
    End Function

    ' Edit a line style
    Public Overrides Function EditValue(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal provider As System.IServiceProvider, ByVal value As Object) As Object
        ' Get an IWindowsFormsEditorService object.
        Dim editor_service As IWindowsFormsEditorService = _
            CType(provider.GetService(GetType(IWindowsFormsEditorService)), _
            IWindowsFormsEditorService)
        If editor_service Is Nothing Then
            Return MyBase.EditValue(context, provider, value)
        End If
        Dim Instance As DDContainer
        If context.Instance.GetType Is GetType(DDContainer) Then
            Instance = CType(context.Instance, DDContainer)
        Else
            Instance = CType(context.Instance, DDPActionList).CurrDDC
        End If

        ' Make the editing control.
        Dim editor_control As New ControlsListBox(Instance.Controls, editor_service)

        ' Display the editing control.
        editor_service.DropDownControl(editor_control)

        If editor_control.Text = "" Then
            Return MyBase.EditValue(context, provider, value)
        Else
            Return CType(Instance.Controls.Find(editor_control.Text, False)(0), Control)
        End If
    End Function

End Class

#Region "ControlsListBox Custom Control"

<ToolboxItem(False)> _
Public Class ControlsListBox
    Inherits ListBox

    ' The editor service displaying this control.
    Private m_EditorService As IWindowsFormsEditorService

    Public Sub New(ByVal ctrls As ControlCollection, _
     ByVal editor_service As IWindowsFormsEditorService)
        MyBase.New()

        m_EditorService = editor_service

        ' Make items for each LineStyles value.
        For Each ctrl As Control In ctrls
            If CStr(ctrl.Tag) <> "IgnoreMe" Then Me.Items.Add(ctrl.Name)
        Next ctrl

        'Me.DrawMode = Windows.Forms.DrawMode.OwnerDrawFixed
        Me.ItemHeight = 18
        Me.Height = 56
    End Sub

    ' When the user selects an item, close the dropdown.
    Private Sub ControlsListBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
        If m_EditorService IsNot Nothing Then
            m_EditorService.CloseDropDown()
        End If
    End Sub

End Class

#End Region 'ControlsListBox Custom Control

#End Region 'DropControlsEditor - UITypeEditor
