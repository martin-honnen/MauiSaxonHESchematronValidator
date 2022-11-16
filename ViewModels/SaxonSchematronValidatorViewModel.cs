using net.sf.saxon.s9api;

using net.liberty_development.SaxonHE11s9apiExtensions;

using System.Reflection;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiSaxonHESchematronValidator.ViewModels
{
    public class SaxonSchematronValidatorViewModel : ObservableObject //INotifyPropertyChanged
    {
        private static string svrlUri = "http://purl.oclc.org/dsdl/svrl";

        private static Processor processor;

        private static HttpClient httpClient;

        //private XsltCompiler xsltCompiler = null;

        //public XsltCompiler MyXsltCompiler
        //{
        //    get
        //    {
        //        if (xsltCompiler == null)
        //        {
        //            xsltCompiler = processor.NewXsltCompiler();

        //            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schxsltSvrlXsltResource))
        //            {
        //                var compiledSchxsltExecutable = xsltCompiler.Compile(resourceStream);
        //                schxsltXsltCompiler = compiledSchxsltExecutable.Load30();
        //            }
        //        }
        //        return xsltCompiler;
        //    }
        //}

        //private Xslt30Transformer schxsltXsltCompiler;
        private XsltExecutable schxsltXsltCompilerExecutable;
        private XsltExecutable SchxsltXsltCompilerExecutable
        {
            get
            {
                if (schxsltXsltCompilerExecutable == null)
                {
                    var xsltCompiler = processor.newXsltCompiler();

                    using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schxsltSvrlXsltResource))
                    {
                        schxsltXsltCompilerExecutable = xsltCompiler.compile(resourceStream.AsSource());
                    }

                }

                return schxsltXsltCompilerExecutable;
            }
        }


        static string schxsltSvrlXsltResource = "MauiSaxonHESchematronValidator.ViewModels._2._0.pipeline-for-svrl.xsl.sef";

        //public event PropertyChangedEventHandler PropertyChanged;


        public SaxonSchematronValidatorViewModel(Processor processor, HttpClient httpClient)
        {
            SaxonSchematronValidatorViewModel.processor = processor;
            SaxonSchematronValidatorViewModel.httpClient = httpClient;

            ValidateCommand = new AsyncRelayCommand(ValidateInstanceAgainstSchema);//new Command(() => { ValidateInstanceAgainstSchema(); }); //new Command(async () => { await ValidateInstanceAgainstSchema(); });
        }

        public string SaxonProductTitle
        {
            get => $"Saxon {processor.getSaxonEdition()} {processor.getSaxonProductVersion()}";
        }

        private async Task<string> ValidateInstanceAgainstSchema()
        {
            var xsltCompiler = processor.newXsltCompiler();

            var compiledSchematron = new XdmDestination();

            var schematronInstanceUri = new Uri(schemaUri);

            using (var schematronStream = await httpClient.GetStreamAsync(schematronInstanceUri).ConfigureAwait(false))
            {
                var schxsltXsltCompilerTransformer = SchxsltXsltCompilerExecutable.load30();
                schxsltXsltCompilerTransformer.transform(schematronStream.AsSource(schemaUri), compiledSchematron);
            }

            var compiledSchematronExecutable = xsltCompiler.compile(compiledSchematron.getXdmNode().asSource());

            var schematronValidator = compiledSchematronExecutable.load30();

            var xmlInstanceUri = new Uri(instanceUri);

            var validationSvrlResult = new XdmDestination();

            using (var xmlSampleStream = await httpClient.GetStreamAsync(xmlInstanceUri))
            {
                schematronValidator.transform(xmlSampleStream.AsSource(instanceUri), validationSvrlResult);
            }

            var xpathCompiler = processor.newXPathCompiler();
            xpathCompiler.declareNamespace("svrl", svrlUri);
            var validationProblems = xpathCompiler.evaluate("/*/svrl:failed-assert | /*/svrl:successful-report", validationSvrlResult.getXdmNode()).stream().asListOfNodes().toArray().OfType<XdmNode>().ToArray();

            if (validationProblems.Length == 0)
            {
                //ValidationResult = $"XML sample {instanceUri} is valid against Schematron schema {schemaUri}.";
                return $"XML sample {instanceUri} is valid against Schematron schema {schemaUri}.";
            }
            else
            {
                //ValidationResult = $"XML sample {instanceUri} is not valid against Schematron schema {schemaUri}:{Environment.NewLine}{string.Join(Environment.NewLine, validationProblems.Select(node => node.GetAttributeValue("location") + ": " + node.StringValue))}";
                return $"XML sample {instanceUri} is not valid against Schematron schema {schemaUri}:{Environment.NewLine}{string.Join(Environment.NewLine, validationProblems.Select(node => node.getAttributeValue(new QName("location")) + ": " + node.getStringValue()))}";
            }
        }

        //public Command ValidateCommand { get; }
        public IAsyncRelayCommand ValidateCommand { get; }

        private string? instanceUri; // = "https://github.com/martin-honnen/SchematronSchxsltSaxonHE11Net/raw/master/SchematronSchxsltSaxonHE11Net/books.xml";

        private string validationResult = "Not yet validated!";

        public string ValidationResult
        {
            get
            {
                return validationResult;
            }
            set
            {
                SetProperty(ref validationResult, value);
                //if (validationResult != value)
                //{
                //    validationResult = value;
                //    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ValidationResult)));
                //}
            }
        }
        public string InstanceUri
        {
            get
            {
                return instanceUri;
            }
            set
            {
                SetProperty(ref instanceUri, value);
                //if (instanceUri != value)
                //{
                //    instanceUri = value;
                //    PropertyChanged(this, new PropertyChangedEventArgs(nameof(InstanceUri)));
                //}
            }
        }

        private string? schemaUri; // = "https://github.com/martin-honnen/SchematronSchxsltSaxonHE11Net/raw/master/SchematronSchxsltSaxonHE11Net/price.sch";

        public string SchemaUri
        {
            get
            {
                return schemaUri;
            }
            set
            {
                SetProperty(ref schemaUri, value);
                //if (schemaUri != value)
                //{
                //    schemaUri = value;
                //    PropertyChanged(this, new PropertyChangedEventArgs(nameof(SchemaUri)));
                //}
            }
        }
    }
}
