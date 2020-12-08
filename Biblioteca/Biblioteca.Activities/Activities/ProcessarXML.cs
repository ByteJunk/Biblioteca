using System;
using System.Activities;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;
using Biblioteca.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Localization;

namespace Biblioteca.Activities
{
    [LocalizedDisplayName(nameof(Resources.ProcessarXML_DisplayName))]
    [LocalizedDescription(nameof(Resources.ProcessarXML_Description))]
    public class ProcessarXML : ContinuableAsyncCodeActivity
    {
        #region Properties

        /// <summary>
        /// If set, continue executing the remaining activities even if the current activity has failed.
        /// </summary>
        [LocalizedCategory(nameof(Resources.Common_Category))]
        [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
        [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
        public override InArgument<bool> ContinueOnError { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessarXML_XElement_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessarXML_XElement_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<XElement> XElement { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessarXML_DataRow_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessarXML_DataRow_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InOutArgument<DataRow> DataRow { get; set; }

        [LocalizedDisplayName(nameof(Resources.ProcessarXML_EvitarColisaoNomes_DisplayName))]
        [LocalizedDescription(nameof(Resources.ProcessarXML_EvitarColisaoNomes_Description))]
        [LocalizedCategory(nameof(Resources.Input_Category))]
        public InArgument<bool> EvitarColisaoNomes { get; set; }

        #endregion


        #region Constructors

        public ProcessarXML()
        {
        }

        #endregion


        #region Protected Methods

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            if (XElement == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(XElement)));
            if (DataRow == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(DataRow)));
            if (EvitarColisaoNomes == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(EvitarColisaoNomes)));

            base.CacheMetadata(metadata);
        }

        protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
        {
            // Inputs
            var xelement = XElement.Get(context);
            var datarow = DataRow.Get(context);
            var evitarcolisaonomes = EvitarColisaoNomes.Get(context);

            datarow = IterateValues(xelement, datarow, evitarcolisaonomes);

            // Outputs
            return (ctx) =>
            {
                DataRow.Set(ctx, datarow);
            };
        }
        private DataRow IterateValues(XElement element, DataRow line, bool avoidnamecollision)
        {
            if (line.Table.Columns.Contains(element.Name.LocalName))
            {
                line[element.Name.LocalName] = element.Value;
            }

            foreach (var xAttribute in element.Attributes())
            {
                if (avoidnamecollision)
                {
                    if (line.Table.Columns.Contains(element.Name.LocalName + xAttribute.Name.LocalName))
                    {
                        line[element.Name.LocalName + xAttribute.Name.LocalName] = xAttribute.Value;
                    }
                    else
                    {
                        //Console.WriteLine("Ignored attribute: " + xAttribute.Name.LocalName);
                    }
                }
                else
                {
                    line[xAttribute.Name.LocalName] = xAttribute.Value;
                }
            }

            foreach (var subElement in element.Elements())
            {
                line = IterateValues(subElement, line, avoidnamecollision);
            }

            return line;
        }

        #endregion
    }
}

