Imports System
Imports System.Globalization

Imports System.Windows.Markup

''' <summary>
''' Used in MainWindow.xaml to converts a scale value to a percentage.
''' It is used to display the 50%, 100%, etc that appears underneath the zoom and pan control.
''' </summary>
<MarkupExtensionReturnType(GetType(Double))>
Public NotInheritable Class ZoomPanScaleConverter
    Inherits MarkupExtension
    Implements IValueConverter

    Public Overrides Function ProvideValue(ByVal serviceProvider As IServiceProvider) As Object
        Return Me
    End Function

    ''' <summary>
    ''' Convert a fraction to a percentage.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="targetType"></param>
    ''' <param name="parameter"></param>
    ''' <param name="culture"></param>
    ''' <returns></returns>
    Public Function Convert(ByVal value As Object, ByVal targetType As Type,
                            ByVal parameter As Object, ByVal culture As CultureInfo) _
                            As Object Implements IValueConverter.Convert
        Return CDbl(CInt((CDbl(value) * 100.0)))
    End Function

    ''' <summary>
    ''' Convert a percentage back to a fraction.
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="targetType"></param>
    ''' <param name="parameter"></param>
    ''' <param name="culture"></param>
    ''' <returns></returns>
    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type,
                                ByVal parameter As Object, ByVal culture As CultureInfo) _
                                As Object Implements IValueConverter.ConvertBack
        Return CDbl(value) / 100.0
    End Function
End Class
