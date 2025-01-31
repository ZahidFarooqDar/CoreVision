using CoreVisionServiceModels.Foundation.Base.CommonResponseRoot;

namespace CoreVisionFoundation.AutoMapperBindings
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ConvertFromUtcToLocalDateAttribute : AutoInjectRootAttribute
    {
        public string? SourcePropertyName { get; set; }
    }
}
