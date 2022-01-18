﻿using System.Collections.Generic;
using Baracuda.Pooling.Utils;

namespace Baracuda.Pooling.Abstractions
{
    public class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
    {
        private static readonly ObjectPoolT<TCollection> _pool 
            = new ObjectPoolT<TCollection>(() => new TCollection(), actionOnRelease: l => l.Clear());

        /// <summary>
        /// Get an object from the pool. Must be manually released back to the pool by calling Release.
        /// This operation is not thread safe!
        /// </summary>
        /// <returns></returns>
        public static TCollection Get()
        {
            return _pool.Get();
        }

        /// <summary>
        /// Release an object to the pool.
        /// This operation is not thread safe!
        /// </summary>
        public static void Release(TCollection toRelease)
        {
            _pool.Release(toRelease);
        }
        
        /// <summary>
        /// Get a disposable object to the pool that will automatically be released back to the pool.
        /// Access the pooled object by the <see cref="PooledObject{T}.Value"/> property. 
        /// </summary>
        public static PooledObject<TCollection> GetDisposable()
        {
            return _pool.GetDisposable();
        }
    }
}