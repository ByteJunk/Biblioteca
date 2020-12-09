using System.Activities.Presentation.Metadata;
using System.ComponentModel;
using System.ComponentModel.Design;
using Biblioteca.Activities.Design.Designers;
using Biblioteca.Activities.Design.Properties;

namespace Biblioteca.Activities.Design
{
    public class DesignerMetadata : IRegisterMetadata
    {
        public void Register()
        {
            var builder = new AttributeTableBuilder();
            builder.ValidateTable();

            var categoryAttribute = new CategoryAttribute($"{Resources.Category}");

            builder.AddCustomAttributes(typeof(ProcessarXML), categoryAttribute);
            builder.AddCustomAttributes(typeof(ProcessarXML), new DesignerAttribute(typeof(ProcessarXMLDesigner)));
            builder.AddCustomAttributes(typeof(ProcessarXML), new HelpKeywordAttribute(""));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
}
