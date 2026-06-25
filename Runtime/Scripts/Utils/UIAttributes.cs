using System;


/// <summary>
/// Reemplaza el nombre del campo por uno más legible en la UI.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class UINameAttribute : Attribute
{
    public string DisplayName { get; }

    public UINameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}


/// <summary>
/// Permite formatear números con formato personalizado y unidad de medida.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class UIFormatAttribute : Attribute
{
    public string Format { get; }
    public string Unit { get; }

    public UIFormatAttribute(string format = null, string unit = null)
    {
        Format = format;
        Unit = unit;
    }
}


/// <summary>
/// Si es true, mostrará H:MM:SS cuando la duración ≥ 1 hora; 
/// si false, siempre MM:SS (aunque supere 60m, hará roll-over de horas en minutos)
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class UITimeFormatAttribute : Attribute
{
    
    public bool ShowHoursIfNeeded { get; }

    public UITimeFormatAttribute(bool showHoursIfNeeded = true)
    {
        ShowHoursIfNeeded = showHoursIfNeeded;
    }
}

/// <summary>
/// Se usa en los atributos de tipo de dato complejo, que están en un campo "detalle", para que cada campo sea mostrado en el detalle,
/// como en el campo TruckInteractionData de la clase CycleDetailItem.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class UIExpandableAttribute : Attribute { }
