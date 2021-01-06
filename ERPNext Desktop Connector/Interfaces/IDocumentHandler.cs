namespace ERPNext_Desktop_Connector.Interfaces
{
    interface IDocumentHandler
    {
        IDocumentHandler SetNext(IDocumentHandler handler);

        object Handle(object request);
    }
}
