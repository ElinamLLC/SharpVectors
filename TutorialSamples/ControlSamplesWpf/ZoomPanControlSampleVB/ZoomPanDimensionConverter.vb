Imports System
Imports System.Globalization
Imports System.Windows.Markup
Imports SharpVectors.Runtime

<MarkupExtensionReturnType(GetType(Double))>
Public NotInheritable Class ZoomPanDimensionConverter
    Inherits MarkupExtension
    Implements IMultiValueConverter

    Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
        Return Me
    End Function

    Public Function Convert(ByVal values As Object(), ByVal targetType As Type,
                            ByVal parameter As Object, ByVal culture As CultureInfo) _
                            As Object Implements IMultiValueConverter.Convert
        Dim zoomPanControl = TryCast(values(3), ZoomPanControl)
        If values(0) Is Nothing OrElse zoomPanControl Is Nothing Then
            Return DependencyProperty.UnsetValue
        End If
        Dim size = CDbl(values(0))
        Dim offset = CDbl(values(1))
        Dim zoom = CDbl(values(2))
        Dim isWidth = String.Equals(parameter?.ToString(), "width", StringComparison.OrdinalIgnoreCase)
        Return Math.Max(If(isWidth, Math.Min(zoomPanControl.ExtentWidth / zoom - offset, size),
                        Math.Min(zoomPanControl.ExtentHeight / zoom - offset, size)), 0)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetTypes As Type(),
                                ByVal parameter As Object, ByVal culture As CultureInfo) _
                                As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class
