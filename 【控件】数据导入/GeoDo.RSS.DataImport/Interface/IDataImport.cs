using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DI
{
    public interface IDataImport
    {
        IDataImportDriver GetDriver(string productIdentify, string subProductIdentify, string filename);
        ImportFilesObj[] AutoFindFiles(string productIdentify, string subProIdentify, IRasterDataProvider dataProvider, string dir);
    }
}
