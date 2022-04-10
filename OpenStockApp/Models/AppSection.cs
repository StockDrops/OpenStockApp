using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Models;
public class AppSection
{
    public AppSection() { }
    public AppSection(string title, string icon, string iconDark, Type? targetType = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        IconDark = iconDark ?? throw new ArgumentNullException(nameof(iconDark));
        TargetType = targetType;
    }

    /// <summary>
    /// The tile of a section
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// The icon used
    /// </summary>
    public string? Icon { get; set; }
    /// <summary>
    /// The dark icon used
    /// </summary>
    public string? IconDark { get; set; }
    /// <summary>
    /// the type
    /// </summary>
    public Type? TargetType { get; set; }
    //public IEnumerable<AppSection> ChildrenAppSections { get; set; }
}
