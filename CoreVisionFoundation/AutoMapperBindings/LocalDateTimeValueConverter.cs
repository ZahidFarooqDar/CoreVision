using AutoMapper;

namespace CoreVisionFoundation.AutoMapperBindings
{
    public class LocalDateTimeValueConverter : IValueConverter<DateTime, DateTime>
    {
        public DateTime Convert(DateTime sourceMember, ResolutionContext context)
        {
            return sourceMember.ConvertFromUTCToSystemTimezone();
        }
    }
}
