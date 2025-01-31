using Microsoft.EntityFrameworkCore;

namespace CoreVisionDAL.Base
{
    public abstract class EfCoreContextRoot : DbContext, IEfCoreContextRoot
    {
        public EfCoreContextRoot(DbContextOptions options)
            : base(options)
        {
        }
    }
}
