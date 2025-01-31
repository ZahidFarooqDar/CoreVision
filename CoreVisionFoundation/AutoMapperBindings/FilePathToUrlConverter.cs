using AutoMapper;
using CoreVisionBAL.Foundation.CommonUtils;

namespace CoreVisionFoundation.AutoMapperBindings
{
    public class FilePathToUrlConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember.ConvertFromFilePathToUrl();
        }
    }
}
