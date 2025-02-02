using AutoMapper;
using CoreVisionBAL.Foundation.Odata;
using CoreVisionDAL.Context;

namespace CoreVisionBAL.Foundation.Base
{
    public class CoreVisionBalBase : BalRoot
    {
        protected readonly IMapper _mapper;
        protected readonly ApiDbContext _apiDbContext;

        public CoreVisionBalBase(IMapper mapper, ApiDbContext apiDbContext)
        {
            _mapper = mapper;
            _apiDbContext = apiDbContext;
        }
    }
    public abstract class CoreVisionBalOdataBase<T> : BalOdataRoot<T>
    {
        protected readonly IMapper _mapper;
        protected readonly ApiDbContext _apiDbContext;

        protected CoreVisionBalOdataBase(IMapper mapper, ApiDbContext apiDbContext)
        {
            _mapper = mapper;
            _apiDbContext = apiDbContext;
        }
    }
}
