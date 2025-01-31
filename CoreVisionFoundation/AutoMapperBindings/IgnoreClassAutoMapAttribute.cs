using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;

namespace CoreVisionFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreClassAutoMapAttribute : AutoInjectRootAttribute
    {
    }
}
