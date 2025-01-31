﻿namespace CoreVisionBAL.Foundation.Odata
{
    public abstract class BalOdataRoot<T> : BalRoot
    {
        public abstract Task<IQueryable<T>> GetServiceModelEntitiesForOdata();
    }
}
